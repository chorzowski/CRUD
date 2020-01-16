using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiNetCore.Models;

namespace ApiNetCore.Services
{
    public class CategoriesRepository : ICategoriesRepository
    {
        BookDbContext _categoryDbContext;
        public CategoriesRepository(BookDbContext categoryDbContext)
        {
            _categoryDbContext = categoryDbContext;
        }

        public ICollection<Book> GetAllBooksForCategory(int categoryId)
        {
            return _categoryDbContext.BookCategories.Where(c => c.CategoryId == categoryId).Select(s => s.Book).ToList(); ;
        }

        public ICollection<Category> GetCategories()
        {
            return _categoryDbContext.Categories.OrderBy(o => o.Name).ToArray();
        }

        public ICollection<Category> GetAllCategoriesOfABook(int bookId)
        {
            return _categoryDbContext.BookCategories.Where(b => b.BookId == bookId).Select(s => s.Category).ToList();
        }

        public Category GetCategoryById(int categoryId)
        {
            return _categoryDbContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }

        public bool CategoryExists(int categoryId)
        {
            return _categoryDbContext.Categories.Where(c => c.Id == categoryId).Any();
        }

        public bool IsDuplicateCategoryName(int categoryId, string categoryName)
        {
            var category = _categoryDbContext.Categories.Where(c => c.Name.Trim().ToUpper() == 
            categoryName && c.Id != categoryId).FirstOrDefault();

            return category == null ? true : false;
        }

        public bool CreateCategory(Category category)
        {
            _categoryDbContext.Add(category);
            return Save();
        }

        public bool UpdateCategory(Category category)
        {
            _categoryDbContext.Update(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _categoryDbContext.Remove(category);
            return Save();
        }

        public bool Save()
        {
            var save = _categoryDbContext.SaveChanges();
            return save >= 0 ? true : false;
        }
    }
}
