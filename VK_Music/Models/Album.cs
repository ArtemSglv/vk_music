using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VK_Music.Models
{
    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Title { get; set; }

        public List<Photo> Photos { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}