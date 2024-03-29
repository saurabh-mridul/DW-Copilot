using System.Collections;
using Azure.DigitalTwins.Core;
using Microsoft.DigitalWorkplace.Integration.Models.DigitalTwins;

namespace AzureDigitalTwinsPlatformAPIs.Interfaces
{
    public interface ISpaceDigitalTwinService
    {
        Task<Building?> GetBuilding(string buildingName);
        Task<IEnumerable<Room>> GetRooms(string buildingName, string floorName);
        Task<IEnumerable<Space>> GetSpaces();
        Task<Space> GetSpace(Guid spaceId);
    }
}