using Infodrom.Shared.Models;
using Infodrom.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        [Route("get-org-list")]
        public async Task<IActionResult> GetAllOrganization()
        {
            List<OrganizationModel> org = new List<OrganizationModel>();

            org = _organizationService.GetAllOrganization().ToList();

            return Ok(org);
        }



        [HttpPost]
        [Route("post-org")]
        public async Task<IActionResult> AddOrganization(OrganizationModel org)
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

        [HttpPost]
        [Route("update-org")]
        public async Task<IActionResult> UpdateOrganization(OrganizationModel org)
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

        [HttpDelete]
        [Route("check/{OrgId}")]
        public async Task<IActionResult> DeleteOrganizationCheck(int? OrgId)
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


        [HttpGet]
        [Route("get-org-with-person")]
        public async Task<IActionResult> GetAllOrganizationsWithPersonel()
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

    }
}
