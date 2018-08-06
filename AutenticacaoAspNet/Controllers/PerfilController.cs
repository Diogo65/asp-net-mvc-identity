using AutenticacaoAspNet.Models;
using AutenticacaoAspNet.Utils;
using AutenticacaoAspNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AutenticacaoAspNet.Controllers
{
    public class PerfilController : Controller
    {
        private UsuariosContext db = new UsuariosContext();

        [Authorize]
        public ActionResult AlterarSenha()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AlterarSenha(AlterarSenhaViewModel viewmodel)
        {
            //validamos o ModelState para verificar as regras definidas via Data Annotationsk
            if (!ModelState.IsValid)
            {
                return View();
            }

            //capturamos o usuário que está logado atualmente;
            var identity = User.Identity as ClaimsIdentity;
            //obtemos o login do usuário conectado;
            var login = identity.Claims.FirstOrDefault(c => c.Type == "Login").Value;
            //filtramos no banco o usuário logado para que possamos comparar sua senha com a que foi digitada;
            var usuario = db.Usuarios.FirstOrDefault(u => u.Login == login);
            //caso a senha digitada esteja incorreta, retornamos para a view com esse erro;
            if (Hash.GerarHash(viewmodel.SenhaAtual) != usuario.Senha)
            {
                ModelState.AddModelError("SenhaAtual", "Senha incorreta");
                return View();
            }

            //alteramos a senha do usuário e gravamos essa modificação no banco de dados;
            usuario.Senha = Hash.GerarHash(viewmodel.NovaSenha);
            db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["Mensagem"] = "Senha alterada com sucesso";

            //Retornamos para a página inicial do painel.
            return RedirectToAction("Index", "Painel");
        }
    }
}