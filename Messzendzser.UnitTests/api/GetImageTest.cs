using Messzendzser.Controllers;
using Messzendzser.Model.Managers.Media;
using Messzendzser.Controllers;
using Messzendzser.Utils;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using MySqlX.XDevAPI.Common;

namespace Messzendzser.UnitTests.api
{
    internal class GetImageTest { 
        GetImageController controller;
        IMediaManager mediaManager;

        /// <summary>
        /// Sets up the environment for each test
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            controller = new GetImageController(null);
            mediaManager = new TestMediaManager();
        }

        /// <summary>
        /// Tests the result if the user token is invalid
        /// </summary>
        [Test]
        public void InvalidUserToken()
        {
            FileResult result = controller.LoadImage("valid", "invalid", mediaManager);
            string pattern = ApiTestUtils.ResponseRegex(3, "Invalid user token");
            Assert.That(result.Success == false);
            Assert.That(result.ErrorJson, Does.Match(pattern));
        }
        /// <summary>
        /// Tests the result is a correct error message if the image is not found by the IMediaSource
        /// </summary>
        [Test]
        public void ImageNotFound()
        {
            UserToken token = new UserToken(1, "testUser", "testuser@localhost.com");
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { {"error", "Image not found" } });
            FileResult result = controller.LoadImage("notfound", token.ToToken(), mediaManager);
            Assert.That(result.Success == false);
            Assert.That(result.ErrorJson, Does.Match(pattern));
        }

        /// <summary>
        /// Tests if the result is a correct error message if the img argument is null
        /// </summary>
        [Test]
        public void ImageIsNullFound()
        {
            UserToken token = new UserToken(1, "testUser", "testuser@localhost.com");
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters",new Dictionary<string, string>() { {"image","Image cannot be empty" } });
            FileResult result = controller.LoadImage(null, token.ToToken(), mediaManager);
            Assert.That(result.Success == false);
            Assert.That(result.ErrorJson, Does.Match(pattern));
        }

        [Test]
        public void Success()
        {
            UserToken token = new UserToken(1, "testUser", "testuser@localhost.com");            
            FileResult result = controller.LoadImage("valid", token.ToToken(), mediaManager);
            Assert.That(result.Success == true);
            CollectionAssert.AreEqual(result.FileContentResult.FileContents, new byte[] { 1, 2, 3, 4 });
        }
    }
}
