using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VK_Music.Models;
using VK_Music.Logic;

namespace VK_Music.Controllers
{
    public class AccountController : Controller
    {
        private readonly IVKManager vk_mngr = new VKManager();

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                User user = null;
                // авторизация в вк
                vk_mngr.Authorize(model.Login, model.Password);

                //берем авторизованного пользователя
                var vk_user = vk_mngr.GetUserById(vk_mngr.UserId);

                // поиск пользователя в бд
                using (DatabaseContext db = new DatabaseContext())
                {
                    //обновлять токен от вк
                    user = db.Users.FirstOrDefault(u => u.Id == vk_user.Id);
                    if (user != null)
                    {
                        user.Tocken = vk_mngr.Token;
                        db.SaveChanges();
                    }
                }

                // создаем нового пользователя
                if (user == null)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        // тут наверно нужен вызов getUserById
                        db.Users.Add(new User
                        {
                            Id = vk_user.Id,
                            Name = vk_user.Name,
                            Lastname = vk_user.Lastname,
                            Email = model.Login,
                            Password = model.Password,
                            Tocken = vk_mngr.Token
                        });
                        db.SaveChanges();

                        user = db.Users.Where(u => u.Id == vk_user.Id).FirstOrDefault();
                    }
                }

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Login, true);
                    return RedirectToAction("ShowAllAlbums", "Album");
                }
                //else
                //{
                //    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                //}
            }

            return View(model);
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}