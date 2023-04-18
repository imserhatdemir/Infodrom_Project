using Dapper;
using Infodrom.Shared.Models;
using Infodrom.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Infodrom.Server.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonelController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PersonelController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet]
        [Route("personeller")]
        public async Task<ActionResult<List<PersonelModel>>> GetPersonel()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<PersonelModel> personeller = await SelectAllPersonelOrderedBySicilNo(connection);
            return Ok(personeller);
        }

        private static async Task<IEnumerable<PersonelModel>> SelectAllPersonel(SqlConnection sqlConnection)
        {
            return await sqlConnection.QueryAsync<PersonelModel>("SELECT * FROM Personel");
        }


        private async Task<IEnumerable<PersonelModel>> SelectAllPersonelOrderedBySicilNo(SqlConnection connection)
        {
            return await connection.QueryAsync<PersonelModel>(@"
        SELECT p.Id, p.Sicilo, p.Ad, p.Soyad, p.Organization_Id, o.Ad AS OrganizationAd
        FROM personel p
        LEFT JOIN organization o ON p.Organization_Id = o.Id
        ORDER BY p.Sicilo ASC");
        }


        [HttpPost]
        [Route("add-personel")]
        public async Task<ActionResult<List<PersonelViewModel>>> CreatePersonal(PersonelViewModel personel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await IsSicilNoExists(personel.Sicilo))
            {
                return Conflict("Aynı Sicil No ile kayıtlı personel zaten mevcut.");
            }

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into personel (Sicilo,Ad,Soyad,Organization_Id) Values(@Sicilo,@Ad,@Soyad,@Organization_Id)", personel);
            return Ok(await SelectAllPersonel(connection));
        }


        [HttpGet("organizations")]
        public async Task<ActionResult<IEnumerable<OrganizationModel>>> GetOrganizationsAsync()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            const string query = "SELECT * FROM organization";
            var result = await connection.QueryAsync<OrganizationModel>(query);
            return Ok(result);
        }


        [HttpPut]
        [Route("add-update")]
        public async Task<ActionResult<List<PersonelViewModel>>> UpdatePersonal(PersonelViewModel personel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await UpdateIsSicilNoExists(personel.Sicilo, personel.Id))
            {
                return Conflict("Aynı Sicil No ile kayıtlı başka bir personel zaten mevcut.");
            }

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Update personel Set Ad=@Ad, Soyad=@Soyad, Sicilo=@Sicilo, Organization_Id=@Organization_Id Where Id=@Id", personel);
            return Ok(await SelectAllPersonel(connection));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<PersonelModel>>> GetPersonelById(int id)
        {

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            // SQLite connection nesnesini oluştur
            var personel = await connection.QueryFirstOrDefaultAsync<PersonelModel>("SELECT * FROM personel WHERE Id = @Id", new { Id = id });

            if (personel == null)
            {
                return NotFound($"Personel with ID {id} not found.");
            }

            return Ok(personel);
        }


        private async Task<bool> IsSicilNoExists(int sicilNo)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return await connection.ExecuteScalarAsync<bool>("Select COUNT(*) from personel where Sicilo = @SicilNo", new { SicilNo = sicilNo });
        }

        private async Task<bool> UpdateIsSicilNoExists(int sicilNo, int? excludeId = null)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string query = "Select Count(*) from personel Where Sicilo=@SicilNo";

            if (excludeId.HasValue)
            {
                query += " And Id <> @ExcludeId";
            }

            int count = await connection.ExecuteScalarAsync<int>(query, new { SicilNo = sicilNo, ExcludeId = excludeId });
            return count > 0;
        }


        [HttpDelete]
        [Route("delete-personel/{id}")]
        public async Task<ActionResult<List<PersonelModel>>> DeletePersonel(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Delete from personel where Id=@Id", new { Id = id });
            return Ok(await SelectAllPersonel(connection));
        }


        [HttpPut]
        [Route("clear-organization/{id}")]
        public async Task<ActionResult<List<PersonelModel>>> ClearOrganization(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            int affectedRows = await connection.ExecuteAsync("Update personel Set Organization_Id = NULL Where Id = @Id", new { Id = id });

            if (affectedRows > 0)
            {
                return Ok("Organization_Id başarıyla NULL olarak güncellendi.");
            }
            else
            {
                return NotFound("Belirtilen ID ile eşleşen personel bulunamadı.");
            }
        }


    }
}
