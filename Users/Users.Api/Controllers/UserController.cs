using Microsoft.AspNetCore.Mvc;
using System;
using Users.Business.Contracts;
using Users.Business.Interfaces;

namespace Users.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _service;

        public UserController(IUserService service, INotifier notifier) : base(notifier)
        {
            _service = service;
        }

        [HttpGet, Route("api/users")]
        public IActionResult GetAll()
        {
            var users = _service.GetAll();

            return Response(users);
        }

        [HttpGet, Route("api/users/{id:guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var user = _service.GetById(id);

            return Response(user);
        }

        [HttpPost, Route("api/users")]
        public IActionResult Post([FromBody] UserCreateDto userDto)
        {
            var user = _service.Create(userDto);

            return Response(user);
        }

        [HttpPut, Route("api/users/{id:guid}")]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UserEditDto userDto)
        {
            if (id != userDto.Id)
                throw new ArgumentException("Request path id and request body id values must match.");

            var user = _service.Update(userDto);

            return Response(user);
        }

        [HttpDelete, Route("api/users/{id:guid}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            _service.Delete(id);

            return Response();
        }
    }
}
