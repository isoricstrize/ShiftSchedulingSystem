using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftSchedulingSystem.Scheduling
{
    public class SchedulingSettings
    {
        public int MaxWeeklyHours { get; set; } = 40;

        public int ShiftDurationHours { get; set; } = 8;
    }
}