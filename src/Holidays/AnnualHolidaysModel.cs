using System.Collections.Generic;

namespace Holidays
{
    public class AnnualHolidaysModel
    {
        public int Year { get; set; }
        public List<MonthHolidaysModel> Holidays { get; set; }
    }
}