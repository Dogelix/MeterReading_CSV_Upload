using CsvHelper.Configuration;
using ENSEK_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENSEK_API.ModelValidation
{
    public class MeterReadingMap : ClassMap<MeterReading>
    {
        public MeterReadingMap()
        {
            Map(m => m.AccountId).Name("AccountId");
            Map(m => m.DateTime).Name("MeterReadingDateTime");
            Map(m => m.Value).Name("MeterReadValue");
        }
    }
}
