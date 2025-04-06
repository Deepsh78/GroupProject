using GroupApi.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Dto
{
    public class AddAddressDto
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
     
        public Guid UserId { get; set; }
     
    }
    }
