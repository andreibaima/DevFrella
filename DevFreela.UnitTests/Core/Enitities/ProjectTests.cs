using DevFreela.Core.Entities;
using DevFreela.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFreela.UnitTests.Core.Enitities
{
    public class ProjectTests
    {
        [Fact]
        public void TestInProjectStartWorks()
        {
            var project = new Project("Nome de Testes", "Descrição de Teste", 1, 2, 1000);
            //Assert -> faz a verificação
            Assert.Equal(ProjectStatusEnum.Created, project.Status);
        }
    }
}
