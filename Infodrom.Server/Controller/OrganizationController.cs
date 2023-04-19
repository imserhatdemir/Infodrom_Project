using Infodrom.Shared.Models;
using Infodrom.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infodrom.Server.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }
        [HttpGet("get-org-list")]
        public IActionResult GetAllOrganization()
        {
            List<OrganizationModel> org = _organizationService.GetAllOrganization().ToList();
            return Ok(org);
        }

        [HttpPost("post-org")]
        public IActionResult AddOrganization([FromBody] OrganizationModel org)
        {
            try
            {
                _organizationService.AddOrganization(org);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-org")]
        public IActionResult UpdateOrganization([FromBody] OrganizationModel org)
        {
            try
            {
                _organizationService.UpdateOrganization(org);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("check/{OrgId}")]
        public IActionResult DeleteOrganizationCheck(int? OrgId)
        {
            var result = _organizationService.DeleteOrganizationCheck(OrgId);
            if (result.IsSuccess)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpGet("get-org-with-person")]
        public IActionResult GetAllOrganizationsWithPersonel()
        {
            var result = _organizationService.GetAllOrganizationsWithPersonel();
            return Ok(result);
        }

        [HttpPut("clearorganization/{id}")]
        public async Task<IActionResult> ClearOrganization(int id)
        {
            try
            {
                string response = await _organizationService.ClearOrganization(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("recursive")]
        public IActionResult GetAllOrganizationRecursive()
        {
            var result = _organizationService.GetAllOrganizationRecursive();
            return Ok(result);
        }
    }
}
