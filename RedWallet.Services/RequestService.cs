using RedWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RedWallet.Models.RequestModels;
using RedWallet.Services.Interfaces;
using RedWallet.Models.WalletModels;

namespace RedWallet.Services
{
    public class RequestService : IRequestService
    {
        // CREATE 
        public async Task<RequestDetail> CreateRequestAsync(WalletIdentity model, string requestAddress)
        {
            var entity = new Request
            {
                WalletId = model.WalletId,
                RequestAddress = requestAddress,
                Created = DateTimeOffset.Now,
            };

            using (var context = new ApplicationDbContext())
            {
                context.Requests.Add(entity);
                await context.SaveChangesAsync();
            }

            var requestIdentity = new RequestIdentity { RequestId = entity.Id, UserId = model.UserId };
            return await GetWalletRequestByIdAsync(requestIdentity);
        }

        // READ
        public async Task<IEnumerable<RequestListItem>> GetWalletRequestsAsync(WalletIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Requests
                    .Where(r => r.Wallet.UserId == model.UserId && r.WalletId == model.WalletId)
                    .Select(r => new RequestListItem
                    {
                        RequestId = r.Id,
                        WalletName = r.Wallet.WalletName,
                        RequestAddress = r.RequestAddress
                    });

                return await query.ToArrayAsync();
            }
        }

        // READ
        public async Task<RequestDetail> GetWalletRequestByIdAsync(RequestIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Requests
                    .SingleAsync(r => r.Wallet.UserId == model.UserId && r.Id == model.RequestId);

                var detail = new RequestDetail
                {
                    RequestId = entity.Id,
                    WalletId = entity.WalletId,
                    WalletName = entity.Wallet.WalletName,
                    RequestAddress = entity.RequestAddress,
                    Created = entity.Created
                };

                return detail;
            }
        }

        // UPDATE -- not needed, there are no properties to change - NICK

        // DELETE -- only for DB purposes, don't really need
        public async Task<bool> DeleteRequestAsync(RequestIdentity model)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Requests
                    .SingleAsync(r => r.Wallet.UserId == model.UserId && r.Id == model.RequestId);

                context.Requests.Remove(entity);
                return await context.SaveChangesAsync() == 1;

            }
        }
    }
}
