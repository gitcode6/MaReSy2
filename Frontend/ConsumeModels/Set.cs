namespace MaReSy2.ConsumeModels
{
    public class Set
    {
        public int setId { get; set; }
        public string setname { get; set; }
        public string? setdescription { get; set; }
        public bool setactive { get; set; }
        public string? productimageLink { get; set; }
        public List<Product>? products { get; set; }

    }
}
