using System;
using System.Collections.Generic;

namespace Holidays
{
    public class MonthModel
    {
        public DateTime ActiveMonth { get; set; }
        public IEnumerable<MonthHolidaysModel> Holidays { get; set; }
    }
}