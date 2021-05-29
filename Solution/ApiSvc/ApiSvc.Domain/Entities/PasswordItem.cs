using System;
using System.ComponentModel.DataAnnotations;
using ApiSvc.Domain.Common;

namespace ApiSvc.Domain.Entities
{
    public class PasswordItem : AuditableEntity
    {
        public Guid UserId { get; set; }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }
    }
}

