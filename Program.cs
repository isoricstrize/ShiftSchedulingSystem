// See https://aka.ms/new-console-template for more information
using ShiftSchedulingSystem.Models;
using ShiftSchedulingSystem.Enums;
using ShiftSchedulingSystem.Rules;
using ShiftSchedulingSystem.SeedData;
using ShiftSchedulingSystem.Generator;


var workers = WorkerSeed.CreateWorkers();

var startDate = new DateOnly(2026, 6, 15);
var occupancies = new List<int>
{
    40, 60, 80, 90, 95, 100, 70
};
var schedule = ScheduleSeed.Create(startDate, occupancies);

var rules = new List<ISchedulingRule>
{
    new OneShiftPerDayRule(),
    new MinimumRestHoursRule(),
    new WorkerRequestRule()
};
var ruleEngine = new RuleEngine(rules);

var generator = new ScheduleGenerator(ruleEngine);
generator.Generate(workers, schedule);



Console.WriteLine("\n\n===== FINAL SCHEDULE =====");
foreach (var d in schedule.Days.OrderBy(d => d.Date))
{
    Console.WriteLine($"\n{d.Date:dd.MM.yyyy} ({d.OccupancyPercentage}%)");

    foreach (var shift in d.Shifts)
    {
        string workerName = shift.AssignedWorker?.Name ?? "UNASSIGNED";

        Console.WriteLine(
            $"  {shift.ShiftType,-10} -> {workerName}");
    }
}


Console.WriteLine();
Console.WriteLine("===== WORKER STATISTICS =====");
foreach (var worker in workers)
{
    Console.WriteLine(
        $"{worker.Name} | " +
        $"Hours: {worker.AssignedHours} | " +
        $"Shifts: {worker.AssignedShifts.Count}");
}


Console.WriteLine();
Console.WriteLine("===== WORKER SHIFTS =====");
foreach (var worker in workers)
{
    Console.WriteLine($"\n{worker.Name}");

    foreach (var shift in worker.AssignedShifts.OrderBy(s => s.StartTime))
    {
        Console.WriteLine(
            $"{shift.StartTime:dd.MM} {shift.ShiftType}");
    }
}