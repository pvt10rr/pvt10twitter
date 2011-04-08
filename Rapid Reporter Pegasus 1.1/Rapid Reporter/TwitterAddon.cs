using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Threading;
using Twitterizer;
using System.Security.Cryptography;
using System.Text;

namespace Rapid_Reporter
{
    class TwitterAddon
    {
        public static string twitterPIN;
        private static string requestToken;
        public static OAuthTokens tokens;
        public static string ScreenName;
        private static decimal UserID;
        public static string hashCode;

        public static void twitterLogin()
        {
            requestToken = OAuthUtility.GetRequestToken("WxOSthlIyUDQzWiGzY7F5Q", "TD3TbCa9BshneTMJTgdTmm57wzfQrQdXa8Ex2Sd7BkM", "oob").Token;

            Process browserProcess = new Process();
            browserProcess.StartInfo.FileName = OAuthUtility.BuildAuthorizationUri(requestToken).AbsoluteUri;
            if (browserProcess.Start())
            {
                browserProcess.WaitForExit();
            }
            else
            {
                //Could not start your browser
            }
        }

        public static void twitterOAuth()
        {
            OAuthTokenResponse accessToken = OAuthUtility.GetAccessToken("WxOSthlIyUDQzWiGzY7F5Q", "TD3TbCa9BshneTMJTgdTmm57wzfQrQdXa8Ex2Sd7BkM", requestToken, TwitterAddon.twitterPIN);

            tokens = new OAuthTokens();
            tokens.AccessToken = accessToken.Token;
            tokens.AccessTokenSecret = accessToken.TokenSecret;
            tokens.ConsumerKey = "WxOSthlIyUDQzWiGzY7F5Q";
            tokens.ConsumerSecret = "TD3TbCa9BshneTMJTgdTmm57wzfQrQdXa8Ex2Sd7BkM";
            ScreenName = accessToken.ScreenName;
            UserID = accessToken.UserId;
        }

        public static string GetUniqueKey(int length)
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
            hashCode = guidResult.Substring(0, length);
            return hashCode;
        }

        public static void PostOnTwitter()
        {

        }
    }
}
