using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Models;

namespace ShiftSchedulingSystem.Rules
{
    public interface ISchedulingRule
    {
        public bool IsValid(Worker worker, Shift shift, DaySchedule day);
    }
}