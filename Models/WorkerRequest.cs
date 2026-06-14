using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;

namespace ShiftSchedulingSystem.Models
{
    public class WorkerRequest
    {
        public DateOnly Date { get; set; }

        public RequestType RequestType { get; set; }
    }
}