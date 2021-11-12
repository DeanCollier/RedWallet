using RedWallet.Models.RequestModels;
using RedWallet.Models.WalletModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedWallet.Services.Interfaces
{
    public interface IRequestService
    {
        Task<RequestDetail> CreateRequestAsync(WalletIdentity model, string requestAddress);
        Task<bool> DeleteRequestAsync(RequestIdentity model);
        Task<RequestDetail> GetWalletRequestByIdAsync(RequestIdentity model);
        Task<IEnumerable<RequestListItem>> GetWalletRequestsAsync(WalletIdentity model);
    }
}