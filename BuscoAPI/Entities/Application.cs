using BuscoAPI.DTOS.Worker;
using System.ComponentModel.DataAnnotations.Schema;

/**
 *Status: 
 *aceptado = 1
 *rechazado = 0
 *pendiente = null
 **/

namespace BuscoAPI.Entities
{
    public class Application
    {
        public int Id { get; set; }

        [Column("Worker_user_id")]
        public int WorkerUserId { get; set; }
        [Column("Proposal_id")]
        public int ProposalId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool? Status { get; set; } = null;

        public Worker? Worker { get; set; }

    /*In the future add budget and message, in version 2*/
    }
}
