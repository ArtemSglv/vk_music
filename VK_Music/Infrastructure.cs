using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using VK_Music.Models;
using System.Net;

namespace VK_Music
{
    public class Infrastructure
    {
        public static string URI= "https://api.vk.com/method"; //private static DatabaseContext db = new DatabaseContext();

        public static string Request(string parameters) // return JSON response
        {
            string jsonResponse = string.Empty;
            string address = $"{URI}/";
            byte[] inputBytes=null;//= Encoding.UTF8.GetBytes(parameters);

            //string sign1 = Sign(parameters, inputBytes);

            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(address);
            if (webRequest != null)
            {
                webRequest.Method = "GET";
                webRequest.Timeout = 20000; //20 000
                //webRequest.ContentType = "application/x-www-form-urlencoded";
                //webRequest.Headers.Add("Key", Key);
                //webRequest.Headers.Add("Sign", sign1);
                //webRequest.Proxy = null;
                //ServicePointManager.Expect100Continue = false;

                //webRequest.ContentLength = parameters.Length;

                //Request
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(inputBytes, 0, parameters.Length);
                }

                //Response
                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {
                        jsonResponse = sr.ReadToEnd();
                    }
                }
            }
            return jsonResponse;
        }

        public static void AuthUser()
        {
            Request("");
        }
    }
}