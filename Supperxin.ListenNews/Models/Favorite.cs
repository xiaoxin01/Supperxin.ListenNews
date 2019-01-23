using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Supperxin.ListenNews.Models
{
    public class Favorite : BaseModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
