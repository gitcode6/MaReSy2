namespace MaReSy2_Api.Models.DTO.SetDTO
{
    public class UpdateSetDTO
    {
        public string? Setname { get; set; }

        public string? Setdescription { get; set; }

        public bool? Setactive { get; set; }

        public List<SetProductAssignDTO>? setProductAssignDTOs { get; set; }

    }
}
