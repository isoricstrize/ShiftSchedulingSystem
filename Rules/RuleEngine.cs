using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Models;

namespace ShiftSchedulingSystem.Rules
{
    public class RuleEngine
    {
        private readonly List<ISchedulingRule> _rules;

        public RuleEngine(List<ISchedulingRule> rules)
        {
            _rules = rules;
        }

        public bool IsValid(Worker worker, Shift shift, DaySchedule day)
        {
            return _rules.All(
                rule => rule.IsValid(worker, shift, day));
        }
    }
}