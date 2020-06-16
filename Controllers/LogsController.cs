using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sors.Authentication;
using sors.Data;
using sors.Data.Dto;
using sors.Data.Entities;
using sors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireSecurity")]
    public class LogsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public LogsController(DataContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetLogs([FromQuery]Params parameters)
        {
            var source = _context.Logs
               .Include(l => l.Account).ThenInclude(a => a.Department)
               .Include(l => l.Account).ThenInclude(a => a.AccountRoles).ThenInclude(r => r.Role)
               .AsQueryable();
            if (!string.IsNullOrEmpty(parameters.Order))
            {
                if (parameters.OrderAsc)
                {
                    switch (parameters.Order)
                    {
                        case "date":
                            source = source.OrderBy(s => s.Timestamp);
                            break;
                        case "name":
                            source = source.OrderBy(s => s.Account.Name);
                            break;
                        //case "fullname":
                        //    source = source.OrderBy(s => s.Account.Fullname);
                        //    break;
                        case "department":
                            source = source.OrderBy(s => s.Account.Department.Name);
                            break;
                    }
                }
                else
                {
                    switch (parameters.Order)
                    {
                        case "date":
                            source = source.OrderByDescending(s => s.Timestamp);
                            break;
                        case "name":
                            source = source.OrderByDescending(s => s.Account.Name);
                            break;
                        //case "fullname":
                        //    source = source.OrderByDescending(s => s.Account.Fullname);
                        //    break;
                        case "department":
                            source = source.OrderByDescending(s => s.Account.Department.Name);
                            break;
                    }
                }
            }
            else
            {
                source = source.OrderBy(s => s.Timestamp);
            }
            if (!string.IsNullOrEmpty(parameters.Filter))
            {
                var f = parameters.Filter.ToLower();
                source = source.Where(s => s.Account.Name.ToLower().Contains(f)
                        || s.Account.Department.Name.ToLower().Contains(f)
                        );
            }
            var result = await PagedList<Log>.CreateAsync(source, parameters.PageNumber, parameters.PageSize);
            var logs = _mapper.Map<IEnumerable<LogDto>>(result);
            foreach (var log in logs)
            {
                log.Account.Fullname = _userService.GetFullname(log.Account.Name);
            }
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(logs);
        }
    }
}
