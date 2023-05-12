using CityInfo.API.src.Services.Interfaces;
namespace CityInfo.API.src.Services.Implementations{
        public class LocalMailService : IMailService{
        private readonly string mailTo ,mailFrom ;

        #pragma warning disable
        public LocalMailService(IConfiguration config){
            this.mailFrom = config["mailSettings:mailToAddress"];
            this.mailTo = config["mailSettings:mailFromAddress"];
        }
        #pragma warning restore

        public void Send(string subject, string message){
            //send mail to console
            Console.WriteLine($"New mail from {this.mailFrom} to {this.mailTo}"
            +$", with {nameof(LocalMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {message}");
        }
    }
}