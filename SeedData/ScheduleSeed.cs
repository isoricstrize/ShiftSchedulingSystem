using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShiftSchedulingSystem.Enums;
using ShiftSchedulingSystem.Models;

namespace ShiftSchedulingSystem.SeedData
{
    public static class ScheduleSeed
    {
        public static Schedule Create(DateOnly startDate, List<int> occupancies, List<(int Morning, int Afternoon, int Night)> workloads)
        {
            var days = new List<DaySchedule>();

            for (int i = 0; i < occupancies.Count; i++)
            {
                var date = startDate.AddDays(i);

                days.Add(new DaySchedule
                {
                    Date = date,
                    OccupancyPercentage = occupancies[i],
                    Shifts = CreateShifts(date, workloads[i])
                });
            }

            return new Schedule
            {
                Days = days
            };
        }

        private static List<Shift> CreateShifts(DateOnly date, (int Morning, int Afternoon, int Night) workload)
        {
            return
            [
                new Shift
                {
                    ShiftType = ShiftType.Morning,
                    StartTime = date.ToDateTime(new TimeOnly(6, 0)),
                    EndTime = date.ToDateTime(new TimeOnly(14, 0)),
                    WorkLoad = workload.Morning
                },

                new Shift
                {
                    ShiftType = ShiftType.Afternoon,
                    StartTime = date.ToDateTime(new TimeOnly(14, 0)),
                    EndTime = date.ToDateTime(new TimeOnly(22, 0)),
                    WorkLoad = workload.Afternoon
                },

                new Shift
                {
                    ShiftType = ShiftType.Night,
                    StartTime = date.ToDateTime(new TimeOnly(22, 0)),
                    EndTime = date.ToDateTime(new TimeOnly(6, 0))
                        .AddDays(1),
                    WorkLoad = workload.Night
                }
            ];
        }


    }
}