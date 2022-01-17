#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ENSEK_API.Data;
using ENSEK_API.Models;

namespace ENSEK_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly EnsekEnergyContext _context;

        public MeterReadingController(EnsekEnergyContext context)
        {
            _context = context;
        }

        // GET: api/MeterReading
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeterReading>>> GetMeterReading()
        {
            return await _context.MeterReading.ToListAsync();
        }

        // GET: api/MeterReading/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MeterReading>> GetMeterReading(int id)
        {
            var meterReading = await _context.MeterReading.FindAsync(id);

            if (meterReading == null)
            {
                return NotFound();
            }

            return meterReading;
        }

        // PUT: api/MeterReading/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeterReading(int id, MeterReading meterReading)
        {
            if (id != meterReading.ReadingId)
            {
                return BadRequest();
            }

            _context.Entry(meterReading).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeterReadingExists(id))
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

        // POST: api/MeterReading
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MeterReading>> PostMeterReading(MeterReading meterReading)
        {
            _context.MeterReading.Add(meterReading);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeterReading", new { id = meterReading.ReadingId }, meterReading);
        }

        // DELETE: api/MeterReading/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeterReading(int id)
        {
            var meterReading = await _context.MeterReading.FindAsync(id);
            if (meterReading == null)
            {
                return NotFound();
            }

            _context.MeterReading.Remove(meterReading);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MeterReadingExists(int id)
        {
            return _context.MeterReading.Any(e => e.ReadingId == id);
        }
    }
}
