using System;

namespace Supperxin.ListenNews.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            this.Created = DateTime.Now;
        }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
