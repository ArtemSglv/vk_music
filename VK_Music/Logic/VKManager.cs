using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VK_Music.Models;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VK_Music.Logic
{
    public class VKManager : IVKManager
    {
        private readonly VkApi vk = new VkApi();

        public long UserId => (long)vk.UserId;

        public string Token => vk.Token;

        public void Authorize(string Login, string Password)
        {
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = 6640902,
                Login = Login,
                Password = Password,
                Settings = Settings.All
            });
        }

        public void AuthorizeByToken(string login)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                vk.Authorize(new ApiAuthParams { AccessToken = db.Users.Where(u => u.Email == login).FirstOrDefault().Tocken });
            }
        }

        public Album GetAlbumById(long id)
        {

            Album album;

            //костыли(((
            var ids = new List<long>
            {
                id
            };

            var vk_album = vk.Photo.GetAlbums(new PhotoGetAlbumsParams { AlbumIds = ids, Count = 1, }).FirstOrDefault(al => al.Id == id);

            album = new Album
            {
                Id = vk_album.Id,
                Title = vk_album.Title,
                UserId = (long)vk_album.OwnerId
            };

            return album;
        }

        public List<Photo> GetAllPhoto()
        {
            List<Photo> photo_list = new List<Photo>();

            var vk_all_photo = vk.Photo.GetAll(new PhotoGetAllParams { Count = 200, Extended = true, PhotoSizes = true, NoServiceAlbums = false });
            foreach(var vk_photo in vk_all_photo)
            {
                photo_list.Add(new Photo
                {
                    PhotoId = (long)vk_photo.Id,
                    AlbumId = (long)vk_photo.AlbumId,
                    Likes = vk_photo.Likes.Count,
                    Title = vk_photo.Text,
                    Path = vk_photo.Sizes.FirstOrDefault(s => s.Type == VkNet.Enums.SafetyEnums.PhotoSizeType.X).Url.AbsoluteUri
                });
            }
            return photo_list;
        }

        public Models.User GetUserById(long id)
        {
            //костыли
            var ids = new List<long>{ id };
            var vk_user = vk.Users.Get(ids)[0];
            return new Models.User
            {
                Id = vk_user.Id,
                Name = vk_user.FirstName,
                Lastname = vk_user.LastName
            };

        }
    }
}