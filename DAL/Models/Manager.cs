using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    // Подход TPT (Table per Type)
    //[Table("Managers")]
    public class Manager : User
    {
        public string? Departament { get; set; }
    }
}
