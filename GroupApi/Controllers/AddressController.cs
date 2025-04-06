using GroupApi.Dto;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Controllers
{
    public class AddressController(IAddressService AdddressService): Controller
    {
        [HttpPost("Add")]
        public IActionResult AddAddress([FromBody] AddAddressDto addressDto)
        {
            try
            {
                AdddressService.AddAddress(addressDto);
                return Ok("Added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    
}
