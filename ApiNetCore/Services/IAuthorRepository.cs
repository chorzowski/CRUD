using ApiNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNetCore.Services
{
    public interface IAuthorRepository
    {
        ICollection<Author> GetAllAuthors();
        Author GetAuthorById(int authorId);
        ICollection<Author> GetAuthorByABook(int bookId);
        ICollection<Book> GetBooksByAuthor(int authorId);
        bool AuthorExists(int authorId);
        bool CreateAuthor(Review review);
        bool UpdateAuthor(Review review);
        bool DeleteAuthor(Review review);
        bool Save();
    }
}
