using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Models;

namespace ShiftSchedulingSystem.Rules
{
    public class WorkerRequestRule : ISchedulingRule
    {
        public bool IsValid(Worker worker, Shift shift, DaySchedule day)
        {
            return !worker.Requests.Any(r => r.Date == day.Date);
        }
    }
}