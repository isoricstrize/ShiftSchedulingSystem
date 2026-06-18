using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;

namespace ShiftSchedulingSystem.Models
{
    public class Shift
    {
        public ShiftType ShiftType { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int WorkLoad { get; set; }

        public Worker? AssignedWorker { get; set; } // todo: list of workers
    }
}