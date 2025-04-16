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
    public class ActivityServiceTests
    {
        [Fact]
        public async Task GetActivitiesByPlanet_ExistingPlanet_ReturnsActivities()
        {
        
            var planetName = "Mars";
            var expectedActivities = new List<ActivityModel>
            {
                new ActivityModel
                {
                    ActivityId = 1,
                    Name = "Hiking",
                    PlanetName = planetName
                },
                new ActivityModel
                {
                    ActivityId = 2,
                    Name = "Climbing",
                    PlanetName = planetName
                }
            };

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupDapperAsync(c => c.QueryAsync<ActivityModel>(
                    It.IsAny<string>(),
                    It.Is<object>(p => p.GetType().GetProperty("PlanetName").GetValue(p).Equals(planetName)),
                    null,
                    null,
                    null))
                .ReturnsAsync(expectedActivities);

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var activityService = new ActivityService(dbConnection);

            
            var result = await activityService.GetActivitiesByPlanet(planetName);

            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedActivities[0].ActivityId, result[0].ActivityId);
            Assert.Equal(expectedActivities[0].Name, result[0].Name);
            Assert.Equal(expectedActivities[1].ActivityId, result[1].ActivityId);
            Assert.Equal(expectedActivities[1].Name, result[1].Name);
        }

        [Fact]
        public async Task GetActivitiesByPlanet_NonExistingPlanet_ReturnsEmptyList()
        {
            
            var planetName = "NonExistingPlanet";
            var emptyList = new List<ActivityModel>();

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.SetupDapperAsync(c => c.QueryAsync<ActivityModel>(
                    It.IsAny<string>(),
                    It.Is<object>(p => p.GetType().GetProperty("PlanetName").GetValue(p).Equals(planetName)),
                    null,
                    null,
                    null))
                .ReturnsAsync(emptyList);

            var dbConnection = new TestDatabaseConnection(connectionMock.Object);
            var activityService = new ActivityService(dbConnection);

            
            var result = await activityService.GetActivitiesByPlanet(planetName);

            
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
} 