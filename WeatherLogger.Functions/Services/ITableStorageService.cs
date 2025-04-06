using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherLogger.Functions.Models;

namespace WeatherLogger.Functions.Services
{
    public interface ITableStorageService
    {
        Task SaveLogAsync(WeatherLog log);
    }

}
