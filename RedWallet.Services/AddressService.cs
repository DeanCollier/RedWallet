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
            var entity = new Address
            {
                PublicAddress = address.PublicAddress,
                WalletId = model.WalletId,
                IsChange = address.IsChange,
                LatestBalance = 0m,
                Created = DateTimeOffset.Now,
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

                    addressIdentity.PublicAddress = entity.PublicAddress;
                    addressIdentity.UserId = model.UserId;
                }
                else
                {
                    addressIdentity.PublicAddress = clone.PublicAddress;
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
                    .OrderByDescending(r => r.Created)
                    .Where(r => r.Wallet.UserId == model.UserId && r.WalletId == model.WalletId)
                    .Select(r => new AddressListItem
                    {
                        PublicAddress = r.PublicAddress,
                        WalletName = r.Wallet.WalletName,
                        WalletId = r.WalletId,
                        Created = r.Created
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
                    .SingleOrDefaultAsync(r => r.Wallet.UserId == model.UserId && r.PublicAddress == model.PublicAddress);

                var detail = new AddressDetail
                {
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
                    .SingleOrDefaultAsync(r => r.Wallet.UserId == model.UserId && r.PublicAddress == model.PublicAddress);

                entity.LatestBalance = newBalance;
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
                    .SingleAsync(r => r.Wallet.UserId == model.UserId && r.PublicAddress == model.PublicAddress);

                context.Addresses.Remove(entity);
                return await context.SaveChangesAsync() == 1;

            }
        }
    }
}
