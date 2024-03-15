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
    }
}