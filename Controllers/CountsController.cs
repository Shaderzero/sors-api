using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sors.Data;
using sors.Data.Dto;
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
    public class CountsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CountsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCounts([FromQuery]Params parameters)
        {
            if (User == null)
                return NoContent();
            var username = User.Identity.Name;
            username = username.Substring(username.IndexOf(@"\") + 1);
            var user = await _context.Accounts
                .Include(a => a.AccountRoles).ThenInclude(ar => ar.Role)
                .Include(a => a.Department)
                .FirstOrDefaultAsync(e => e.Name.Equals(username));
            if (user == null)
                return null;
            var isRC = user.AccountRoles.Where(r => r.Role.Name == "riskCoordinator").Any();
            var isRM = user.AccountRoles.Where(r => r.Role.Name == "riskManager").Any();
            var isAdmin = user.AccountRoles.Where(r => r.Role.Name == "admin").Any();
            int draftCount = 0;
            int rcCount = 0;
            int rmCount = 0;
            int openCount = 0;
            int refineCount = 0;
            int closeCount = 0;
            int waitIncidentCount = 0;
            int refineIncidentCount = 0;
            int openIncidentCount = 0;
            if (isAdmin)
            {
                draftCount = await _context.Drafts
                .Where(d => d.Status == "draft")
                .CountAsync();
            } else {
                draftCount = await _context.Drafts
                    .Where(d => d.AuthorId == user.Id && d.Status == "draft")
                    .CountAsync();
            }
            if (isRM || isAdmin)
            {
                rcCount = await _context.Drafts
                    .Where(d => d.Status == "check")
                    .CountAsync();
                rmCount = await _context.Drafts
                    .Where(d => d.Status == "sign")
                    .CountAsync();
                openCount = await _context.Drafts
                    .Where(d => d.Status == "open")
                    .CountAsync();
                refineCount = await _context.Drafts
                    .Where(d => d.Status == "refine")
                    .CountAsync();
                closeCount = await _context.Drafts
                    .Where(d => d.Status == "close")
                    .CountAsync();
                waitIncidentCount = await _context.Incidents
                    .Where(e => e.Status.Equals("wait"))
                    .CountAsync();
                refineIncidentCount = await _context.Incidents
                    .Where(e => e.Status.Equals("refine"))
                    .CountAsync();
                openIncidentCount = await _context.Incidents
                    .Where(e => e.Status.Equals("open"))
                    .CountAsync();
            }
            else if (isRC)
            {
                rcCount = await _context.Drafts
                    .Where(d => d.DepartmentId == user.DepartmentId && d.Status == "check")
                    .CountAsync();
                rmCount = await _context.Drafts
                    .Where(d => d.DepartmentId == user.DepartmentId && d.Status == "sign")
                    .CountAsync();
                openCount = await _context.Drafts
                    .Where(d => d.DepartmentId == user.DepartmentId && d.Status == "open")
                    .CountAsync();
                refineCount = await _context.Drafts
                    .Where(d => d.DepartmentId == user.DepartmentId && d.Status == "refine")
                    .CountAsync();
                closeCount = await _context.Drafts
                    .Where(d => d.DepartmentId == user.DepartmentId && d.Status == "close")
                    .CountAsync();
                waitIncidentCount = await _context.Incidents
                    .Where(e => _context.Responsibles.Any(r => r.DepartmentId == user.DepartmentId && r.Result.Equals("wait") && r.IncidentId == e.Id))
                    .CountAsync();
                openIncidentCount = await _context.Incidents
                    .Where(e => e.Status.Equals("open"))
                    .Where(e => _context.Responsibles.Any(r => r.IncidentId == e.Id && r.DepartmentId == user.DepartmentId))
                    .CountAsync();
            }
            else
            {
                rcCount = await _context.Drafts
                    .Where(d => d.AuthorId == user.Id && d.Status == "check")
                    .CountAsync();
                rmCount = await _context.Drafts
                    .Where(d => d.AuthorId == user.Id && d.Status == "sign")
                    .CountAsync();
                openCount = await _context.Drafts
                    .Where(d => d.AuthorId == user.Id && d.Status == "open")
                    .CountAsync();
                refineCount = await _context.Drafts
                    .Where(d => d.AuthorId == user.Id && d.Status == "refine")
                    .CountAsync();
                closeCount = await _context.Drafts
                    .Where(d => d.AuthorId == user.Id && d.Status == "close")
                    .CountAsync();
                openIncidentCount = await _context.Incidents
                    .Where(e => e.Status.Equals("open"))
                    .Where(e => _context.ResponsibleAccounts.Any(r => r.Responsible.IncidentId == e.Id && r.AccountId == user.Id))
                    .CountAsync();
            }
            Counters counters = new Counters()
            {
                draftCount = draftCount,
                rmCount = rmCount,
                rcCount = rcCount,
                refineCount = refineCount,
                openCount = openCount,
                closeCount = closeCount,
                waitIncidentCount = waitIncidentCount,
                refineIncidentCount = refineIncidentCount,
                openIncidentCount = openIncidentCount
            };
            return Ok(counters);
        }

        public async Task<AccountForListDto> GetCurrentUser()
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
