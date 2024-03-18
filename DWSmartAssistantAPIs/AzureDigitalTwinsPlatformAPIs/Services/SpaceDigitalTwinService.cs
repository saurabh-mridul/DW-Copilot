using Azure.DigitalTwins.Core;
using AzureDigitalTwinsPlatformAPIs.Interfaces;
using Microsoft.DigitalWorkplace.DigitalTwins.QueryBuilder;
using Microsoft.DigitalWorkplace.Integration.Extensions.DigitalTwins;
using Microsoft.DigitalWorkplace.Integration.Models.DigitalTwins;

namespace AzureDigitalTwinsPlatformAPIs.Services
{
    public class SpaceDigitalTwinService : ISpaceDigitalTwinService
    {
        private readonly DigitalTwinsClient _digitalTwinsClient;
        public SpaceDigitalTwinService(DigitalTwinsClient digitalTwinsClient)
        {
            _digitalTwinsClient = digitalTwinsClient;
        }

        public async Task<Space> GetSpace(Guid spaceId)
        {
            return await _digitalTwinsClient.GetDigitalTwinDipAsync<Space>(spaceId.ToString()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Space>> GetSpaces()
        {
            var spaceList = new List<Space>();
            var spaceQuery = QueryBuilder
                                .From<Space>()
                                .Top(5);

            var spaces = _digitalTwinsClient.QueryDipAsync(spaceQuery);
            await foreach (var space in spaces)
            {
                spaceList.Add(space);
            }
            return spaceList;
        }

        public async Task<IEnumerable<Room>> GetRooms(string buildingName, string floorName)
        {
            var roomList = new List<Room>();
            var building = await GetBuilding(buildingName);
            var floorQuery = QueryBuilder
                                .From<Building>()
                                .Join<Building, Floor>(f => f.HasChildren)
                                .Where<Building>(f => f.Id, ComparisonOperators.IsEqualTo, building?.Id)
                                .Select<Floor>();

            var floors = _digitalTwinsClient.QueryDipAsync(floorQuery);
            await foreach (var floor in floors)
            {
                if (floor.Name == floorName)
                {
                    var roomQuery = QueryBuilder
                                    .From<Floor>()
                                    .Join<Floor, Room>(r => r.HasChildren)
                                    .Where<Floor>(r => r.Id, ComparisonOperators.IsEqualTo, floor.Id)
                                    .Select<Room>();

                    var rooms = _digitalTwinsClient.QueryDipAsync(roomQuery);
                    await foreach (var room in rooms)
                    {
                        roomList.Add(room);
                    }
                    break;
                }
            }
            return roomList;
        }

        public async Task<Building?> GetBuilding(string buildingName)
        {
            var buildingQuery = QueryBuilder
                                 .From<Building>()
                                 .Where<Building>(b => b.Name == buildingName)
                                 .Top(1);

            var buildingResult = _digitalTwinsClient.QueryDipAsync<Building>(buildingQuery);
            await foreach (var building in buildingResult)
            {
                return building;
            }
            return null;
        }
    }
}