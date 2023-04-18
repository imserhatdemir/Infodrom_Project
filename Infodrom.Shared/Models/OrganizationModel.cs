using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infodrom.Shared.Models
{
    public class OrganizationModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bu alan boş bırakılamaz")]
        public string Ad { get; set; }
    }
}
