namespace CityInfo.API.Services{
        public class CloudMailService : IMailService{
        private string mailTo = "admin@company.com";
        private string mailFrom = "noreply@company.com";

        public void Send(string subject, string message){
            //send mail to console
            Console.WriteLine($"New mail from {this.mailFrom} to {this.mailTo}"
            +$", with {nameof(CloudMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {message}");
        }
    }
}