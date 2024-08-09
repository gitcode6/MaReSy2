using System.Reflection.Metadata;

namespace MaReSy2.Pages
{
    public class Produkte
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public Blob productImage { get; set; }
        public int productActive { get; set; }
        public int productAmount { get; set; }
    }
}
