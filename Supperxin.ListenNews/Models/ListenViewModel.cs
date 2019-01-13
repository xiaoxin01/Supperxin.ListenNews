using System;
using System.Collections.Generic;

namespace Supperxin.ListenNews.Models
{
    public class ListenViewModel
    {
        public List<Audio> Audios { get; set; }
        public int TotalUnlisten { get; set; }
    }
    public class Audio
    {
        public int id { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string url { get; set; }
        public DateTime created { get; set; }
    }
}
