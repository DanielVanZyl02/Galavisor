using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using Moq;
using GalavisorApi.Data;
using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Tests
{
    public class ReviewRepositoryTests
    {
        [Fact]
        public async Task GetById_ExistingId_ReturnsReviewModel()
        {
            var expectedReview = new ReviewReturnModel
            {
                ReviewId = 1,
                PlanetId = 2,
                PlanetName = "Neonara",
                UserId = 3,
                UserName = "Daniel",
                Rating = 4,
                Comment = "Great planet!"
            };

            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);

            var repositoryMock = new Mock<ReviewRepository>(dbConnection);

            repositoryMock.Setup(r => r.GetById(1))
                .ReturnsAsync(expectedReview);


            var result = await repositoryMock.Object.GetById(1);

            Assert.NotNull(result);
            Assert.Equal(expectedReview.ReviewId, result.ReviewId);
            Assert.Equal(expectedReview.PlanetId, result.PlanetId);
            Assert.Equal(expectedReview.UserId, result.UserId);
            Assert.Equal(expectedReview.Rating, result.Rating);
            Assert.Equal(expectedReview.Comment, result.Comment);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNull()
        {
            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var repositoryMock = new Mock<ReviewRepository>(dbConnection);

            repositoryMock.Setup(r => r.GetById(999))
                .ReturnsAsync((ReviewReturnModel)null);

            var result = await repositoryMock.Object.GetById(999);
            Assert.Null(result);
        }
    }
}