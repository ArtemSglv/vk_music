using System.Data.Entity;

namespace VK_Music.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext():base("DatabaseContext")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> PhotoList { get; set; }
        public DbSet<Album> Albums { get; set; }
    }
}