using Microsoft.AspNetCore.Mvc;
using System;
using Users.Business.Contracts;
using Users.Business.Interfaces;

namespace Users.Api.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet, Route("api/users")]
        public IActionResult GetAll()
        {
            var users = _service.GetAll();

            return Ok(users);
        }

        [HttpGet, Route("api/users/{id:guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var user = _service.GetById(id);

            return Ok(user);
        }

        [HttpPost, Route("api/users")]
        public IActionResult Post([FromBody] UserCreateDto userDto)
        {
            var user = _service.Create(userDto);

            return Ok(user);
        }

        [HttpPut, Route("api/users/{id:guid}")]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UserEditDto userDto)
        {
            if (id != userDto.Id)
                throw new ArgumentException("Request path id and request body id values must match.");

            var user = _service.Update(userDto);

            return Ok(user);
        }

        [HttpDelete, Route("api/users/{id:guid}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            _service.Delete(id);

            return NoContent();
        }
    }
}
