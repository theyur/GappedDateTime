using System;
using System.Collections.Generic;
using System.Linq;

namespace GappedDateTime
{
    public class GapDescriptor
    {
        public DayOfWeek DayOfWeek { get; }
        public int StartHour { get; }

        public GapDescriptor(DayOfWeek dayOfWeek, int startHour)
        {
            DayOfWeek = dayOfWeek;
            StartHour = startHour;
        }
    }
    
    /// <summary>
    /// V1.0:
    /// - 1 gap per week
    /// </summary>
    public class GappedDateTimeCalculator
    {
        private readonly GapDescriptor _gap;
        private readonly TimeSpan _gapLength;
        
        public GappedDateTimeCalculator(GapDescriptor gap, TimeSpan gapLength)
        {
            _gap = gap;
            _gapLength = gapLength;
        }

        private IEnumerable<DateTime> GetGapsForMonth(DateTime dateTime)
        {
            var month1st = DateTime.Parse($"{dateTime.Year}-{dateTime.Month}-1");

            var gapDayNum = 1;
            while (month1st.AddDays(gapDayNum-1).DayOfWeek != _gap.DayOfWeek) gapDayNum++;

            IList<DateTime> gapDayNumbers = new List<DateTime>();  // gap day:hour dates for month 
            while (gapDayNum <= DateTime.DaysInMonth(month1st.Year, month1st.Month))
            {
                gapDayNumbers.Add(DateTime.Parse($"{dateTime.Year}-{dateTime.Month}-{gapDayNum} {_gap.StartHour}:00:00"));
                gapDayNum += 7; // all other gap day nums in month
            }

            return gapDayNumbers;
        }

        private DateTime AlignWithGaps(DateTime dateTime)
        {
            var closestGapInPast = GetGapsForMonth(dateTime).LastOrDefault(g => g < dateTime);

            if (closestGapInPast == DateTime.MinValue) return dateTime;

            return dateTime - closestGapInPast >= _gapLength ? dateTime : closestGapInPast.Add(_gapLength);
        }

        private DateTime ShiftOverGap(DateTime dateTime)
        {
            var closestGapInPast = GetGapsForMonth(dateTime).LastOrDefault(g => g < dateTime);

            if (closestGapInPast == DateTime.MinValue) return dateTime;
            
            var diff = dateTime - closestGapInPast;
            return diff >= _gapLength ? dateTime : closestGapInPast.Add(_gapLength).Add(diff);
        }

        public DateTime Add(DateTime dateTime, TimeSpan timeSpan)
        {
            var startDate = AlignWithGaps(dateTime);

            static int Int(double d) => Convert.ToInt32(Math.Truncate(d));
            
            var endDateTime = startDate
                .AddHours(Int(timeSpan.TotalHours) + Int(timeSpan.TotalDays) / 7 * Int(_gapLength.TotalHours))
                .AddMinutes(timeSpan.Minutes)
                .AddSeconds(timeSpan.Seconds);

            return ShiftOverGap(endDateTime);
        }
    }
}