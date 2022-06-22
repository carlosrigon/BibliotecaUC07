using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Biblioteca.Models;



namespace Biblioteca.Controllers
{
    public class Autenticacao
    {
        public static void CheckLogin(Controller controller)
        {   
            if(string.IsNullOrEmpty(controller.HttpContext.Session.GetString("login")))
            {
                controller.Request.HttpContext.Response.Redirect("/Home/Login");
            }
        }

        public static bool verificaLoginSenha(string login, string senha, Controller controller)
        {
            using(BibliotecaContext bc = new BibliotecaContext())
            {
                verificaSeUsuarioAdminExiste(bc);

                senha = Criptografia.TextoCriptografado(senha);

                IQueryable<Usuario> UsuarioEncontrado = bc.Usuarios.Where(u => u.Login == login && u.Senha == senha);
                List<Usuario> ListaUsuarioEncontrado = UsuarioEncontrado.ToList();

                if (ListaUsuarioEncontrado.Count==0){
                    return false;
                } else {
                    //Adicionar na sessão da controller encaminhada os dados do usuário localizado//
                    controller.HttpContext.Session.SetString("login", ListaUsuarioEncontrado[0].Login);
                    controller.HttpContext.Session.SetString("nome", ListaUsuarioEncontrado[0].Nome);
                    controller.HttpContext.Session.SetInt32("tipo", ListaUsuarioEncontrado[0].Tipo);
                    
                    return true;
                }
                
            }
   
        }

        public static void verificaSeUsuarioAdminExiste(BibliotecaContext bc)
        {
            IQueryable<Usuario> UsuarioEncontrado = bc.Usuarios.Where(u => u.Login == "admin");    

            if (UsuarioEncontrado.ToList().Count==0){
                //Criar o usuário admin//
                Usuario admin = new Usuario();
                admin.Nome = "Administrador";
                admin.Login = "admin";
                admin.Senha = Criptografia.TextoCriptografado("123");
                admin.Tipo = Usuario.ADMIN;

                bc.Usuarios.Add(admin);
                bc.SaveChanges();
            }
        }

        public static void verificaSeUsuarioEAdmin(Controller controller)
        {
            if (!(controller.HttpContext.Session.GetInt32("tipo") == Usuario.ADMIN)){
                controller.Request.HttpContext.Response.Redirect("/Usuario/needAdmin");
            }
        }
    }
}