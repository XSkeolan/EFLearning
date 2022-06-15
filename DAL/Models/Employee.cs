using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    // Подход TPT (Table per Type)
    //[Table("Employees")]
    public class Employee : User
    {
        public int Salary { get; set; }
    }
}
