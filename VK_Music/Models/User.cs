using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VK_Music.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Tocken { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }        

        public List<Photo> PhotoList { get; set; }
    }
}