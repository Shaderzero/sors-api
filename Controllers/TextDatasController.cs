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
    public class TextDatasController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public TextDatasController(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetTextDatas([FromQuery]TextDataForCreateDto parameters)
        {
            var source = _context.TextDatas
                .OrderBy(a => a.Name)
                .AsQueryable();
            if (parameters != null && parameters.Name != null)
            {
                source = source.Where(e => e.Name.Equals(parameters.Name) && e.Param.Equals(parameters.Param));
                var result = await source.FirstOrDefaultAsync();
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            else
            {
                var result = await source.ToListAsync();
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTextData(int id)
        {
            var data = await _context.TextDatas
                .FirstOrDefaultAsync(a => a.Id == id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpPut("{id}")]
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

        [HttpPost()]
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

        [HttpDelete("{id}")]
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