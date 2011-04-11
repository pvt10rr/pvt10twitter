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
        // Post to Twitter enabled (true) or disabled (false)
        public static bool twitter = true;

        public static OAuthTokens tokens;
        public static string twitterPIN;
        public static string ScreenName;
        public static string hashCode;
        private static string requestToken;
        private static decimal UserID;

        // PostOnTwitter variables
        public static string tempNote;
        public static string tempTester;
        public static string tempCharter;
        public static string[] tempNoteTypes;
        public static int tempType;

        public static void TwitterLogin()
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

        public static void TwitterOAuth()
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

        public static void PostOnTwitter() // Method for truncating message if too long and posting it on Twitter
        {
            string twitterPost = "[Reporter: " + tempTester + ", Charter: " + tempCharter + "] " + tempNote + " #" + tempNoteTypes[tempType] + " #" + TwitterAddon.hashCode;
            string twitterNote;
            int twitterNoteLength;
            int twitterNumberOfCharsToRemove;

            if (twitterPost.Length > 140)
            {
                twitterNumberOfCharsToRemove = twitterPost.Length - 140; // how many characters to remove
                twitterNoteLength = tempNote.Length - twitterNumberOfCharsToRemove; // on which character to start removing
                twitterNote = tempNote.Substring(0, twitterNoteLength);
                twitterPost = "[Reporter: " + tempTester + ", Charter: " + tempCharter + "] " + twitterNote + " #" + tempNoteTypes[tempType] + " #" + TwitterAddon.hashCode;
            }

            TwitterResponse<TwitterStatus> tweetResponse = TwitterStatus.Update(TwitterAddon.tokens, twitterPost);
        }
    }
}
