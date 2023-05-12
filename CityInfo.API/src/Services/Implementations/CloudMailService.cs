using CityInfo.API.src.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.src.Services.Implementations
{
    public class CloudMailService : IMailService
    {
        private readonly ILogger<CloudMailService> _logger;
        private readonly string mailTo;
        private readonly string mailFrom;

        #pragma warning disable
        public CloudMailService(IConfiguration config, ILogger<CloudMailService> logger)
        {
            this.mailFrom = config["mailSettings:mailToAddress"];
            this.mailTo = config["mailSettings:mailFromAddress"];
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #pragma warning restore
        public void Send(string subject, string message){
            //send mail to console
            _logger.LogInformation($"New mail from {this.mailFrom} to {this.mailTo}"
            +$", with {nameof(CloudMailService)}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body: {message}");
        }
    }
}