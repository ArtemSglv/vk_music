using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VK_Music.Models;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VK_Music.Controllers
{
    public class AccountController : Controller
    {
        private readonly VkApi vk = new VkApi();
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
                vk.Authorize(new VkNet.Model.ApiAuthParams
                {
                    ApplicationId = 6640902,
                    Login = model.Login,
                    Password = model.Password,
                    Settings = Settings.All
                });
                //костыли
                var ids = new long[] { (long)vk.UserId };
                var vk_user = vk.Users.Get(ids)[0];

                // поиск пользователя в бд
                using (DatabaseContext db = new DatabaseContext())
                {
                    //обновлять токен от вк
                    user = db.Users.FirstOrDefault(u => u.Id == vk_user.Id);
                    if (user != null)
                    {
                        user.Tocken = vk.Token;
                        db.SaveChanges();
                    }
                }

                // создаем нового пользователя
                if (user == null)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        db.Users.Add(new User
                        {
                            Id = vk_user.Id,
                            Name = vk_user.FirstName,
                            Lastname = vk_user.LastName,
                            Email = model.Login,
                            Password = model.Password,
                            Tocken = vk.Token
                        });
                        db.SaveChanges();

                        user = db.Users.Where(u => u.Id == vk_user.Id).FirstOrDefault();
                    }
                }

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Login, true);
                    return RedirectToAction("ShowSaved", "Home");
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