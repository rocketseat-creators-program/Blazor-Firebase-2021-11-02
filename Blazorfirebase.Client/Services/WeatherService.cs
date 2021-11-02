using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorfirebase.Client.Models;
using Blazorfirebase.Client.Network;

namespace Blazorfirebase.Client.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherApiService _api;
        public WeatherService(IWeatherApiService api)
        {
            _api = api;
        }
        public async Task<List<WeatherForecast>> GetWeather()
        {
            return await _api.GetWeather();
        }
    }
    public interface IWeatherService
    {
        Task<List<WeatherForecast>> GetWeather();
    }
}