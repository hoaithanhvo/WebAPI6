using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI6.Data;
using WebAPI6.Models;

namespace WebAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TIotMoldMastersController : ControllerBase
    {
        private readonly NIDEC_IOTContext _context;
        private readonly IMapper _mapper;
        public static int PAGESIZE = 5;

        public TIotMoldMastersController(NIDEC_IOTContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/TIotMoldMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TIotMoldMasterModel>>> GetTIotMoldMasters(int item)
        {

          if (_context.TIotMoldMasters == null)
          {
              return NotFound();
          }
          var itemP = await _context.TIotMoldMasters.Skip((item-1) * PAGESIZE).Take(PAGESIZE).ToListAsync();
            return Ok(_mapper.Map<List<TIotMoldMasterModel>>(itemP));
        }

        // GET: api/TIotMoldMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TIotMoldMaster>> GetTIotMoldMaster(string id)
        {
          if (_context.TIotMoldMasters == null)
          {
              return NotFound();
          }
            var tIotMoldMaster = await _context.TIotMoldMasters.FindAsync(id);

            if (tIotMoldMaster == null)
            {
                return NotFound();
            }

            return tIotMoldMaster;
        }

        // PUT: api/TIotMoldMasters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTIotMoldMaster(string id, TIotMoldMaster tIotMoldMaster)
        {
            if (id != tIotMoldMaster.MoldSerial)
            {
                return BadRequest();
            }

            _context.Entry(tIotMoldMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TIotMoldMasterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TIotMoldMasters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TIotMoldMaster>> PostTIotMoldMaster(TIotMoldMaster tIotMoldMaster)
        {
          if (_context.TIotMoldMasters == null)
          {
              return Problem("Entity set 'NIDEC_IOTContext.TIotMoldMasters'  is null.");
          }
            _context.TIotMoldMasters.Add(tIotMoldMaster);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TIotMoldMasterExists(tIotMoldMaster.MoldSerial))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTIotMoldMaster", new { id = tIotMoldMaster.MoldSerial }, tIotMoldMaster);
        }

        // DELETE: api/TIotMoldMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTIotMoldMaster(string id)
        {
            if (_context.TIotMoldMasters == null)
            {
                return NotFound();
            }
            var tIotMoldMaster = await _context.TIotMoldMasters.FindAsync(id);
            if (tIotMoldMaster == null)
            {
                return NotFound();
            }

            _context.TIotMoldMasters.Remove(tIotMoldMaster);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TIotMoldMasterExists(string id)
        {
            return (_context.TIotMoldMasters?.Any(e => e.MoldSerial == id)).GetValueOrDefault();
        }
    }
}
