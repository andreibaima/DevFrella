using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DevFreela.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            /*
             *  objeto context é do tipo ActionExecutingContext e ele representa o contexto da rota que está sendo executada no Controller, 
             *  então você consegue verificar parâmetro que foram passados na rota e até modificar o resultado antes de retornar para o usuário, 
             *  que é o que está sendo feito no método OnActionExecuting.
             */
            /*
            * model state é um dicionario 
            * select many, para cada item do dicionario ele acessa o valor dela e uma lista de .erros, nesse caso seleciona todos os erro e junda, 
            * ai depois desse erro seleciona somente o ErroeMessage
            */
            if (!context.ModelState.IsValid)
            {
                var messages = context.ModelState
                    .SelectMany(ms => ms.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                context.Result = new BadRequestObjectResult(messages);
            }
        }
    }
}
