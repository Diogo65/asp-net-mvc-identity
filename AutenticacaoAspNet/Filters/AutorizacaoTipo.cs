using AutenticacaoAspNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutenticacaoAspNet.Filters
{
    //indicando ao usuario que ele não pôde acessar aquela página por falta de privilégios, e o mantendo dentro da aplicação.
    //para ser um filtro de autenticação e autorização a classe deve herdar de AuthorizeAttribute;
    public class AutorizacaoTipo : AuthorizeAttribute
    {
        //declaramos um array de TipoUsuario que representa os tipos permitidos por esse filtro;
        private TipoUsuario[] tiposAutorizados;

        public AutorizacaoTipo(TipoUsuario[] tiposUsuarioAutorizados) 
        {
            tiposAutorizados = tiposUsuarioAutorizados;
        }

        //o método OnAuthorization é invocado quando a requisição está sendo filtrada. 
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //verificamos se o usuário logado pertence a um dos tipos permitidos.
            //O método Any retornará true se a expressão passada como parâmetro for verdadeira para pelo menos um item do array;
            bool autorizado = tiposAutorizados.Any(t => filterContext.HttpContext
                .User
                .IsInRole(t.ToString()));

            //se o usuário não está autorizado, então o redirecionamos para a URL
            if (!autorizado)
            {
                filterContext.Controller.TempData["ErrorAutorizacao"] = "Voçê não tem permissão para acessar essa página";
                filterContext.Result = new RedirectResult("Painel");
            }
        }
    }
}