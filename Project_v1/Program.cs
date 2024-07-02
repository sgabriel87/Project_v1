using Project_v1.Models;
using Project_v1;

using (var context = new ApplicationDbContext())
{
    context.Database.EnsureCreated();

    var studentService = new StudentService(context);
    var students = new List<Student>
                {
                    new() { FirstName = "James", LastName = "Hetfield", Age = 25 },
                    new() { FirstName = "Freddie", LastName = "Mercury", Age = 22 },
                    new() { FirstName = "Elvis", LastName = "P", Age = 23 },
                    new() { FirstName = "Alice", LastName = "Cooper", Age = 24 },
                    new() { FirstName = "Gabriel", LastName = "S", Age = 24 }
                };

    foreach (var student in students)
    {
        var existingStudent = await studentService.GetStudentByNameAsync(student.FirstName, student.LastName);
        if (existingStudent == null)
        {
            var createdStudent = await studentService.CreateStudentAsync(student);
            Console.WriteLine($"Created Student: {createdStudent.FirstName} {createdStudent.LastName}, ID: {createdStudent.StudentId}");
        }
        else
        {
            Console.WriteLine($"Student {student.FirstName} {student.LastName} already exists with ID: {existingStudent.StudentId}");
        }
    }


    int studentIdToOperateOn = 1;

    var studentById = await studentService.GetStudentByIdAsync(studentIdToOperateOn);
    if (studentById != null)
    {
        Console.WriteLine($"Student by ID: {studentById.FirstName} {studentById.LastName}, Age: {studentById.Age}");


        studentById.Age = 25;
        await studentService.UpdateStudentAsync(studentById);
        Console.WriteLine($"Updated Student Age: {studentById.FirstName} {studentById.LastName}, Age: {studentById.Age}");


        var studentAddress = await studentService.GetStudentAddressAsync(studentIdToOperateOn);
        if (studentAddress == null)
        {
            Console.WriteLine("Student does not have an address. Creating one...");
            studentAddress = new Address
            {
                City = "New York",
                Street = "5th Avenue",
                Number = "123",
                StudentId = studentIdToOperateOn
            };
            await studentService.UpdateStudentAddressAsync(studentIdToOperateOn, studentAddress);
            Console.WriteLine($"Created Address: {studentAddress.City}, {studentAddress.Street} {studentAddress.Number}");
        }
        else
        {
            Console.WriteLine($"Student Address: {studentAddress.City}, {studentAddress.Street} {studentAddress.Number}");


            studentAddress.City = "Los Angeles";
            await studentService.UpdateStudentAddressAsync(studentIdToOperateOn, studentAddress);
            Console.WriteLine($"Updated Address: {studentAddress.City}, {studentAddress.Street} {studentAddress.Number}");
        }

        /* var deleteWithAddress = await studentService.DeleteStudentAsync(studentIdToOperateOn, true);
         Console.WriteLine($"Deleted Student with Address: {deleteWithAddress}");*/
    }
    else
    {
        Console.WriteLine($"Student with ID {studentIdToOperateOn} not found.");
    }
}
