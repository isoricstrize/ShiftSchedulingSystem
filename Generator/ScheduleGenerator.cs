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
            var allShifts = schedule.Days
                .SelectMany(d => d.Shifts)
                .OrderByDescending(s => s.WorkLoad);

            foreach (var shift in allShifts)
            {
                var day = schedule.Days
                    .First(d => d.Shifts.Contains(shift));

                Console.WriteLine($"\n=== {day.Date} ({day.OccupancyPercentage}%) ===");

                var sortedShifts = day.Shifts
                    .OrderByDescending(s => s.WorkLoad);

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

                var selectedWorker = SelectBestWorker(validWorkers, shift);

                // Assign worker
                shift.AssignedWorker = selectedWorker;
                // Update worker state
                selectedWorker.AssignedHours += 8;
                selectedWorker.AssignedShifts.Add(shift);

                Console.WriteLine($"==> {shift.ShiftType} -> {selectedWorker.Name}");
            }
        }


        private Worker SelectBestWorker(List<Worker> validWorkers, Shift shift)
        {
            Worker? selectedWorker = null;
            int bestScore = int.MinValue;

            foreach (var worker in validWorkers)
            {
                int score = CalculateScore(worker, shift);

                Console.WriteLine($"{worker.Name} - Score: {score} (workload: {shift.WorkLoad})");

                if (score > bestScore)
                {
                    bestScore = score;
                    selectedWorker = worker;
                }
            }

            return selectedWorker!;
        }


        private int GetConsecutiveNightShifts(Worker worker)
        {
            var ordered = worker.AssignedShifts
                .OrderByDescending(s => s.StartTime)
                .ToList();

            int count = 0;

            foreach (var shift in ordered)
            {
                if (shift.ShiftType == ShiftType.Night)
                {
                    count++;
                }
                else
                {
                    break; // streak is broken
                }
            }

            return count;
        }


        private int CalculateScore(Worker worker, Shift shift)
        {
            int remainingHours = MaxWeeklyHours - worker.AssignedHours;

            // Small penalty to reduce preference for workers who already worked night shifts 
            // (encourages rotation of night shifts across the team)
            int nightShiftPenalty = worker.AssignedShifts.Count(s => s.ShiftType == ShiftType.Night);

            // Worker seniority is weighted by shift workload, 
            // so it matters more on busy shifts and less on quiet shifts.
            return remainingHours + ((int)worker.Seniority * shift.WorkLoad) - (nightShiftPenalty * 5);
        }


    }
}