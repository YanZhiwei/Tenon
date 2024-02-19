using Refit;

namespace ConsulRefitSample
{
    public interface IWeatherForecastApi
    {
        [Get("/WeatherForecast")]
        Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);
    }
}
