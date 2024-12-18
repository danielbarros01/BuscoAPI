﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BuscoAPI.Entities
{
    public class Proposal
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public DateTime Date { get; set; }
        [Column("min_budget")]
        public decimal MinBudget { get; set; }
        [Column("max_budget")]
        public decimal MaxBudget { get; set; }
        [Column("image_path")]
        public string Image { get; set; }
        public bool? Status { get; set; }
        public Point Ubication { get; set; }

        [Column("user_id")]
        public int userId { get; set; }

        public User user { get; set; }

        [Column("profession_id")]
        public int professionId { get; set; }

        public Profession? profession { get; set; }


        public List<Application>? Applications { get; set; }

        //Future
        [Column("limit_date")]
        [AllowNull]
        public DateTime LimitDate { get; set; }
        [Column("finish_date")]
        [AllowNull]
        public DateTime FinishDate { get; set; }
    }
}
