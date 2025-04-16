using System.Data;
using Xunit;
using Moq;
using GalavisorApi.Data;
using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task GetById_ExistingId_ReturnsUserModel()
        {
            var expectedUser = new UserModel
            {
                UserId = 1,
                Name = "Test",
                RoleName = "Traveller",
                IsActive = true,
                GoogleSubject = "1234567890"
            };

            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);

            var repositoryMock = new Mock<UserRepository>(dbConnection);

            repositoryMock.Setup(r => r.GetById(1))
                .ReturnsAsync(expectedUser);


            var result = await repositoryMock.Object.GetById(1);

            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserId, result.UserId);
            Assert.Equal(expectedUser.Name, result.Name);
            Assert.Equal(expectedUser.RoleName, result.RoleName);
            Assert.Equal(expectedUser.IsActive, result.IsActive);
            Assert.Equal(expectedUser.GoogleSubject, result.GoogleSubject);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNull()
        {
            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var repositoryMock = new Mock<UserRepository>(dbConnection);

            _ = repositoryMock.Setup(r => r.GetById(999))
                .ReturnsAsync((UserModel)null);

            var result = await repositoryMock.Object.GetById(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetBySub_ExistingId_ReturnsUserModel()
        {
            var expectedUser = new UserModel
            {
                UserId = 1,
                Name = "Test",
                RoleName = "Traveller",
                IsActive = true,
                GoogleSubject = "1234567890"
            };

            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);

            var repositoryMock = new Mock<UserRepository>(dbConnection);

            repositoryMock.Setup(r => r.GetBySub("1234567890"))
                .ReturnsAsync(expectedUser);


            var result = await repositoryMock.Object.GetBySub("1234567890");

            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserId, result.UserId);
            Assert.Equal(expectedUser.Name, result.Name);
            Assert.Equal(expectedUser.RoleName, result.RoleName);
            Assert.Equal(expectedUser.IsActive, result.IsActive);
            Assert.Equal(expectedUser.GoogleSubject, result.GoogleSubject);
        }

        [Fact]
        public async Task GetBySub_NonExistingId_ReturnsNull()
        {
            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var repositoryMock = new Mock<UserRepository>(dbConnection);

            _ = repositoryMock.Setup(r => r.GetById(999))
                .ReturnsAsync((UserModel)null);

            var result = await repositoryMock.Object.GetById(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ReturnsUserModels()
        {
            var expectedUser = new UserModel
            {
                UserId = 1,
                Name = "Test",
                RoleName = "Traveller",
                IsActive = true,
                GoogleSubject = "1234567890"
            };

            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);

            var repositoryMock = new Mock<UserRepository>(dbConnection);

            repositoryMock.Setup(r => r.GetAll())
                .ReturnsAsync([expectedUser, expectedUser]);


            var result = await repositoryMock.Object.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Get_ReturnsEmpty()
        {
            var connectionMock = new Mock<IDbConnection>();

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var repositoryMock = new Mock<UserRepository>(dbConnection);

            _ = repositoryMock.Setup(r => r.GetAll())
                .ReturnsAsync([]);

            var result = await repositoryMock.Object.GetAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}