using System;
using System.IO;
using NUnit.Framework;
using Primelabs.Twingly.MogileFsApi;

namespace MogileFsApi.Tests
{
    [TestFixture]
    public class MogileFsClientTests
    {
        [Test, Explicit]
        public void Can_download_file()
        {
            var uri = new Uri("mogile://mogile1:6001");

            var d = "avatars";
            var key = "10160316052018900881";
            var client = new MogileFsClient(new Uri[] {uri});
            var paths = client.GetPaths(d, key, true);
            Assert.AreEqual(1, paths.Count);
            Assert.AreEqual("http://10.20.11.37:7500/dev1/0/006/751/0006751171.fid", paths[0]);

            byte[] arr = client.GetFileBytes(d, key, 10000);
            using (var file = File.Open(@"C:\mogile_test_small_avatar.jpg", FileMode.Create)) {
                file.Write(arr, 0, arr.Length);
                file.Flush();
            }
        }

        [Test, Explicit]
        public void Can_download_file_screenshot()
        {
            var uri = new Uri("mogile://mogile1:6001");

            var d = "screenshots";
            var key = "o_9093049415805545204";
            var client = new MogileFsClient(new Uri[] { uri });
            var paths = client.GetPaths(d, key, true);
            Assert.AreEqual(2, paths.Count);
            Assert.AreEqual("http://10.20.11.37:7500/dev3/0/027/695/0027695936.fid", paths[0]);
            Assert.AreEqual("http://10.20.11.38:7500/dev4/0/027/695/0027695936.fid", paths[1]);

            byte[] arr = client.GetFileBytes(d, key, 10000);
            using (var file = File.Open(@"C:\mogile_test_screenshot_larger.jpg", FileMode.Create))
            {
                file.Write(arr, 0, arr.Length);
                file.Flush();
            }
        }

        [Test, Explicit]
        public void Can_upload_file_screenshot()
        {
            var uri = new Uri("mogile://mogile1:6001");

            byte[] arr;
            using (var filestream = new FileStream(@"C:\mogile_test_screenshot_larger.jpg", FileMode.Open))
            {
                arr = new byte[filestream.Length];
                Primelabs.Twingly.MogileFsApi.Utils.ByteReader.ReadWholeArray(filestream, arr);
            }

            var d = "screenshots";
            var key = "test_1";
            var client = new MogileFsClient(new Uri[] { uri });
            var paths = client.GetPaths(d, key, true);
            if (paths != null && paths.Count > 0)
                client.Delete(d, key);

            using (var filestream = new FileStream(@"C:\mogile_test_screenshot_larger.jpg", FileMode.Open)) {
                client.StoreFile(d, key, "original", filestream, 100000);
            }
            paths = client.GetPaths(d, key, true);
            Assert.AreEqual(2, paths.Count);

            byte[] arr2 = client.GetFileBytes(d, key, 10000);
            Assert.AreEqual(arr.Length, arr2.Length, "arr lengths not ok");
            for (int i = 0; i < arr2.Length; i++)
                Assert.AreEqual(arr[i], arr2[i], "contents do not match..");
        }
    }
}