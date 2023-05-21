using DevFreela.Core.Entities;
using DevFreela.Core.Repositories;
using DevFreela.Infrastructure.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFreela.Application.Commands.CreateProject
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
    {
        private readonly IProjectRepository _projectRepository;

        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        // vai tratar as informações e realizar efetimanete as informações no banco de dados, fazer como nosso serviço ta fazendo
        public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project(request.Title, request.Description, request.IdClient, request.IdFreelancer, request.TotalCost);

            //async -> quando se faz uma requisição no banco de datao a thread fica esperando a resposta, fica inativa
            //quando usa o await você delega essa operação E/S de acesso a banco de dados a trhead fica limpara para fazer outras coisas
            await _projectRepository.AddAsync(project);

            return project.Id;
        }
    }
}
