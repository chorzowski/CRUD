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
    public class CountriesController : Controller
    {
        private ICountryRepository _countryRepository;
        private IAuthorRepository _authorRepository;

        public CountriesController(ICountryRepository countryRepository, IAuthorRepository authorRepository)
        {
            _countryRepository = countryRepository;
            _authorRepository = authorRepository;
        }

        //api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200,Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult getAllCountries()
        {
            var countries = _countryRepository.GetCountries().ToArray();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countriesDto = new List<CountryDto>();
            foreach(var country in countries)
            {
                countriesDto.Add(new CountryDto
                {
                    Id = country.Id,
                    Name = country.Name
                }
                );
            }
            return Ok(countriesDto);
        }
        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult getCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };

            return Ok(countryDto);
        }
        [HttpGet("author/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult getCountryOfAnAuthor(int authorId)
        {
           
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryDto = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name
            };
            return Ok(countryDto);
        }

        //api/countries/countryId/authors
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]//to do
        public IActionResult GetAuthorsFromACountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var authors = _countryRepository.GetAuthorsFromCountry(countryId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<AuthorDto> authorsDto = new List<AuthorDto>();
            
            foreach(var author in authors)
            {
                authorsDto.Add(
                    new AuthorDto
                    {
                        Id = author.Id,
                        FirstName = author.FirstName,
                        LastName = author.LastName
                    }
                );
            }
            return Ok(authorsDto);
        }

        //api/countries
        [HttpPost]
        [ProducesResponseType(201, Type=typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateACountry([FromBody]Country countryFromBody)
        {
            if (countryFromBody == null)
                return BadRequest(ModelState);

            var country = _countryRepository.GetCountries().Where(c => c.Name.Trim().ToUpper() == countryFromBody.Name).FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", $"Country {countryFromBody} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_countryRepository.CreateCountry(countryFromBody))
            {
                ModelState.AddModelError("", $"Something went wrong saving {countryFromBody}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCountry", new { countryId = countryFromBody.Id }, countryFromBody);
        }  

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult UpdateCountry(int countryId, [FromBody]Country updatedCountry)
        {
            if (updatedCountry == null)
                return BadRequest(ModelState);

            if (countryId != updatedCountry.Id)
                return BadRequest(ModelState);

            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            if(_countryRepository.IsDuplicateCountryName(countryId, updatedCountry.Name))
            {
                ModelState.AddModelError("", $"Country {updatedCountry.Name} alredy exists");
                return StatusCode(422, ModelState);
            }

            if (!_countryRepository.UpdateCountry(updatedCountry))
            {
                ModelState.AddModelError("", $"Something went wrong updating {updatedCountry.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);

            if(_countryRepository.GetAuthorsFromCountry(countryId).Count > 0)
            {
                ModelState.AddModelError("", $"Country {country.Name} " +
                    "can not be deleted bacause it is used by at least one author");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_countryRepository.DeleteCountry(country))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {country.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
