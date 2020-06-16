using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class MeasuresController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MeasuresController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet("responsible/{responsibleId}")]
        public async Task<IActionResult> GetResponsibleMeasures([FromQuery]Params parameters, int responsibleId)
        {
            var source = _context.Measures
                .Include(m => m.Reports)
                .Where(m => m.ResponsibleId == responsibleId)
                .AsQueryable();
            switch (parameters.Status)
            {
                case "any":
                    source = source.Where(s => !s.Status.Equals("closed"));
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
                        case "dateCreate":
                            source = source.OrderBy(s => s.DateCreate);
                            break;
                        case "description":
                            source = source.OrderBy(s => s.Description);
                            break;
                        case "expectedResult":
                            source = source.OrderBy(s => s.ExpectedResult);
                            break;
                        case "deadLine":
                            source = source.OrderBy(s => s.DeadLine);
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
                        case "dateCreate":
                            source = source.OrderByDescending(s => s.DateCreate);
                            break;
                        case "description":
                            source = source.OrderByDescending(s => s.Description);
                            break;
                        case "expectedResult":
                            source = source.OrderByDescending(s => s.ExpectedResult);
                            break;
                        case "deadLine":
                            source = source.OrderByDescending(s => s.DeadLine);
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
                        || s.ExpectedResult.ToLower().Contains(f)
                        || s.DeadLineText.ToLower().Contains(f)
                        || s.Status.ToLower().Contains(f)
                        );
            }

            var result = await source.ToListAsync();
            var measures = _mapper.Map<IEnumerable<MeasureForListDto>>(result);
            return Ok(measures);
        }

        [HttpPut("{measureId}")]
        public async Task<IActionResult> UpdateMeasure(int measureId, MeasureForListDto measure)
        {
            var dbMeasure = await _context.Measures
                .FirstOrDefaultAsync(m => m.Id == measureId);
            if (dbMeasure == null)
                return NotFound();
            dbMeasure.Description = measure.Description;
            dbMeasure.ExpectedResult = measure.ExpectedResult;
            dbMeasure.DeadLine = measure.DeadLine;
            dbMeasure.DeadLineText = measure.DeadLineText;
            dbMeasure.Status = "check";
            dbMeasure.Props.Add(new MeasureProp
            {
                Action = "updating",
                AuthorId = measure.AuthorId,
                Comment = measure.Comment,
                DateCreate = DateTime.UtcNow,
                MeasureId = dbMeasure.Id
            });
            if (await _context.SaveChangesAsync() > 0)
            {
                var measureToReturn = _mapper.Map<MeasureForListDto>(dbMeasure);
                return Ok(measureToReturn);
            }
            return BadRequest("Failed to update measure");
        }

        [HttpPost()]
        public async Task<IActionResult> CreateMeasure(MeasureForListDto measure)
        {
            measure.DateCreate = DateTime.UtcNow;
            var measureToCreate = new Measure
            {
                DateCreate = DateTime.UtcNow,
                DeadLine = measure.DeadLine,
                DeadLineText = measure.DeadLineText,
                Description = measure.Description,
                ExpectedResult = measure.ExpectedResult,
                ResponsibleId = measure.ResponsibleId,
                Status = measure.Status
            };
            measureToCreate.Props.Add(new MeasureProp
            {
                Action = "creation",
                AuthorId = measure.AuthorId,
                Comment = measure.Comment,
                DateCreate = DateTime.UtcNow,
                Measure = measureToCreate
            });
            _context.Add(measureToCreate);
            if (await _context.SaveChangesAsync() > 0)
            {
                var measureToReturn = _mapper.Map<MeasureForListDto>(measureToCreate);
                return Ok(measureToReturn);
            }
            return BadRequest("Failed to create new measure");
        }

        [HttpDelete("{measureId}")]
        public async Task<IActionResult> DeleteMeasure(int measureId)
        {
            var measureToDelete = await _context.Measures
                .Include(m => m.Reports).ThenInclude(r => r.Props)
                .Include(m => m.Props)
                .FirstOrDefaultAsync(m => m.Id == measureId);
            if (measureToDelete == null)
                return NotFound();
            if (measureToDelete.Status.Equals("sign"))
            {
                return BadRequest("Not allowed to delete measure with status sign");
            }
            _context.Remove(measureToDelete);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to delete measure");
        }

        [HttpPatch("{measureId}")]
        public async Task<IActionResult> PatchMeasure(int measureId, List<PatchDto> patchDtos)
        {
            var username = User.Identity.Name;
            username = username.Substring(username.IndexOf(@"\") + 1);
            var userFromDb = await _context.Accounts
                .Where(u => u.Name == username)
                .FirstOrDefaultAsync();
            if (userFromDb == null)
            {
                return Unauthorized();
            }
            var measureForPatch = await _context.Measures
                .FirstOrDefaultAsync(m => m.Id == measureId);
            if (measureForPatch == null)
                return NotFound();
            var nameValuePairProperties = patchDtos.ToDictionary(a => a.PropertyName, a => a.PropertyValue);
            var dbEntityEntry = _context.Entry(measureForPatch);
            dbEntityEntry.CurrentValues.SetValues(nameValuePairProperties);
            dbEntityEntry.State = EntityState.Modified;
            measureForPatch.Props.Add(new MeasureProp
            {
                Action = "patching",
                AuthorId = userFromDb.Id,
                DateCreate = DateTime.UtcNow,
                MeasureId = measureForPatch.Id
            });
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest("Failed to update measure");
        }

    }
}