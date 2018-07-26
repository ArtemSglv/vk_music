using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using VK_Music.Models;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VK_Music.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        private readonly VkApi vk = new VkApi();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Login, string Password, string Token = "")
        {
            if (Token != "")
                vk.Authorize(new VkNet.Model.ApiAuthParams
                { AccessToken = Token });
            else
                vk.Authorize(new VkNet.Model.ApiAuthParams
                {
                    ApplicationId = 6640902,
                    Login = Login,
                    Password = Password,
                    Settings = Settings.All
                    //TwoFactorAuthorization = () =>
                    //{ 
                    //    if(Code>0)
                    //    {
                    //        return Code.ToString();
                    //    }

                    //    return null;
                    //}
                });

            //костыли
            var ids = new long[] { (long)vk.UserId };
            var vk_user = vk.Users.Get(ids);


            if (db.User.Find(vk.UserId) == null)
                RegisterUserInDB(vk_user[0]);

            var photo_list = vk.Photo.GetAll(new PhotoGetAllParams { Count = 200, Extended = true, PhotoSizes = true, NoServiceAlbums = false });
            
            // получение ифны с вк о пользователе   \/ 
            //запись в БД пользователя, если он новый \/
            // запрос на vk api и стягивание списка изображений \/
            // вывод вьюшки со списком, на котором можно выбрать треки для скачивания

            //
            //ViewBag.Token = vk.Token;
            return View("TrackList", FillList(photo_list));
        }

        [HttpPost]
        public ActionResult Download(List<PhotoList> list)
        {
            

            return View();
        }

        private void RegisterUserInDB(VkNet.Model.User vk_user)
        {
            db.User.Add(new User { Id = vk_user.Id, Name = vk_user.FirstName, Lastname = vk_user.LastName,Tocken=vk.Token });
            db.SaveChanges();
        }

        private List<PhotoList> FillList(VkCollection<VkNet.Model.Attachments.Photo> photo_list)
        {
            var list = new List<PhotoList>();
            PhotoList row;//= new PhotoList();
            foreach(var p in photo_list)
            {
                row = new PhotoList();
                row.photo.PhotoId = (long)p.Id;
                row.photo.Title = p.Text;
                row.photo.Likes = p.Likes.Count;
                row.photo.Path = p.Sizes.First().Url.AbsoluteUri;
                

                list.Add(row);
            }
            return list;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}