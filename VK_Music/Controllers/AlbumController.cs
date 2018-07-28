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
using VkNet.Model;

namespace VK_Music.Controllers
{
    public class AlbumController : Controller
    {
        private readonly VkApi vk = new VkApi();
        private VkCollection<VkNet.Model.Attachments.Photo> vk_photo_list;

        // показывает список сохранненых фото
        [Authorize]
        public ActionResult ShowOnline()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                vk.Authorize(new ApiAuthParams { AccessToken = db.Users.Where(u => u.Email == HttpContext.User.Identity.Name).FirstOrDefault().Tocken });
            }
            vk_photo_list = vk.Photo.GetAll(new PhotoGetAllParams { Count = 200, Extended = true, PhotoSizes = true, NoServiceAlbums = false });
            return View("OnlinePhoto", vk_photo_list);
        }

        [Authorize]
        public ActionResult ShowAllAlbums()
        {
            List<Album> albums = null;

            using (DatabaseContext db = new DatabaseContext())
            {
                albums = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name).Albums;
            }
            return View("UserSavedAlbums", albums);
        }

        [Authorize]
        public ActionResult ShowAlbum(long? id)
        {
            Album album = null;

            using (DatabaseContext db = new DatabaseContext())
            {
                album = db.Albums.FirstOrDefault(u => u.Id == id);
            }
            return View("UserAlbum", album);
        }

        [HttpPost]
        public ActionResult Download(List<string> list)
        {
            
            foreach (var l in list)
            {
                var vk_photo=vk_photo_list.ToList().Find(vp => vp.Id == Convert.ToInt64(l));


            }
            Download();

            return View();
        }

        private void Download(List<Photo> toDownloadPhoto)
        {

        }

        private List<Photo> FillList(VkCollection<VkNet.Model.Attachments.Photo> photo_list)
        {
            var list = new List<Photo>();
            Photo row;
            foreach (var p in photo_list)
            {
                row = new Photo();
                row.PhotoId = (long)p.Id;
                row.AlbumId = (long)p.AlbumId;
                row.Title = p.Text;
                row.Likes = p.Likes.Count;
                row.Path = p.Sizes.First().Url.AbsoluteUri; // нужно выбирать размер нормальный См. описание API VK                

                list.Add(row);
            }
            return list;
        }
    }
}