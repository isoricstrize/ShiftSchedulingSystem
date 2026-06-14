using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftSchedulingSystem.Models
{
    public class Schedule
    {
        public List<DaySchedule> Days { get; set; } = [];
    }
}