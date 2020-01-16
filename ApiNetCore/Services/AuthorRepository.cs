using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiNetCore.Models;

namespace ApiNetCore.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private BookDbContext _authorDbContext;
        public AuthorRepository(BookDbContext authorDbContext)
        {
            _authorDbContext = authorDbContext;
        }

        public bool AuthorExists(int authorId)
        {
            return _authorDbContext.Authors.Any(a => a.Id == authorId);
        }

        public bool CreateAuthor(Review review)
        {
            _authorDbContext.Add(review);
            return Save();
        }

        public bool DeleteAuthor(Review review)
        {
            _authorDbContext.Remove(review);
            return Save();
        }

        public ICollection<Author> GetAllAuthors()
        {
            return _authorDbContext.Authors.OrderBy(a => a.LastName).ToList();
        }

        public ICollection<Author> GetAuthorByABook(int bookId)
        {
            return _authorDbContext.BookAuthors.Where(b => b.BookId == bookId).Select(a => a.Author).ToList();            
        }

        public Author GetAuthorById(int authorId)
        {
            return _authorDbContext.Authors.Where(a => a.Id == authorId).FirstOrDefault();
        }

        public ICollection<Book> GetBooksByAuthor(int authorId)
        {
            return _authorDbContext.BookAuthors.Where(a => a.AuthorId == authorId).Select(b => b.Book).ToList();
        }

        public bool Save()
        {
            var save = _authorDbContext.SaveChanges();
            return save >= 0 ? true : false;
        }

        public bool UpdateAuthor(Review review)
        {
            _authorDbContext.Update(review);
            return Save();
        }
    }
}
