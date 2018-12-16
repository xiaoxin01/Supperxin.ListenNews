using System;

namespace Supperxin.ListenNews.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }
        public DateTime Created { get; set; }
        public DateTime CrawledTime { get; set; }
    }
}