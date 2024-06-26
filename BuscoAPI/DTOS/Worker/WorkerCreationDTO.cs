﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BuscoAPI.DTOS.Worker
{
    public class WorkerCreationDTO
    {
        [Required(ErrorMessage = "El título es requerido.")]
        [MaxLength(45, ErrorMessage = "El título debe tener como máximo 45 caracteres.")]
        public String Title { get; set; }

        [Required(ErrorMessage = "Los años de experiencia son requeridos.")]
        [Range(0, 99, ErrorMessage = "Los años de experiencia deben ser mayores o iguales a cero.")]
        public int YearsExperience { get; set; }

        [MaxLength(45, ErrorMessage = "La página web debe tener como máximo 45 caracteres.")]
        public String? WebPage { get; set; }

        [Required(ErrorMessage = "La descripción es requerida.")]
        [MaxLength(255, ErrorMessage = "La descripción debe tener como máximo 255 caracteres.")]
        public String Description { get; set; }

        public int ProfessionId { get; set; }
    }
}
