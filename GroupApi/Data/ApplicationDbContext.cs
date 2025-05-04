using GroupApi.Entities;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Books;
using GroupApi.Entities.Oders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GroupApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet properties for all entities
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookFormat> BookFormats { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BookMark> BookMarks { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Format> Formats { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OtpRecord> OtpRecords { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PendingUser> PendingUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define composite key for BookAuthor
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.AuthorId, ba.BookId });

            // Define composite key for BookCategory
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.CategoryId, bc.BookId });

            // Define composite key for BookFormat
            modelBuilder.Entity<BookFormat>()
                .HasKey(bf => new { bf.FormatId, bf.BookId });

            // Define composite key for BookGenre
            modelBuilder.Entity<BookGenre>()
                .HasKey(bg => new { bg.BookId, bg.GenreId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}