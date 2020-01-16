using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ApiNetCore.Models;

namespace ApiNetCore.DataBase
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<DbContext> options) : base(options)
        {
            Database.Migrate();
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Author> Books { get; set; }
        public virtual DbSet<Review> Books { get; set; }
        public virtual DbSet<Reviewer> Books { get; set; }
        public virtual DbSet<Category> Books { get; set; }
        public virtual DbSet<Country> Books { get; set; }
        public virtual DbSet<BookAuthor> Books { get; set; }
        public virtual DbSet<BookCategory> Books { get; set; }
    }
}
