using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_v1.Data;
using Project_v1.Models;


namespace Project_v1.Services
{
    public class StudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            return await _context.Students.Include(s => s.Address).Include(s => s.Marks).ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Students.Include(s => s.Address).Include(s => s.Marks).FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteStudentAsync(int id, bool deleteAddress)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                if (deleteAddress)
                {
                    var address = await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == id);
                    if (address != null)
                    {
                        _context.Addresses.Remove(address);
                    }
                }
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task<Address> GetStudentAddressAsync(int studentId)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == studentId);
        }

        public async Task UpdateStudentAddressAsync(int studentId, Address address)
        {
            var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.StudentId == studentId);
            if (existingAddress != null)
            {
                existingAddress.City = address.City;
                existingAddress.Street = address.Street;
                existingAddress.Number = address.Number;
                _context.Addresses.Update(existingAddress);
            }
            else
            {
                address.StudentId = studentId;
                _context.Addresses.Add(address);
            }
            await _context.SaveChangesAsync();
        }

       public async Task AddMarkAsync(int studentId, int value, string subject)
        {
            var student = await _context.Students.Include(s => s.Marks).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student != null)
            {
                var mark = new Mark
                {
                    Value = value,
                    DateGiven = DateTime.Now,
                    Subject = subject,
                    StudentId = studentId,
                    Student = student
                };
                student.Marks.Add(mark);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<double?> GetStudentAverageAsync(int studentId)
        {
            var student = await _context.Students.Include(s => s.Marks).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student != null && student.Marks.Any())
            {
                return student.Marks.Average(m => m.Value);
            }
            return null;
        }

        public async Task<Dictionary<string, double?>> GetSubjectAveragesAsync(int studentId)
        {
            var student = await _context.Students.Include(s => s.Marks).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student != null && student.Marks.Any())
            {
                return student.Marks.GroupBy(m => m.Subject)
                                    .ToDictionary(g => g.Key, g => (double?)g.Average(m => m.Value));
            }
            return null;
        }
    }
}
