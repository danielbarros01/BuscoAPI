﻿using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BuscoAPI.DTOS.Users;

namespace BuscoAPI.DTOS.Worker
{
    public class WorkerDTO
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public int YearsExperience { get; set; }
        public string WebPage { get; set; }
        public string Description { get; set; }
        public float? AverageQualification { get; set; }
        public int? NumberOfQualifications { get; set; }
        public List<Profession> Professions { get; set; }
        public UserWithoutWorker User { get; set; }
    }
}
