using System;
using System.Collections.Generic;
using System.Text;
using FlickrNet;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Rapid_Reporter
{
   public class FlickrAddon
    {            
        private Flickr m_flickr;
        private string m_frob;
        private string m_token;
        private Auth m_auth;
//        public static string hashcode;
        bool m_flickrLoggedIn = false;

        private string m_tags;

        private string API_KEY = "d06d4e9bb977706b443f5cb969b29a94";
        private string SECRET = "a62d4e4bed745bf0";



        public void Login()
        {

            // Create Flickr instance
            m_flickr = new Flickr(API_KEY, SECRET);

            m_frob = m_flickr.AuthGetFrob();

            string flickrUrl = m_flickr.AuthCalcUrl(m_frob, AuthLevel.Write);

            // The following line will load the URL in the users default browser.
            System.Diagnostics.Process.Start(flickrUrl);

            bool bIsAuthorized = false;
            m_auth = new Auth();

            // do nothing until flickr authorizes.
            while (!bIsAuthorized)
            {
                try
                {
                    m_auth = m_flickr.AuthGetToken(m_frob);
                    m_flickr.AuthToken = m_auth.Token;
                }
                catch (FlickrException ex)
                {
                    ;
                }

                if (m_flickr.IsAuthenticated)
                {
                    bIsAuthorized = true;
                }
            }
        }

/*        public static string GetUniqueKey(int length)
        {
            string guidResult = string.Empty;

            while (guidResult.Length < length)
            {
                // Get the GUID.
                guidResult += Guid.NewGuid().ToString().GetHashCode().ToString("x");
            }

            // Make sure length is valid.
            if (length <= 0 || length > guidResult.Length)
                throw new ArgumentException("Length must be between 1 and " + guidResult.Length);

            // Return the first length bytes.
            hashcode = guidResult.Substring(0, length);
            return hashcode;
        }*/

        public string GetCurrentUser()
        {
                return m_auth.User.UserName; 
 
        }

        public string Upload(string pFile, string pName, string pDesc, string pTags)
        {

            if (SMWidget.ToggleUpload2 == true)
            {
                try
                {
                    bool b = m_flickr.IsAuthenticated;
                }
                catch (FlickrException f)
                {
                    System.Windows.MessageBox.Show(f.Message); 
                }

                if (IsAuthenticated())
                {
                    return m_flickr.UploadPicture(pFile, pName, pDesc, pTags);
                }
                else
                {
                    Login();
                    return m_flickr.UploadPicture(pFile, pName, pDesc, pTags);
                }
            }
            return "";
        }

        public bool IsAuthenticated()
        {
            return m_flickr.IsAuthenticated;
        }


        public string GetUrl(string id)
        {
            PhotoInfo info = m_flickr.PhotosGetInfo(id);
            return info.WebUrl;
        }

        public void LogOut()
        {
            m_flickr.AuthToken = null;
            System.Diagnostics.Process.Start("http://www.flickr.com/logout.gne?");
        }

    }

    



    
}


