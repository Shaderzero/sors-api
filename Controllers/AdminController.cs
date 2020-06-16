using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sors.Authentication;
using sors.Data;
using sors.Data.Dto;
using sors.Data.Entities;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using sors.Helpers;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AdminController(DataContext context, IMapper mapper, IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
        }


        [HttpGet("domaindepartments")]
        public async Task<IActionResult> GetDepartmentADs([FromQuery]Params parameters)
        {
            if (!(parameters.AccountId > 0))
            {
                var allDepartments = await _context.DomainDepartments
                    .Include(d => d.Department)
                    .ToListAsync();
                var toReturn = _mapper.Map<IEnumerable<DomainDepartmentForListDto>>(allDepartments);
                return Ok(toReturn);
            }
            var source = _context.DomainDepartments
                .Include(d => d.Department)
                .AsQueryable();
            if (!string.IsNullOrEmpty(parameters.Order))
            {
                if (parameters.OrderAsc)
                {
                    switch (parameters.Order)
                    {
                        case "adName":
                            source = source.OrderBy(s => s.Name);
                            break;
                        case "dbName":
                            source = source.OrderBy(s => s.Department.Name);
                            break;
                    }
                }
                else
                {
                    switch (parameters.Order)
                    {
                        case "domainName":
                            source = source.OrderByDescending(s => s.Name);
                            break;
                        case "dbName":
                            source = source.OrderByDescending(s => s.Department.Name);
                            break;
                    }
                }
            }
            else
            {
                source = source.OrderBy(s => s.Name);
            }
            if (!string.IsNullOrEmpty(parameters.Filter))
            {
                var f = parameters.Filter.ToLower();
                source = source.Where(s => s.Name.ToLower().Contains(f)
                        || s.Department.Name.ToLower().Contains(f)
                        );
            }
            var result = await PagedList<DomainDepartment>.CreateAsync(source, parameters.PageNumber, parameters.PageSize);
            var departments = _mapper.Map<IEnumerable<DomainDepartmentForListDto>>(result);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(departments);
        }

        [HttpPost("domaindepartments")]
        public async Task<IActionResult> CreateDepartmentAD(DomainDepartmentForCreateDto departmentForCreate)
        {
            var depToCreate = new DomainDepartment
            {
                Name = departmentForCreate.Name,
                DepartmentId = departmentForCreate.Department.Id
            };
            _context.Add(depToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                var createdDep = await _context.DomainDepartments
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.Id == depToCreate.Id);
                var depToReturn = _mapper.Map<DomainDepartmentForListDto>(createdDep);
                return Ok(depToReturn);
            }
            return BadRequest("Creating the domain department failed on save");
        }

        [HttpGet("domaindepartments/{departmentId}")]
        public async Task<IActionResult> GetDomainDepartment(int departmentId)
        {
            var department = await _context.DomainDepartments
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            if (department == null)
                return NotFound();
            return Ok(department);
        }

        [HttpDelete("domaindepartments/{departmentId}")]
        public async Task<IActionResult> DeleteDomainDepartment(int departmentId)
        {
            var departmentForDelete = await _context.DomainDepartments
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            if (departmentForDelete == null)
                return NotFound();
            _context.Remove(departmentForDelete);
            if (await _context.SaveChangesAsync() > 0)
                return NoContent();
            return BadRequest("Failed to delete the department AD");
        }

        [HttpPut("domaindepartments/{departmentId}")]
        public async Task<IActionResult> UpdateDomainDepartment(int departmentId, DomainDepartmentForListDto departmentForUpdate)
        {
            var departmentToUpdate = await _context.DomainDepartments
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            if (departmentToUpdate == null)
                return NotFound();
            departmentToUpdate.Name = departmentForUpdate.Name;
            departmentToUpdate.DepartmentId = departmentForUpdate.Id;
            _context.Update(departmentToUpdate);
            if (await _context.SaveChangesAsync() > 0)
                return Ok(departmentToUpdate);
            return BadRequest("Failed to update the domain department");
        }

        [HttpPatch("domaindepartments/{departmentId}")]
        public async Task<IActionResult> PatchDepartmentAD(int departmentId, List<PatchDto> patchDtos)
        {
            var departmentForPatch = await _context.DomainDepartments
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            if (departmentForPatch == null)
                return NotFound();
            var nameValuePairProperties = patchDtos.ToDictionary(a => a.PropertyName, a => a.PropertyValue);
            var dbEntityEntry = _context.Entry(departmentForPatch);
            dbEntityEntry.CurrentValues.SetValues(nameValuePairProperties);
            dbEntityEntry.State = EntityState.Modified;
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to patch domain department");
        }

        [HttpGet("accounts/roles")]
        public async Task<IActionResult> GetAccountRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            var rolesToReturn = _mapper.Map<List<RoleForListDto>>(roles);
            return Ok(rolesToReturn);
        }

        [HttpGet("domainusers")]
        public IActionResult GetDomainUsers()
        {
            //var myDomainUsers = new List<DomainUser>();
            //using (var ctx = new PrincipalContext(ContextType.Domain, _configuration["Domain"]))
            //{
            //    var userPrincipal = new UserPrincipal(ctx);
            //    userPrincipal.Enabled = true;
            //    using (var search = new PrincipalSearcher(userPrincipal))
            //    {
            //        foreach (UserPrincipal domainUser in search.FindAll())
            //        {
            //            if (domainUser.Name != null)
            //            {
            //                    DirectoryEntry dirEntry = (DirectoryEntry)domainUser.GetUnderlyingObject();
            //                    if (dirEntry.Properties["Department"].Value != null)
            //                    {
            //                        string eMail = domainUser.EmailAddress;
            //                        string userDepartment = dirEntry.Properties["Department"].Value.ToString();
            //                        DomainUser dUser = new DomainUser
            //                        {
            //                            Name = domainUser.SamAccountName,
            //                            Fullname = domainUser.DisplayName,
            //                            DomainDepartment = userDepartment,
            //                            Email = eMail
            //                        };
            //                        myDomainUsers.Add(dUser);
            //                    }
            //            }
            //        }
            //    }
            //}
            var myDomainUsers = _userService.GetDomainUsers();
            return Ok(myDomainUsers);
        }

        [HttpGet("domainusersOLD")]
        public IActionResult GetDomainUsersOLD()
        {
            var myDomainUsers = new List<DomainUser>();
            using (var ctx = new PrincipalContext(ContextType.Domain, _configuration["Domain"]))
            {
                var userPrinciple = new UserPrincipal(ctx);
                using (var search = new PrincipalSearcher(userPrinciple))
                {
                    foreach (var domainUser in search.FindAll())
                    {
                        if (domainUser.Name != null)
                        {
                            UserPrincipal userPrincipal = (UserPrincipal)domainUser;
                            if (!userPrincipal.IsAccountLockedOut())
                            {
                                DirectoryEntry dirEntry = (DirectoryEntry)domainUser.GetUnderlyingObject();
                                if (dirEntry.Properties["Department"].Value != null)
                                {
                                    string eMail = userPrincipal.EmailAddress;
                                    string userDepartment = dirEntry.Properties["Department"].Value.ToString();
                                    DomainUser dUser = new DomainUser
                                    {
                                        Name = domainUser.SamAccountName,
                                        Fullname = domainUser.DisplayName,
                                        DomainDepartment = userDepartment,
                                        Email = eMail
                                    };
                                    myDomainUsers.Add(dUser);
                                }
                            }
                        }
                    }
                }
            }
            return Ok(myDomainUsers);
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetDbUsers([FromQuery]Params parameters)
        {
            var allAccounts = await _context.Accounts
                    .Include(a => a.AccountRoles).ThenInclude(r => r.Role)
                    .Include(d => d.Department).ThenInclude(d => d.DomainDepartments)
                    .ToListAsync();
            var source = _mapper.Map<IEnumerable<AccountForListDto>>(allAccounts);
            if (!(parameters.AccountId > 0))
            {
                return Ok(source);
            }
            if (!string.IsNullOrEmpty(parameters.Order))
            {
                if (parameters.OrderAsc)
                {
                    switch (parameters.Order)
                    {
                        case "name":
                            source = source.OrderBy(s => s.Name);
                            break;
                        case "fullname":
                            source = source.OrderBy(s => s.Fullname);
                            break;
                        case "department":
                            source = source.OrderBy(s => s.Department.Name);
                            break;
                    }
                }
                else
                {
                    switch (parameters.Order)
                    {
                        case "name":
                            source = source.OrderByDescending(s => s.Name);
                            break;
                        case "fullname":
                            source = source.OrderByDescending(s => s.Fullname);
                            break;
                        case "department":
                            source = source.OrderByDescending(s => s.Department.Name);
                            break;
                    }
                }
            }
            else
            {
                source = source.OrderBy(s => s.Name);
            }
            if (!string.IsNullOrEmpty(parameters.Filter))
            {
                var f = parameters.Filter.ToLower();
                source = source.Where(s => s.Name.ToLower().Contains(f)
                                           || s.Fullname.ToLower().Contains(f)
                                           || s.Department.Name.ToLower().Contains(f)
                                           || s.AccountRoles.Any(r => r.Name.ToLower().Contains(f))
                );
            }
            var result = PagedList<AccountForListDto>.Create(source, parameters.PageNumber, parameters.PageSize);
            //var accounts = _mapper.Map<IEnumerable<AccountForListDto>>(result);
            Response.AddPagination(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpPost("accounts")]
        public async Task<IActionResult> CreateUser(AccountForCreateDto userForCreate)
        {
            var userToCreate = new Account
            {
                Name = userForCreate.Name,
                //Fullname = userForCreate.Fullname,
                //Email = userForCreate.Email,
                DepartmentId = userForCreate.Department.Id
            };
            foreach (var role in userForCreate.AccountRoles)
            {
                userToCreate.AccountRoles.Add(new AccountRole
                {
                    Account = userToCreate,
                    RoleId = role.Id
                });
            }
            _context.Add(userToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                var createdUser = _mapper.Map<AccountForListDto>(userToCreate);
                return Ok(createdUser);
            }
            return BadRequest("Creating user failed on save");
        }

        [HttpPost("accounts/array")]
        public async Task<IActionResult> CreateUsers(AccountForCreateDto[] usersForCreate)
        {
            foreach (var userForCreate in usersForCreate)
            {
                var userToCreate = new Account
                {
                    Name = userForCreate.Name,
                    DepartmentId = userForCreate.Department.Id
                };
                var defaultRole = await _context.Roles
                    .Where(r => r.Name.Equals("user"))
                    .FirstOrDefaultAsync();
                userToCreate.AccountRoles.Add(new AccountRole
                {
                    Account = userToCreate,
                    RoleId = defaultRole.Id
                });
                _context.Add(userToCreate);
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Creating users failed on save");
        }


        [HttpGet("accounts/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var userFromDb = await _context.Accounts
                    .Include(a => a.AccountRoles).ThenInclude(r => r.Role)
                    .Include(d => d.Department).ThenInclude(d => d.DomainDepartments)
                    .FirstOrDefaultAsync(u => u.Id == userId);
            if (userFromDb == null)
                return NotFound();
            var toReturn = _mapper.Map<AccountForListDto>(userFromDb);
            return Ok(toReturn);
        }

        [HttpDelete("accounts/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var userForDelete = await _context.Accounts
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (userForDelete == null)
                return NotFound();
            _context.Remove(userForDelete);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to delete user");
        }

        [HttpPut("accounts/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, AccountForCreateDto userForUpdate)
        {
            var userToUpdate = await _context.Accounts
                .Include(u => u.AccountRoles)
                .FirstOrDefaultAsync(d => d.Id == userId);
            if (userToUpdate == null)
                return NotFound();
            userToUpdate.Name = userForUpdate.Name;
            userToUpdate.DepartmentId = userForUpdate.Department.Id;
            // MUST HAVE UPDATE METHOD!
            List<AccountRole> userRolesToBeDeleted = userToUpdate.AccountRoles
                .Where(c1 => userForUpdate.AccountRoles.All(c2 => c2.Id != c1.RoleId)).ToList();
            foreach (AccountRole role in userRolesToBeDeleted)
            {
                userToUpdate.AccountRoles.Remove(role);
            }
            List<RoleForListDto> userRolesToBeAdded = userForUpdate.AccountRoles
                .Where(c1 => userToUpdate.AccountRoles.All(c2 => c2.RoleId != c1.Id)).ToList();
            foreach (RoleForListDto role in userRolesToBeAdded)
            {
                userToUpdate.AccountRoles.Add(new AccountRole
                {
                    RoleId = role.Id,
                    AccountId = userToUpdate.Id
                });
            }
            _context.Update(userToUpdate);
            if (await _context.SaveChangesAsync() > 0)
            {
                var updatedUserFromDb = await _context.Accounts
                    .Include(u => u.AccountRoles).ThenInclude(r => r.Role)
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Id == userToUpdate.Id);
                var updatedUser = _mapper.Map<AccountForListDto>(updatedUserFromDb);
                return Ok(updatedUser);
            }
            return BadRequest("Failed to update user");
        }

        [HttpPut("accounts/array")]
        public async Task<IActionResult> UpdateUsers(AccountForListDto[] usersForUpdate)
        {
            var usersToUpdate = new List<Account>();
            foreach (var userForUpdate in usersForUpdate)
            {
                var userToUpdate = await _context.Accounts
                    .FirstOrDefaultAsync(u => u.Id == userForUpdate.Id);
                userToUpdate.DepartmentId = userForUpdate.Department.Id;
                usersToUpdate.Add(userToUpdate);
            }
            _context.UpdateRange(usersToUpdate);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
                
            return BadRequest("Failed to update user");
        }

        [HttpPatch("accounts/{userId}")]
        public async Task<IActionResult> PatchUser(int userId, List<PatchDto> patchDtos)
        {
            var userForPatch = await _context.Accounts
                .FirstOrDefaultAsync(d => d.Id == userId);
            if (userForPatch == null)
                return NotFound();
            var nameValuePairProperties = patchDtos.ToDictionary(a => a.PropertyName, a => a.PropertyValue);
            var dbEntityEntry = _context.Entry(userForPatch);
            dbEntityEntry.CurrentValues.SetValues(nameValuePairProperties);
            dbEntityEntry.State = EntityState.Modified;
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to patch user");
        }
    }
}