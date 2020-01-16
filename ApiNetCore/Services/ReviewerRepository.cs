using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiNetCore.Models;

namespace ApiNetCore.Services
{
    public class ReviewerRepository : IReviewerRepository
    {
        private BookDbContext _reviwerDbContext;
        public ReviewerRepository(BookDbContext reviewerDbContext)
        {
            _reviwerDbContext = reviewerDbContext;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _reviwerDbContext.Reviewers.Add(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            _reviwerDbContext.Reviewers.Remove(reviewer);
            return Save();
        }

        public bool Exists(int reviewerId)
        {
            return _reviwerDbContext.Reviewers.Where(r => r.Id == reviewerId).Any();
        }

        public ICollection<Reviewer> GetAllReviewers()
        {
            return _reviwerDbContext.Reviewers.OrderBy(r => r.LastName).ToArray();
        }

        public Reviewer GetReviewerById(int reviewerId)
        {
            return _reviwerDbContext.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public Reviewer GetReviewerByReview(int reviewId)
        {
            var reviewerId = _reviwerDbContext.Reviews.Where(r => r.Id == reviewId).Select(s => s.Reviewer.Id).FirstOrDefault();
            return _reviwerDbContext.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _reviwerDbContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();          
        }

        public bool ReviewExists(int reviewId)
        {
           return _reviwerDbContext.Reviews.Where(r => r.Id == reviewId).Any();
        }

        public bool Save()
        {
            var save = _reviwerDbContext.SaveChanges();
            return save >= 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _reviwerDbContext.Reviewers.Update(reviewer);
            return Save();
        }
    }
}
