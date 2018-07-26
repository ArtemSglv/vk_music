using System.Data.Entity;

namespace VK_Music.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext():base("DatabaseContext")
        { }

        public DbSet<User> User { get; set; }
        public DbSet<Photo> PhotoList { get; set; }
    }
}