using System;
using System.Diagnostics;
using Twitterizer;

namespace Rapid_Reporter
{
    public class TwitterAddon
    {
        // Post to Twitter enabled (true) or disabled (false)
        public static bool twitter = true;

        // TwitterLogin variables
        private static string requestToken;

        // TwitterOAuth variables
        public static OAuthTokens tokens;
        public static string twitterPIN;
        public static string ScreenName;
        private static decimal UserID;

        // GetUniqueKey variables
        public static string hashCode;

        // PostOnTwitter variables
        public static string tempNote;
        public static string tempTester;
        public static string tempCharter;
        public static string[] tempNoteTypes;
        public static int tempType;

        // Starts your default internet browser and takes you to a twitter page where you can
        // login with any account and allow Rapid Reporter to use the account for Twitter posting.
        // You then get a PIN code to enter in Rapid Reporter.
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

        // Verifies the entered PIN code and creates token, ScreenName and so on.
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

        // Generates a unique hash code.
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

        // Posts message on Twitter and truncates if too long.
        public static void PostOnTwitter()
        {
            // What's going to show on Twitter
            string twitterPost = "[Reporter: " + tempTester + ", Charter: " + tempCharter + "] " + tempNote + " #" + tempNoteTypes[tempType] + " #" + TwitterAddon.hashCode;

            string twitterNote;
            int twitterNoteLength;
            int twitterNumberOfCharsToRemove;

            if (twitterPost.Length > 140)
            {
                // How many characters to remove.
                twitterNumberOfCharsToRemove = twitterPost.Length - 140;
                // On which character to start removing.
                twitterNoteLength = tempNote.Length - twitterNumberOfCharsToRemove;
                // Removes the characters
                twitterNote = tempNote.Substring(0, twitterNoteLength);
                // What's going to show on Twitter if 'note' is too long and truncated.
                twitterPost = "[Reporter: " + tempTester + ", Charter: " + tempCharter + "] " + twitterNote + " #" + tempNoteTypes[tempType] + " #" + TwitterAddon.hashCode;
            }

            // Posts on Twitter.
            TwitterResponse<TwitterStatus> tweetResponse = TwitterStatus.Update(TwitterAddon.tokens, twitterPost);
        }
    }
}
