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
using System.IO;
using System.Net;

namespace VK_Music.Controllers
{
    public class AlbumController : Controller
    {
        private readonly VkApi vk = new VkApi();
        //private VkCollection<VkNet.Model.Attachments.Photo> vk_photo_list;

        // показывает список сохранненых фото
        [Authorize]
        public ActionResult ShowOnline()
        {
            VKAuthorize();
            return View("OnlinePhoto", vk.Photo.GetAll(new PhotoGetAllParams { Count = 200, Extended = true, PhotoSizes = true, NoServiceAlbums = false }));
        }

        [Authorize]
        public ActionResult ShowAllAlbums()
        {
            List<Album> albums = null;

            using (DatabaseContext db = new DatabaseContext())
            {
                albums = db.Albums.Include(a=>a.Photos).Where(u => u.User.Email == User.Identity.Name).ToList();
            }
            return View("UserSavedAlbums", albums);
        }

        [Authorize]
        public ActionResult ShowAlbum(long? id)
        {
            Album album = null;

            using (DatabaseContext db = new DatabaseContext())
            {
                album = db.Albums.Include(a=>a.Photos).FirstOrDefault(u => u.Id == id);
            }
            return View("UserAlbum", album);
        }


        [HttpPost]
        [Authorize]
        public ActionResult Download(List<string> list) // лист содержит id фоток
        {
            List<Photo> photo_list = new List<Photo>();
            VKAuthorize();
            var vk_all_photo = vk.Photo.GetAll(new PhotoGetAllParams { Count = 200, Extended = true, PhotoSizes = true, NoServiceAlbums = false });
            foreach (var l in list)
            {
                var vk_photo = vk_all_photo.FirstOrDefault(vp => vp.Id == Convert.ToInt64(l));

                photo_list.Add(new Photo
                {
                    PhotoId = (long)vk_photo.Id,
                    AlbumId = (long)vk_photo.AlbumId,
                    Likes = vk_photo.Likes.Count,
                    Title = vk_photo.Text,
                    Path = vk_photo.Sizes.FirstOrDefault(s => s.Type == VkNet.Enums.SafetyEnums.PhotoSizeType.X).Url.AbsoluteUri
                });
            }
            Download(photo_list);

            return RedirectToAction("ShowAllAlbums");
        }

        private void Download(List<Photo> toDownloadPhoto)
        {
            toDownloadPhoto.ForEach(p =>
            {
                Foo(p);
            });
        }
        private void Foo(Photo p)
        {
            string path = String.Empty;
            using (DatabaseContext db = new DatabaseContext())
            {
                var basepath = "\\Downloads\\";//Server.MapPath("~/Downloads/");
                var user_dir = db.Users.FirstOrDefault(u => u.Email == User.Identity.Name).Id.ToString();
                var album_dir = p.AlbumId.ToString();
                var photo_name = p.PhotoId + Path.GetExtension(p.Path);

                var full_path = Path.Combine(Server.MapPath(basepath), user_dir, album_dir);
                Directory.CreateDirectory(full_path);

                path = Path.Combine(full_path, photo_name);

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(p.Path, path);
                }

                p.Path = Path.Combine(basepath,user_dir,album_dir,photo_name).Replace('\\','/');

                // что если альбомы уже есть???
                if (db.Albums.Count() == 0 || db.Albums.FirstOrDefault(a => a.Id == p.AlbumId) == null)
                {
                    var a = new Album();
                    a.Id = p.AlbumId;
                    //костыли(((
                    var ids = new List<long>();
                    ids.Add(a.Id);

                    a.Title = vk.Photo.GetAlbums(new PhotoGetAlbumsParams { AlbumIds = ids, Count = 1, }).FirstOrDefault(al => al.Id == p.AlbumId).Title;
                    a.UserId = Convert.ToInt64(user_dir);

                    db.Albums.Add(a);
                }

                db.PhotoList.Add(p);
                db.SaveChanges();
            }
        }
        private void VKAuthorize()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                vk.Authorize(new ApiAuthParams { AccessToken = db.Users.Where(u => u.Email == HttpContext.User.Identity.Name).FirstOrDefault().Tocken });
            }
        }
    }
}