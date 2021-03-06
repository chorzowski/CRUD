﻿using ApiNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNetCore.Services
{
    public interface ICategoriesRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategoryById(int categoryId);
        ICollection<Category> GetAllCategoriesOfABook(int bookId);
        ICollection<Book> GetAllBooksForCategory(int categoryId);
        bool CategoryExists(int categoryId);
        bool IsDuplicateCategoryName(int categoryId, string categoryName);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}
