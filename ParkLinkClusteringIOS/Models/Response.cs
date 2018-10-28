using System;
namespace ParkLinkClusteringIOS.Models
{
    public abstract class Response
    {
        public Boolean hasError { get; set; }
        //public Output.Error error { get; set; }
        public Boolean triggerReview { get; set; }
        //public ReviewTrigger reviewTrigger { get; set; }
    }

    public class Response<T> : Response where T : class
    {
        public T result { get; set; }
    }
}
