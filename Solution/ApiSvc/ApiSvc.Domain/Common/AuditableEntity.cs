using System;
using System.ComponentModel.DataAnnotations;

namespace ApiSvc.Domain.Common
{
    public abstract class AuditableEntity
    {
        // TODO: make it Required when Auth process will be implemented
        // we will get the name (or maybe user id) from the auth context
        public string CreatedBy { get; set; }

        [Required]
        public DateTime Created { get; set; }

        // TODO: make it Required when Auth process will be implemented
        // we will get the name (or maybe user id) from the auth context
        public string LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
