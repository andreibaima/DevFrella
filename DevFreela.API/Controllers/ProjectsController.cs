

using DevFreela.API.Models;
using DevFreela.Application.Commands.CreateComment;
using DevFreela.Application.Commands.CreateProject;
using DevFreela.Application.Commands.DeleteProject;
using DevFreela.Application.Commands.FinishProject;
using DevFreela.Application.Commands.StartProject;
using DevFreela.Application.Commands.UpdateProject;
using DevFreela.Application.InputModels;
using DevFreela.Application.Queries.GetAllProjects;
using DevFreela.Application.Queries.GetProjectById;
using DevFreela.Application.Services.Implementations;
using DevFreela.Application.Services.Interfaces;
using DevFreela.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DevFreela.API.Controllers
{
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        //private readonly IProjectService _projectServices;
        private readonly IMediator _mediator;
        //pegando a variavel que foi configurada no program.cs
        public ProjectsController(IMediator mediator)
        {
            //_projectServices = projectService;
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "client, freelancer")]
        public async Task<IActionResult> Get(string query)
        {
            /*var projects = _projectServices.GetAll(query);

            return Ok(projects);*/
            var getAllProjectsQuery = new GetAllProjectsQuery(query);

            var projects = await _mediator.Send(getAllProjectsQuery);

            return Ok(projects);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "client, freelancer")]
        public async Task<IActionResult> GetById(int id)
        {
            /*var project = _projectServices.GetById(id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);*/

            var query = new GetProjectByIdQuery(id);

            var project = await _mediator.Send(query);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Post([FromBody] CreateProjectCommand command)
        {
            if (command.Title.Length > 50)
            {
                return BadRequest();
            }

            //var id = _projectServices.Create(inputModel);

            //padrão mediator vai controlar acesso a outras dependencias
            var id = await _mediator.Send(command);
            // 1- Nome da api que vai obter detalhes do que foi cadastrado
            // 2- objeto autonimo que vai ter o parametro para achar o objeto criado
            // 3- objeto cadastrado
            return CreatedAtAction(nameof(GetById), new { id = id }, command);

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateProjectCommand command)
        {
            /*if (inputModel.Description.Length > 200)
            {
                return BadRequest();
            }

            _projectServices.Update(inputModel);

            return NoContent();*/

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteProjectCommand(id);

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost("{id}/comments")]
        [Authorize(Roles = "client, freelancer")]
        //public IActionResult PostComment(int id, [FromBody] CreateCommentInputModel inputModel)
        public async Task<IActionResult> PostComment(int id, [FromBody] CreateCommentCommand command)
        {
            /*_projectServices.CreateComment(inputModel);

            return NoContent();*/
            await _mediator.Send(command);

            return NoContent();

        }

        [HttpPut("{id}/start")]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Start(int id)
        {
            var command = new StartProjectCommand(id);

            await _mediator.Send(command);

            return NoContent();
        }
        /* public IActionResult Start(int id)
        {
            _projectServices.Start(id);

            return NoContent();
        } */


        [HttpPut("{id}/finish")]
        [Authorize(Roles = "client")]
        public async Task<IActionResult> Finish(int id)
        {
            var command = new FinishProjectCommand(id);

            await _mediator.Send(command);

            return NoContent();
        }
    }
}
