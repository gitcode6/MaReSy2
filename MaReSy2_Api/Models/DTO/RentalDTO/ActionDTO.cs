namespace MaReSy2_Api.Models.DTO.RentalDTO
{
    public class ActionDTO
    {
        public int action { get; set; }
        /*
        1 --> ablehnen,
        2 --> freigeben,
        3 --> ausliefern,
        4 --> zurücknahme,
         */
        public int rentalId { get; set; }
    }
}
