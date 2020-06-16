using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using sors.Authentication;
using sors.Data;
using sors.Data.Dto;
using sors.Data.Dto.Incidents;
using sors.Data.Entities.Incidents;
using sors.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : ControllerBase
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUserService _userService;
        private AccountForListDto _currentUser;

        public IncidentsController(DataContext context, IMapper mapper, IWebHostEnvironment IHostingEnvironment, IUserService userService, IMailService mailService)
        {
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = IHostingEnvironment;
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
        public async Task<IActionResult> GetIncidents([FromQuery]Params parameters)
        {
            // источник данных
            var source = _context.Incidents.AsQueryable();
            // определяем юзера
            var currentUser = CurrentUser;
            // определяем роли пользователя
            if (currentUser.AccountRoles.Any(e => e.Name == "admin"))
            {
                //source = source;
            }
            else if (currentUser.AccountRoles.Any(e => e.Name == "riskManager"))
            {
                //source = source;
            }
            else if (currentUser.AccountRoles.Any(e => e.Name == "riskCoordinator"))
            {
                source = source
                        .Where(incident => _context.Responsibles
                            .Any(r => r.IncidentId == incident.Id && r.DepartmentId == currentUser.Department.Id && r.Result != "deleted"));
            }
            else
            {
                source = source
                        .Where(incident => _context.ResponsibleAccounts
                            .Any(r => r.Responsible.IncidentId == incident.Id && r.AccountId == currentUser.Id && r.Responsible.Result != "deleted"));
            }
            switch (parameters.Status)
            {
                case "any":
                    source = source.Where(s => !s.Status.Equals("close"));
                    break;
                case "wait":
                    if (currentUser.AccountRoles.Any(e => e.Name == "admin" || e.Name == "riskManager"))
                    {
                        source = source.Where(s => s.Status.Equals(parameters.Status));
                    }
                    else if (currentUser.AccountRoles.Any(e => e.Name == "riskCoordinator"))
                    {
                        source = source
                                .Where(incident => _context.Responsibles
                                    .Any(r => r.IncidentId == incident.Id && r.Result.Equals("wait") && r.DepartmentId == currentUser.Department.Id));
                    }
                    else
                    {
                        source = source.Where(s => s.Status.Equals(parameters.Status));
                    }
                    break;
                default:
                    source = source.Where(s => s.Status.Equals(parameters.Status));
                    break;
            }
            if (!string.IsNullOrEmpty(parameters.Order))
            {
                if (parameters.OrderAsc)
                {
                    switch (parameters.Order)
                    {
                        case "dateIncident":
                            source = source.OrderBy(s => s.DateIncident);
                            break;
                        case "description":
                            source = source.OrderBy(s => s.Description);
                            break;
                        case "status":
                            source = source.OrderBy(s => s.Status);
                            break;
                    }
                }
                else
                {
                    switch (parameters.Order)
                    {
                        case "dateIncident":
                            source = source.OrderByDescending(s => s.DateIncident);
                            break;
                        case "description":
                            source = source.OrderByDescending(s => s.Description);
                            break;
                        case "status":
                            source = source.OrderByDescending(s => s.Status);
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
                source = source.Where(s => s.Description.ToLower().Contains(f)
                        || s.Status.ToLower().Contains(f)
                        );
            }
            var result = await PagedList<Incident>.CreateAsync(source, parameters.PageNumber, parameters.PageSize);
            var incidents = _mapper.Map<IEnumerable<IncidentForListDto>>(result);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(incidents);
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery]Params parameters)
        {
            // источник данных
            var source = _context.Incidents.AsQueryable();
            // определяем юзера
            var currentUser = CurrentUser;
            // определяем роли пользователя
            if (currentUser.AccountRoles.Any(e => e.Name == "admin"))
            {
                source = source;
            }
            else if (currentUser.AccountRoles.Any(e => e.Name == "riskManager"))
            {
                source = source;
            }
            else if (currentUser.AccountRoles.Any(e => e.Name == "riskCoordinator"))
            {
                source = source
                        .Where(incident => _context.Responsibles
                            .Any(r => r.IncidentId == incident.Id && r.DepartmentId == currentUser.Department.Id));
            }
            else
            {
                source = source
                        .Where(incident => _context.ResponsibleAccounts
                            .Any(r => r.Responsible.IncidentId == incident.Id && r.AccountId == currentUser.Id));
            }
            switch (parameters.Status)
            {
                case "any":
                    source = source.Where(s => !s.Status.Equals("close"));
                    break;
                default:
                    source = source.Where(s => s.Status.Equals(parameters.Status));
                    break;
            }
            // убираем РС без категорий
            if (currentUser.AccountRoles.Any(e => e.Name == "admin"))
            {
                source = source;
            }
            else
            {
                source = source.Where(s => s.IncidentType != null);
            }
            if (!string.IsNullOrEmpty(parameters.Order))
            {
                if (parameters.OrderAsc)
                {
                    switch (parameters.Order)
                    {
                        case "dateIncident":
                            source = source.OrderBy(s => s.DateIncident);
                            break;
                        case "description":
                            source = source.OrderBy(s => s.Description);
                            break;
                        case "status":
                            source = source.OrderBy(s => s.Status);
                            break;
                    }
                }
                else
                {
                    switch (parameters.Order)
                    {
                        case "dateIncident":
                            source = source.OrderByDescending(s => s.DateIncident);
                            break;
                        case "description":
                            source = source.OrderByDescending(s => s.Description);
                            break;
                        case "status":
                            source = source.OrderByDescending(s => s.Status);
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
                source = source.Where(s => s.Description.ToLower().Contains(f)
                        || s.Status.ToLower().Contains(f)
                        );
            }
            var result = await source
                .Include(s => s.Drafts).ThenInclude(d => d.Draft)
                .Include(s => s.Drafts).ThenInclude(d => d.Draft).ThenInclude(d => d.Department)
                .Include(s => s.Drafts).ThenInclude(d => d.Draft).ThenInclude(d => d.Author)
                .Include(s => s.Responsibles).ThenInclude(r => r.Department)
                .Include(s => s.IncidentType)
                .ToListAsync();

            var fileDownloadName = "report.xlsx";
            var reportsFolder = "reports";

            using (var package = createExcelPackage(result))
            {
                package.SaveAs(new FileInfo(Path.Combine(_hostingEnvironment.WebRootPath, reportsFolder, fileDownloadName)));
            }
            return File($"~/{reportsFolder}/{fileDownloadName}", XlsxContentType, fileDownloadName);
        }

        [HttpGet("{incidentId}", Name = "GetIncident")]
        public async Task<IActionResult> GetIncident(int incidentId)
        {
            var incident = await _context.Incidents
                .Include(e => e.IncidentType)
                .Include(e => e.Drafts).ThenInclude(d => d.Draft).ThenInclude(d => d.Department)
                .Include(e => e.Drafts).ThenInclude(d => d.Draft).ThenInclude(d => d.Author)
                .Include(e => e.Responsibles).ThenInclude(r => r.Department)
                .Include(e => e.Responsibles).ThenInclude(d => d.Accounts).ThenInclude(u => u.Account)
                .FirstOrDefaultAsync(e => e.Id == incidentId);
            if (incident == null)
                return NotFound();
            // проверка прав доступа юзера
            var access = false;
            var currentUser = CurrentUser;
            if (currentUser.AccountRoles.Any(e => e.Name == "admin" || e.Name == "riskManager"))
            {
                access = true;
            } 
            else if (currentUser.AccountRoles.Any(e => e.Name == "riskCoordinator"))
            {
                if (incident.Responsibles.Any(e => e.DepartmentId == currentUser.Department.Id && e.Result != "deleted"))
                {
                    access = true;
                }
            }
            else
            {
                if (incident.Responsibles.Any(e => e.Accounts.Any(a => a.AccountId == currentUser.Id)))
                {
                    access = true;
                }
            }
            if (!access)
            {
                return StatusCode(403);
            }
            var eventToReturn = _mapper.Map<IncidentForDetailDto>(incident);
            return Ok(eventToReturn);
        }

        [HttpGet("{incidentId}/excel")]
        public async Task<IActionResult> GetIncidentExcel(int incidentId)
        {
            var incident = await _context.Incidents
                .Include(e => e.IncidentType)
                .Include(e => e.Props).ThenInclude(x => x.Author)
                .Include(e => e.Drafts).ThenInclude(d => d.Draft).ThenInclude(d => d.Department)
                .Include(e => e.Drafts).ThenInclude(d => d.Draft).ThenInclude(d => d.Author)
                .Include(e => e.Responsibles).ThenInclude(r => r.Department)
                .Include(e => e.Responsibles).ThenInclude(r => r.Accounts).ThenInclude(u => u.Account)
                .Include(e => e.Responsibles).ThenInclude(r => r.Measures).ThenInclude(m => m.Reports)
                .FirstOrDefaultAsync(e => e.Id == incidentId);
            if (incident == null)
                return NotFound();
            //var eventToReturn = _mapper.Map<IncidentForDetailDto>(incident);
            var fileDownloadName = "report.xlsx";
            var reportsFolder = "reports";

            using (var package = createExcelPackage2(incident))
            {
                package.SaveAs(new FileInfo(Path.Combine(_hostingEnvironment.WebRootPath, reportsFolder, fileDownloadName)));
            }
            return File($"~/{reportsFolder}/{fileDownloadName}", XlsxContentType, fileDownloadName);
        }

        [Authorize(Policy = "RequireRM")]
        [HttpPost()]
        public async Task<IActionResult> CreateIncident(IncidentForCreateDto incidentForCreate)
        {
            var currentUser = CurrentUser;
            if (currentUser == null)
                return NotFound();
            var incident = new Incident
            {
                DateCreate = DateTime.UtcNow,
                IncidentTypeId = incidentForCreate.IncidentTypeId,
                DateIncident = incidentForCreate.DateIncident,
                Description = incidentForCreate.Description,
                Status = "wait"
            };
            foreach (var responsible in incidentForCreate.Responsibles)
            {
                incident.Responsibles.Add(new Responsible
                {
                    Incident = incident,
                    DepartmentId = responsible.Department.Id,
                    Result = "wait"
                });
            }
            foreach (var draft in incidentForCreate.Drafts)
            {
                var d = await _context.Drafts
                    .FirstOrDefaultAsync(e => e.Id == draft.Id);
                if (d == null)
                    return BadRequest("Draft not found, id:=" + draft.Id);
                d.Status = "open";
                incident.Drafts.Add(new IncidentDraft
                {
                    DraftId = d.Id,
                    Incident = incident
                });
            }
            incident.Props.Add(new IncidentProp()
            {
                Incident = incident,
                Action = "create",
                AuthorId = currentUser.Id,
                Comment = incidentForCreate.Comment,
                DateCreate = DateTime.UtcNow
            });
            _context.Add(incident);
            if (await _context.SaveChangesAsync() > 0)
            {
                var createdIncident = await _context.Incidents
                    .Include(i => i.Responsibles).ThenInclude(r => r.Department)
                    .Where(i => i.Id == incident.Id)
                    .FirstOrDefaultAsync();
                var eventToReturn = _mapper.Map<IncidentForListDto>(createdIncident);
                await _mailService.SendResignMail(createdIncident.Responsibles, createdIncident.Id, incidentForCreate.Comment);
                return Ok(eventToReturn);
            }
            return BadRequest("Creating the RiskEvent failed on save");
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost("{incidentId}")]
        public async Task<IActionResult> DeleteIncident(int incidentId)
        {
            var incident = await _context.Incidents
                .Include(e => e.Responsibles).ThenInclude(r => r.Accounts).ThenInclude(u => u.Responsible).ThenInclude(u => u.Props)
                .Include(e => e.Responsibles).ThenInclude(r => r.Measures).ThenInclude(m => m.Reports).ThenInclude(m => m.Props)
                .Include(e => e.Responsibles).ThenInclude(r => r.Measures).ThenInclude(m => m.Props)
                .Include(e => e.Responsibles).ThenInclude(r => r.Props)
                .Include(e => e.Drafts)
                .Include(e => e.Props)
                .FirstOrDefaultAsync(e => e.Id == incidentId);
            if (incident == null)
                return NotFound();
            foreach (var incidentDraft in incident.Drafts)
            {
                _context.Remove(incidentDraft);
            }
            _context.Remove(incident);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to delete incident");
        }

        [Authorize(Policy = "RequireRM")]
        [HttpPatch("{incidentId}")]
        public async Task<IActionResult> PatchIncident(int incidentId, List<PatchDto> patchDtos)
        {
            var incidentForPatch = await _context.Incidents
                .Include(e => e.Props)
                .FirstOrDefaultAsync(d => d.Id == incidentId);
            if (incidentForPatch == null)
                return NotFound();
            var nameValuePairProperties = patchDtos.ToDictionary(a => a.PropertyName, a => a.PropertyValue);
            var dbEntityEntry = _context.Entry(incidentForPatch);
            dbEntityEntry.CurrentValues.SetValues(nameValuePairProperties);
            dbEntityEntry.State = EntityState.Modified;
            incidentForPatch.Props.Add(new IncidentProp()
            {
                IncidentId = incidentId,
                Action = "update",
                AuthorId = patchDtos[0].AuthorId,
                Comment = "",
                DateCreate = DateTime.UtcNow
            });
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to patch incident");
        }

        [Authorize(Policy = "RequireRM")]
        [HttpGet("{incidentId}/draftadd/{draftId}")]
        public async Task<IActionResult> AddDraftToIncident(int incidentId, int draftId)
        {
            var currentUser = CurrentUser;
            if (currentUser == null)
                return NotFound();
            var incident = await _context.Incidents
                .Include(e => e.Drafts)
                .FirstOrDefaultAsync(e => e.Id == incidentId);
            if (incident == null)
                return NotFound();
            var draft = await _context.Drafts
                .Include(e => e.Props)
                .FirstOrDefaultAsync(d => d.Id == draftId);
            if (draft == null)
                return NotFound();
            //draft.Status = "open";
            incident.Drafts.Add(new IncidentDraft
            {
                IncidentId = incident.Id,
                DraftId = draft.Id
            });
            draft.Status = "open";
            draft.Props.Add(new DraftProp
            {
                DraftId = draft.Id,
                DateCreate = DateTime.UtcNow,
                AuthorId = currentUser.Id,
                Action = "sign",
                Comment = "Статус сообщения обновлён в связи с включением в рисковое событие"
            });
            if (await _context.SaveChangesAsync() > 0)
            {

                return NoContent();
            }
            return BadRequest("Adding draft to incident failed on save");
        }

        [Authorize(Policy = "RequireRM")]
        [HttpGet("{incidentId}/draftremove/{draftId}")]
        public async Task<IActionResult> RemoveDraftFromIncident(int incidentId, int draftId)
        {
            var currentUser = CurrentUser;
            if (currentUser == null)
                return NotFound();
            var linkToDelete = await _context.IncidentDrafts
                .Include(e => e.Draft).ThenInclude(d => d.Props)
                .Where(l => l.DraftId == draftId && l.IncidentId == incidentId)
                .FirstOrDefaultAsync();
            if (linkToDelete == null)
                return NotFound();
            var draft = linkToDelete.Draft;
            _context.Remove(linkToDelete);
            if (await _context.SaveChangesAsync() > 0)
            {
                draft.Props.Add(new DraftProp
                {
                    DraftId = draft.Id,
                    DateCreate = DateTime.UtcNow,
                    AuthorId = currentUser.Id,
                    Action = "sign",
                    Comment = "Статус сообщения обновлён в связи с исключением из рискового события"
                });
                _context.Update(draft);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return NoContent();
                }
                return BadRequest("Failed to set draft status after link remove");
            }
            return BadRequest("Can not delete link between draft and incident");
        }

        [Authorize(Policy = "RequireRM")]
        [HttpGet("{incidentId}/responsibleadd/{departmentId}")]
        public async Task<IActionResult> AddResponsibleToIncident(int incidentId, int departmentId)
        {
            var responsible = await _context.Responsibles
                .Include(e => e.Incident).ThenInclude(i => i.Props)
                .Where(e => e.IncidentId == incidentId && e.DepartmentId == departmentId)
                .FirstOrDefaultAsync();
            var currentUser = CurrentUser;
            if (responsible != null)
            {
                responsible.Incident.Status = "wait";
                responsible.Props.Add(new ResponsibleProp
                {
                    ResponsibleId = responsible.Id,
                    DateCreate = DateTime.UtcNow,
                    AuthorId = currentUser.Id,
                    Action = "wait",
                    Comment = ""
                });
                _context.Update(responsible);
                if (await _context.SaveChangesAsync() > 0)
                {
                    await _mailService.SendResignMail(responsible, incidentId, "");
                    return Ok(responsible);
                }
            }
            else
            {
                var incident = await _context.Incidents
                    .Include(e => e.Responsibles)
                    .FirstOrDefaultAsync(e => e.Id == incidentId);
                if (incident == null)
                    return NotFound();
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == departmentId);
                if (department == null)
                    return NotFound();
                responsible = new Responsible
                {
                    IncidentId = incident.Id,
                    DepartmentId = department.Id,
                    Result = "wait"
                };
                incident.Responsibles.Add(responsible);
                incident.Status = "wait";
                incident.Props.Add(new IncidentProp
                {
                    IncidentId = incident.Id,
                    DateCreate = DateTime.UtcNow,
                    AuthorId = currentUser.Id,
                    Action = "wait",
                    Comment = "добавлено подразделение " + department.ShortName
                });
                _context.Update(incident);
                if (await _context.SaveChangesAsync() > 0)
                {
                    await _mailService.SendResignMail(responsible, incidentId, "");
                    return Ok(responsible);
                }
            }
            
            return BadRequest("Failed to add responsible department to incident");
        }

        // исключено из работы
        [Authorize(Policy = "RequireRM")]
        [HttpGet("{incidentId}/responsibleremove/{responsibleId}")]
        public async Task<IActionResult> RemoveResponsibleFromIncident(int incidentId, int responsibleId)
        {
            var currentUser = CurrentUser;
            if (currentUser == null)
                return Unauthorized();
            var responsible = await _context.Responsibles
                .Include(r => r.Accounts)
                .Include(r => r.Department)
                .Include(r => r.Props)
                .FirstOrDefaultAsync(r => r.Id == responsibleId);
            if (responsible == null)
                return NotFound();
            if (responsible.Accounts.Count > 0)
                return BadRequest("Can not delete responsible department, because it have resposnible users");
            responsible.Result = "deleted";
            responsible.Props.Add(new ResponsibleProp
            {
                ResponsibleId = responsible.Id,
                DateCreate = DateTime.UtcNow,
                AuthorId = currentUser.Id,
                Action = "подразделение " + responsible.Department.ShortName + "исключено из списка"
            });
            _context.Update(responsible);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Can not delete link between draft and responsible");
        }

        [Authorize(Policy = "RequireRM")]
        [HttpPut("{incidentId}")]
        public async Task<IActionResult> UpdateEvent(IncidentForCreateDto incidentForUpdate, int eventId)
        {
            var currentUser = CurrentUser;
            var incident = await _context.Incidents
                .Include(e => e.Responsibles)
                .Include(e => e.Props)
                .FirstOrDefaultAsync(e => e.Id == eventId);
            if (incident == null)
                return NotFound();
            incident.IncidentTypeId = incidentForUpdate.IncidentTypeId;
            incident.DateIncident = incidentForUpdate.DateIncident;
            incident.Description = incidentForUpdate.Description;
            incident.Props.Add(new IncidentProp
            {
                IncidentId = incident.Id,
                DateCreate = DateTime.UtcNow,
                AuthorId = currentUser.Id,
                Action = "update",
                Comment = ""
            });
            if (await _context.SaveChangesAsync() > 0)
            {
                var eventToReturn = _mapper.Map<IncidentForListDto>(incident);
                return Ok(eventToReturn);
            }
            return BadRequest("Failed to update incident");
        }

        [Authorize(Policy = "RequireRM")]
        [HttpGet("{incidentId}/close")]
        public async Task<IActionResult> CloseIncident(int incidentId)
        {
            var currentUser = CurrentUser;
            var incident = await _context.Incidents
                .Include(e => e.Responsibles).ThenInclude(r => r.Measures).ThenInclude(m => m.Reports)
                .Include(e => e.Drafts).ThenInclude(d => d.Draft).ThenInclude(d => d.Props)
                .Include(e => e.Props)
                .FirstOrDefaultAsync(e => e.Id == incidentId);
            if (incident == null)
                return NotFound();
            incident.Status = "close";
            foreach (var r in incident.Responsibles)
            {
                r.Result = "close";
                foreach (var measure in r.Measures)
                {
                    measure.Status = "close";
                    foreach (var report in measure.Reports)
                    {
                        report.Status = "close";
                    }
                }
            }
            foreach (var draft in incident.Drafts)
            {
                draft.Draft.Status = "close";
                draft.Draft.Props.Add(new DraftProp
                {
                    DraftId = draft.DraftId,
                    Action = "close",
                    AuthorId = currentUser.Id,
                    Comment = "Рисковое событие закрыто",
                    DateCreate = DateTime.UtcNow
                });
            }
            incident.Props.Add(new IncidentProp
            {
                IncidentId = incident.Id,
                Action = "close",
                AuthorId = currentUser.Id,
                Comment = "Рисковое событие закрыто",
                DateCreate = DateTime.UtcNow
            });
            if (await _context.SaveChangesAsync() > 0)
            {
                await _mailService.SendCloseIncidentMail(incident, "");
                var incidentToReturn = _mapper.Map<IncidentForListDto>(incident);
                return Ok(incidentToReturn);
            }
            return BadRequest("Failed to close incident");
        }

        [Authorize(Policy = "RequireCoordinator")]
        [HttpPatch("{incidentId}/status")]
        public async Task<IActionResult> SetResponsibleStatus(StatusHelper statusHelper, int incidentId)
        {
            var currentUser = CurrentUser;
            var responsible = await _context.Responsibles
                .Include(d => d.Props)
                .Where(d => d.IncidentId == incidentId && d.DepartmentId == statusHelper.DepartmentId)
                .FirstOrDefaultAsync();
            if (responsible == null)
                return NotFound();
            responsible.Result = statusHelper.Status;
            responsible.Props.Add(new ResponsibleProp
            {
                ResponsibleId = responsible.Id,
                DateCreate = DateTime.UtcNow,
                AuthorId = statusHelper.AuthorId,
                Action = statusHelper.Status,
                Comment = statusHelper.Comment
            });
            _context.Update(responsible);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (await UpdateIncidentStatus(incidentId, statusHelper) > 0)
                    return NoContent();
            }
            return BadRequest("Failed to set status " + statusHelper.Status);
        }

        [Authorize(Policy = "RequireRM")]
        [HttpPatch("{incidentId}/resignall")]
        public async Task<IActionResult> ResignAll(StatusHelper statusHelper, int incidentId)
        {
            var responsibles = await _context.Responsibles
                .Include(d => d.Props)
                .Where(d => d.IncidentId == incidentId)
                .ToListAsync();
            if (responsibles == null)
                return NotFound();
            foreach (var r in responsibles)
            {
                r.Result = "wait";
                r.Props.Add(new ResponsibleProp
                {
                    ResponsibleId = r.Id,
                    DateCreate = DateTime.UtcNow,
                    AuthorId = statusHelper.AuthorId,
                    Action = "resign",
                    Comment = statusHelper.Comment
                });
            }
            _context.UpdateRange(responsibles);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (await UpdateResignStatus(responsibles, incidentId, statusHelper.Comment) > 0)
                    return NoContent();
            }
            return BadRequest("Failed to set status " + statusHelper.Status);
        }

        [Authorize(Policy = "RequireRM")]
        [HttpPatch("{incidentId}/resign")]
        public async Task<IActionResult> Resign(StatusHelper statusHelper, int incidentId)
        {
            var responsibles = await _context.Responsibles
                .Include(d => d.Props)
                .Where(d => d.IncidentId == incidentId && d.Result.Equals("refine"))
                .ToListAsync();
            if (responsibles == null)
                return NotFound();
            foreach (var r in responsibles)
            {
                r.Result = "wait";
                r.Props.Add(new ResponsibleProp
                {
                    ResponsibleId = r.Id,
                    DateCreate = DateTime.UtcNow,
                    AuthorId = statusHelper.AuthorId,
                    Action = "resign",
                    Comment = statusHelper.Comment
                });
            }
            _context.UpdateRange(responsibles);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (await UpdateResignStatus(responsibles, incidentId, statusHelper.Comment) > 0)
                    return NoContent();
            }
            return BadRequest("Failed to set status " + statusHelper.Status);
        }
        
        [Authorize(Policy = "RequireRM")]
        [HttpPatch("{incidentId}/type")]
        public async Task<IActionResult> UpdateType(StatusHelper statusHelper, int incidentId)
        {
            var incident = await _context.Incidents
                .Include(d => d.Props)
                .Where(d => d.Id == incidentId)
                .FirstOrDefaultAsync();
            if (incident == null)
                return NotFound();
            incident.IncidentTypeId = statusHelper.IncidentTypeId;
            incident.Props.Add(new IncidentProp
            {
                AuthorId = statusHelper.AuthorId,
                DateCreate = DateTime.UtcNow,
                Action = "update",
                Comment = "изменена категория РС"
            });
            _context.Update(incident);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to update incident type");
        }

        [Authorize(Policy = "RequireCoordinator")]
        [HttpPut("responsible/{responsibleId}")]
        public async Task<IActionResult> UpdateResponsibleUsers(int responsibleId, AccountForListDto[] accounts)
        {
            var responsible = await _context.Responsibles
                .Include(r => r.Accounts)
                .FirstOrDefaultAsync(r => r.Id == responsibleId);
            if (responsible == null)
                return NotFound();
            var accountsToAdd = new List<ResponsibleAccount>();
            var accountsToRemove = new List<ResponsibleAccount>();
            foreach (var account in accounts)
            {
                if (!responsible.Accounts.Any(a => a.AccountId == account.Id))
                {
                    accountsToAdd.Add(new ResponsibleAccount
                    {
                        AccountId = account.Id,
                        ResponsibleId = responsible.Id
                    });
                }
            }
            foreach (var responsibleAccount in responsible.Accounts)
            {
                if (!accounts.Any(a => a.Id == responsibleAccount.AccountId))
                {
                    accountsToRemove.Add(responsibleAccount);
                }
            }
            //responsible.Result = "watch";
            await _context.AddRangeAsync(accountsToAdd);
            _context.RemoveRange(accountsToRemove);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (accountsToAdd.Count > 0)
                {
                    await _mailService.SendAssignMail(accountsToAdd, responsible.IncidentId);
                }
                return NoContent();
            }
            return BadRequest("Failed to update responsible accounts");
        }

        [HttpGet("{incidentId}/history")]
        public async Task<IActionResult> GetHistory(int incidentId)
        {
            var incident = await _context.Incidents
                .Include(e => e.Props).ThenInclude(e => e.Author)
                .FirstOrDefaultAsync(e => e.Id == incidentId);
            if (incident == null)
                return NoContent();
            var responsibles = await _context.Responsibles
                .Include(e => e.Props).ThenInclude(e => e.Author)
                .Where(e => e.IncidentId == incidentId)
                .ToListAsync();
            List<LogDto> result = new List<LogDto>();
            foreach (var log in incident.Props)
            {
                result.Add(_mapper.Map<LogDto>(log));
            }
            foreach (var r in responsibles)
            {
                foreach (var log in r.Props)
                {
                    result.Add(_mapper.Map<LogDto>(log));
                }
            }
            result.Sort(delegate (LogDto x, LogDto y)
            {
                if (x.Timestamp == null && y.Timestamp == null) return 0;
                else if (x.Timestamp == null) return -1;
                else if (y.Timestamp == null) return 1;
                else return x.Timestamp.CompareTo(y.Timestamp);
            });
            return Ok(result);
        }

        private ExcelPackage createExcelPackage(List<Incident> result)
        {
            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Отчёт СОРС";
            package.Workbook.Properties.Author = "Drannikov";
            package.Workbook.Properties.Subject = "Отчёт";
            package.Workbook.Properties.Keywords = "Отчёт";


            var worksheet = package.Workbook.Worksheets.Add("report");

            //First add the headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Дата создания";
            worksheet.Cells[1, 3].Value = "Дата события";
            worksheet.Cells[1, 4].Value = "Описание";
            worksheet.Cells[1, 5].Value = "Статус";
            worksheet.Cells[1, 6].Value = "Сообщение";
            worksheet.Cells[1, 7].Value = "Владельцы";
            worksheet.Cells[1, 8].Value = "Категория";
            //worksheet.Cells[1, 8].Value = "Мероприятия";

            //Add values
            //var dataCellStyleName = "TableNumber";
            //var numStyle = package.Workbook.Styles.CreateNamedStyle(dataCellStyleName);
            //numStyle.Style.Numberformat.Format = numberformat;

            var row = 1;
            foreach (var incident in result)
            {
                row++;
                worksheet.Cells[row, 1].Value = incident.Id;
                worksheet.Cells[row, 2].Value = incident.DateCreate;
                worksheet.Cells[row, 3].Value = incident.DateIncident;
                worksheet.Cells[row, 4].Value = incident.Description;
                worksheet.Cells[row, 5].Value = incident.Status;
                var message = "";
                foreach (var draft in incident.Drafts)
                {
                    var draftMessage =
                        "Дата создания: " + draft.Draft.DateCreate.ToShortDateString() + "\r\n" +
                        "Подразделение: " + draft.Draft.Department.Name + "\r\n" +
                        "Автор: " + _userService.GetFullname(draft.Draft.Author.Name) + "\r\n" +
                        "Описание: " + draft.Draft.Description2 + "\r\n";
                    //"Причины: " + draft.Draft.Description3 + ";\r\n" +
                    //"Последствия: " + draft.Draft.Description4 + ";\r\n" +
                    //"Предлагаемые мероприятия: " + draft.Draft.Description5 + "\r\n" +
                    message += draftMessage;
                }
                worksheet.Cells[row, 6].Value = message;
                var owners = "";
                foreach (var owner in incident.Responsibles)
                {
                    owners += owner.Department.Name + "\r\n";
                }
                worksheet.Cells[row, 7].Value = owners;
                if (incident.IncidentType != null)
                {
                    worksheet.Cells[row, 8].Value = incident.IncidentType.Name;
                }
                //worksheet.Cells[row, 8].Value = "здесь будут мероприятия";         
            }

            // Add to table / Add summary row
            var tbl = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: row, toColumn: 8), "Data");
            tbl.ShowHeader = true;
            tbl.TableStyle = TableStyles.Medium9;

            //date formate
            //worksheet.Cells[2, 2, row, 3].Style.Numberformat.Format = "yyyy-mm-dd";
            worksheet.Cells[2, 2, row, 3].Style.Numberformat.Format = "dd.mm.yyyy";

            //worksheet.Cells[1, 1, row, 3].AutoFitColumns();
            worksheet.Row(1).Height = 25;
            worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Column(1).Width = 7;
            worksheet.Column(2).Width = 16;
            worksheet.Column(3).Width = 16;
            worksheet.Column(4).Width = 70;
            worksheet.Column(4).Style.WrapText = true;
            worksheet.Column(5).Width = 10;
            worksheet.Column(6).Width = 70;
            worksheet.Column(6).Style.WrapText = true;
            worksheet.Column(7).Width = 50;
            worksheet.Column(7).Style.WrapText = true;
            worksheet.Column(8).Width = 50;
            worksheet.Column(8).Style.WrapText = true;

            // AutoFitColumns
            //worksheet.Cells[1, 1, row, 8].AutoFitColumns();

            //worksheet.HeaderFooter.OddFooter.InsertPicture(
            //    new FileInfo(Path.Combine(_hostingEnvironment.WebRootPath, "images", "captcha.jpg")),
            //    PictureAlignment.Right);

            return package;
        }

        private ExcelPackage createExcelPackage2(Incident incident)
        {
            var package = new ExcelPackage();
            package.Workbook.Properties.Title = "Отчёт СОРС";
            package.Workbook.Properties.Author = "k.drannikov";
            package.Workbook.Properties.Subject = "Отчёт";
            package.Workbook.Properties.Keywords = "Отчёт";

            var worksheet = package.Workbook.Worksheets.Add("incident");

            //First add the headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[2, 1].Value = incident.Id;
            worksheet.Cells[1, 2].Value = "Дата создания";
            worksheet.Cells[2, 2].Value = incident.DateCreate;
            worksheet.Cells[2, 2].Style.Numberformat.Format = "dd.mm.yyyy";
            worksheet.Cells[1, 3].Value = "Дата события";
            worksheet.Cells[2, 3].Value = incident.DateIncident;
            worksheet.Cells[2, 3].Style.Numberformat.Format = "dd.mm.yyyy";
            worksheet.Cells[1, 4].Value = "Статус";
            worksheet.Cells[2, 4].Value = incident.Status;
            worksheet.Cells[3, 1].Value = "Категория";
            if (incident.IncidentType != null)
            {
                worksheet.Cells[3, 2].Value = incident.IncidentType.Name;
            }
            worksheet.Cells[3, 2, 3, 4].Merge = true;
            worksheet.Cells[4, 1].Value = "Описание";
            worksheet.Cells[4, 1, 4, 4].Merge = true;
            worksheet.Cells[5, 1].Value = incident.Description;
            worksheet.Cells[5, 1, 5, 4].Merge = true;
            worksheet.Row(5).Height = MeasureTextHeight(incident.Description, 120);
            worksheet.Cells[6, 1].Value = "Включённые сообщения";
            worksheet.Cells[6, 1, 6, 4].Merge = true;
            var row = 7;
            foreach (var item in incident.Drafts)
            {
                worksheet.Cells[row, 1].Value = "Дата создания";
                worksheet.Cells[row + 1, 1].Value = item.Draft.DateCreate;
                worksheet.Cells[row + 1, 1].Style.Numberformat.Format = "dd.mm.yyyy";
                worksheet.Cells[row, 2].Value = "Подразделение";
                worksheet.Cells[row, 2, row, 3].Merge = true;
                worksheet.Cells[row + 1, 2].Value = item.Draft.Department.Name;
                worksheet.Cells[row + 1, 2, row + 1, 3].Merge = true;
                worksheet.Cells[row, 4].Value = "Автор";
                var author = _mapper.Map<AccountForListDto>(item.Draft.Author);
                worksheet.Cells[row + 1, 4].Value = author.Fullname;
                row = row + 2;
                worksheet.Cells[row, 1].Value = "Наименование";
                worksheet.Cells[row, 2].Value = item.Draft.Description1;
                worksheet.Cells[row, 2, row, 4].Merge = true;
                worksheet.Row(row).Height = MeasureTextHeight(item.Draft.Description1, 90);
                row++;
                worksheet.Cells[row, 1].Value = "Описание";
                worksheet.Cells[row, 2].Value = item.Draft.Description2;
                worksheet.Cells[row, 2, row, 4].Merge = true;
                worksheet.Row(row).Height = MeasureTextHeight(item.Draft.Description2, 90);
                row++;
                worksheet.Cells[row, 1].Value = "Причины";
                worksheet.Cells[row, 2].Value = item.Draft.Description3;
                worksheet.Cells[row, 2, row, 4].Merge = true;
                worksheet.Row(row).Height = MeasureTextHeight(item.Draft.Description3, 90);
                row++;
                worksheet.Cells[row, 1].Value = "Последствия";
                worksheet.Cells[row, 2].Value = item.Draft.Description4;
                worksheet.Cells[row, 2, row, 4].Merge = true;
                worksheet.Row(row).Height = MeasureTextHeight(item.Draft.Description4, 90);
                row++;
                worksheet.Cells[row, 1].Value = "Предлагаемые мероприятия";
                worksheet.Cells[row, 2].Value = item.Draft.Description5;
                worksheet.Cells[row, 2, row, 4].Merge = true;
                worksheet.Row(row).Height = MeasureTextHeight(item.Draft.Description5, 90);
                row++;
            }
            worksheet.Cells[row, 1].Value = "Владельцы рискового события";
            worksheet.Cells[row, 1, row, 4].Merge = true;
            row++;
            foreach (var resp in incident.Responsibles)
            {
                worksheet.Cells[row, 1].Value = resp.Department.Name;
                worksheet.Cells[row, 1, row, 4].Merge = true;
                row++;
                worksheet.Cells[row, 1].Value = "Ответственные сотрудники";
                worksheet.Cells[row, 1, row, 4].Merge = true;
                var units = "";
                var uCount = 0;
                foreach (var unit in resp.Accounts)
                {
                    uCount++;
                    var account = _mapper.Map<AccountForListDto>(unit.Account);
                    units += account.Fullname;
                    if (uCount < resp.Accounts.Count())
                    {
                        units += "\r\n";
                    }
                }
                row++;
                worksheet.Cells[row, 1].Value = units;
                worksheet.Cells[row, 1, row, 4].Merge = true;
                worksheet.Row(row).Height = MeasureTextHeight(units, 120);
                row++;
                worksheet.Cells[row, 1].Value = "Мероприятия";
                worksheet.Cells[row, 1, row, 2].Merge = true;
                worksheet.Cells[row, 3].Value = "Отчёт о выполнении";
                worksheet.Cells[row, 3, row, 4].Merge = true;
                row++;
                foreach (var mer in resp.Measures)
                {
                    var measure = "Дата создания: " + mer.DateCreate.ToShortDateString() + "\r\n" +
                        "Описание: " + mer.Description + "\r\n" +
                        "Ожидаемый результат: " + mer.ExpectedResult + "\r\n" +
                        "Срок выполнения: " + mer.DeadLine + " " + mer.DeadLineText;
                    worksheet.Cells[row, 1].Value = measure;
                    worksheet.Cells[row, 1, row, 2].Merge = true;
                    worksheet.Row(row).Height = MeasureTextHeight(measure, 60);
                    var results = "";
                    foreach (var rep in mer.Reports)
                    {
                        results += rep.DateCreate.ToShortDateString() + " " + rep.Description + "\r\n";
                    }
                    worksheet.Cells[row, 3].Value = results;
                    worksheet.Cells[row, 3, row, 4].Merge = true;
                    row++;
                }
            }
            worksheet.Cells[row, 1].Value = "История";
            worksheet.Cells[row, 1, row, 4].Merge = true;
            row++;
            worksheet.Cells[row, 1].Value = "Время";
            worksheet.Cells[row, 2].Value = "Событие";
            worksheet.Cells[row, 3].Value = "Комментарий";
            worksheet.Cells[row, 4].Value = "Автор";
            worksheet.Row(row).OutlineLevel = 1;
            worksheet.Row(row).Collapsed = true;
            row++;
            var props = incident.Props.OrderBy(x => x.DateCreate);
            foreach (var prop in props)
            {
                worksheet.Cells[row, 1].Value = prop.DateCreate;
                worksheet.Cells[row, 1].Style.Numberformat.Format = "dd.mm.yyyy hh:mm";
                worksheet.Cells[row, 2].Value = prop.Action;
                worksheet.Cells[row, 3].Value = prop.Comment;
                var account = _mapper.Map<AccountForListDto>(prop.Author);
                worksheet.Cells[row, 4].Value = account.Fullname;
                worksheet.Row(row).OutlineLevel = 1;
                worksheet.Row(row).Collapsed = true;
                row++;
            }

            // Add to table / Add summary row
            //var tbl = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: row, toColumn: 4), "Data");
            //tbl.ShowHeader = true;
            //tbl.TableStyle = TableStyles.Medium9;

            //worksheet.Row(1).Height = 25;
            //worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //worksheet.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.Column(1).Width = 30;
            worksheet.Column(2).Width = 30;
            worksheet.Column(3).Width = 30;
            worksheet.Column(4).Width = 30;
            worksheet.Column(1).Style.WrapText = true;
            worksheet.Column(2).Style.WrapText = true;
            worksheet.Column(3).Style.WrapText = true;
            worksheet.Column(4).Style.WrapText = true;

            worksheet.Cells[1, 1, row, 4].Style.Font.Size = 11;
            worksheet.Cells[1, 1, row, 4].Style.Font.Name = "Times New Roman";
            worksheet.Cells[1, 1, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[1, 1, row, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            return package;
        }

        private double MeasureTextHeight(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) return 15.0;
            var bitmap = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(bitmap);



            var pixelWidth = Convert.ToInt32(width * 7.5);  //7.5 pixels per excel column width
            var drawingFont = new Font("Times New Roman", 13);
            var size = graphics.MeasureString(text, drawingFont, pixelWidth);

            //72 DPI and 96 points per inch.  Excel height in points with max of 409 per Excel requirements.
            var result = Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
            return result;
        }

        private async Task<int> UpdateIncidentStatus(int incidentId, StatusHelper statusHelper)
        {
            var incident = await _context.Incidents
                .Include(e => e.Responsibles)
                .Where(e => e.Id == incidentId)
                .FirstOrDefaultAsync();
            if (incident == null)
                return 0;
            if (incident.Responsibles.Any(e => e.Result.Equals("wait")))
            {
                return 1;
            }
            else if (incident.Responsibles.Any(e => e.Result.Equals("refine")))
            {
                incident.Status = "refine";
                _context.Update(incident);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return await _mailService.SendRefineMail(incident, statusHelper.Comment);
                }
                return -2;
            } 
            else
            {
                incident.Status = "open";
                incident.Props.Add(new IncidentProp
                {
                    IncidentId = incident.Id,
                    DateCreate = DateTime.UtcNow,
                    AuthorId = statusHelper.AuthorId,
                    Action = "open",
                    Comment = ""
                });
                _context.Update(incident);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return await _mailService.SendApprovalMail(incident);
                }
                return -2;
            }
        }

        private async Task<int> UpdateResignStatus(IEnumerable<Responsible> responsibles, int incidentId, string comment)
        {
            var incident = await _context.Incidents
                .Where(e => e.Id == incidentId)
                .FirstOrDefaultAsync();
            if (incident == null)
                return 0;
            incident.Status = "wait";
            _context.Update(incident);
            if (await _context.SaveChangesAsync() > 0)
            {
                return await _mailService.SendResignMail(responsibles, incidentId, comment);
            }
            return -2;
        }

    }
}