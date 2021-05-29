using ApiSvc.Domain.Entities;

namespace ApiSvc.Application.Password.Commands.CreatePassword
{
    public class CreatePasswordItemDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Password { get; set; }

        public PasswordItem ToPasswordItem() =>
            new PasswordItem
            {
                Title = this.Title,
                Description = this.Description,
                Password = this.Password
            };
    }
}
