using Azure.DigitalTwins.Core;
using AzureDigitalTwinsPlatformAPIs.Interfaces;
using Microsoft.DigitalWorkplace.DigitalTwins.QueryBuilder;
using Microsoft.DigitalWorkplace.Integration.Extensions.DigitalTwins;
using Microsoft.DigitalWorkplace.Integration.Models.DigitalTwins;

namespace AzureDigitalTwinsPlatformAPIs.Services
{
    public class SensorDigitalTwinService : ISensorDigitalTwinService
    {
        private DigitalTwinsClient _digitalTwinsClient;

        public SensorDigitalTwinService(DigitalTwinsClient digitalTwinsClient)
        {
            _digitalTwinsClient = digitalTwinsClient;
        }

        public async Task<Sensor> GetSensor(Guid sensorId)
        {
            return await _digitalTwinsClient.GetDigitalTwinDipAsync<Sensor>(sensorId.ToString()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Sensor>> GetSensors()
        {
             var sensorList = new List<Sensor>();
            var sensorQuery = QueryBuilder
                                .From<Sensor>()
                                .Top(5);

            var sensors = _digitalTwinsClient.QueryDipAsync(sensorQuery);
            await foreach (var sensor in sensors)
            {
                sensorList.Add(sensor);
            }
            return sensorList;
        }
    }
}