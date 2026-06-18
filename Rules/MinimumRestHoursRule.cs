using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Models;

namespace ShiftSchedulingSystem.Rules
{
    public class MinimumRestHoursRule : ISchedulingRule
    {
        public bool IsValid(Worker worker, Shift shift, DaySchedule day)
        {
            if (worker.AssignedShifts.Count == 0) return true;

            const int MinRestHours = 12;

            foreach (var existingShift in worker.AssignedShifts)
            {
                double hoursAfterExisting =
                    (shift.StartTime - existingShift.EndTime)
                        .TotalHours;

                double hoursBeforeExisting =
                    (existingShift.StartTime - shift.EndTime)
                        .TotalHours;

                bool enoughRest =
                    hoursAfterExisting >= MinRestHours
                    ||
                    hoursBeforeExisting >= MinRestHours;

                /*Console.WriteLine($"Worker: {worker.Name}");
                Console.WriteLine($"hoursAfterExisting: {hoursAfterExisting}");
                Console.WriteLine($"hoursBeforeExisting: {hoursBeforeExisting}");
                Console.WriteLine($"enoughRest: {enoughRest}");*/

                if (!enoughRest) return false;
            }
            return true;
        }
    }
}