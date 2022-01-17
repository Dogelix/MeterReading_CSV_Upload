#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using ENSEK_API.Data;
using ENSEK_API.Models;
using System.Globalization;
using ENSEK_API.ModelValidation;

namespace ENSEK_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly EnsekEnergyContext _context;
        private IWebHostEnvironment _hostingEnvironment;

        public MeterReadingController(EnsekEnergyContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _hostingEnvironment = environment;
        }

        // GET: api/MeterReading
        [HttpGet]
        public ActionResult<MeterReading> GetMeterReading()
        {
            return BadRequest("AccountId is Required");
        }

        // GET: api/MeterReading/5
        [HttpGet("{AccountId}")]
        public ActionResult<MeterReading[]> GetMeterReadingsForAccount(int AccountId)
        {
            var meterReading =  _context.MeterReading.Where(e => e.AccountId == AccountId).ToArray();

            if (meterReading == null)
            {
                return NotFound();
            }

            return meterReading;
        }

        // PUT: api/MeterReading/5
        [HttpPut("{id}")]
        public IActionResult PutMeterReading(int id)
        {
            return StatusCode(405);
        }

        // POST: api/MeterReading
        [HttpPost("meter-reading-uploads")]
        public async Task<ActionResult<MeterReading>> UploadMeterReadingsCsv(IFormFile csvFile)
        {
            string uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "TemporaryStore");
            string filePath = "";
            if (csvFile.FileName.EndsWith(".csv"))
            {
                try
                {
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    filePath = Path.Combine(uploads, csvFile.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await csvFile.CopyToAsync(fileStream);
                    }
                }
                catch (Exception e)
                {
                    return StatusCode(500, e);
                }

                List<MeterReading> meterReadings = new List<MeterReading>();
                int failedResults = 0;

                using (var reader = new StreamReader(filePath))
                {
                    string[] headers = reader.ReadLine().Split(",");
                    while (!reader.EndOfStream)
                    {
                        string[] row = reader.ReadLine().Split(",");
                        DateTime dateTime;
                        try
                        {
                            dateTime = DateTime.Parse(row[1]);
                        }
                        catch
                        {
                            failedResults++;
                            continue;
                        }

                        int outValue = 0;
                        bool isInt = int.TryParse(row[2], out outValue);
                        if(!isInt || outValue < 0)
                        {
                            failedResults++;
                            continue;
                        }

                        int outId = 0;
                        isInt = int.TryParse(row[0], out outId);
                        if (!isInt || outId < 0)
                        {
                            failedResults++;
                            continue;
                        }

                        meterReadings.Add(new MeterReading()
                        {
                            AccountId = outId,
                            DateTime = dateTime,
                            Value = outValue
                        });
                    }

                    var validAccountIds = _context.Accounts.Select(a => a.AccountId).ToList();

                    failedResults += meterReadings.Where(a => !validAccountIds.Contains(a.AccountId)).Count();

                    meterReadings.RemoveAll(a => !validAccountIds.Contains(a.AccountId));

                    _context.MeterReading.AddRange(meterReadings);

                    await _context.SaveChangesAsync();
                }

                return Ok(new { SuccessCount = meterReadings.Count, FailureCount = failedResults });
            }
            else
            {
                return BadRequest();
            }
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
    }
}
