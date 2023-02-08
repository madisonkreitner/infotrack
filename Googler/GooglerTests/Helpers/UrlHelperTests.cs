using Googler.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglerTests.Helpers
{
    [TestFixture]
    internal class UrlHelperTests
    {
        [Test]
        [Category("UnitTest")]
        [TestCase("lkjahsdkfjhasldkfjh","")]
        // expect to see http or https, this should not work, this can be extended later
        [TestCase("www.infotrack.com", "")]
        // we look for a slash after, this should not work, extend this to look for all valid domain suffix
        [TestCase("https://www.infotrack.com", "www.infotrack.com")]
        [TestCase("https://www.infotrack.com/blog/integrated-efiling-improve-efficiency-law-firm/&amp;sa=U&amp;ved=2ahUKEwjn-aLUqYb9AhU_TDABHdLCD8QQFnoECAIQAg&amp;usg=AOvVaw0ZF2B9m2o7o_1c55jBQ8Rp", "www.infotrack.com")]
        [TestCase("http://www.infotrack.com/blog/integrated-efiling-improve-efficiency-law-firm/&amp;sa=U&amp;ved=2ahUKEwjn-aLUqYb9AhU_TDABHdLCD8QQFnoECAIQAg&amp;usg=AOvVaw0ZF2B9m2o7o_1c55jBQ8Rp", "www.infotrack.com")]
        [TestCase("/url?q=https://www.infotrack.com/blog/integrated-efiling-improve-efficiency-law-firm/&amp;sa=U&amp;ved=2ahUKEwjn-aLUqYb9AhU_TDABHdLCD8QQFnoECAIQAg&amp;usg=AOvVaw0ZF2B9m2o7o_1c55jBQ8Rp", "www.infotrack.com")]
        [TestCase("/url?q=http://www.infotrack.com/blog/integrated-efiling-improve-efficiency-law-firm/&amp;sa=U&amp;ved=2ahUKEwjn-aLUqYb9AhU_TDABHdLCD8QQFnoECAIQAg&amp;usg=AOvVaw0ZF2B9m2o7o_1c55jBQ8Rp", "www.infotrack.com")]
        public void GetDomainNameTest(string url, string expectedDomain)
        {
            Assert.That(UrlHelper.GetDomainName(url), Is.EqualTo(expectedDomain));
        }
    }
}
