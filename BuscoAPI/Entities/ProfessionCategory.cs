namespace BuscoAPI.Entities
{
    public class ProfessionCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Profession> Proffesions { get; set; }
    }
}
