using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace WebODataDataService
{
    public class WeatherForecastModelConfiguration : IModelConfiguration
    {
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
        {
            var forecast = builder.EntitySet<WeatherForecast>("WeatherForecast").EntityType.HasKey(li => li.Id);

            if (apiVersion < ApiVersions.V2)
            {
                forecast.Ignore(f => f.ExtendedForcasting);
            }
        }
    }
}