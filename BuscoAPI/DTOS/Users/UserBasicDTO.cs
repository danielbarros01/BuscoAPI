﻿using BuscoAPI.DTOS.Worker;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Users
{
    public class UserBasicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Image { get; set; }
        public WorkerWithoutUser Worker { get; set; }
    }
}
