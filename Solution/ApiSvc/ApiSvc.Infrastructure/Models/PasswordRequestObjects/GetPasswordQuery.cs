using System;

namespace ApiSvc.Infrastructure.Models.PasswordRequestObjects
{
    public class GetPasswordQuery
    {
        public GetPasswordQuery(Guid id)
        {
            PasswordId = id;
        }

        public Guid PasswordId { get; set; }
    }
}