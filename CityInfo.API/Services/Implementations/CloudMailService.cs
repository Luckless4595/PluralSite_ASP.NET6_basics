using CityInfo.API.Services.Interfaces;
namespace CityInfo.API.Services.Implementations{
        public class CloudMailService : IMailService{
        private readonly string mailTo ,mailFrom ;

        #pragma warning disable 
        public CloudMailService(IConfiguration config){
            this.mailFrom = config["mailSettings:mailToAddress"];
            this.mailTo = config["mailSettings:mailFromAddress"];
        }
        #pragma warning restore

        public void Send(string subject, string message){
            //send mail to console
            Console.WriteLine($"New mail from {this.mailFrom} to {this.mailTo}"
            +$", with {nameof(CloudMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {message}");
        }
    }
}