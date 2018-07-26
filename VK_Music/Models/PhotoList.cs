using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VK_Music.Models
{
    public class PhotoList
    {
        public PhotoList()
        {
            isChecked = false;
            photo = new Photo();
        }
        public Photo photo;
        public bool isChecked;
    }
}