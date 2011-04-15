using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rapid_Reporter;

namespace FlickraddonTest
{
    [TestFixture]
    public class UnitTest1
    {
        private Session m_session;
        private FlickrAddon m_flickrAddon;
        
        [SetUp]
        public void TestMethod1()
        {
            m_flickrAddon = new FlickrAddon();
            m_session = new Session();
        }


        [Test]
        public void TestingLoginTrue()
        {
            m_flickrAddon.Login();
            bool result = m_flickrAddon.IsAuthenticated();
            Assert.IsTrue(result);
        }

        [Test]
        public void TestLogout()
        {
            m_flickrAddon.Login();
            m_flickrAddon.LogOut();
            bool result = m_flickrAddon.IsAuthenticated();
            Assert.IsFalse(result);
        }

 [Test]
        public void GetCurrentUserFlickRRPvt10()
        {
            m_flickrAddon.Login();
            
            string result = m_flickrAddon.GetCurrentUser();
            Assert.AreEqual("FlickrRRPvt10", result);
        }

 
        [Test]
 
        public void GetUniqueKeyTest10()
        {
            string result = FlickrAddon.GetUniqueKey(10);
            Assert.AreEqual(10, result.Length);
        }
    }
}
