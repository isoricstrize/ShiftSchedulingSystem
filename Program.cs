// See https://aka.ms/new-console-template for more information
using ShiftSchedulingSystem.Models;
using ShiftSchedulingSystem.Enums;


// ------------ WORKERS INIT ------------
var workers = new List<Worker>
{
    new Worker
    {
        Id = 1,
        Name = "Ana",
        Seniority = SeniorityLevel.Senior,
        Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 19), RequestType = RequestType.DayOff}]
    },
    new Worker
    {
        Id = 2,
        Name = "Lorena",
        Seniority = SeniorityLevel.Mid,
        Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 19), RequestType = RequestType.DayOff}]

    },
    new Worker
    {
        Id = 3,
        Name = "Andrej",
        Seniority = SeniorityLevel.Junior,
        Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 17), RequestType = RequestType.DayOff}]
    },
    new Worker
    {
        Id = 4,
        Name = "Andrea",
        Seniority = SeniorityLevel.Mid
    },
    new Worker
    {
        Id = 5,
        Name = "Stella",
        Seniority = SeniorityLevel.Junior
    },
    new Worker
    {
        Id = 6,
        Name = "Luka",
        Seniority = SeniorityLevel.Mid,
        Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 16), RequestType = RequestType.DayOff}]
    }
};

foreach (var worker in workers)
{
    Console.WriteLine(
        $"{worker.Name} - {worker.Seniority}");
}




// ------------ DAYS INIT ------------

var startDate = new DateOnly(2026, 6, 15);

var days = new List<DaySchedule>
{
    new() { Date = startDate, OccupancyPercentage = 40 },
    new() { Date = startDate.AddDays(1), OccupancyPercentage = 60 },
    new() { Date = startDate.AddDays(2), OccupancyPercentage = 80 },
    new() { Date = startDate.AddDays(3), OccupancyPercentage = 90 },
    new() { Date = startDate.AddDays(4), OccupancyPercentage = 95 },
    new() { Date = startDate.AddDays(5), OccupancyPercentage = 100 },
    new() { Date = startDate.AddDays(6), OccupancyPercentage = 70 }
};



// ------------ SCHEDULE INIT ------------

var schedule = new Schedule() { Days = days };

foreach (var day in schedule.Days)
{
    day.Shifts = new List<Shift>
    {
        new Shift
        {
            ShiftType = ShiftType.Morning,
            StartTime = day.Date.ToDateTime(new TimeOnly(6, 0)),
            EndTime = day.Date.ToDateTime(new TimeOnly(14, 0))
        },

        new Shift
        {
            ShiftType = ShiftType.Afternoon,
            StartTime = day.Date.ToDateTime(new TimeOnly(14, 0)),
            EndTime = day.Date.ToDateTime(new TimeOnly(22, 0))
        },

        new Shift
        {
            ShiftType = ShiftType.Night,
            StartTime = day.Date.ToDateTime(new TimeOnly(22, 0)),
            EndTime = day.Date.ToDateTime(new TimeOnly(6, 0))
                .AddDays(1)
        }
    };
}


foreach (var day in schedule.Days)
{
    Console.WriteLine($"{day.Date} ({day.OccupancyPercentage}%)");

    foreach (var shift in day.Shifts)
    {
        Console.WriteLine($"  {shift.ShiftType}");
    }
}


static int CalculateScore(
    Worker worker,
    DaySchedule day)
{
    const int WeeklyHours = 40;

    int remainingHours =
        WeeklyHours - worker.AssignedHours;

    int occupancyFactor =
        day.OccupancyPercentage / 10;

    return remainingHours +
           ((int)worker.Seniority * occupancyFactor);
}

static bool HasMinimumRestHours(
    Worker worker,
    Shift candidateShift)
{
    const int MinRestHours = 12;

    foreach (var existingShift in worker.AssignedShifts)
    {
        double hoursAfterExisting =
            (candidateShift.StartTime - existingShift.EndTime)
                .TotalHours;

        double hoursBeforeExisting =
            (existingShift.StartTime - candidateShift.EndTime)
                .TotalHours;

        bool enoughRest =
            hoursAfterExisting >= MinRestHours
            ||
            hoursBeforeExisting >= MinRestHours;

        Console.WriteLine($"Worker: {worker.Name}");
        Console.WriteLine($"hoursAfterExisting: {hoursAfterExisting}");
        Console.WriteLine($"hoursBeforeExisting: {hoursBeforeExisting}");
        Console.WriteLine($"enoughRest: {enoughRest}");

        if (!enoughRest)
        {
            return true;
        }
    }

    return false;
}


// ------------ GENERATOR ------------

var sortedDays = schedule.Days
    .OrderByDescending(d => d.OccupancyPercentage)
    .ToList();

foreach (var day in sortedDays)
{
    Console.WriteLine($"\n=== {day.Date} ({day.OccupancyPercentage}%) ===");

    // Build candidate pool
    var validWorkers = workers
        .Where(w => w.IsActive)
        .ToList();

    foreach (var shift in day.Shifts)
    {
        // Rule 1 - One shift per day
        validWorkers.RemoveAll(worker =>
            day.Shifts.Any(s => s.AssignedWorker?.Id == worker.Id));

        // Rule 2 - Minimum rest hours
        validWorkers.RemoveAll(worker =>
        {
            if (worker.AssignedShifts.Count == 0)
                return false;

            return HasMinimumRestHours(worker, shift);
        });

        // Rule 3 - Worker request
        validWorkers.RemoveAll(worker =>
            worker.Requests.Any(r => r.Date == day.Date));


        if (!validWorkers.Any())
        {
            Console.WriteLine(
                $"No valid worker for {shift.ShiftType}");

            continue;
        }


        Worker? selectedWorker = null;
        int bestScore = int.MinValue;

        foreach (var worker in validWorkers)
        {
            int score = CalculateScore(worker, day);

            Console.WriteLine(
        $"{worker.Name} - Score: {score}");

            if (score > bestScore)
            {
                bestScore = score;
                selectedWorker = worker;
            }
        }


        // Assign worker
        shift.AssignedWorker = selectedWorker;

        // Update worker state
        selectedWorker.AssignedHours += 8;

        selectedWorker.AssignedShifts.Add(shift);

        Console.WriteLine(
            $"==> {shift.ShiftType} -> {selectedWorker.Name}");

    }

}


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