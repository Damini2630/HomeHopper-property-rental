using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineRentalPropertyManagement.DTOs;
using OnlineRentalPropertyManagement.Models;
using OnlineRentalPropertyManagement.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalPropertyManagement.Controllers
{
    [Authorize] // Requires authentication for all actions in this controller
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet("all")]
       
        public async Task<ActionResult<IEnumerable<PropertyDTO>>> GetProperties()
        {
            var properties = await _propertyService.GetPropertiesAsync();
            return Ok(properties.Select(p => new PropertyDTO
            {
                PropertyID = p.PropertyID, // Include PropertyID here
                PropertyName = p.PropertyName,
                Address = p.Address,
                State = p.State,
                Country = p.Country,
                RentAmount = p.RentAmount,
                AvailabilityStatus = p.AvailabilityStatus,
                Amenities = p.Amenities,
                OwnerID = p.OwnerID,
                ImagePath = p.ImagePath
            }));
        }
        [HttpGet]

        //[Authorize(Roles = "Owner")]

        public async Task<ActionResult<IEnumerable<PropertyDTO>>> GetOwnerProperties()

        {

            var ownerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || c.Type == "ownerId");

            if (ownerIdClaim == null)

                return Unauthorized("Owner ID not found in token.");

            int ownerId = int.Parse(ownerIdClaim.Value);

            var properties = await _propertyService.GetPropertiesByOwnerIdAsync(ownerId);

            //return Ok(properties);

            var dtoList = properties.Select(p => new PropertyDTO

            {

                PropertyID = p.PropertyID,

                PropertyName = p.PropertyName,

                Address = p.Address,

                State = p.State,

                Country = p.Country,

                RentAmount = p.RentAmount,

                ImagePath = p.ImagePath,

                AvailabilityStatus = p.AvailabilityStatus

            });

            return Ok(dtoList);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDTO>> GetProperty(int id)
        {
            var property = await _propertyService.GetPropertyAsync(id);

            if (property == null)
            {
                return NotFound();
            }

            return Ok(new PropertyDTO
            {
                PropertyID = property.PropertyID, // Include PropertyID here
                PropertyName = property.PropertyName,
                Address = property.Address,
                State = property.State,
                Country = property.Country,
                RentAmount = property.RentAmount,
                AvailabilityStatus = property.AvailabilityStatus,
                Amenities = property.Amenities,
                OwnerID = property.OwnerID,
                ImagePath = property.ImagePath
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty([FromBody] PropertyDTO propertyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get OwnerID from the User Claims
            var ownerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || c.Type == "ownerId");
            if (ownerIdClaim == null || !int.TryParse(ownerIdClaim.Value, out int ownerId))
            {
                return BadRequest("Owner ID not found in the token.");
            }

            var property = new Property
            {
                PropertyName = propertyDto.PropertyName,
                Address = propertyDto.Address,
                State = propertyDto.State,
                Country = propertyDto.Country,
                RentAmount = propertyDto.RentAmount,
                AvailabilityStatus = propertyDto.AvailabilityStatus,
                Amenities = propertyDto.Amenities,
                OwnerID = ownerId, // Use the OwnerID from the token
                ImagePath = propertyDto.ImagePath
            };

            var addedProperty = await _propertyService.AddPropertyAsync(property);

            if (addedProperty != null)
            {
                return CreatedAtAction(nameof(GetProperty), new { id = addedProperty.PropertyID }, new PropertyDTO
                {
                    PropertyID = addedProperty.PropertyID, // Include PropertyID here
                    PropertyName = addedProperty.PropertyName,
                    Address = addedProperty.Address,
                    State = addedProperty.State,
                    Country = addedProperty.Country,
                    RentAmount = addedProperty.RentAmount,
                    AvailabilityStatus = addedProperty.AvailabilityStatus,
                    Amenities = addedProperty.Amenities,
                    OwnerID = addedProperty.OwnerID,
                    ImagePath = addedProperty.ImagePath
                });
            }
            else
            {
                return StatusCode(500, "Failed to add property");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> UpdateProperty(int id, [FromBody] PropertyDTO propertyDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _propertyService.UpdatePropertyAsync(id, propertyDTO);
            if (result)
                return Ok("Property updated successfully.");
            return NotFound("Property not found.");
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var existingProperty = await _propertyService.GetPropertyAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Verify ownership
            var ownerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || c.Type == "ownerId");
            if (ownerIdClaim == null || !int.TryParse(ownerIdClaim.Value, out int ownerId) || existingProperty.OwnerID != ownerId)
            {
                return Unauthorized("You are not authorized to delete this property.");
            }

            var deleted = await _propertyService.DeletePropertyAsync(id);

            if (deleted)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "Failed to delete property");
            }
        }
    }
}