using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;
using ShiftSchedulingSystem.Models;

namespace ShiftSchedulingSystem.Rules
{
    public class ConsecutiveNightShiftsRule : ISchedulingRule
    {
        public bool IsValid(Worker worker, Shift shift, DaySchedule day)
        {
            if (shift.ShiftType != ShiftType.Night)
            {
                return true;
            }

            var nightDates = worker.AssignedShifts
                .Where(s => s.ShiftType == ShiftType.Night)
                .Select(s => DateOnly.FromDateTime(s.StartTime))
                .ToHashSet();

            var currentDate =
                DateOnly.FromDateTime(shift.StartTime);

            bool twoBefore =
                nightDates.Contains(currentDate.AddDays(-1))
                &&
                nightDates.Contains(currentDate.AddDays(-2));

            bool oneBeforeOneAfter =
                nightDates.Contains(currentDate.AddDays(-1))
                &&
                nightDates.Contains(currentDate.AddDays(1));

            bool twoAfter =
                nightDates.Contains(currentDate.AddDays(1))
                &&
                nightDates.Contains(currentDate.AddDays(2));

            /*Console.WriteLine($"\n--- ConsecutiveNightShiftsRule ---");
            Console.WriteLine($"Worker: {worker.Name}");
            Console.WriteLine($"Candidate Night Shift: {currentDate}");

            Console.WriteLine($"Two nights before: {twoBefore}");
            Console.WriteLine($"One before & one after: {oneBeforeOneAfter}");
            Console.WriteLine($"Two nights after: {twoAfter}");*/

            bool isValid = !(twoBefore || oneBeforeOneAfter || twoAfter);

            //Console.WriteLine($"Valid: {isValid}");

            return isValid;
        }
    }
}