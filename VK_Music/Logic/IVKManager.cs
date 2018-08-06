using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VK_Music.Models;

namespace VK_Music.Logic
{
    public interface IVKManager
    {
        //object VK_API { get; set; }

        void Authorize(string Login,string Password);
        void AuthorizeByToken(string login);

        long UserId { get; }
        string Token { get; }
        User GetUserById(long id);
        Album GetAlbumById(long id);
        List<Photo> GetAllPhoto();
    }
}