using RedWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RedWallet.Models.AddressModels;
using RedWallet.Services.Interfaces;
using RedWallet.Models.WalletModels;

namespace RedWallet.Services
{
    public class AddressService : IAddressService
    {
        // CREATE 
        public async Task<AddressDetail> CreateAddressAsync(WalletIdentity model, AddressCreate address)
        {
            var created = address.Created == null ? DateTimeOffset.Now:address.Created;
            var entity = new Address
            {
                PublicAddress = address.PublicAddress,
                WalletId = model.WalletId,
                IsChange = address.IsChange,
                LatestBalance = 0m,
                Created = created.GetValueOrDefault()
            };

            var addressIdentity = new AddressIdentity();

            using (var context = new ApplicationDbContext())
            {
                var clone = await context
                    .Addresses
                    .SingleOrDefaultAsync(r => r.Wallet.UserId == model.UserId && r.PublicAddress == address.PublicAddress);

                if (clone == null)
                {
                    context.Addresses.Add(entity);
                    await context.SaveChangesAsync();

                    addressIdentity.AddressId = entity.Id;
                    addressIdentity.UserId = model.UserId;
                }
                else
                {
                    addressIdentity.AddressId = clone.Id;
                    addressIdentity.UserId = model.UserId;
                }
            }

            return await GetWalletAddressByIdAsync(addressIdentity);
        }

        // READ
        public async Task<IEnumerable<AddressListItem>> GetWalletAddressesAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Addresses
                    .OrderByDescending(a => a.Created)
                    .Where(a => a.Wallet.UserId == model.UserId && a.WalletId == model.WalletId)
                    .Select(a => new AddressListItem
                    {
                        AddressId = a.Id,
                        PublicAddress = a.PublicAddress,
                        WalletName = a.Wallet.WalletName,
                        WalletId = a.WalletId,
                        LatestBalance = a.LatestBalance,
                        Created = a.Created
                    });

                return await query.ToArrayAsync();
            }
        }

        // READ
        public async Task<AddressDetail> GetWalletAddressByIdAsync(AddressIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Addresses
                    .SingleOrDefaultAsync(a => a.Wallet.UserId == model.UserId && a.Id == model.AddressId);

                var detail = new AddressDetail
                {
                    AddressId = entity.Id,
                    PublicAddress = entity.PublicAddress,
                    WalletId = entity.WalletId,
                    WalletName = entity.Wallet.WalletName,
                    IsChange = entity.IsChange,
                    LatestBalance = entity.LatestBalance,
                    Created = entity.Created
                };

                return detail;
            }
        }

        // UPDATE
        public async Task<bool> UpdateAddressLatestBalance(AddressIdentity model, decimal newBalance)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Addresses
                    .SingleOrDefaultAsync(a => a.Wallet.UserId == model.UserId && a.Id == model.AddressId);

                entity.LatestBalance = newBalance;
                return await context.SaveChangesAsync() == 1;
            }
        }
        // UPDATE
        public async Task<bool> UpdateAddressCreatedDate(AddressIdentity model, DateTimeOffset created)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Addresses
                    .SingleOrDefaultAsync(a => a.Wallet.UserId == model.UserId && a.Id == model.AddressId);

                entity.Created = created;
                return await context.SaveChangesAsync() == 1;
            }
        }

        // DELETE 
        public async Task<bool> DeleteAddressAsync(AddressIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Addresses
                    .SingleAsync(a => a.Wallet.UserId == model.UserId && a.Id == model.AddressId);

                context.Addresses.Remove(entity);
                return await context.SaveChangesAsync() == 1;

            }
        }
    }
}
