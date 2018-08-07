using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using VK_Music.Models;
using System.Net;
using VK_Music.Logic;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Security.Principal;
using VkNet.Model.RequestParams;

namespace VK_Music
{
    public class PhotoDownloader
    {
        private readonly IVKManager vk_mngr;
        private readonly HttpServerUtilityBase server;
        private readonly IPrincipal user;

        public PhotoDownloader(HttpServerUtilityBase server, IPrincipal user)
        {
            vk_mngr = new VKManager();
            this.server = server;
            this.user = user;
        }

        public async Task DownloadAsync(List<string> id_list)
        {
            VKAuthorize();
            await Download(vk_mngr.GetAllPhoto().FindAll(p=>id_list.Contains(p.PhotoId.ToString())));
        }
        private async Task Download(List<Photo> toDownloadPhoto)
        {
            await Task.Run(() => toDownloadPhoto.ForEach(p =>
            {
                Foo(p);
            }));
        }
        private void Foo(Photo p)
        {
            string path = String.Empty;
            using (DatabaseContext db = new DatabaseContext())
            {
                var basepath = "\\Downloads\\";//Server.MapPath("~/Downloads/");
                var user_dir = db.Users.FirstOrDefault(u => u.Email == user.Identity.Name).Id.ToString();
                var album_dir = p.AlbumId.ToString();
                var photo_name = p.PhotoId + Path.GetExtension(p.Path);

                var full_path = Path.Combine(server.MapPath(basepath), user_dir, album_dir);
                Directory.CreateDirectory(full_path);

                path = Path.Combine(full_path, photo_name);

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(p.Path, path);
                }

                p.Path = Path.Combine(basepath, user_dir, album_dir, photo_name).Replace('\\', '/');

                // что если альбомы уже есть???
                if (db.Albums.Count() == 0 || db.Albums.FirstOrDefault(a => a.Id == p.AlbumId) == null)
                {
                    db.Albums.Add(vk_mngr.GetAlbumById(p.AlbumId));
                }

                db.PhotoList.Add(p);
                db.SaveChanges();
            }
        }

        private void VKAuthorize()
        {
            vk_mngr.AuthorizeByToken(user.Identity.Name);
        }
    }
}