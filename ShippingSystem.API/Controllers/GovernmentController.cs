using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO.Government;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

namespace ShippingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernmentController : ControllerBase
    {
        private readonly IUnitOfWork work;
        private readonly IMapper map;

        public GovernmentController(IUnitOfWork work , IMapper map)
        {
            this.work = work;
            this.map = map;
        }
        [HttpGet]
        public  async Task<IActionResult> GetAll()
        {
            var governments = await work.GovernmentRepository.GetGovernmentsWithCitiesAsync();
            if (governments == null || !governments.Any())
            
                return NotFound("No governments found.");
            var allGovernmentsDTO = map.Map<List<GovernmentDTO>>(governments);

            return Ok(allGovernmentsDTO);
        }

        [HttpGet("Gov-Names")]
        public async Task<IActionResult> GetGovernmentsName()
        {
            var governments= await work.GovernmentRepository.GetAll();
            var result = governments.Select(g=>new GovernmentNameDTO
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
            return Ok(result);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var gov = await  work.GovernmentRepository.GetGovernmentWithCitiesByIdAsync(id);
            if(gov == null)
                return NotFound($"Government with id {id} not found.");
            var governmentDTO = map.Map<GovernmentDTO>(gov);
            return Ok(governmentDTO);
        }
        [HttpGet("GetGovernmentByName/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var gov = await work.GovernmentRepository.GetGovernmentByNameAsync(name);
            if (gov == null)
                return NotFound($"This Government with {name} is Not Found");
            var govDto = map.Map<GovernmentDTO>(gov);
            return Ok(govDto);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddGovernmentDTO addgovdto)
        {
            if(addgovdto == null)
             return BadRequest("Government data is null.");
            var newGovernment = map.Map<Government>(addgovdto);
             await work.GovernmentRepository.Add(newGovernment);
             await work.SaveAsync();
            var addedGovernment = map.Map<GovernmentDTO>(newGovernment);
            return Ok(addedGovernment);
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, GovernmentDTO govdto)
        {
          var government = await  work.GovernmentRepository.GetGovernmentWithCitiesByIdAsync(id);
            if (government == null)
                return NotFound($"Government with id {id} not found.");
             if(!ModelState.IsValid)
                return BadRequest(ModelState);
            government.Id = govdto.Id;
            government.Name = govdto.Name;
            government.Cities = govdto.ListCities?.Select(name=> new City { Name = name }).ToList() ?? new List<City>();
            await work.GovernmentRepository.Update(government);
            await work.SaveAsync();
             var updatedGovernmentDTO = map.Map<GovernmentDTO>(government);

            return Ok(updatedGovernmentDTO);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var gov = await work.GovernmentRepository.GetGovernmentWithCitiesByIdAsync(id);
            if (gov == null)
                return NotFound($"This Government with {id} is Not Found");
            await work.GovernmentRepository?.Delete(gov);
            await work.SaveAsync();
            return Ok(new {message= "Deleted Successfully" });
        }


    }
}
