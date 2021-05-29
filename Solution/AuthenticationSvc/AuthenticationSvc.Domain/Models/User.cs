namespace AuthenticationSvc.Domain.Models
{
    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Claims { get; set; }
    }
}
