using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;
using ShiftSchedulingSystem.Models;
using ShiftSchedulingSystem.Rules;

namespace ShiftSchedulingSystem.Scheduling
{
    public class ScheduleOptimizer
    {
        private readonly RuleEngine _ruleEngine;
        private readonly SchedulingSettings _settings;

        public ScheduleOptimizer(RuleEngine ruleEngine, SchedulingSettings settings)
        {
            _ruleEngine = ruleEngine;
            _settings = settings;
        }

        public void Optimize(List<Worker> workers, Schedule schedule)
        {
            var daysByWorkload = schedule.Days
                .OrderByDescending(
                    d => d.Shifts.Sum(s => s.WorkLoad));

            foreach (var day in daysByWorkload)
            {
                if (workers.All(w => w.AssignedHours >= _settings.MaxWeeklyHours))
                {
                    break;
                }

                var supportShift = new Shift
                {
                    ShiftType = ShiftType.Support,
                    StartTime = day.Date.ToDateTime(new TimeOnly(10, 0)),
                    EndTime = day.Date.ToDateTime(new TimeOnly(18, 0)),
                    WorkLoad = 0
                };

                var validWorkers = workers
                    .Where(w => w.IsActive)
                    .Where(w => w.AssignedHours < _settings.MaxWeeklyHours)
                    .Where(w => _ruleEngine.IsValid(w, supportShift, day))
                    .ToList();

                if (!validWorkers.Any())
                {
                    Console.WriteLine(
                        $"No valid worker for support shift on {day.Date:dd.MM.yyyy}");

                    continue;
                }

                var selectedWorker = SelectBestWorker(validWorkers);

                supportShift.AssignedWorker = selectedWorker;

                selectedWorker.AssignedShifts.Add(supportShift);
                selectedWorker.AssignedHours += _settings.ShiftDurationHours;

                day.Shifts.Add(supportShift);

                Console.WriteLine(
                    $"SUPPORT SHIFT {day.Date:dd.MM.yyyy} -> {selectedWorker.Name}");
            }
        }

        private Worker SelectBestWorker(
            List<Worker> workers)
        {
            Worker? selectedWorker = null;
            int bestScore = int.MinValue;

            foreach (var worker in workers)
            {
                int score = CalculateScore(worker);

                Console.WriteLine(
                    $"OPTIMIZER SCORE: {worker.Name} -> {score}");

                if (score > bestScore)
                {
                    bestScore = score;
                    selectedWorker = worker;
                }
            }

            return selectedWorker!;
        }

        private int CalculateScore(Worker worker)
        {
            int remainingHours = _settings.MaxWeeklyHours - worker.AssignedHours;

            return remainingHours + (int)worker.Seniority;
        }
    }
}