using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supperxin.ListenNews.Models
{
    public class ListenHistory
    {
        public ListenHistory()
        {
            this.Created = DateTime.Now;
        }
        public int Id { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
    }
}