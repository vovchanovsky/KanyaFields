using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiSvc.Application.Password.Commands.CreatePassword;
using ApiSvc.Application.Password.Commands.DeletePassword;
using ApiSvc.Application.Password.Queries.GetPassword;
using ApiSvc.Application.Password.Queries.GetPasswords;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiSvc.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        public PasswordController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<IEnumerable<GetPasswordItemDto>> Get() =>
            await _mediator.Send(new GetPasswordsQuery());
        
        [HttpGet("{id}")]
        public async Task<GetPasswordItemDto> Get(Guid id) =>
            await _mediator.Send(new GetPasswordQuery(id));

        [HttpPost]
        public async Task<Guid> Create(CreatePasswordItemDto createPasswordItemDto) =>
            await _mediator.Send(new CreatePasswordCommand(createPasswordItemDto));

        [HttpDelete("{id}")]
        public async Task<StatusCodeResult> Delete(Guid id)
        {
            await _mediator.Send(new DeletePasswordCommand(id));
            return NoContent();
        }

        
        private readonly IMediator _mediator;
    }
}
