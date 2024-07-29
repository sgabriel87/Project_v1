using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_v1.Data;
using Project_v1.Models;

namespace Project_v1.SeedData
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Students.AnyAsync()) return;

            var students = new List<Student>
        {
            new Student { FirstName = "James", LastName = "Hetfield", Age = 25 },
            new Student { FirstName = "Freddie", LastName = "Mercury", Age = 22 },
            new Student { FirstName = "Elvis", LastName = "P", Age = 23 },
            new Student { FirstName = "Alice", LastName = "Cooper", Age = 24 },
            new Student { FirstName = "Gabriel", LastName = "S", Age = 24 }
        };

            _context.Students.AddRange(students);
            await _context.SaveChangesAsync(); 

          
            var marks = new List<Mark>
        {
            new Mark { StudentId = students[0].StudentId, Value = 9, DateGiven = DateTime.Now, Subject = "Math" },
            new Mark { StudentId = students[0].StudentId, Value = 8, DateGiven = DateTime.Now, Subject = "English" },
            new Mark { StudentId = students[1].StudentId, Value = 7, DateGiven = DateTime.Now, Subject = "History" },
            new Mark { StudentId = students[1].StudentId, Value = 6, DateGiven = DateTime.Now, Subject = "Math" },
            
        };

            _context.Marks.AddRange(marks);
            await _context.SaveChangesAsync(); 
        }
    }
}
