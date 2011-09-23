using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MogileFsApi.Tests
{
    [TestFixture]
    public class MogileFsProtocolImplementationTests
    {
        [Test]
        public void Uri_works_as_expected()
        {
            var uri = new Uri("mogile://mogile1:6001");
            Assert.AreEqual("mogile1", uri.Host);
            Assert.AreEqual(6667, uri.Port);
        }
    }
}
