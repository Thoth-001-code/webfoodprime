using webfoodprime.DTOs.Address;
using webfoodprime.Models;

namespace webfoodprime.Services.Interfaces
{
    public interface IAddressService
    {
        Task Create(string userId, CreateAddressDTO dto);

        Task<List<Address>> GetMyAddresses(string userId);

        Task Update(string userId, UpdateAddressDTO dto);

        Task Delete(string userId, int addressId);
    }
}
