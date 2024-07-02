using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_v1.Models;

namespace Project_v1
{
    public class StudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.Include(s => s.Address).ToListAsync();
        }
        public async Task<Student> GetStudentByNameAsync(string firstName, string lastName)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.FirstName == firstName && s.LastName == lastName);
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteStudentAsync(int id, bool deleteAddress)
        {
            var student = await _context.Students.Include(s => s.Address).FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null) return false;

            if (deleteAddress && student.Address != null)
            {
                _context.Addresses.Remove(student.Address);
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Address> GetStudentAddressAsync(int studentId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.StudentId == studentId);
        }

        public async Task<Address> UpdateStudentAddressAsync(int studentId, Address newAddress)
        {
            var student = await _context.Students.Include(s => s.Address).FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null) return null;

            if (student.Address == null)
            {
                student.Address = new Address
                {
                    City = newAddress.City,
                    Street = newAddress.Street,
                    Number = newAddress.Number,
                    StudentId = studentId
                };
                _context.Addresses.Add(student.Address);
            }
            else
            {
                student.Address.City = newAddress.City;
                student.Address.Street = newAddress.Street;
                student.Address.Number = newAddress.Number;
                _context.Entry(student.Address).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return student.Address;
        }
    }
}
