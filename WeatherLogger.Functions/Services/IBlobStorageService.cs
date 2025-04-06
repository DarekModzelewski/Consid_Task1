using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherLogger.Functions.Services
{
    public interface IBlobStorageService
    {
        Task SavePayloadAsync(string blobName, string content);
    }

}
