using BuscoAPI.DTOS.Proposals;
using BuscoAPI.DTOS.Worker;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.DTOS
{
    public class ApplicationDTO
    {
        public int Id { get; set; }
        public int WorkerUserId { get; set; }
        public int ProposalId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool? Status { get; set; } = null;
        public WorkerDTO? Worker { get; set; }
        public ProposalDTO? Proposal { get; set; }
    }
}
