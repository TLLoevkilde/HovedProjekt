using Microsoft.EntityFrameworkCore;
using ResourceApi.Models;


namespace ResourceApi.Data
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options) { }

        public DbSet<Note> Notes => Set<Note>();
    }
}
