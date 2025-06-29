﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO.City;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly IUnitOfWork work;
        private readonly IMapper map;

        public CityController(IUnitOfWork work , IMapper _map)
        {
            this.work = work;
            map = _map;
        }
        [HttpGet]
        public  async Task <IActionResult> GetAllCitiesAsync()
        {
            var cities = await work.CityRepository.GetCitiesWithGovernmentsNameAsync();
            if (cities == null || !cities.Any())
            {
                return NotFound("No cities found.");
            }
            var citiesDTO= map.Map<List<CityDTO>>(cities);
            return Ok(citiesDTO);
        }

        [HttpGet("{id:int}")]
        public  async Task<IActionResult> GetById(int id)
        {
            var city =  await work.CityRepository.GetCityWithGovernmentByIdAsync(id);
            if (city == null)
            {
                return NotFound($"City with id {id} not found.");
            }
            var cityDTO = map.Map<CityDTO>(city);
            return Ok(cityDTO);
        }
        [HttpGet("GetCityByName/{name}")]
        public async Task<IActionResult> GetCityByName(string name)
        {
            var city = await work.CityRepository.GetCityWithName(name);
            if(city == null)
            
                return NotFound($"City with name {name} not found.");
            
            var mycity = map.Map<CityDTO>(city);
            return Ok(mycity);
        }
        [HttpPost]
        public async Task<IActionResult> AddCity(CityDTO cdto)
        {
            if (cdto == null)
            {
                return BadRequest("City data is null.");
            }
            var city = map.Map<City>(cdto);
            var addedCity =   work.CityRepository.Add(city);
            await  work.SaveAsync();
            var addedCityDTO = map.Map<CityDTO>(city);
            return Ok(addedCityDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id  , CityDTO cdto)
        {
            var city =   await work.CityRepository.GetCityWithGovernmentByIdAsync(id);
            if (city == null)
            
                return NotFound($"City with id {id} not found.");
            
            if(!ModelState.IsValid)
            
                 return BadRequest(ModelState);
            
            city.Name = cdto.Name;
            city.Price = cdto.Price;
            city.PickedPrice = cdto.PickedPrice;
            city.GovernmentId = cdto.GovernmentId;
            
            //or by mapper map.Map(city,ctdo)
            await work.CityRepository.Update(city);
            var updatedCityDTO = map.Map<CityDTO>(city);
            return Ok(updatedCityDTO);
        }
        [HttpDelete("{id:int}")]
        public  async Task<IActionResult> Delete(int id)
        {
            var city = await work.CityRepository.GetCityWithGovernmentByIdAsync(id);
            if (city == null)
          
                return NotFound($"City with id {id} not found.");
             await work.CityRepository.Delete(city);
            await work.SaveAsync();
            return Ok($"City with id {id} deleted successfully.");
        }

    }
}
