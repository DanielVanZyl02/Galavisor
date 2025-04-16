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
            var ExpectedUser = new UserModel
            {
                UserId = 1,
                Name = "Test",
                RoleName = "Traveller",
                IsActive = true,
                GoogleSubject = "1234567890"
            };

            var ConnectionMock = new Mock<IDbConnection>();

            var DbConnection = new TestDatabaseConnection(ConnectionMock.Object);

            var RepositoryMock = new Mock<UserRepository>(DbConnection);

            RepositoryMock.Setup(r => r.GetById(1))
                .ReturnsAsync(ExpectedUser);


            var Result = await RepositoryMock.Object.GetById(1);

            Assert.NotNull(Result);
            Assert.Equal(ExpectedUser.UserId, Result.UserId);
            Assert.Equal(ExpectedUser.Name, Result.Name);
            Assert.Equal(ExpectedUser.RoleName, Result.RoleName);
            Assert.Equal(ExpectedUser.IsActive, Result.IsActive);
            Assert.Equal(ExpectedUser.GoogleSubject, Result.GoogleSubject);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNull()
        {
            var ConnectionMock = new Mock<IDbConnection>();

            var DbConnection = new TestDatabaseConnection(ConnectionMock.Object);
            var RepositoryMock = new Mock<UserRepository>(DbConnection);

            _ = RepositoryMock.Setup(r => r.GetById(999))
                .ReturnsAsync((UserModel)null);

            var Result = await RepositoryMock.Object.GetById(999);
            Assert.Null(Result);
        }

        [Fact]
        public async Task GetBySub_ExistingId_ReturnsUserModel()
        {
            var ExpectedUser = new UserModel
            {
                UserId = 1,
                Name = "Test",
                RoleName = "Traveller",
                IsActive = true,
                GoogleSubject = "1234567890"
            };

            var ConnectionMock = new Mock<IDbConnection>();

            var DbConnection = new TestDatabaseConnection(ConnectionMock.Object);

            var RepositoryMock = new Mock<UserRepository>(DbConnection);

            RepositoryMock.Setup(r => r.GetBySub("1234567890"))
                .ReturnsAsync(ExpectedUser);


            var Result = await RepositoryMock.Object.GetBySub("1234567890");

            Assert.NotNull(Result);
            Assert.Equal(ExpectedUser.UserId, Result.UserId);
            Assert.Equal(ExpectedUser.Name, Result.Name);
            Assert.Equal(ExpectedUser.RoleName, Result.RoleName);
            Assert.Equal(ExpectedUser.IsActive, Result.IsActive);
            Assert.Equal(ExpectedUser.GoogleSubject, Result.GoogleSubject);
        }

        [Fact]
        public async Task GetBySub_NonExistingId_ReturnsNull()
        {
            var ConnectionMock = new Mock<IDbConnection>();

            var DbConnection = new TestDatabaseConnection(ConnectionMock.Object);
            var RepositoryMock = new Mock<UserRepository>(DbConnection);

            _ = RepositoryMock.Setup(r => r.GetById(999))
                .ReturnsAsync((UserModel)null);

            var Result = await RepositoryMock.Object.GetById(999);
            Assert.Null(Result);
        }

        [Fact]
        public async Task Get_ReturnsUserModels()
        {
            var ExpectedUser = new UserModel
            {
                UserId = 1,
                Name = "Test",
                RoleName = "Traveller",
                IsActive = true,
                GoogleSubject = "1234567890"
            };

            var ConnectionMock = new Mock<IDbConnection>();

            var DbConnection = new TestDatabaseConnection(ConnectionMock.Object);

            var RepositoryMock = new Mock<UserRepository>(DbConnection);

            RepositoryMock.Setup(r => r.GetAll())
                .ReturnsAsync([ExpectedUser, ExpectedUser]);


            var Result = await RepositoryMock.Object.GetAll();

            Assert.NotNull(Result);
            Assert.Equal(2, Result.Count);
        }

        [Fact]
        public async Task Get_ReturnsEmpty()
        {
            var ConnectionMock = new Mock<IDbConnection>();

            var DbConnection = new TestDatabaseConnection(ConnectionMock.Object);
            var RepositoryMock = new Mock<UserRepository>(DbConnection);

            _ = RepositoryMock.Setup(r => r.GetAll())
                .ReturnsAsync([]);

            var Result = await RepositoryMock.Object.GetAll();
            Assert.NotNull(Result);
            Assert.Empty(Result);
        }
    }
}