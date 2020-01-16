using ApiNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNetCore.Services
{
    public interface IBookRepository
    {
        ICollection<Book> GetAllBooks();
        Book GetBook(int bookId);
        Book GetBook(string ISBN);
        bool BookExists(int bookId);
        bool BookExists(string ISBN);
        bool IsDuplicateISBN(int bookId, string ISBN);
        decimal GetBookRating(int bookId);
    }
}
