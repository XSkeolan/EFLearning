using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Student : EntityBase
    {
        public string? Name { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Enrollment> Enrollments { get; set; } = new();
    }
}
