using RedWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using RedWallet.Models.RequestModels;

namespace RedWallet.Services
{
    public class RequestService
    {
        private readonly Guid _userId;

        public RequestService(Guid userId)
        {
            _userId = userId;
        }

        // CREATE 
        public async Task<RequestDetail> CreateRequestAsync(int walletId, string requestAddress)
        {
            var entity = new Request
            {
                WalletId = walletId,
                RequestAddress = requestAddress,
                Created = DateTimeOffset.Now,
            };

            using (var context = new ApplicationDbContext())
            {
                context.Requests.Add(entity);
                await context.SaveChangesAsync();
            }

            return await GetWalletRequestByIdAsync(entity.Id);
        }

        // READ
        public async Task<IEnumerable<RequestListItem>> GetWalletRequestsAsync(int walletId)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context
                    .Requests
                    .Where(r => r.Wallet.UserId == _userId.ToString() && r.WalletId == walletId)
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
        public async Task<RequestDetail> GetWalletRequestByIdAsync(int Id)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Requests
                    .SingleAsync(r => r.Wallet.UserId == _userId.ToString() && r.Id == Id);

                var model = new RequestDetail
                {
                    Id = entity.Id,
                    WalletId = entity.WalletId,
                    WalletName = entity.Wallet.WalletName,
                    RequestAddress = entity.RequestAddress,
                    Created = entity.Created
                };

                return model;
            }
        }

        // UPDATE -- NONE

        // DELETE -- only for DB purposes, don't really need
        public async Task<bool> DeleteRequestAsync(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var entity = await context
                    .Requests
                    .SingleAsync(r => r.Wallet.UserId == _userId.ToString() && r.Id == id);

                context.Requests.Remove(entity);
                return await context.SaveChangesAsync() == 1; 

            }
        }
    }
}
