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
    80, 85, 100, 100, 100, 100, 60
};
var shiftWorkloads = new List<(int Morning, int Afternoon, int Night)>
{
    (21, 13, 0), // 15.6
    (23, 26, 0), // 16.6
    (28, 38, 0), // 17.6
    (0, 0, 0), // 18.6
    (56, 59, 0), // 19.6
    (0, 0, 0), // 20.6
    (59, 33, 0) // 21.6
};
var schedule = ScheduleSeed.Create(startDate, occupancies, shiftWorkloads);

var rules = new List<ISchedulingRule>
{
    new OneShiftPerDayRule(),
    new WorkerRequestRule(),
    new MinimumRestHoursRule(),
    new ConsecutiveNightShiftsRule()
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
            $"  {shift.ShiftType,-10} " +
            $"| {shift.StartTime:HH:mm}-{shift.EndTime:HH:mm} " +
            $"| Workload: {shift.WorkLoad,3} -> {workerName}");
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