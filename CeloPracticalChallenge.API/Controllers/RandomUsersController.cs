using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using CeloPracticalChallenge.API.DTOs;
using CeloPracticalChallenge.API.Entities;
using CeloPracticalChallenge.API.Repositories;
using CeloPracticalChallenge.API.ResourceParameters;
using CeloPracticalChallenge.API.Profiles;

namespace CeloPracticalChallenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomUsersController : ControllerBase
    {
        private readonly IRandomUserRepository _repository;
        private readonly IMapper _mapper;

        public RandomUsersController(IRandomUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new RandomUsersProfile());
            });
            _mapper = config.CreateMapper();

        }

        // GET: api/RandomUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RandomUserDto>>> List([FromQuery] RandomUsersResourceParameters resourceParameters)
        {
            var list = await _repository.ListAsync(resourceParameters);
            if (list == null) {
                return BadRequest();
            }
            return Ok(_mapper.Map<IEnumerable<RandomUserDto>>(list));
        }

        // GET: api/RandomUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RandomUserDto>> Get(int id)
        {
            var user = await _repository.GetAsync(id);
            if (user == null) {
                return NotFound();
            }
            return Ok(_mapper.Map<RandomUserDto>(user, opts => {
                opts.AfterMap((src, dst) => ((RandomUserDto)dst).ProfileImage = ((RandomUser)src).LargeImageURL);
            }));
        }

        // PUT: api/RandomUsers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RandomUserForUpdateDto randomUserToModifyDto)
        {
            var randomUser = await _repository.GetAsync(id);
            if (randomUser == null) {
                return NotFound();
            }
            //if (!TryValidateModel(randomUserToModifyDto)) {
            //    return ValidationProblem(ModelState);
            //}
            _mapper.Map(randomUserToModifyDto, randomUser);
            await _repository.ModifyAsync(randomUser);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdate(int id, JsonPatchDocument<RandomUserForUpdateDto> patchDocument)
        {
            var randomUser = await _repository.GetAsync(id);
            if (randomUser == null) {
                return NotFound();
            }

            var randomUserToModifyDto = _mapper.Map<RandomUserForUpdateDto>(randomUser);
            patchDocument.ApplyTo(randomUserToModifyDto, ModelState);
            if (!TryValidateModel(randomUserToModifyDto)) {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(randomUserToModifyDto, randomUser);
            await _repository.ModifyAsync(randomUser);
            return NoContent();
        }

        // DELETE: api/RandomUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _repository.DeleteAsync(id)) {
                return NoContent();
            } else {
                return NotFound();
            }
        }

        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
