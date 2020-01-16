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
    public class ReviewersController : Controller
    {
        private IReviewerRepository _reviewerRepository;
        private IReviewRepository _reviewRepository;
        public ReviewersController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }

        //api/reviewers
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetAllReviewers()
        {
            var allReviewers = _reviewerRepository.GetAllReviewers();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<ReviewerDto> reviewers = new List<ReviewerDto>();
            foreach (var reviewer in allReviewers)
            {
                reviewers.Add(new ReviewerDto
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName
                });
            }
            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}", Name = "GetReviewerById")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerById(int reviewerId)
        {
            if (!_reviewerRepository.Exists(reviewerId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewerById(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

        //[HttpGet("{reviewerId}/reviews")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(200, Type = typeof(ReviewDto))]
        //IActionResult GetReviewsByReviewer(int reviewerId)
        //{
        //    var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    List<ReviewDto> ReviewDto = new List<ReviewDto>();

        //    foreach(var reviwDto in reviews)
        //    {
        //        ReviewDto.Add(new ReviewDto
        //        {
        //            Id = reviwDto.Id,
        //            Headline = reviwDto.Headline,
        //            ReviewText = reviwDto.ReviewText,
        //            Rating = reviwDto.Rating
        //        });
        //    }

        //    return Ok(ReviewDto);
        //}

        //[HttpGet("{reviewId}/reviewer")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(200, Type = typeof(ReviewDto))]
        //public IActionResult GetReviewerByReview(int reviewId)
        //{
        //    var reviwer = _reviewerRepository.GetReviewerByReview(reviewId);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Reviewer reviewerDto = new Reviewer()
        //    {
        //        Id = reviwer.Id,
        //        FirstName = reviwer.FirstName,
        //        LastName = reviwer.LastName,
        //        Reviews = reviwer.Reviews
        //    };
        //    return Ok(reviewerDto);
        //}

        [HttpGet("reviewer/{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer (int reviewerId)
        {
            if (!_reviewerRepository.Exists(reviewerId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewerById(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.Exists(reviewerId))
                return NotFound();

            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewersDto = new List<ReviewDto>();
            {
                foreach(var review in reviews)
                {
                    reviewersDto.Add(new ReviewDto()
                    {
                        Id = review.Id,
                        Headline = review.Headline,
                        ReviewText = review.ReviewText,
                        Rating = review.Rating
                    });
                }
            }
            return Ok(reviewersDto);
        }

        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var reviwer = _reviewerRepository.GetReviewerByReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest();

            var reviewerDto = new ReviewerDto()
            {
                Id = reviwer.Id,
                FirstName = reviwer.FirstName,
                LastName = reviwer.LastName
            };

            return Ok(reviewerDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult CreateReviewer([FromBody]Reviewer reviewerToCreate)
        {
            if (reviewerToCreate == null)
                return BadRequest();
               
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {reviewerToCreate}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetReviewerById", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody]Reviewer reviewer)
        {
            if (reviewer == null)
                return BadRequest(ModelState);

            if (reviewerId != reviewer.Id)
                return BadRequest(ModelState);

            if (!_reviewerRepository.Exists(reviewer.Id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.UpdateReviewer(reviewer))
            {
                ModelState.AddModelError("", $"Something went wrong updating {reviewer.FirstName} {reviewer.LastName}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.Exists(reviewerId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewerById(reviewerId);
            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);

             if (!ModelState.IsValid)
                return BadRequest();

            if (!_reviewerRepository.DeleteReviewer(reviewer))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {reviewer.FirstName} {reviewer.LastName}");
                return StatusCode(500, ModelState);
            }

            if (!_reviewRepository.DeleteReviews(reviews.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleting reviews");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
