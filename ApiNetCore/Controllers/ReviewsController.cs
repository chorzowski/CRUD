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
    public class ReviewsController : Controller
    {
        private IReviewerRepository _reviewerRepository;
        private IReviewRepository _reviewRepository;
        private IBookRepository _bookRepository;
        public ReviewsController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository, IBookRepository bookRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
        }
        //api/reviews
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = _reviewRepository.GetAllReviews();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewsDto.Add(new ReviewDto
                {
                    Id = review.Id,
                    Headline = review.Headline,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                });
            }
            return Ok(reviewsDto);
        }
        [HttpGet("{reviewId}", Name = "GetReviewById")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReviewById(int reviewId)
        {
            if (!_reviewerRepository.Exists(reviewId))
                return BadRequest();

            var review = _reviewRepository.GetReviewById(reviewId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewDto = new ReviewDto()
            {
                Id = review.Id,
                Headline = review.Headline,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
            };
            return Ok(reviewDto);
        }

        //api/reviews/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsForABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var reviews = _reviewRepository.GetReviewsOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            {
                foreach (var review in reviews)
                {
                    reviewsDto.Add(new ReviewDto()
                    {
                        Id = review.Id,
                        Headline = review.Headline,
                        ReviewText = review.ReviewText,
                        Rating = review.Rating
                    });
                }
            }
            return Ok(reviewsDto);
        }

        //api/reviews/reviewId/book
        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBookOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var book = _reviewRepository.GetBookOfAReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest();

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                DatePublished = book.DatePublished
            };

            return Ok(bookDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateReview([FromBody]Review reviewToCreate)
        {
            if (reviewToCreate == null)
                return BadRequest();

            var review = _reviewRepository.GetAllReviews().Where(r => r.Id == reviewToCreate.Id).FirstOrDefault();

            if (review != null)
            {
                ModelState.AddModelError("", $"Review {reviewToCreate.Id} already exists");
                return StatusCode(422, ModelState);
            }

            if (!_reviewerRepository.Exists(reviewToCreate.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesn't exist !");

            if (!_bookRepository.BookExists(reviewToCreate.Book.Id))
                ModelState.AddModelError("", "Book doesn't exist !");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            //?
            reviewToCreate.Book = _bookRepository.GetBook(reviewToCreate.Book.Id);
            reviewToCreate.Reviewer = _reviewerRepository.GetReviewerById(reviewToCreate.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.CreateReview(reviewToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {reviewToCreate}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetReviewById", new { reviewId = reviewToCreate.Id }, reviewToCreate);
        }
        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReview(int reviewId, [FromBody]Review updatedReview)
        {
            if (updatedReview == null)
                return BadRequest();

            if (reviewId != updatedReview.Id)
                return BadRequest();

            if (!_reviewRepository.ReviewExists(updatedReview.Id))
                return NotFound();

            if (!_reviewerRepository.Exists(updatedReview.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesn't exist !");

            if (!_bookRepository.BookExists(updatedReview.Book.Id))
                ModelState.AddModelError("", "Book doesn't exist !");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            updatedReview.Book = _bookRepository.GetBook(updatedReview.Book.Id);
            updatedReview.Reviewer = _reviewerRepository.GetReviewerById(updatedReview.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.UpdateReview(updatedReview))
            {
                ModelState.AddModelError("", $"Something went wrong updating the review {updatedReview.Id}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var review = _reviewRepository.GetReviewById(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReview(review))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {review.Id}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
