using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;

namespace ShiftSchedulingSystem.Models
{
    public class Worker
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }

        public SeniorityLevel Seniority { get; set; }

        public bool IsActive { get; set; } = true;

        public int AssignedHours { get; set; }

        public List<Shift> AssignedShifts { get; set; } = [];

        public List<WorkerRequest> Requests { get; set; } = [];

    }
}