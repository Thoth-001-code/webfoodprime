   using Microsoft.EntityFrameworkCore;
    using webfoodprime.DTOs.Address;
    using webfoodprime.Models;
    using webfoodprime.Services.Interfaces;


namespace webfoodprime.Services.Implementations
{
 
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;

        public AddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(string userId, CreateAddressDTO dto)
        {
            var address = new Address
            {
                UserId = userId,
                FullAddress = dto.FullAddress,
                Phone = dto.phone, // 🔥 thêm
                Note = dto.Note
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Address>> GetMyAddresses(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task Update(string userId, UpdateAddressDTO dto)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == dto.AddressId);

            if (address == null)
                throw new Exception("Address not found");

            // 🔥 CHECK OWNER
            if (address.UserId != userId)
                throw new Exception("Unauthorized");

            address.FullAddress = dto.FullAddress;
            address.Note = dto.Note;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(string userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId);

            if (address == null)
                throw new Exception("Address not found");

            // 🔥 CHECK OWNER
            if (address.UserId != userId)
                throw new Exception("Unauthorized");

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}
