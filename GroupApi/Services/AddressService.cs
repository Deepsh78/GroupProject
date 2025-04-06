using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using GroupApi.Data;
using GroupApi.Dto;
using GroupApi.Services.Interface;

namespace GroupApi.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicaionDbContext _context;
        public AddressService(ApplicaionDbContext context)
        {
            _context = context;
        }
        public void AddAddress(AddAddressDto addressData)
        {
            try
            {
                var address = new Entities.Address
                {
                    Address1 = addressData.Address1,
                    Address2 = addressData.Address2,
                    City = addressData.City,
                    State = addressData.State,
                    Zip = addressData.Zip,
                    UserId = addressData.UserId
                };
                _context.Addresses.Add(address);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdateAddress(AddAddressDto addressData, Guid id)
        {
            try
            {
                var address = _context.Addresses.FirstOrDefault(x => x.Id == id);
                if (address == null)
                {
                    throw new Exception("No address found");
                }
                address.Address1 = addressData.Address1;
                address.Address2 = addressData.Address2;
                address.City = addressData.City;
                address.State = addressData.State;
                address.Zip = addressData.Zip;
                _context.Addresses.Update(address);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating address: {ex.Message}");
            }
        }

        public GetAddressDto GetAllAddress()
        {
            try
            {
                var addresses = _context.Addresses.ToList();
                if (addresses == null)
                {
                    throw new Exception("No address found");
                }
                var addressDto = new List<GetAddressDto>();
                foreach (var address in addresses)
                {
                    addressDto.Add(new GetAddressDto
                    {
                        Address1 = address.Address1,
                        Address2 = address.Address2,
                        City = address.City,
                        State = address.State,
                        Zip = address.Zip,
                        UserId = address.UserId
                    });
                }
                return addressDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting addresses: {ex.Message}");
            }
        }
}
