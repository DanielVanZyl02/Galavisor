using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Dapper;
using Dapper;
using GalavisorApi.Data;
using GalavisorApi.Models;
using GalavisorApi.Services;

namespace GalavisorApi.Tests
{
    public class TransportServiceTests
    {
        [Fact]
        public async Task GetTransportByPlanet_ExistingPlanet_ReturnsTransports()
        {
            
            var planetName = "Jupiter";
            var expectedTransports = new List<TransportModel>
            {
                new TransportModel
                {
                    TransportId = 1,
                    Name = "Rocket",
                    PlanetName = planetName
                },
                new TransportModel
                {
                    TransportId = 2,
                    Name = "Space Train",
                    PlanetName = planetName
                }
            };

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupDapperAsync(c => c.QueryAsync<TransportModel>(
                    It.IsAny<string>(),
                    It.Is<object>(p => p.GetType().GetProperty("PlanetName").GetValue(p).Equals(planetName)),
                    null,
                    null,
                    null))
                .ReturnsAsync(expectedTransports);

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var transportService = new TransportService(dbConnection);

            
            var result = await transportService.GetTransportByPlanet(planetName);

            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedTransports[0].TransportId, result[0].TransportId);
            Assert.Equal(expectedTransports[0].Name, result[0].Name);
            Assert.Equal(expectedTransports[1].TransportId, result[1].TransportId);
            Assert.Equal(expectedTransports[1].Name, result[1].Name);
        }

        [Fact]
        public async Task GetTransportByPlanet_NonExistingPlanet_ReturnsEmptyList()
        {
            
            var planetName = "NonExistingPlanet";
            var emptyList = new List<TransportModel>();

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupDapperAsync(c => c.QueryAsync<TransportModel>(
                    It.IsAny<string>(),
                    It.Is<object>(p => p.GetType().GetProperty("PlanetName").GetValue(p).Equals(planetName)),
                    null,
                    null,
                    null))
                .ReturnsAsync(emptyList);

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var transportService = new TransportService(dbConnection);

            
            var result = await transportService.GetTransportByPlanet(planetName);

            
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
} 