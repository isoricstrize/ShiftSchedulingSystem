using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;
using ShiftSchedulingSystem.Models;

namespace ShiftSchedulingSystem.SeedData
{
    public static class WorkerSeed
    {
        public static List<Worker> CreateWorkers()
        {
            return new List<Worker>
            {
                new Worker
                {
                    Name = "Ana",
                    Seniority = SeniorityLevel.Senior,
                    //Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 19), RequestType = RequestType.DayOff}]
                },
                new Worker
                {
                    Name = "Lorena",
                    Seniority = SeniorityLevel.Mid,
                    //Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 19), RequestType = RequestType.DayOff}]

                },
                new Worker
                {
                    Name = "Andrej",
                    Seniority = SeniorityLevel.Junior,
                    //Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 17), RequestType = RequestType.DayOff}]
                },
                new Worker
                {
                    Name = "Andrea",
                    Seniority = SeniorityLevel.Mid
                },
                new Worker
                {
                    Name = "Stella",
                    Seniority = SeniorityLevel.Junior
                },
                new Worker
                {
                    Name = "Luka",
                    Seniority = SeniorityLevel.Mid,
                    //Requests = [new WorkerRequest{Date = new DateOnly(2026, 6, 16), RequestType = RequestType.DayOff}],
                    IsActive = true
                }
            };
        }

        // TODO
        /*public static List<Worker> CreateWorkersFromConsole()
        {
            
        }*/
    }
}