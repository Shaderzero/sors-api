using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sors.Authentication;
using sors.Data;
using sors.Data.Dto;
using sors.Data.Dto.Incidents;
using sors.Data.Entities.Incidents;
using sors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireUser")]
    public class DraftsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private AccountForListDto _currentUser;

        public DraftsController(DataContext context, IMapper mapper, IConfiguration configuration, IUserService userService, IMailService mailService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
            _mailService = mailService;
        }

        private AccountForListDto CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    if (User != null)
                    {
                        var username = User.Identity.Name;
                        username = username.Substring(username.IndexOf(@"\") + 1);
                        var userFromDb = _context.Accounts
                            .Include(u => u.Department)
                            .Include(u => u.AccountRoles).ThenInclude(r => r.Role)
                            .Where(u => u.Name == username)
                            .FirstOrDefault();
                        if (userFromDb != null)
                        {
                            var userToReturn = _mapper.Map<AccountForListDto>(userFromDb);
                            _currentUser = userToReturn;
                        }
                        else
                        {
                            _currentUser = null;
                        }
                    }
                    else
                    {
                        _currentUser = null;
                    }
                }
                return _currentUser;
            }
            set { _currentUser = value; }
        }

        [HttpGet()]
        public async Task<IActionResult> GetDrafts([FromQuery]Params parameters)
        {
            var source = _context.Drafts
                .Include(d => d.Author)
                .Include(d => d.IncidentType)
                .Include(d => d.Department)
                .AsQueryable();
            switch (parameters.Status)
            {
                case "any":
                    source = source.Where(s => !s.Status.Equals("close") && !s.Status.Equals("example"));
                    break;
                default:
                    source = source.Where(s => s.Status.Equals(parameters.Status));
                    break;
            }
            if (!parameters.Status.Equals("example"))
            {
                switch (parameters.Type)
                {
                    case "any":
                        break;
                    case "forAdmin":
                        break;
                    case "forUser":
                        source = source.Where(s => s.AuthorId == parameters.AccountId);
                        break;
                    case "forRC":
                        source = source.Where(s => (!s.Status.Equals("draft") && s.DepartmentId == parameters.DepartmentId) || (s.AuthorId == parameters.AccountId));
                        break;
                    case "forRM":
                        source = source.Where(s => (!s.Status.Equals("draft")) || (s.DepartmentId == parameters.DepartmentId) || (s.AuthorId == parameters.AccountId));
                        break;
                }
            }
            if (!string.IsNullOrEmpty(parameters.Order))
            {
                if (parameters.OrderAsc)
                {
                    switch (parameters.Order)
                    {
                        case "dateCreate":
                            source = source.OrderBy(s => s.DateCreate);
                            break;
                        case "description1":
                            source = source.OrderBy(s => s.Description1);
                            break;
                        case "description2":
                            source = source.OrderBy(s => s.Description2);
                            break;
                        case "author":
                            source = source.OrderBy(s => s.Author.Name);
                            break;
                        case "department":
                            source = source.OrderBy(s => s.Department.Name);
                            break;
                        case "status":
                            source = source.OrderBy(s => s.Status);
                            break;
                        case "type":
                            source = source.OrderBy(s => s.IncidentType.Name);
                            break;
                    }
                }
                else
                {
                    switch (parameters.Order)
                    {
                        case "dateCreate":
                            source = source.OrderByDescending(s => s.DateCreate);
                            break;
                        case "description1":
                            source = source.OrderByDescending(s => s.Description1);
                            break;
                        case "description2":
                            source = source.OrderByDescending(s => s.Description2);
                            break;
                        case "author":
                            source = source.OrderByDescending(s => s.Author.Name);
                            break;
                        case "department":
                            source = source.OrderByDescending(s => s.Department.Name);
                            break;
                        case "status":
                            source = source.OrderByDescending(s => s.Status);
                            break;
                        case "type":
                            source = source.OrderByDescending(s => s.IncidentType);
                            break;
                    }
                }
            }
            else
            {
                source = source.OrderBy(s => s.DateCreate);
            }
            if (!string.IsNullOrEmpty(parameters.Filter))
            {
                var f = parameters.Filter.ToLower();
                source = source.Where(s => s.Description1.ToLower().Contains(f)
                        || s.Description2.ToLower().Contains(f)
                        || s.Description3.ToLower().Contains(f)
                        || s.Description4.ToLower().Contains(f)
                        || s.Description5.ToLower().Contains(f)
                        || s.Author.Name.ToLower().Contains(f)
                        || s.Department.Name.ToLower().Contains(f)
                        || s.Status.ToLower().Contains(f)
                        || s.IncidentType.Name.ToLower().Contains(f)
                        );
            }
            var result = await PagedList<Draft>.CreateAsync(source, parameters.PageNumber, parameters.PageSize);
            var drafts = _mapper.Map<IEnumerable<DraftForListDto>>(result);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(drafts);
        }
        
        [HttpGet("forincident")]
        public async Task<IActionResult> GetDraftsForIncident()
        {
            var drafts = await _context.Drafts
                .Include(d => d.Department)
                .Include(d => d.IncidentType)
                .Include(d => d.Author)
                .Where(d => d.Status.Equals("sign"))
                .ToListAsync();
            var draftsToReturn = _mapper.Map<IEnumerable<DraftForListDto>>(drafts);
            return Ok(draftsToReturn);
        }

        [HttpGet("counts")]
        public async Task<IActionResult> GetCountsOLD([FromQuery]Params parameters)
        {
            var source = _context.Drafts
                .Include(d => d.Author)
                .Include(d => d.Department)
                .AsQueryable();
            switch (parameters.Status)
            {
                case "any":
                    source = source.Where(s => !s.Status.Equals("close"));
                    break;
                default:
                    source = source.Where(s => s.Status.Equals(parameters.Status));
                    break;
            }
            switch (parameters.Type)
            {
                case "forAdmin":
                    break;
                case "any":
                    break;
                case "forUser":
                    source = source.Where(s => s.AuthorId == parameters.AccountId);
                    break;
                case "forRC":
                    source = source.Where(s => (!s.Status.Equals("draft") && s.DepartmentId == parameters.DepartmentId) || (s.AuthorId == parameters.AccountId));
                    break;
                case "forRM":
                    source = source.Where(s => (!s.Status.Equals("draft")) || (s.DepartmentId == parameters.DepartmentId) || (s.AuthorId == parameters.AccountId));
                    break;
            }
            var count = await source.CountAsync();
            return Ok(count);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDraft(DraftForCreateDto draftForCreate)
        {
            Draft draftToCreate = new Draft
            {
                DateCreate = DateTime.UtcNow,
                AuthorId = draftForCreate.AuthorId,
                IncidentTypeId = draftForCreate.IncidentTypeId,
                DepartmentId = draftForCreate.DepartmentId,
                Description1 = draftForCreate.Description1,
                Description2 = draftForCreate.Description2,
                Description3 = draftForCreate.Description3,
                Description4 = draftForCreate.Description4,
                Description5 = draftForCreate.Description5,
                Status = draftForCreate.Status
            };
            draftToCreate.Props.Add(new DraftProp()
            {
                Draft = draftToCreate,
                Action = "creation",
                AuthorId = draftForCreate.AuthorId,
                Comment = draftForCreate.Comment,
                DateCreate = DateTime.UtcNow
            });
            _context.Add(draftToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                var draftToReturn = _mapper.Map<DraftForListDto>(draftToCreate);
                return Ok(draftToReturn);
            }
            return BadRequest("Creating draft failed on save");
        }

        [HttpGet("{draftId}")]
        public async Task<IActionResult> GetDraft(int draftId)
        {
            var draft = await _context.Drafts
                .Include(d => d.IncidentType)
                .Include(d => d.Author)
                .Include(d => d.Department)
                .Include(d => d.Props).ThenInclude(p => p.Author)
                .Where(d => d.Id == draftId)
                .FirstOrDefaultAsync();
            if (draft == null)
                return NotFound();
            var draftToReturn = _mapper.Map<DraftForDetailDto>(draft);
            // проверка прав доступа юзера
            var access = false;
            var currentUser = CurrentUser;
            if (currentUser.AccountRoles.Any(e => e.Name == "admin"))
            {
                return Ok(draftToReturn);
            }
            if (draft.AuthorId == currentUser.Id)
            {
                return Ok(draftToReturn);
            }
            if (currentUser.AccountRoles.Any(e => e.Name == "riskManager"))
            {
                if (!draft.Status.Equals("draft") && !draft.Status.Equals("deleted"))
                {
                    return Ok(draftToReturn);
                }
            }
            if (currentUser.AccountRoles.Any(e => e.Name == "riskCoordinator"))
            {
                if (draft.DepartmentId == currentUser.Department.Id)
                {
                    if (!draft.Status.Equals("draft") && !draft.Status.Equals("deleted"))
                    {
                        return Ok(draftToReturn);
                    }
                }
            }
            return StatusCode(403);
        }

        [HttpPatch("{draftId}/status", Name = "UpdateStatus")]
        public async Task<IActionResult> SetStatus(StatusHelper statusHelper, int draftId)
        {
            var currentUser = await GetCurrentUser();
            var draft = await _context.Drafts
                .Include(d => d.Props)
                .Where(d => d.Id == draftId)
                .FirstOrDefaultAsync();
            if (draft == null)
                return NotFound();
            draft.Status = statusHelper.Status;
            draft.Props.Add(new DraftProp
            {
                DraftId = draft.Id,
                DateCreate = DateTime.UtcNow,
                AuthorId = currentUser.Id,
                Action = statusHelper.Status,
                Comment = statusHelper.Comment
            });
            _context.Update(draft);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (statusHelper.Status.Equals("check"))
                {
                    await _mailService.SendCheckDraftMail(draft, statusHelper.Comment);
                } else if (statusHelper.Status.Equals("sign"))
                {
                    await _mailService.SendSignDraftMail(draft, statusHelper.Comment);
                }
                else if (statusHelper.Status.Equals("refine"))
                {
                    await _mailService.SendRefineDraftMail(draft, statusHelper.Comment);
                }
                else if (statusHelper.Status.Equals("close"))
                {
                    await _mailService.SendCloseDraftMail(draft, statusHelper.Comment);
                }
                return NoContent();
            }
            return BadRequest("Failed to set draft status " + statusHelper.Status);
        }

        [HttpPost("{draftId}")]
        public async Task<IActionResult> DeleteDraft(int draftId)
        {
            var draftForDelete = await _context.Drafts
                .Include(d => d.Props)
                .FirstOrDefaultAsync(d => d.Id == draftId);
            if (draftForDelete == null)
                return NotFound();
            _context.Remove(draftForDelete);
            if (await _context.SaveChangesAsync() > 0)
                return NoContent();
            return BadRequest("Failed to delete the draft");
        }

        [HttpPut("{draftId}")]
        public async Task<IActionResult> UpdateDraft(DraftForCreateDto draftForUpdate, int draftId)
        {
            var draftToUpdate = await _context.Drafts
                .Include(d => d.Props).ThenInclude(p => p.Author)
                .FirstOrDefaultAsync(d => d.Id == draftId);
            if (draftToUpdate == null)
                return NotFound();
            draftToUpdate.IncidentTypeId = draftForUpdate.IncidentTypeId;
            draftToUpdate.Description1 = draftForUpdate.Description1;
            draftToUpdate.Description2 = draftForUpdate.Description2;
            draftToUpdate.Description3 = draftForUpdate.Description3;
            draftToUpdate.Description4 = draftForUpdate.Description4;
            draftToUpdate.Description5 = draftForUpdate.Description5;
            draftToUpdate.Props.Add(new DraftProp
            {
                DraftId = draftId,
                AuthorId = draftForUpdate.AuthorId,
                Action = "updating",
                Comment = draftForUpdate.Comment,
                DateCreate = DateTime.UtcNow
            });
            _context.Update(draftToUpdate);
            if (await _context.SaveChangesAsync() > 0)
            {
                var draftForReturn = _mapper.Map<DraftForDetailDto>(draftToUpdate);
                return Ok(draftForReturn);
            }

            return BadRequest("Failed to update draft");
        }

        private async Task<AccountForListDto> GetCurrentUser()
        {
            if (User != null)
            {
                var username = User.Identity.Name;
                username = username.Substring(username.IndexOf(@"\") + 1);
                var userFromDb = await _context.Accounts
                    .Include(u => u.Department)
                    .Include(u => u.AccountRoles).ThenInclude(r => r.Role)
                    .Where(u => u.Name == username)
                    .FirstOrDefaultAsync();
                if (userFromDb != null)
                {
                    var userToReturn = _mapper.Map<AccountForListDto>(userFromDb);
                    return userToReturn;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}