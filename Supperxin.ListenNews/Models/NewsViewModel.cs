namespace Supperxin.ListenNews.Models
{
    public class NewsViewModel
    {
        public DateCount[] Dates { get; set; }
        public Item[] News { get; set; }
    }

    public class DateCount
    {
        public string DateString { get; set; }
        public int Count { get; set; }
        public bool Visited { get; set; }
    }
}
