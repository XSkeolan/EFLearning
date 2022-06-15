using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    // Если нужно добавить дополнительные данные, для промежуточной таблицы, то лучше создать ее модель
    public class Enrollment
    {
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }

        public Guid CourseId { get; set; }
        public Course? Course { get; set; }

        public int Mark { get; set; }       // оценка студента
    }
}
