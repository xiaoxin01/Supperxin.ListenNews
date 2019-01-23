using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supperxin.ListenNews.Models
{
    public class Item
    {
        public Item()
        {
            this.CrawledTime = DateTime.Now;
        }
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }
        public DateTime Created { get; set; }
        public DateTime CrawledTime { get; set; }
        public int AudioStatus { get; set; }
        public string AudioErrorMessage { get; set; }

        [NotMapped]
        public int FavoriteId { get; set; }

        public bool Favorited { get { return 0 != FavoriteId; } }
    }
}
