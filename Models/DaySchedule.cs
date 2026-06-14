using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftSchedulingSystem.Models
{
    public class DaySchedule
    {
        public required DateOnly Date { get; set; }

        public int OccupancyPercentage { get; set; }

        public List<Shift> Shifts { get; set; } = [];
    }
}