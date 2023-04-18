using Infodrom.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infodrom.Shared.Services
{
    public interface IOrganizationService
    {
        IEnumerable<OrganizationModel> GetAllOrganization();
        IEnumerable<OrganizationViewModel> GetAllOrganizationsWithPersonel();
        void AddOrganization(OrganizationModel org);
        void UpdateOrganization(OrganizationModel org);
        ErrorResponse DeleteOrganizationCheck(int? OrgId);
        Task<string> ClearOrganization(int id);
    }
}
