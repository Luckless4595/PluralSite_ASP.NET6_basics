namespace CityInfo.API.Services{
        public class LocalMailService : IMailService{
        private readonly string mailTo ,mailFrom ;

        public LocalMailService(IConfiguration config){
            this.mailFrom = config["mailSettings:mailToAddress"];
            this.mailTo = config["mailSettings:mailFromAddress"];
        }

        public void Send(string subject, string message){
            //send mail to console
            Console.WriteLine($"New mail from {this.mailFrom} to {this.mailTo}"
            +$", with {nameof(LocalMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {message}");
        }
    }
}