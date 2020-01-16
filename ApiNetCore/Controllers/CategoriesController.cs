using ApiNetCore.Dtos;
using ApiNetCore.Models;
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
    public class CategoriesController : Controller
    {
        private ICategoriesRepository _categoriesRepository;
        private IBookRepository _bookRepository;

        public CategoriesController(ICategoriesRepository categoriesRepository, IBookRepository bookRepository)
        {
            _categoriesRepository = categoriesRepository;
            _bookRepository = bookRepository;
        }

        //api/categories
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetAllCategories()
        {
            var allCategories = _categoriesRepository.GetCategories().ToArray();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<CategoryDto> categoriesDto = new List<CategoryDto>();
            foreach (var category in allCategories)
            {
                categoriesDto.Add(new CategoryDto
                {
                    CategoryId = category.Id,
                    Name = category.Name
                });
            }
            return Ok(categoriesDto);
        }
        
        [HttpGet("{categoryId}", Name = "GetCategoryById")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        public IActionResult GetCategoryById(int categoryId)
        {
            var category = _categoriesRepository.GetCategoryById(categoryId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryDto = new CategoryDto()
            {
                CategoryId = category.Id,
                Name = category.Name
            };
            return Ok(categoryDto);
        }

        //api/categories/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetAllCategoriesForABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var categories = _categoriesRepository.GetAllCategoriesOfABook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(new CategoryDto
                {
                    CategoryId = category.Id,
                    Name = category.Name
                });
            }
            return Ok(categoriesDto);
        }

        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetAllBooksForCategory(int categoryId)
        {
            var books = _categoriesRepository.GetAllBooksForCategory(categoryId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<BookDto> booksDto = new List<BookDto>();

            foreach(var bookDto in books)
            {
                booksDto.Add(new BookDto
                {
                    Id = bookDto.Id,
                    Title = bookDto.Title,
                    Isbn = bookDto.Isbn,
                    DatePublished = bookDto.DatePublished
                });
            }
            return Ok(booksDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody]Category categoryToCreate)
        {
            if (categoryToCreate == null)
                return BadRequest(ModelState);

            var category = _categoriesRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryToCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if(category != null)
            {
                ModelState.AddModelError("", $"Category {category.Name} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoriesRepository.CreateCategory(categoryToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {categoryToCreate}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoryById", new { categoryId = categoryToCreate.Id }, categoryToCreate );
        } 

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCategory(int categoryId, [FromBody]Category category)
        {
            if (category == null)
                return BadRequest(ModelState);

            if(categoryId != category.Id)
                return
                BadRequest(ModelState);

            if (!_categoriesRepository.CategoryExists(categoryId))
                return NotFound();

            if(!_categoriesRepository.IsDuplicateCategoryName(categoryId, category.Name))
            {
                ModelState.AddModelError("", $"Category {category.Name} alredy exists");
                return StatusCode(422, ModelState);
            }

            if (!_categoriesRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong updating {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoriesRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _categoriesRepository.GetCategories().Where(c => c.Id == categoryId).FirstOrDefault();

            if(_categoriesRepository.GetAllBooksForCategory(category.Id).Count > 0)
            {
                ModelState.AddModelError("", $"Category {category.Name} " +
                    "can not be deleted bacause it is used by at least one book");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_categoriesRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        }
}