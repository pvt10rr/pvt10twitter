using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rapid_Reporter;

namespace RapidReporterTest
{
    [TestFixture]
    public class UnitTest1
    {
        private Session m_Session;

        [SetUp]
        public void SetUp()
        {
            m_Session = new Session();

        }

        [Test]
        public void HashCodeLengthIsSame()
        {
            string result = Session.GetUniqueKey(7);
            Assert.AreEqual(7, result.Length);
        }

        [Test]
        public void AnotherTest()
        {
            m_Session.StartSession();
            //bool result = SMWidget.twitter;
            Assert.IsTrue(result);
        }

        [Ignore]
        [Test]
        public void UpdateNotesTrunkarMess()
        {
            //m_Session.StartSession();

            string note = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890abc";
            string notetrunk = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";

            
            m_Session.UpdateNotes(1, note, "screen", "RTFNote");

            Assert.AreEqual(notetrunk, note);
        }

        [TearDown]
        public void TearDown()
        {
            m_Session = null;
        }
    }
}
