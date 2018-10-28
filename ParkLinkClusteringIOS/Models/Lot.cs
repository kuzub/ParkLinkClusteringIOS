namespace ParkLinkClusteringIOS.Models
{
    public class Lot
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string info { get; set; }
        public string infoUrl { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public int type { get; set; }
        public float utcOffset { get; set; }
        public System.TimeSpan? minTime { get; set; }
        public System.TimeSpan? maxTime { get; set; }
        public bool active { get; set; }
        public int discountType { get; set; }
        public float minPrice { get; set; }
        public System.TimeSpan? rateResolution { get; set; }
        public float hourlyRate { get; set; }
        public float discountedRate { get; set; }
    }
}
