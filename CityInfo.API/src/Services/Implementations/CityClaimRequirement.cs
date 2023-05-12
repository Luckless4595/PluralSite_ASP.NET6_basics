using CityInfo.API.src.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;


namespace CityInfo.API.src.Services.Implementations{

    public class CityRequirement : IAuthorizationRequirement{}

    public class CityRequirementHandler : AuthorizationHandler<CityRequirement>
    {
        private readonly ILogger<CityRequirementHandler> _logger;
        private readonly ICityInfoRepository _repository;

        public CityRequirementHandler(
            ILogger<CityRequirementHandler> logger, 
            ICityInfoRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CityRequirement requirement)
        {
            // Assuming there is a route value named "cityId".
            if (context.Resource is AuthorizationFilterContext mvcContext
                && mvcContext.RouteData.Values.TryGetValue("cityId", out var cityIdObj)
                && int.TryParse(cityIdObj?.ToString(), out var cityId))
            {
                var cityNameClaim = context.User.FindFirst(c => c.Type == "city")?.Value;
                _logger.LogInformation($"City name from context: {cityNameClaim}");
                if (cityNameClaim != null
                    && await _repository.CheckCityNameMatchesCityId(cityNameClaim, cityId))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            context.Fail();
        }
    }



}