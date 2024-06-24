using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoAPI.Entities
{
    public class Profession
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Column("Category_id")]
        public int CategoryId { get; set; }
        public ProfessionCategory Category { get; set; }
    }
}
