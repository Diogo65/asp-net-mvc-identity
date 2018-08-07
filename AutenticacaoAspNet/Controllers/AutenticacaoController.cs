using AutenticacaoAspNet.Models;
using AutenticacaoAspNet.ViewModels;
using AutenticacaoAspNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using AutenticacaoAspNet.Filters;

namespace AutenticacaoAspNet.Controllers
{
    public class AutenticacaoController : Controller
    {
        private UsuariosContext db = new UsuariosContext();

        //filtro customizado 
        //TipoUsuario com os tipos autorizados, Classe Filter-AturizaçãoTupo
        //Nome do Atributo(filtro) - Parâmetros do Atributo - Atributos entre colchetes
        [AutorizacaoTipo(new[] { TipoUsuario.Administrador})]
        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Cadastrar(CadastroUsuarioViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewmodel);
            }

            if (db.Usuarios.Count(u => u.Login == viewmodel.Login) > 0)
            {
                ModelState.AddModelError("Login", "Esse login já está em uso");
                return View(viewmodel);
            }

            Usuario novoUsuario = new Usuario
            {
                Nome = viewmodel.Nome,
                Login = viewmodel.Login,
                Senha = Hash.GerarHash(viewmodel.Senha)
            };

            db.Usuarios.Add(novoUsuario);
            db.SaveChanges();

            TempData["Mensagem"] = "Cadastro realizado com sucesso. Efetue login.";

            return RedirectToAction("Login");
        }

        public ActionResult Login(string ReturnUrl)
        {
            var viewmodel = new LoginViewModel
            {
                UrlRetorno = ReturnUrl
            };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel viewmodel)
        {
            //Validamos o ModelState para verificar se todas as regras de validação definidas via Data Annotations foram atendidas.
            if (!ModelState.IsValid)
            {
                return View(viewmodel);
            }

            //filtramos o usuário no banco de dados a partir do login informado;
            var usuario = db.Usuarios.FirstOrDefault(u => u.Login == viewmodel.Login);

            //caso o usuário não seja localizado, retornamos para a view com um erro vinculado ao campo Login, informando que essa informação está incorreta;
            if (usuario == null)
            {
                ModelState.AddModelError("Login", "Login incorreto");
                return View(viewmodel);
            }

            //se o usuário foi localizado, comparamos a senha gravada no banco com a que foi informada na tela de login
            if (usuario.Senha != Hash.GerarHash(viewmodel.Senha))
            {
                ModelState.AddModelError("Senha", "Senha incorreta");
                return View(viewmodel);
            }

            //se o login e a senha estão corretos, então criamos um novo objeto do tipo ClaimsIdentity.
            //Essa classe representa um conjunto de informações/ características sobre o usuário, ou seja, sua identidade;
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("Login", usuario.Login),
                //Role, que é uma definição padrão para o papel/função do usuário.
                new Claim(ClaimTypes.Role,usuario.Tipo.ToString())
            }, "ApplicationCookie");

            //criamos o cookie de autenticação usando o midleware do OWIN, Para isso passamos o ClaimsIdentity que acabamos de criar;
            Request.GetOwinContext().Authentication.SignIn(identity);

            //se o usuário estava vindo de uma outra página, redirecionamos ele para ela
            //Caso contrário, enviamos ele para a página inicial do painel administrativo(página de exemplo).
            if (!String.IsNullOrWhiteSpace(viewmodel.UrlRetorno) || Url.IsLocalUrl(viewmodel.UrlRetorno))
                return Redirect(viewmodel.UrlRetorno);
            else
                return RedirectToAction("Index", "Painel");
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}