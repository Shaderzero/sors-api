using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sors.Data;
using sors.Data.Dto.References;
using sors.Data.Entities;
using sors.Data.Entities.Passports;
using sors.Data.Entities.References;

namespace sors.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReferencesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ReferencesController(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("incidenttypes")]
        public async Task<IActionResult> GetIncidentTypes()
        {
            var entities = await _context.IncidentTypes
                .OrderBy(p => p.Name)
                .ToListAsync();
            return Ok(entities);
        }

        [HttpGet("incidenttypes/{id}", Name = "GetIncidentType")]
        public async Task<IActionResult> GetIncidentType(int id)
        {
            var entity = await _context.IncidentTypes
                .FirstOrDefaultAsync(b => b.Id == id);
            if (entity == null)
                return NotFound();
            return Ok(entity);
        }

        [HttpPut("incidenttypes/{id}")]
        public async Task<IActionResult> UpdateIncidentType(int id, IncidentType entityForUpdate)
        {
            var entity = await _context.IncidentTypes
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (entity == null)
                return NotFound();
            entity.Name = entityForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetIncidentType",
                    new { controller = "References", id = entity.Id }, entity);
            }
            return BadRequest("Error update Incident Type");
        }

        [HttpPost("incidenttypes")]
        public async Task<IActionResult> CreateIncidentType(IncidentTypeForCreateDto entityForCreate)
        {
            IncidentType newEntity = _mapper.Map<IncidentType>(entityForCreate);
            _context.Add(newEntity);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newEntity);
            }
            return BadRequest("Error create Incident Type");
        }

        [HttpDelete("incidenttypes/{id}")]
        public async Task<IActionResult> DeleteIncidentType(int id)
        {
            var entity = await _context.IncidentTypes
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (entity == null)
                return NotFound();
            _context.Remove(entity);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Incident Type id {id}");
        }

        [HttpGet("businesstypes")]
        public async Task<IActionResult> GetBusinessTypes()
        {
            var bTypes = await _context.BusinessTypes
                .OrderBy(p => p.Code)
                .ToListAsync();       
            return Ok(bTypes);
        }

        [HttpGet("businesstypes/{id}", Name="GetBusinessType")]
        public async Task<IActionResult> GetBusinessType(int id)
        {
            var bType = await _context.BusinessTypes
                .FirstOrDefaultAsync(b => b.Id == id);
            if (bType == null)
                return NotFound();
            return Ok(bType);
        }

        [HttpPut("businesstypes/{id}")]
        public async Task<IActionResult> UpdateBusinessType(int id, BusinessType btForUpdate)
        {
            var bType = await _context.BusinessTypes
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (bType == null)
                return NotFound();
            bType.Code = btForUpdate.Code;
            bType.Name = btForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetBusinessType",
                    new { controller = "References", btId = bType.Id }, bType);
            }
            return BadRequest("Error update Business Type");
        }

        [HttpPost("businesstypes")]
        public async Task<IActionResult> CreateBusinessType(BusinessTypeForCreateDto btForCreate)
        {
            BusinessType newBt = _mapper.Map<BusinessType>(btForCreate);
            _context.Add(newBt);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newBt);
            }
            return BadRequest("Error create Business Type");
        }

        [HttpDelete("businesstypes/{id}")]
        public async Task<IActionResult> DeleteBusinessType(int id)
        {
            var bType = await _context.BusinessTypes
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (bType == null)
                return NotFound();
            _context.Remove(bType);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Business Type id {id}");
        }

        [HttpGet("areas")]
        public async Task<IActionResult> GetAreas()
        {
            var areas = await _context.RiskAreas
                .OrderBy(a => a.Code)
                .ToListAsync();
            return Ok(areas);
        }

        [HttpGet("areas/{id}", Name="GetArea")]
        public async Task<IActionResult> GetArea(int id)
        {
            var area = await _context.RiskAreas
                .FirstOrDefaultAsync(a => a.Id == id);
            if (area == null)
                return NotFound();
            return Ok(area);
        }

        [HttpPut("areas/{id}")]
        public async Task<IActionResult> UpdateArea(int id, RiskArea areaForUpdate)
        {
            var area = await _context.RiskAreas
                .FirstOrDefaultAsync(a => a.Id == id);
            if (area == null)
                return NotFound();
            area.Code = areaForUpdate.Code;
            area.Name = areaForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetArea",
                    new { controller = "References", id = area.Id }, area);
            }
            return BadRequest("Error update Risk Area");
        }

        [HttpPost("areas")]
        public async Task<IActionResult> CreateArea(RiskAreaForCreateDto areaForCreate)
        {
            RiskArea newArea = _mapper.Map<RiskArea>(areaForCreate);
            _context.Add(newArea);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newArea);
            }
            return BadRequest("Error create Risk Area");
        }

        [HttpDelete("areas/{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var area = await _context.RiskAreas
                .FirstOrDefaultAsync(a => a.Id == id);
            if (area == null)
                return NotFound();
            _context.Remove(area);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Risk Area id {id}");
        }

        [HttpGet("durations")]
        public async Task<IActionResult> GetDurations()
        {
            var durations = await _context.RiskDurations
                .OrderBy(d => d.Code)
                .ToListAsync();       
            return Ok(durations);
        }

        [HttpGet("durations/{id}", Name="GetDuration")]
        public async Task<IActionResult> GetDuration(int id)
        {
            var duration = await _context.RiskDurations
                .FirstOrDefaultAsync(d => d.Id == id);
            if (duration == null)
                return NotFound();
            return Ok(duration);
        }

        [HttpPut("durations/{id}")]
        public async Task<IActionResult> UpdateDuration(int id, RiskDuration durationForUpdate)
        {
            var duration = await _context.RiskDurations
                .FirstOrDefaultAsync(d => d.Id == id);
            if (duration == null)
                return NotFound();
            duration.Code = durationForUpdate.Code;
            duration.Name = durationForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetDuration",
                    new { controller = "References", id = duration.Id }, duration);
            }
            return BadRequest("Error update Risk Duration");
        }

        [HttpPost("durations")]
        public async Task<IActionResult> CreateDuration(RiskDurationForCreateDto durationForCreate)
        {
            RiskDuration newDuration = _mapper.Map<RiskDuration>(durationForCreate);
            _context.Add(newDuration);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newDuration);
            }
            return BadRequest("Error create Risk Duration");
        }

        [HttpPost("durations/{id}")]
        public async Task<IActionResult> DeleteDuration(int id)
        {
            var duration = await _context.RiskDurations
                .FirstOrDefaultAsync(d => d.Id == id);
            if (duration == null)
                return NotFound();
            _context.Remove(duration);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Risk Duration id {id}");
        }

        [HttpGet("manageabilities")]
        public async Task<IActionResult> GetManageabilities()
        {
            var manageabilities = await _context.RiskManageabilities
                .OrderBy(m => m.Name)
                .ToListAsync();       
            return Ok(manageabilities);
        }

        [HttpGet("manageabilities/{id}", Name="GetManageability")]
        public async Task<IActionResult> GetManageability(int id)
        {
            var manageability = await _context.RiskManageabilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manageability == null)
                return NotFound();
            return Ok(manageability);
        }

        [HttpPut("manageabilities/{id}")]
        public async Task<IActionResult> UpdateManageability(int id, RiskManageability manageabilityForUpdate)
        {
            var manageability = await _context.RiskManageabilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manageability == null)
                return NotFound();
            manageability.Name = manageabilityForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetManageability",
                    new { controller = "References", id = manageability.Id }, manageability);
            }
            return BadRequest("Error update Risk Manageability");
        }

        [HttpPost("manageabilities")]
        public async Task<IActionResult> CreateManageability(RiskManageabilityForCreateDto manageabilityForCreateDto)
        {
            RiskManageability newManageability = _mapper.Map<RiskManageability>(manageabilityForCreateDto);
            _context.Add(newManageability);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newManageability);
            }
            return BadRequest("Error create Risk Manageability");
        }

        [HttpDelete("manageabilities/{id}")]
        public async Task<IActionResult> DeleteManageability(int id)
        {
            var manageability = await _context.RiskManageabilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manageability == null)
                return NotFound();
            _context.Remove(manageability);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Risk Manageability id {id}");
        }

        [HttpGet("significances")]
        public async Task<IActionResult> GetSignificances()
        {
            var significances = await _context.RiskSignificances
                .OrderBy(s => s.Name)
                .ToListAsync();       
            return Ok(significances);
        }

        [HttpGet("significances/{id}", Name="GetSignificance")]
        public async Task<IActionResult> GetSignificance(int id)
        {
            var significance = await _context.RiskSignificances
                .FirstOrDefaultAsync(s => s.Id == id);
            if (significance == null)
                return NotFound();
            return Ok(significance);
        }

        [HttpPut("significances/{id}")]
        public async Task<IActionResult> UpdateSignificance(int id, RiskSignificance significanceForUpdate)
        {
            var significance = await _context.RiskSignificances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (significance == null)
                return NotFound();
            significance.Name = significanceForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetSignificance",
                    new { controller = "References", id = significance.Id }, significance);
            }
            return BadRequest("Error update Risk Significance");
        }

        [HttpPost("significances")]
        public async Task<IActionResult> CreateSignificance(RiskSignificanceForCreateDto significanceForCreate)
        {
            var newSignificance = _mapper.Map<RiskSignificance>(significanceForCreate);
            _context.Add(newSignificance);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newSignificance);
            }
            return BadRequest("Error create Risk Significance");
        }

        [HttpPost("significances/{id}")]
        public async Task<IActionResult> DeleteSignificance(int id)
        {
            var significance = await _context.RiskSignificances
                .FirstOrDefaultAsync(m => m.Id == id);
            if (significance == null)
                return NotFound();
            _context.Remove(significance);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Risk Significance id {id}");
        }

        [HttpGet("reactions")]
        public async Task<IActionResult> GetReactions()
        {
            var reactions = await _context.RiskReactions
                .OrderBy(r => r.Name)
                .ToListAsync();       
            return Ok(reactions);
        }

        [HttpGet("reactions/{id}", Name="GetReaction")]
        public async Task<IActionResult> GetReactions(int id)
        {
            var reaction = await _context.RiskReactions
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reaction == null)
                return NotFound();
            return Ok(reaction);
        }

        [HttpPut("reactions/{id}")]
        public async Task<IActionResult> UpdateReaction(int id, RiskReaction reactionForUpdate)
        {
            var reaction = await _context.RiskReactions
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reaction == null)
                return NotFound();
            reaction.Name = reactionForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetReaction",
                    new { controller = "References", id = reaction.Id }, reaction);
            }
            return BadRequest("Error update Risk Reaction");
        }

        [HttpPost("reactions")]
        public async Task<IActionResult> CreateReaction(RiskReactionForCreateDto reactionForCreate)
        {
            var newReaction = _mapper.Map<RiskReaction>(reactionForCreate);
            _context.Add(newReaction);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newReaction);
            }
            return BadRequest("Error create Risk Reaction");
        }

        [HttpDelete("reactions/{id}")]
        public async Task<IActionResult> DeleteReaction(int id)
        {
            var reaction = await _context.RiskReactions
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reaction == null)
                return NotFound();
            _context.Remove(reaction);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Risk Reaction id {id}");
        }

        [HttpGet("riskstatuses")]
        public async Task<IActionResult> GetRiskStatuses()
        {
            var riskStatuses = await _context.RiskStatuses
                .OrderBy(s => s.Name)
                .ToListAsync();       
            return Ok(riskStatuses);
        }

        [HttpGet("riskstatuses/{id}", Name="GetRiskStatus")]
        public async Task<IActionResult> GetRiskStatus(int id)
        {
            var riskStatus = await _context.RiskStatuses
                .FirstOrDefaultAsync(s => s.Id == id);
            if (riskStatus == null)
                return NotFound();
            return Ok(riskStatus);
        }

        [HttpPut("riskstatuses/{id}")]
        public async Task<IActionResult> UpdateRiskStatus(int id, RiskStatus riskStatusForUpdate)
        {
            var riskStatus = await _context.RiskStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (riskStatus == null)
                return NotFound();
            riskStatus.Name = riskStatusForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetRiskStatus",
                    new { controller = "References", id = riskStatus.Id }, riskStatus);
            }
            return BadRequest("Error update Risk Status");
        }

        [HttpPost("riskstatuses")]
        public async Task<IActionResult> CreateRiskStatus(RiskStatusForCreateDto riskStatusForCreate)
        {
            var newRiskStatus = _mapper.Map<RiskStatus>(riskStatusForCreate);
            _context.Add(newRiskStatus);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newRiskStatus);
            }
            return BadRequest("Error create Risk Status");
        }

        [HttpDelete("riskstatuses/{id}")]
        public async Task<IActionResult> DeleteRiskStatus(int id)
        {
            var riskStatus = await _context.RiskStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (riskStatus == null)
                return NotFound();
            _context.Remove(riskStatus);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Risk Status id {id}");
        }

        [HttpGet("factors")]
        public async Task<IActionResult> GetFactors()
        {
            var factors = await _context.RiskFactors
                .Where(f => f.Parent == null)
                .Include(f => f.Children).ThenInclude(f => f.Children)
                .OrderBy(f => f.Code)
                .ToListAsync();
            return Ok(factors);
        }

        [HttpGet("factors/{id}")]
        public async Task<IActionResult> GetFactor(int id)
        {
            var factor = await _context.RiskFactors
                .Include(f => f.Parent).ThenInclude(f => f.Parent)
                .Include(f => f.Children).ThenInclude(f => f.Children)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (factor == null)
                return NotFound();
            return Ok(factor);
        }

        [HttpGet("factors/categories")]
        public async Task<IActionResult> GetFactorCategories()
        {
            var factors = await _context.RiskFactors
                .Where(f => f.Parent == null)
                .OrderBy(f => f.Code)
                .ToListAsync();
            return Ok(factors);
        }

        [HttpPost("factors")]
        public async Task<IActionResult> CreateFactor(RiskFactorForCreateDto factorForCreate)
        {
            var newFactor = _mapper.Map<RiskFactor>(factorForCreate);
            _context.Add(newFactor);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (newFactor.ParentId > 0)
                {
                    var factorToReturn = await _context.RiskFactors
                        .Include(f => f.Parent).ThenInclude(p => p.Parent)
                        .FirstOrDefaultAsync(f => f.Id == newFactor.Id);
                        return Ok(factorToReturn);
                }
                return Ok(newFactor);
            }
            return BadRequest("Error create Risk Factor");
        }

        [HttpPut("factors/{id}")]
        public async Task<IActionResult> UpdateFactor(int id, RiskFactor factorForUpdate)
        {
            var updatedFactor = await _context.RiskFactors.FirstOrDefaultAsync(f => f.Id == id);
            if (updatedFactor == null)
                return NotFound();
            updatedFactor.Code = factorForUpdate.Code;
            updatedFactor.Name = factorForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(updatedFactor);
            }
            return BadRequest("Error update Risk Factor");
        }

        [HttpDelete("factors/{id}")]
        public async Task<IActionResult> DeleteFactor(int id)
        {
            var factor = await _context.RiskFactors
                .FirstOrDefaultAsync(f => f.Id == id);
            if (factor == null)
                return NotFound();
            _context.Remove(factor);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Risk Factor id {id}");
        }

        [HttpGet("activitytypes")]
        public async Task<IActionResult> GetActivityTypes()
        {
            var activities = await _context.ActivityTypes
                .Where(f => f.Parent == null)
                .Include(f => f.Children).ThenInclude(f => f.Children)
                .OrderBy(f => f.Code)
                .ToListAsync();
            return Ok(activities);
        }

        [HttpGet("activitytypes/{id}")]
        public async Task<IActionResult> GetActivityType(int id)
        {
            var activity = await _context.ActivityTypes
                .Include(f => f.Parent).ThenInclude(f => f.Parent)
                .Include(f => f.Children).ThenInclude(f => f.Children)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (activity == null)
                return NotFound();
            return Ok(activity);
        }

        [HttpPost("activitytypes")]
        public async Task<IActionResult> CreateActivityType(ActivityTypeForCreateDto activityForCreate)
        {
            var newActivity = _mapper.Map<ActivityType>(activityForCreate);
            _context.Add(newActivity);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (newActivity.ParentId > 0)
                {
                    var activityToReturn = await _context.ActivityTypes
                        .Include(f => f.Parent).ThenInclude(p => p.Parent)
                        .FirstOrDefaultAsync(f => f.Id == newActivity.Id);
                        return Ok(activityToReturn);
                }
                return Ok(newActivity);
            }
            return BadRequest("Error create Activity Type");
        }

        [HttpPut("activitytypes/{id}")]
        public async Task<IActionResult> UpdateActivityType(int id, ActivityType activityForUpdate)
        {
            var updatedActivity = await _context.ActivityTypes.FirstOrDefaultAsync(a => a.Id == id);
            if (updatedActivity == null)
                return NotFound();
            updatedActivity.Code = activityForUpdate.Code;
            updatedActivity.Name = activityForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(updatedActivity);
            }
            return BadRequest("Error update activity Type");
        }

        [HttpDelete("activitytypes/{id}")]
        public async Task<IActionResult> DeleteActivityType(int id)
        {
            var activity = await _context.ActivityTypes
                .FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null)
                return NotFound();
            _context.Remove(activity);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Activity Type id {id}");
        }

        [HttpGet("businessprocesses")]
        public async Task<IActionResult> GetBusinessProcesses()
        {
            var bps = await _context.BusinessProcesses
                .Where(f => f.Parent == null)
                .Include(f => f.Children).ThenInclude(f => f.Children).ThenInclude(f => f.Children)
                .OrderBy(f => f.Code)
                .ToListAsync();
            return Ok(bps);
        }

        [HttpGet("businessprocesses/{id}")]
        public async Task<IActionResult> GetBusinessProcess(int id)
        {
            var bp = await _context.BusinessProcesses
                .Include(f => f.Parent).ThenInclude(f => f.Parent).ThenInclude(f => f.Parent)
                .Include(f => f.Children).ThenInclude(f => f.Children).ThenInclude(f => f.Children)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (bp == null)
                return NotFound();
            return Ok(bp);
        }

        [HttpPost("businessprocesses")]
        public async Task<IActionResult> CreateBusinessProcess(BusinessProcessForCreateDto bpForCreate)
        {
            var newBp = _mapper.Map<BusinessProcess>(bpForCreate);
            _context.Add(newBp);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (newBp.ParentId > 0)
                {
                    BusinessProcess bpToReturn = await _context.BusinessProcesses
                        .Include(f => f.Parent).ThenInclude(p => p.Parent)
                        .FirstOrDefaultAsync(f => f.Id == newBp.Id);
                        return Ok(bpToReturn);
                }
                return Ok(newBp);
            }
            return BadRequest("Error create Business Process");
        }

        [HttpPut("businessprocesses/{id}")]
        public async Task<IActionResult> UpdateBusinessProcess(int id, BusinessProcess bpForUpdate)
        {
            var updatedBp = await _context.BusinessProcesses.FirstOrDefaultAsync(a => a.Id == id);
            if (updatedBp == null)
                return NotFound();
            updatedBp.Code = bpForUpdate.Code;
            updatedBp.Name = bpForUpdate.Name;
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(updatedBp);
            }
            return BadRequest("Error update Business Process");
        }

        [HttpDelete("businessprocesses/{id}")]
        public async Task<IActionResult> DeleteBusinessProcess(int id)
        {
            var bp = await _context.BusinessProcesses
                .FirstOrDefaultAsync(a => a.Id == id);
            if (bp == null)
                return NotFound();
            _context.Remove(bp);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete Business Process id {id}");
        }

        [HttpGet("textdatas")]
        public async Task<IActionResult> GetTextDatas()
        {
            var datas = await _context.TextDatas
                .OrderBy(a => a.Name)
                .ToListAsync();
            return Ok(datas);
        }

        [HttpGet("textdatas/{id}", Name = "GetTextData")]
        public async Task<IActionResult> GetTextData(int id)
        {
            var data = await _context.TextDatas
                .FirstOrDefaultAsync(a => a.Id == id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpPut("textdatas/{id}")]
        public async Task<IActionResult> UpdateTextData(int id, TextData dataForUpdate)
        {
            var data = await _context.TextDatas
                .FirstOrDefaultAsync(a => a.Id == id);
            if (data == null)
                return NotFound();
            data.Name = dataForUpdate.Name;
            data.Value = dataForUpdate.Value;
            data.Param = dataForUpdate.Param;
            if (await _context.SaveChangesAsync() > 0)
            {
                return CreatedAtRoute("GetTextData",
                    new { controller = "References", id = data.Id }, data);
            }
            return BadRequest("Error update Text Data");
        }

        [HttpPost("textdatas")]
        public async Task<IActionResult> CreateTextData(TextDataForCreateDto dataForCreate)
        {
            TextData newArea = _mapper.Map<TextData>(dataForCreate);
            _context.Add(newArea);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok(newArea);
            }
            return BadRequest("Error create Text Data");
        }

        [HttpDelete("textdatas/{id}")]
        public async Task<IActionResult> DeleteTextData(int id)
        {
            var data = await _context.TextDatas
                .FirstOrDefaultAsync(a => a.Id == id);
            if (data == null)
                return NotFound();
            _context.Remove(data);
            if (await _context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            return BadRequest($"Error delete TextData id {id}");
        }
    }
}