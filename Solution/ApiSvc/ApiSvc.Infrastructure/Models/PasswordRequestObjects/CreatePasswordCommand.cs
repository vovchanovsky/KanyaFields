using ApiSvc.Domain.Entities;

namespace ApiSvc.Infrastructure.Models.PasswordRequestObjects
{
    public class CreatePasswordCommand
    {
        public CreatePasswordCommand(PasswordItem passwordItem)
        {
            PasswordItem = passwordItem;
        }

        public PasswordItem PasswordItem { get; set; }
    }
}