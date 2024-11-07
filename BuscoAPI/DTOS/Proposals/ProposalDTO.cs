using BuscoAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS.Proposals
{
    public class ProposalDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public DateTime Date { get; set; }
        public decimal MinBudget { get; set; }
        public decimal MaxBudget { get; set; }
        public string Image { get; set; }
        public bool? Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int userId { get; set; }
        public Profession profession { get; set; }
        public List<ApplicationDTO>? Applications { get; set; }
    }
}
