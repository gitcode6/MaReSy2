using System;
using System.Text.Json.Serialization;

namespace MaReSy2.ConsumeModels
{
    public class CreateSingleProductModel
    {
        
        public string singleProductName {  get; set; }
        public string singleProductSerialNumber { get; set; }
        public bool singleProductActive { get; set; }
        public int productId { get; set; }
    }
}
