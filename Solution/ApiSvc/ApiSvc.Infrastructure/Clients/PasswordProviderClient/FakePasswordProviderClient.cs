using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiSvc.Domain.Entities;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient;
using ApiSvc.InfrastructureInterfaces.Clients.PasswordProviderClient.Exceptions;

namespace ApiSvc.Infrastructure.Clients.PasswordProviderClient
{
    public class FakePasswordProviderClient : IPasswordProviderClient
    {
        public FakePasswordProviderClient()
        {
            _passwordItems = GetPasswordItems();
        }


        public Task<IEnumerable<PasswordItem>> GetPasswords() =>
            Task.FromResult(_passwordItems.AsEnumerable());

        public Task<PasswordItem> GetPasswordItem(Guid id) =>
            Task.FromResult(_passwordItems
                .FirstOrDefault(pass => pass.Id.Equals(id)));

        public Task DeletePassword(Guid id)
        {
            if (_passwordItems.Any(pass => pass.Id.Equals(id)))
            {
                _passwordItems.RemoveAll(pass => pass.Id.Equals(id));
                return Task.CompletedTask;
            }

            throw new NotFoundException(nameof(PasswordItem), id);
        }

        public Task<Guid> InsertPassword(PasswordItem passwordItem)
        {
            // this step should be done by MongoDB (or Cassandra)
            passwordItem.Id = Guid.NewGuid();

            _passwordItems.Add(passwordItem);
            return Task.FromResult(passwordItem.Id);
        }


        private static List<PasswordItem> GetPasswordItems()
        {
            return new()
            {
                new PasswordItem
                {
                    Id = Guid.NewGuid(),
                    Created = new DateTime(2015, 10, 12),
                    CreatedBy = "John",
                    LastModified = new DateTime(2015, 11, 1),
                    LastModifiedBy = "John",
                    Title = "nh pass",
                    Password = "123"
                },
                new PasswordItem
                {
                    Id = Guid.NewGuid(),
                    Created = new DateTime(2018, 3, 2),
                    CreatedBy = "Vasilii",
                    LastModified = new DateTime(2020, 1, 16),
                    LastModifiedBy = "Vasilii",
                    Title = "Animevost.org password",
                    Password = "4564"
                },
                new PasswordItem
                {
                    Id = Guid.NewGuid(),
                    Created = new DateTime(2012, 3, 14),
                    CreatedBy = "Solomon",
                    LastModified = new DateTime(2015, 9, 11),
                    LastModifiedBy = "Solomon",
                    Title = "redit fake acc",
                    Password = "abc"
                },
                new PasswordItem
                {
                    Id = Guid.NewGuid(),
                    Created = new DateTime(2016, 6, 1),
                    CreatedBy = "Vasilii",
                    LastModified = new DateTime(2016, 6, 1),
                    LastModifiedBy = "Vasilii",
                    Title = "Nox BIP",
                    Description =
                        "The first & only cloud-delivered NAC solutions designed for today's ever-expanding networks",
                    Password = "!q2w3e4r"
                },
            };
        }
        
        private readonly List<PasswordItem> _passwordItems;
    }
}
