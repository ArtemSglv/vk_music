using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VK_Music.Models
{
    public class Photo
    {
        public long PhotoId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public int Likes { get; set; }
    }
}