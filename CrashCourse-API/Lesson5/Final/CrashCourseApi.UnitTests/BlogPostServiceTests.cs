using CrashCourseApi.Web.DataStores;
using CrashCourseApi.Web.Models;
using CrashCourseApi.Web.Services;
using Moq;
using Serilog;
using System;
using System.Linq;
using Xunit;

namespace CrashCourseApi.UnitTests
{
    public class BlogPostServiceTests
    {
        private readonly Mock<IBlogPostDataStore> _mockBlogPostDataStore;
        private readonly Mock<ILogger> _mockLogger;

        private IBlogPostService _service;

        public BlogPostServiceTests()
        {
            _mockBlogPostDataStore = new Mock<IBlogPostDataStore>();
            _mockLogger = new Mock<ILogger>();

            _service = new BlogPostService(_mockBlogPostDataStore.Object, _mockLogger.Object);
        }

        [Fact]
        public void GivenExistingPostBlog_WhenGetByIdCalled_ThenBlogPostReturned()
        {
            // arrange
            BuildMockBlogPost("Lorem ipsum dolor sit amet, essent referrentur mea no, ea perfecto principes vim. No nusquam ancillae cum: IMG:picture1.png. " +
                "Hendrerit ullamcorper eam ne: picture3.png Malis munere IMG: dolore ne per, verear img:picture.png omnesque quaerendum ei has: IMG:picture2.jpg." +
                "Quo ex hinc ornatus erroribus, et alii labore pro. Ad sea sint meliore docendi: IMG:picture4.svg. ");

            // act
            var result = _service.GetById(1);

            // assert
            Assert.NotNull(result);
            Assert.True(result.Item2);
            Assert.Equal("Title", result.Item1.Title);

            var picture = result.Item1.PictureReferences.ToList();
            Assert.Equal(2, picture.Count);
            Assert.Equal("picture1.png", picture[0]);
            Assert.Equal("picture2.jpg", picture[1]);
        }

        [Fact]
        public void GivenExistingPostBlog_WhenGetByIdCalled_ThenBlogPostReturned_AndContentContainsPngPicture()
        {
            // arrange
            BuildMockBlogPost("IMG:picture1.png");

            // act
            var result = _service.GetById(1);

            // assert
            var picture = result.Item1.PictureReferences.ToList();
            Assert.Single(picture);                   // test if there is 1 item in the list
            Assert.Equal("picture1.png", picture[0]); // test if the value is correct
        }

        [Fact]
        public void GivenExistingPostBlog_WhenGetByIdCalled_ThenBlogPostReturned_AndContentContainsJpgPicture()
        {
            // arrange
            BuildMockBlogPost("IMG:picture2.jpg");

            // act
            var result = _service.GetById(1);

            // assert
            var picture = result.Item1.PictureReferences.ToList();
            Assert.Single(picture);                   // test if there is 1 item in the list
            Assert.Equal("picture2.jpg", picture[0]); // test if the value is correct
        }

        [Fact]
        public void GivenExistingPostBlog_WhenGetByIdCalled_ThenBlogPostReturned_AndContentContainsSvgPicture()
        {
            // arrange
            BuildMockBlogPost("IMG:picture4.svg");

            // act
            var result = _service.GetById(1);

            // assert
            var picture = result.Item1.PictureReferences.ToList();
            Assert.Empty(picture); 
        }

        [Fact]
        public void GivenExistingPostBlog_WhenGetByIdCalled_ThenBlogPostReturned_AndContentContainsPngPictureButNoTag()
        {
            // arrange
            BuildMockBlogPost("picture3.png");

            // act
            var result = _service.GetById(1);

            // assert
            var picture = result.Item1.PictureReferences.ToList();
            Assert.Empty(picture);
        }

        [Fact]
        public void GivenExistingPostBlog_WhenGetByIdCalled_ThenBlogPostReturned_AndContentContainsPngPictureButTagInLowercase()
        {
            // arrange
            BuildMockBlogPost("img:picture3.png");

            // act
            var result = _service.GetById(1);

            // assert
            var picture = result.Item1.PictureReferences.ToList();
            Assert.Empty(picture);
        }

        private void BuildMockBlogPost(string content)
        {
            var mockResult = new BlogPost()
            {
                Title = "Title",
                Content = content
            };
            _mockBlogPostDataStore
                .Setup(x => x.SelectById(1)) // Setup the mock's method
                .Returns(new Tuple<BlogPost, bool>(mockResult, true)); // return the mock result
        }
    }
}
