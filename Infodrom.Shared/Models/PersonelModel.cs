using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infodrom.Shared.Models
{
    public class PersonelModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        [Range(10000, int.MaxValue, ErrorMessage = "Minimum 5 haneli olmalıdır")]
        public int Sicilo { get; set; }
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        public string Ad { get; set; }
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        public string Soyad { get; set; }
        public int? Organization_Id { get; set; }
        public string OrganizationAd { get; set; }
    }
}
