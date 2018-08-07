using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VK_Music.Models
{
    // route to photo: /Download/<id_user>/<id_album>/<id_photo>
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PhotoId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public int Likes { get; set; }

        public long AlbumId { get; set; }
        public Album Album { get; set; }
    }
}