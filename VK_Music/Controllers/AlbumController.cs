using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using VK_Music.Models;
using System.Threading.Tasks;
using VK_Music.Logic;

namespace VK_Music.Controllers
{
    public class AlbumController : Controller
    {
        private readonly IVKManager vk_mngr = new VKManager();

        // показывает список сохранненых фото
        [Authorize]
        public ActionResult ShowOnline()
        {
            VKAuthorize();
            return View("OnlinePhoto", vk_mngr.GetAllPhoto());
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
        public async Task Download(List<string> list) // лист содержит id фоток
        {
            PhotoDownloader downloader = new PhotoDownloader(Server,User);
            await downloader.DownloadAsync(list);
        }        

        private void VKAuthorize()
        {
            vk_mngr.AuthorizeByToken(User.Identity.Name);
        }
    }
}