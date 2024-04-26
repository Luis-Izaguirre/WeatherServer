using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CountryModel;
using WeatherServer.DTO;
using Microsoft.AspNetCore.Authorization;

namespace WeatherServer.Controllers
{
    //This is the url that you will invoke from the client.
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController(CountriesSilverContext context) : ControllerBase
    {

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await context.Cities.ToListAsync();
        }

        //Totday lecture 04/12/24
        //If not authenticated we get error 401 unauthorized, not logged in (unauthenticated), 403 Forbidden  (unauthorized), 404 page not found
        //We need an attribute called authorized,
        [Authorize]
        // GET: api/Population returns the same thing not just 
        [HttpGet("GetPopulation")]
        public async Task<ActionResult<IEnumerable<CountryPopulation>>> GetPopulation()
        {
            //Old way to query old synax
            //Await is like a query, because it
            //var x = await (from c in context.Countries
            //               select new CountryPopulation
            //        {
            //            Name = c.Name,
            //            CountryId = c.CountryId,
            //            //Population = c.Cities.Sum(t => t.Population)
            //        }).ToListAsync();
            //return x;
            // New way old syntax
            //    var x = from c in context.Countries
            //            select new CountryPopulation
            //            {
            //                Name = c.Name,
            //                CountryId = c.CountryId,
            //                //Population = c.Cities.Sum(t => t.Population)
            //            };
            //    return await x.ToListAsync();
            //}

            IQueryable<CountryPopulation> x = context.Countries.
                    Select( c => new CountryPopulation
                    {
                        Name = c.Name,
                        CountryId = c.CountryId,
                        Population = c.Cities.Sum(t => t.Population)
                    });

            return await x.ToListAsync();
        }

    }
}
