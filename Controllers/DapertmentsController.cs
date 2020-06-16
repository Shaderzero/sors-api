using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sors.Data;
using sors.Data.Dto;
using sors.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public DepartmentsController(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet()]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.Departments
                .Include(d => d.DomainDepartments)
                .OrderBy(r => r.Name)
                .ToListAsync();
            var depsToReturn = _mapper.Map<IEnumerable<DepartmentForListDto>>(departments);
            return Ok(depsToReturn);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateDepartment(DepartmentForCreateDto departmentForCreateDto)
        {
            var departmentToCreate = _mapper.Map<Department>(departmentForCreateDto);
            _context.Add(departmentToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(departmentToCreate);
            }
            return BadRequest("Creating the department failed on save");
        }

        [HttpGet("{departmentId}")]
        public async Task<IActionResult> GetDepartment(int departmentId)
        {
            var department = await _context.Departments
                .Include(d => d.DomainDepartments)
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            if (department == null)
                return NotFound();
            var depToReturn = _mapper.Map<DepartmentForListDto>(department);
            return Ok(depToReturn);
        }

        [HttpGet("{departmentId}/accounts")]
        public async Task<IActionResult> GetDepartmentAccounts(int departmentId)
        {
            var accounts = await _context.Accounts
                .Include(a => a.AccountRoles).ThenInclude(r => r.Role)
                .Where(a => a.DepartmentId == departmentId)
                .ToListAsync();
            var accountsToReturn = _mapper.Map<IEnumerable<AccountForListDto>>(accounts);
            return Ok(accountsToReturn);
        }


        [HttpDelete("{departmentId}")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            var departmentForDelete = await _context.Departments
                .Include(d => d.DomainDepartments)
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            if (departmentForDelete == null)
                return NotFound();
            _context.Remove(departmentForDelete);
            if (await _context.SaveChangesAsync() > 0)
                return NoContent();
            return BadRequest("Failed to delete the department");
        }

        [HttpPut("{departmentId}")]
        public async Task<IActionResult> UpdateDepartment(int departmentId, DepartmentForCreateDto departmentForUpdateDto)
        {
            var departmentToUpdate = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            if (departmentToUpdate == null)
                return NotFound();
            departmentToUpdate.Code = departmentForUpdateDto.Code;
            departmentToUpdate.Name = departmentForUpdateDto.Name;
            departmentToUpdate.ShortName = departmentForUpdateDto.ShortName;
            _context.Update(departmentToUpdate);
            if (await _context.SaveChangesAsync() > 0)
                return Ok(departmentToUpdate);
            return BadRequest("Failed to update the department");
        }

        [HttpPatch("{departmentId}")]
        public async Task<IActionResult> PatchDepartment(int departmentId, List<PatchDto> patchDtos)
        {
            var departmentForPatch = await _context.Departments
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
            return BadRequest("Failed to patch department");
        }

    }
}
