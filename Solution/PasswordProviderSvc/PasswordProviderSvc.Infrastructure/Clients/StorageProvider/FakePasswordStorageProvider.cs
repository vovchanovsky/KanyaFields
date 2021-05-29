using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasswordProviderSvc.Domain.Entities;
using PasswordProviderSvc.InfrastructureInterfaces.Clients.StorageProvider;

namespace PasswordProviderSvc.Infrastructure.Clients.StorageProvider
{
    public class FakePasswordStorageProvider : IPasswordStorageProvider
    {
        public FakePasswordStorageProvider()
        {
            _passwordItems = GetPasswordItems();
        }


        public Task<IEnumerable<PasswordItem>> GetPasswords(Guid userId)
        {
            var passwords = _passwordItems
                .Where(pair => pair.Key.Item1 == userId)
                .Select(pair => pair.Value)
                .ToList();

            return Task.FromResult(passwords.AsEnumerable());
        }

        public Task<PasswordItem> GetPasswordItem(Guid userId, Guid id)
        {
            if (_passwordItems.TryGetValue((userId, id), out PasswordItem passwordItem) is false)
            {
                throw new ApplicationException("FakePasswordStorageProvider.GetPasswordItem fail");
            }

            return Task.FromResult(passwordItem);
        }

        public Task DeletePassword(Guid userId, Guid id)
        {
            if (_passwordItems.TryRemove((userId, id), out _) is false)
            {
                throw new ApplicationException("FakePasswordStorageProvider.DeletePassword fail");
            }

            return Task.CompletedTask;
        }

        public Task<Guid> InsertPassword(Guid userId, PasswordItem passwordItem)
        {
            if (_passwordItems.TryAdd((userId, passwordItem.PasswordId), passwordItem) is false)
            {
                throw new ApplicationException("FakePasswordStorageProvider.InsertPassword fail");
            }

            return Task.FromResult(passwordItem.PasswordId);
        }


        private static ConcurrentDictionary<(Guid, Guid), PasswordItem> GetPasswordItems()
        {
            var dictionary = new ConcurrentDictionary<(Guid, Guid), PasswordItem>();
            dictionary.TryAdd((Guid.Parse("f8f7d5e9-21df-4b01-82c7-23decd1a57a8"), Guid.Parse("88e7ecc3-0bf5-42cc-bd71-e51b65b0dfc5")), new PasswordItem
            {
                UserId = Guid.Parse("f8f7d5e9-21df-4b01-82c7-23decd1a57a8"),
                PasswordId = Guid.Parse("88e7ecc3-0bf5-42cc-bd71-e51b65b0dfc5"),
                Created = new DateTime(2015, 10, 12),
                CreatedBy = "John",
                LastModified = new DateTime(2015, 11, 1),
                LastModifiedBy = "John",
                Title = "nh pass",
                Password = "123"
            });

            dictionary.TryAdd((Guid.Parse("f8f7d5e9-21df-4b01-82c7-23decd1a57a8"), Guid.Parse("51ffb25b-a7dd-456a-b435-f8154137e08d")), new PasswordItem
            {
                UserId = Guid.Parse("f8f7d5e9-21df-4b01-82c7-23decd1a57a8"),
                PasswordId = Guid.Parse("51ffb25b-a7dd-456a-b435-f8154137e08d"),
                Created = new DateTime(2018, 3, 2),
                CreatedBy = "Vasilii",
                LastModified = new DateTime(2020, 1, 16),
                LastModifiedBy = "Vasilii",
                Title = "Animevost.org password",
                Password = "4564"
            });

            dictionary.TryAdd((Guid.Parse("c5ac1bda-521e-4995-944e-18abb5fb54af"), Guid.Parse("c78a3e5a-eafc-4643-bfe0-ce925f15c40b")), new PasswordItem
            {
                UserId = Guid.Parse("c5ac1bda-521e-4995-944e-18abb5fb54af"),
                PasswordId = Guid.Parse("c78a3e5a-eafc-4643-bfe0-ce925f15c40b"),
                Created = new DateTime(2012, 3, 14),
                CreatedBy = "Solomon",
                LastModified = new DateTime(2015, 9, 11),
                LastModifiedBy = "Solomon",
                Title = "redit fake acc",
                Password = "abc"
            });

            dictionary.TryAdd((Guid.Parse("c5ac1bda-521e-4995-944e-18abb5fb54af"), Guid.Parse("308fb315-c335-4cec-befe-8b0c7157ddec")), new PasswordItem
            {
                UserId = Guid.Parse("c5ac1bda-521e-4995-944e-18abb5fb54af"),
                PasswordId = Guid.Parse("308fb315-c335-4cec-befe-8b0c7157ddec"),
                Created = new DateTime(2016, 6, 1),
                CreatedBy = "Vasilii",
                LastModified = new DateTime(2016, 6, 1),
                LastModifiedBy = "Vasilii",
                Title = "Nox BIP",
                Description =
                    "The first & only cloud-delivered NAC solutions designed for today's ever-expanding networks",
                Password = "!q2w3e4r"
            });

            return dictionary;
        }

        private readonly ConcurrentDictionary<(Guid, Guid), PasswordItem> _passwordItems;
    }
}