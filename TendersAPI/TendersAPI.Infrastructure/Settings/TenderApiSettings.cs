namespace TendersApi.Infrastucture.Settings
{
    public class TenderApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int ConcurrencyLimit { get; set; } = 5;
    }
}
