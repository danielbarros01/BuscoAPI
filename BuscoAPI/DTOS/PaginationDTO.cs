namespace BuscoAPI.DTOS
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;

        private int numberRecordsPerPage = 10;
        private readonly int maxNumberRecordsPerPage = 50;

        public int NumberRecordsPerPage
        {
            get => numberRecordsPerPage;
            set
            {
                numberRecordsPerPage = (value > maxNumberRecordsPerPage) 
                    ? maxNumberRecordsPerPage : value;
            }
        }
    }
}
