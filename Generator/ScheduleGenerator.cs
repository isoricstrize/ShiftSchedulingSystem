using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;
using ShiftSchedulingSystem.Models;
using ShiftSchedulingSystem.Rules;

namespace ShiftSchedulingSystem.Generator
{
    public class ScheduleGenerator
    {
        private const int MaxWeeklyHours = 40;

        private readonly RuleEngine _ruleEngine;

        public ScheduleGenerator(RuleEngine ruleEngine)
        {
            _ruleEngine = ruleEngine;
        }

        public void Generate(List<Worker> workers, Schedule schedule)
        {
            // Days with higher occupancy percantage are priority
            var sortedDays = schedule.Days
                .OrderByDescending(d => d.OccupancyPercentage)
                .ToList();

            foreach (var day in sortedDays)
            {
                Console.WriteLine($"\n=== {day.Date} ({day.OccupancyPercentage}%) ===");

                foreach (var shift in day.Shifts)
                {
                    var validWorkers = workers
                        .Where(w => w.IsActive)
                        .Where(w => _ruleEngine.IsValid(w, shift, day))
                        .ToList();

                    // Skip the current shift if there is no valid worker. Go to the next shift.
                    if (!validWorkers.Any())
                    {
                        Console.WriteLine($"No valid worker for {shift.ShiftType}");
                        continue;
                    }

                    foreach (var worker in validWorkers)
                    {
                        Console.WriteLine($"VALID WORKERS: {worker.Name} - {worker.Seniority}");
                    }

                    var selectedWorker = SelectBestWorker(validWorkers, day);

                    // Assign worker
                    shift.AssignedWorker = selectedWorker;
                    // Update worker state
                    selectedWorker.AssignedHours += 8;
                    selectedWorker.AssignedShifts.Add(shift);

                    Console.WriteLine($"==> {shift.ShiftType} -> {selectedWorker.Name}");

                }
            }
        }


        private Worker SelectBestWorker(List<Worker> validWorkers, DaySchedule day)
        {
            Worker? selectedWorker = null;
            int bestScore = int.MinValue;

            foreach (var worker in validWorkers)
            {
                int score = CalculateScore(worker, day);

                Console.WriteLine($"{worker.Name} - Score: {score}");

                if (score > bestScore)
                {
                    bestScore = score;
                    selectedWorker = worker;
                }
            }

            return selectedWorker!;
        }


        private int CalculateScore(Worker worker, DaySchedule day)
        {
            int remainingHours = MaxWeeklyHours - worker.AssignedHours;

            int occupancyFactor = day.OccupancyPercentage / 10;

            return remainingHours + ((int)worker.Seniority * occupancyFactor);
        }

    }
}