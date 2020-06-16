using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sors.Authentication;
using sors.Data;
using sors.Data.Dto;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(DataContext context, IMapper mapper, IConfiguration configuration, IUserService userService) {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AuthenticateUser()
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
                //var userFromDb = _userService.GetUserByName(username);
                if (userFromDb != null)
                {
                    var userToReturn = _mapper.Map<AccountForListDto>(userFromDb);
                    var newLog = new Data.Entities.Log
                    {
                        AccountId = userFromDb.Id,
                        Action = "login",
                        Timestamp = DateTime.Now
                    };
                    _context.Add(newLog);
                    _context.SaveChanges();
                    return Ok(userToReturn);
                }
                else
                {
                    return Unauthorized("Пользователь не найден в базе дынных СОРС");
                }
            }
            else
            {
                return Unauthorized("Ошибка Windows Authentication");
            }
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.Departments
                .Include(d => d.DomainDepartments)
                .ToListAsync();
            var departmentsToReturn = _mapper.Map<List<DepartmentForListDto>>(departments);
            return Ok(departmentsToReturn);
        }
        
        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var dbAccounts = await _context.Accounts
                    .Include(a => a.AccountRoles).ThenInclude(r => r.Role)
                    .Include(d => d.Department).ThenInclude(d => d.DomainDepartments)
                    .ToListAsync();
            var toReturn = _mapper.Map<IEnumerable<AccountForListDto>>(dbAccounts);
            return Ok(toReturn);
        }

        [HttpGet("user")]
        [Authorize]
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