using System.Threading.Channels;
using AutoFixture;
using Microsoft.Extensions.Options;
using NSubstitute;
using RedditScraper.Domain;
using RedditScraper.Domain.RedditApi;
using RedditScraper.Poller;
using RedditScraper.Poller.Services;

namespace RedditScraper.Tests
{
    public class DataStorageTests
    {
        private static Fixture _fixture = new Fixture();
        [Fact]
        public async Task TestChildPublish_HappyPath()
        {
            var childStub = _fixture.Create<Child>();
            childStub.Data.Subreddit = "csharp";
            var options = _fixture.Create<ScraperOptions>();
            options.TopNPosts = 5;
            options.TopNUsers = 5;
            var optionsStub = Options.Create<ScraperOptions>(options);
            var userPostChannel = Channel.CreateBounded<List<UserPosts>>(1);
            var topPostsChannel = Channel.CreateBounded<List<UpvotedPosts>>(1);

            var dataStorage = new DataStorage(optionsStub, userPostChannel, topPostsChannel);

            await dataStorage.UpdateStorage(childStub);
            var userRead1 = await userPostChannel.Reader.ReadAsync();
            var postRead1 = await topPostsChannel.Reader.ReadAsync();
            await dataStorage.UpdateStorage(childStub);
            var userRead2 = await userPostChannel.Reader.ReadAsync();
            var postRead2 = await topPostsChannel.Reader.ReadAsync();
            // Assert that the post count behaves with unique de-duping and concurrency control
            Assert.Equal(userRead1[0].UserId, userRead2[0].UserId);
            Assert.Equal(postRead1[0].Upvotes, postRead2[0].Upvotes);
            Assert.Equal(userRead1[0].PostCount, userRead2[0].PostCount);

            var newChildStub = _fixture.Create<Child>();
            newChildStub.Data.Subreddit = "csharp";
            // assign the same author to both posts
            newChildStub.Data.Author = childStub.Data.Author;
            await dataStorage.UpdateStorage(newChildStub);

            userRead1 = await userPostChannel.Reader.ReadAsync();
            Assert.Equal(2, userRead1[0].PostCount);
            postRead1 = await topPostsChannel.Reader.ReadAsync();
            Assert.Equal(2, postRead1.Count);
        }

        [Fact]
        public async Task TestChildPublish_SubredditBucketing()
        {
            var childStub = _fixture.Create<Child>();
            childStub.Data.Subreddit = "1";
            var options = _fixture.Create<ScraperOptions>();
            options.TopNPosts = 5;
            options.TopNUsers = 5;
            var optionsStub = Options.Create<ScraperOptions>(options);
            var userPostChannel = Channel.CreateBounded<List<UserPosts>>(1);
            var topPostsChannel = Channel.CreateBounded<List<UpvotedPosts>>(1);

            var dataStorage = new DataStorage(optionsStub, userPostChannel, topPostsChannel);

            await dataStorage.UpdateStorage(childStub);
            var userRead1 = await userPostChannel.Reader.ReadAsync();
            var postRead1 = await topPostsChannel.Reader.ReadAsync();
            await dataStorage.UpdateStorage(childStub);
            var userRead2 = await userPostChannel.Reader.ReadAsync();
            var postRead2 = await topPostsChannel.Reader.ReadAsync();
            // Assert that the post count behaves with unique de-duping and concurrency control
            Assert.Equal(userRead1[0].UserId, userRead2[0].UserId);
            Assert.Equal(postRead1[0].Upvotes, postRead2[0].Upvotes);
            Assert.Equal(userRead1[0].PostCount, userRead2[0].PostCount);

            var newChildStub = _fixture.Create<Child>();
            newChildStub.Data.Subreddit = "2";
            // assign the same author to both posts
            newChildStub.Data.Author = childStub.Data.Author;
            await dataStorage.UpdateStorage(newChildStub);

            userRead1 = await userPostChannel.Reader.ReadAsync();
            Assert.Equal(2, userRead1[0].PostCount);
            postRead1 = await topPostsChannel.Reader.ReadAsync();
            //This test is slightly different from the above in that it is testing
            //the bucketing/grouping logic of the subreddit name
            Assert.Equal(1, postRead2.Count);
        }
    }
}