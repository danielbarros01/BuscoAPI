﻿using BuscoAPI.DTOS.Users;
using BuscoAPI.Entities;

namespace BuscoAPI.DTOS.Worker
{
    public class WorkerApplicationDTO
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public int YearsExperience { get; set; }
        public string WebPage { get; set; }
        public string Description { get; set; }
        public List<WorkersProfessions> WorkersProfessions { get; set; }
        public UserApplicationDTO? User { get; set; }
    }
}
