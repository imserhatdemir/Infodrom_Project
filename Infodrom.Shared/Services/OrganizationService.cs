using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infodrom.Shared.Models;

namespace Infodrom.Shared.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IConfiguration _configuration;
        string ConnectionString = string.Empty;

        public OrganizationService(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public IEnumerable<OrganizationModel> GetAllOrganization()
        {
            List<OrganizationModel> lstOrg = new List<OrganizationModel>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("GetOrganization", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    OrganizationModel student = new OrganizationModel();
                    student.Id = Convert.ToInt32(rdr["Id"]);
                    student.Ad = rdr["Ad"].ToString();
                    student.ParentId = rdr["ParentId"] as int?;
                    lstOrg.Add(student);
                }
                con.Close();
            }
            return lstOrg;
        }


        public void AddOrganization(OrganizationModel org)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("AddNewOrganization", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ad", org.Ad);
                cmd.Parameters.AddWithValue("@ParentId", (object)org.ParentId ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


        public void UpdateOrganization(OrganizationModel org)
        {
            if (org == null || org.Id <= 0 || string.IsNullOrEmpty(org.Ad))
            {
                throw new ArgumentException("Invalid organization data");
            }

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateOrganization", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", org.Id);
                cmd.Parameters.AddWithValue("@Ad", org.Ad);
                cmd.Parameters.AddWithValue("@ParentId", (object)org.ParentId ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public ErrorResponse DeleteOrganizationCheck(int? OrgId)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteOrganizationCheck", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", OrgId);
                cmd.Parameters.Add("@PersonnelCount", SqlDbType.Int).Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                int personnelCount = Convert.ToInt32(cmd.Parameters["@PersonnelCount"].Value);
                if (personnelCount > 0)
                {
                    return new ErrorResponse { IsSuccess = false, Message = "Organizasyon, personel kaydı bulunduğu için silinemedi" };
                }
                else
                {
                    return new ErrorResponse { IsSuccess = true, Message = "Organizasyon başarıyla silindi" };
                }
            }
        }
        public async Task<string> ClearOrganization(int id)
        {
            using var connection = new SqlConnection(ConnectionString);
            var command = new SqlCommand("Update personel Set Organization_Id = NULL Where Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.CommandType = CommandType.Text;

            await connection.OpenAsync();
            int affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows > 0)
            {
                return "Organizasyondan başarılı olarak çıkarıldı.";
            }
            else
            {
                return "Belirtilen ID ile eşleşen personel bulunamadı.";
            }
        }



        public IEnumerable<OrganizationViewModel> GetAllOrganizationsWithPersonel()
        {
            var organizationDict = new Dictionary<int, OrganizationViewModel>();

            using (var con = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("GetAllOrganizationsWithPersonel", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                con.Open();
                var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    int orgId = int.Parse(rdr["OrganizationId"].ToString());
                    if (!organizationDict.TryGetValue(orgId, out var organization))
                    {
                        organization = new OrganizationViewModel
                        {
                            Id = orgId,
                            Ad = rdr["OrganizationAd"].ToString(),
                            Personel = new List<PersonelModel>()
                        };
                        organizationDict.Add(orgId, organization);
                    }

                    if (!rdr.IsDBNull(rdr.GetOrdinal("PersonelId")))
                    {
                        organization.Personel.Add(new PersonelModel
                        {
                            Id = int.Parse(rdr["PersonelId"].ToString()),
                            Sicilo = int.Parse(rdr["Sicilo"].ToString()),
                            Ad = rdr["PersonelAd"].ToString(),
                            Soyad = rdr["Soyad"].ToString(),
                            Organization_Id = orgId
                        });
                    }
                }
            }

            return organizationDict.Values.ToList();
        }


        private List<OrganizationModel> BuildRecursiveOrganizationList(IEnumerable<OrganizationModel> orgs, int? parentId)
        {
            var result = new List<OrganizationModel>();

            var childOrgs = orgs.Where(o => o.ParentId == parentId);
            foreach (var org in childOrgs)
            {
                org.Children = BuildRecursiveOrganizationList(orgs, org.Id);
                result.Add(org);
            }

            return result;
        }


        public IEnumerable<OrganizationModel> GetAllOrganizationRecursive()
        {
            var allOrgs = GetAllOrganization();
            return BuildRecursiveOrganizationList(allOrgs, null);
        }




    }
}
