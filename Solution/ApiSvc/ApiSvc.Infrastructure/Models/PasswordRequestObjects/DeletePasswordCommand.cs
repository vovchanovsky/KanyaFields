using System;

namespace ApiSvc.Infrastructure.Models.PasswordRequestObjects
{
    public class DeletePasswordCommand
    {
        public DeletePasswordCommand(Guid id)
        {
            PasswordId = id;
        }

        public Guid PasswordId { get; set; }
    }
}