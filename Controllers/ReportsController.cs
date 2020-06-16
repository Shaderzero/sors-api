using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sors.Data;
using sors.Data.Dto;
using sors.Data.Dto.Incidents;
using sors.Data.Entities;
using sors.Data.Entities.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReportsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private async Task<Account> GetCurrentAccount()
        {
            var username = User.Identity.Name;
            username = username.Substring(username.IndexOf(@"\") + 1);
            var userFromDb = await _context.Accounts.FirstOrDefaultAsync(u => u.Name == username);
            return userFromDb;
        }

        [HttpGet("measure/{measureId}")]
        public async Task<IActionResult> GetMeasureReports(int measureId)
        {
            var measureFromDb = await _context.Measures
                .Include(m => m.Reports)
                .FirstOrDefaultAsync(m => m.Id == measureId);
            if (measureFromDb == null)
                return NotFound();
            var reports = _mapper.Map<IEnumerable<ReportForListDto>>(measureFromDb.Reports);
            return Ok(reports);

        }

        [HttpPut("{reportId}")]
        public async Task<IActionResult> UpdateReport(int reportId, ReportForCreateDto report)
        {
            var dbReport = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == reportId);
            if (dbReport == null)
                return NotFound();
            dbReport.Description = report.Description;
            dbReport.Status = "check";
            dbReport.Props.Add(new ReportProp
            {
                Action = "updating",
                AuthorId = report.AuthorId,
                DateCreate = DateTime.UtcNow,
                ReportId = dbReport.Id
            });
            if (await _context.SaveChangesAsync() > 0)
            {
                var reportToReturn = _mapper.Map<ReportForListDto>(dbReport);
                return Ok(reportToReturn);
            }
            return BadRequest("Failed to update report");
        }

        [HttpPost()]
        public async Task<IActionResult> CreateReport(ReportForCreateDto report)
        {
            var reportToCreate = new Report
            {
                MeasureId = report.MeasureId,
                DateCreate = DateTime.UtcNow,
                Description = report.Description,
                Status = report.Status
            };
            reportToCreate.Props.Add(new ReportProp
            {
                Action = "creation",
                AuthorId = report.AuthorId,
                DateCreate = DateTime.UtcNow,
                Report = reportToCreate
            });
            _context.Add(reportToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                var reportToReturn = _mapper.Map<ReportForListDto>(reportToCreate);
                return Ok(reportToReturn);
            }
            return BadRequest("Failed to create new report");
        }

        [HttpDelete("{reportId}")]
        public async Task<IActionResult> DeleteReport(int reportId)
        {
            var reportToDelete = await _context.Reports
                .Include(m => m.Props)
                .FirstOrDefaultAsync(m => m.Id == reportId);
            if (reportToDelete == null)
                return NotFound();
            _context.Remove(reportToDelete);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to delete report");
        }

        [HttpPatch("{reportId}")]
        public async Task<IActionResult> PatchReport(int reportId, List<PatchDto> patchDtos)
        {
            var reportForPatch = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == reportId);
            if (reportForPatch == null)
                return NotFound();
            var nameValuePairProperties = patchDtos.ToDictionary(a => a.PropertyName, a => a.PropertyValue);
            var dbEntityEntry = _context.Entry(reportForPatch);
            dbEntityEntry.CurrentValues.SetValues(nameValuePairProperties);
            dbEntityEntry.State = EntityState.Modified;
            var account = await GetCurrentAccount();
            reportForPatch.Props.Add(new ReportProp
            {
                Action = "patching",
                AuthorId = account.Id,
                DateCreate = DateTime.UtcNow,
                ReportId = reportForPatch.Id
            });
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to update report");
        }

    }
}