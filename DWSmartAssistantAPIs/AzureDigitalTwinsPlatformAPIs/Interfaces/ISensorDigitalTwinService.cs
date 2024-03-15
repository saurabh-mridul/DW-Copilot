using Microsoft.DigitalWorkplace.Integration.Models.DigitalTwins;

namespace AzureDigitalTwinsPlatformAPIs.Interfaces
{
    public interface ISensorDigitalTwinService
    {
        Task<IEnumerable<Sensor>> GetSensors();
        Task<Sensor> GetSensor(Guid sensorId);
    }
}