namespace MaReSy2.ConsumeModels
{
    public class CreateSetModel
    {
        public string setname {  get; set; }
        public string setdescription { get; set; }
        public bool setactive { get; set; }
        public List<CreateSetProductAmount>? products { get; set; }
    }
}
