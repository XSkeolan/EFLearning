using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Course : EntityBase
    {
        public string? Name { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
