using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutenticacaoAspNet.Controllers
{
    public class PainelController : Controller
    {
        //o atributo Authorize indica que essa action só pode ser acessada por usuários autenticados.
        [Authorize]
        public ActionResult Index()
        {
            //verificando o tipo de usuário nas action
            if(User.IsInRole("Padrao"))
            {
                ViewBag.Mensagem = "Você é um usuário padrão e não poderá alterar dados do sistema.";
            }
            return View();
        }

        [Authorize]
        public ActionResult Mensagens()
        {
            return View();
        }
    }
}