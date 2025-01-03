namespace MaReSy2.ConsumeModels
{
    public class RentalActions
    {
        public static readonly Dictionary<string, (int? PreviousAction, int? NextAction)> Actions = new()
        {
            {"storniert", (null,null) },
            {"abgelehnt", (null, 0) },
            {"angefordert", (-1,1) },
            {"freigegeben", (0, 2) },
            {"ausgeliefert", (1, 3) },
            {"zurückgegeben", (2, null) }
        };
    }
}
