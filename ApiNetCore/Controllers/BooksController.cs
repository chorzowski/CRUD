using ApiNetCore.Dtos;
using ApiNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type=typeof(IEnumerable<BookDto>))]
        public IActionResult GetAllBooks()
        {
            var books = _bookRepository.GetAllBooks();

            if (!ModelState.IsValid)
                return NotFound();

            var bookDto = new List<BookDto>();

            foreach(var book in books)
            {
                bookDto.Add(new BookDto()
                {
                    Id = book.Id,
                    Title = book.Title,
                    DatePublished = book.DatePublished,
                    Isbn = book.Isbn
                });
            }
            return Ok(bookDto);
        }

        [HttpGet("{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type=typeof(BookDto))]
        public IActionResult GetBookById(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var book = _bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest();

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                DatePublished = book.DatePublished,
                Isbn = book.Isbn
            };

            return Ok(bookDto);
        }

        [HttpGet("isbn/{isbn}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBookByIsbn(string isbn)
        {
            if (!_bookRepository.BookExists(isbn))
                return NotFound();

            var book = _bookRepository.GetBook(isbn);

            if (!ModelState.IsValid)
                return BadRequest();

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                DatePublished = book.DatePublished,
                Isbn = book.Isbn
            };

            return Ok(bookDto);
        }

        [HttpGet("{bookId}/rating")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var rating = _bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(rating);
        }
    }
}
