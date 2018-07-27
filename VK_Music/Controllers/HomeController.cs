﻿using System;
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
    public class HomeController : Controller
    {
        //private DatabaseContext db = new DatabaseContext();
        private readonly VkApi vk = new VkApi();

        // показывает список сохранненых фото
        [Authorize]
        public ActionResult ShowOnline()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                vk.Authorize(new ApiAuthParams { AccessToken = db.Users.Where(u => u.Email == HttpContext.User.Identity.Name).FirstOrDefault().Tocken });
            }
            var photo_list = vk.Photo.GetAll(new PhotoGetAllParams { Count = 200, Extended = true, PhotoSizes = true, NoServiceAlbums = false });
            return View("OnlinePhoto", FillList(photo_list));
        }

        [Authorize]
        public ActionResult ShowSaved()
        {
            List<Photo> photo_list = null;

            using (DatabaseContext db = new DatabaseContext())
            {
                 photo_list= db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault().PhotoList;
            }
            
            //if (photo_list == null || photo_list.Count==0)
            //{
            //    return Content("Список фотографий пуст!");
            //}

            return View("SavedPhoto", photo_list);
        }

        [ChildActionOnly]
        public ActionResult Download(List<string> list)
        {
            return View();
        }

        private List<PhotoList> FillList(VkCollection<VkNet.Model.Attachments.Photo> photo_list)
        {
            var list = new List<PhotoList>();
            PhotoList row;
            foreach (var p in photo_list)
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
    }
}