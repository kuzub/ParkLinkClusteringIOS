using System;

namespace ParkLinkClusteringIOS.Models
{
    public class Token
    {
        public String scope { get; set; }
        public String token_type { get; set; }
        public String access_token { get; set; }
        public Int32 expires_in { get; set; }
    }
}
