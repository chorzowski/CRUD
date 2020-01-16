using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiNetCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiNetCore.Services
{
    public class ReviewRepository : IReviewRepository
    {
        private BookDbContext _reviewDbContext;

        public ReviewRepository(BookDbContext reviewDbContext)
        {
            _reviewDbContext = reviewDbContext;
        }

        public ICollection<Review> GetAllReviews()
        {
            return _reviewDbContext.Reviews.OrderBy(r => r.Rating).ToList();
        }

        public Book GetBookOfAReview(int reviewId)
        {
            var bookId = _reviewDbContext.Reviews.Where(r => r.Id == reviewId).Select(b => b.Book.Id).FirstOrDefault();
            return _reviewDbContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Review GetReviewById(int reviewId)
        {
            return _reviewDbContext.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviewsOfABook(int bookId)
        {
            return _reviewDbContext.Reviews.Where(r => r.Book.Id == bookId).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _reviewDbContext.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            var save = _reviewDbContext.SaveChanges();
            return save >= 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _reviewDbContext.Update(review);
            return Save();
        }

        public bool CreateReview(Review review)
        {
            _reviewDbContext.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _reviewDbContext.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _reviewDbContext.RemoveRange(reviews);
            return Save();
        }
    }
}
