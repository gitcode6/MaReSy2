namespace MaReSy2_Api.Models.DTO.RentalDTO
{
    public class ActionDTO
    {
        public int action { get; set; }
        /*
        1 --> ablehnen,
        2 --> freigeben,
        2 --> ausliefern,
        3 --> zurücknahme,
         */



        public int rentalId { get; set; }
        public int actionUserId { get; set; }
    }
}
