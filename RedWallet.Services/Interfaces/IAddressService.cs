using RedWallet.Models.AddressModels;
using RedWallet.Models.WalletModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedWallet.Services.Interfaces
{
    public interface IAddressService
    {
        Task<AddressDetail> CreateAddressAsync(WalletIdentity model, AddressCreate address);
        Task<bool> DeleteAddressAsync(AddressIdentity model);
        Task<AddressDetail> GetWalletAddressByIdAsync(AddressIdentity model);
        Task<IEnumerable<AddressListItem>> GetWalletAddressesAsync(WalletIdentity model);
        Task<bool> UpdateAddressLatestBalance(AddressIdentity model, decimal newBalance);
        Task<bool> UpdateAddressCreatedDate(AddressIdentity model, DateTimeOffset created);
    }
}