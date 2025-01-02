namespace MaReSy2.ConsumeModels
{
    public class MakeRental
    {
        public int? setId { get; set; }
        public int? productId { get; set; }
        public int? productAmount { get; set; }
        public DateOnly fromDate { get; set; }
        public DateOnly endDate { get; set; }
        public string? rentalNote { get; set; }
    }
}
