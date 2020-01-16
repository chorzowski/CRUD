using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiNetCore.Models;

namespace ApiNetCore.Services
{
    public class BookRepository : IBookRepository
    {
        private BookDbContext _bookDbContext;
        public BookRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        } 

        public bool BookExists(int bookId)
        {
            return _bookDbContext.Books.Any(b => b.Id == bookId);
        }

        public bool BookExists(string ISBN)
        {
            return _bookDbContext.Books.Any(b => b.Isbn == ISBN);
        }

        public ICollection<Book> GetAllBooks()
        {
            return _bookDbContext.Books.OrderBy(b => b.Title).ToList();
        }

        public Book GetBook(int bookId)
        {
            return _bookDbContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBook(string ISBN)
        {
            return _bookDbContext.Books.Where(b => b.Isbn == ISBN).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var reviews = _bookDbContext.Reviews.Where(r => r.Book.Id == bookId);

            if (reviews.Count() <= 0)
                return 0;
            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
        }

        public bool IsDuplicateISBN(int bookId, string ISBN)
        {
            var book = _bookDbContext.Books.Where(b => b.Isbn.Trim().ToUpper() == ISBN.Trim().ToUpper()
            && b.Id != bookId).FirstOrDefault();

            return book == null ? false : true;
        }
    }
}
