using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiNetCore.Models;

namespace ApiNetCore.Services
{
    public class CountryRepository : ICountryRepository
    {
        private BookDbContext _bookDbContext;

        public CountryRepository(BookDbContext bookDbContext)
        {
            _bookDbContext = bookDbContext;
        }

        public bool CountryExists(int countryId)
        {
            return _bookDbContext.Countries.Any(a => a.Id == countryId);
        }

        public bool CreateCountry(Country country)
        {
            _bookDbContext.Add(country);
            return Save();
        }

        public bool DeleteCountry(Country country)
        {
            _bookDbContext.Remove(country);
            return Save();
        }

        public ICollection<Author> GetAuthorsFromCountry(int countryId)
        {
            return _bookDbContext.Authors.Where(a => a.Country.Id == countryId).ToList();
        }

        public ICollection<Country> GetCountries()
        {
            return _bookDbContext.Countries.OrderBy(o => o.Name).ToList();
        }

        public Country GetCountry(int countryId)
        {
            return _bookDbContext.Countries.Where(c => c.Id == countryId).FirstOrDefault();
        }

        public Country GetCountryOfAnAuthor(int authodId)
        {
            return _bookDbContext.Authors.Where(a => a.Id == authodId).Select(s => s.Country).FirstOrDefault();
        }

        public bool IsDuplicateCountryName(int countryId, string countryName)
        {
            var name = _bookDbContext.Countries.Where(c => c.Name.Trim().ToUpper() == 
            countryName.Trim().ToUpper() && c.Id != countryId).FirstOrDefault();
            return name == null ? false : true;
        }

        public bool Save()
        {
            var save = _bookDbContext.SaveChanges();
            return save >= 0 ? true : false;
        }

        public bool UpdateCountry(Country country)
        {
            _bookDbContext.Update(country);
            return Save();
        }
    }
}
