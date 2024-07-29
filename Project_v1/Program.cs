using Project_v1.Models;
using Project_v1.Services;
using Project_v1.Data;
using Project_v1.SeedData;

try
{
    var menu = "Choose an option:\n" +
               "1. Display all students\n" +
               "2. Display a student by ID\n" +
               "3. Add a student\n" +
               "4. Delete a student\n" +
               "5. Update student details\n" +
               "6. Update student address\n" +
               "7. Add a mark to a student\n" +
               "8. Display general average of a student\n" +
               "9. Display subject averages of a student\n" +
               "10. Display students in descending order of averages\n" +
               "11. Exit\n" +
               "Option=";

    using (var context = new ApplicationDbContext())
    {
        context.Database.EnsureCreated();

        var seeder = new DatabaseSeeder(context);
        await seeder.SeedAsync();

        var studentService = new StudentService(context);

        while (true)
        {
            Console.WriteLine(menu);
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    var students = await studentService.GetStudentsAsync();
                    students.ForEach(s => Console.WriteLine($"{s.StudentId}: {s.FirstName} {s.LastName}, Age: {s.Age}, Address: {s.Address?.City} {s.Address?.Street} {s.Address?.Number}"));
                    break;
                case "2":
                    Console.Write("Enter student ID: ");
                    if (int.TryParse(Console.ReadLine(), out int studentId))
                    {
                        var student = await studentService.GetStudentByIdAsync(studentId);
                        if (student != null)
                        {
                            Console.WriteLine($"{student.StudentId}: {student.FirstName} {student.LastName}, Age: {student.Age}, Address: {student.Address?.City} {student.Address?.Street} {student.Address?.Number}");
                        }
                        else
                        {
                            Console.WriteLine("Student not found.");
                        }
                    }
                    break;
                case "3":
                    Console.Write("Enter student's first name: ");
                    string firstName = Console.ReadLine();
                    Console.Write("Enter student's last name: ");
                    string lastName = Console.ReadLine();
                    Console.Write("Enter student's age: ");
                    if (int.TryParse(Console.ReadLine(), out int age))
                    {
                        var newStudent = new Student { FirstName = firstName, LastName = lastName, Age = age };
                        var createdStudent = await studentService.CreateStudentAsync(newStudent);
                        Console.WriteLine($"Created student: {createdStudent.FirstName} {createdStudent.LastName}, ID: {createdStudent.StudentId}");
                    }
                    break;
                case "4":
                    Console.Write("Enter student ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int studentIdToDelete))
                    {
                        var deleted = await studentService.DeleteStudentAsync(studentIdToDelete, true);
                        if (deleted)
                        {
                            Console.WriteLine("Student successfully deleted.");
                        }
                        else
                        {
                            Console.WriteLine("Student not found.");
                        }
                    }
                    break;
                case "5":
                    Console.Write("Enter student ID: ");
                    if (int.TryParse(Console.ReadLine(), out int studentIdToUpdate))
                    {
                        var studentToUpdate = await studentService.GetStudentByIdAsync(studentIdToUpdate);
                        if (studentToUpdate != null)
                        {
                            Console.Write("Enter new first name: ");
                            studentToUpdate.FirstName = Console.ReadLine();
                            Console.Write("Enter new last name: ");
                            studentToUpdate.LastName = Console.ReadLine();
                            Console.Write("Enter new age: ");
                            if (int.TryParse(Console.ReadLine(), out int newAge))
                            {
                                studentToUpdate.Age = newAge;
                                await studentService.UpdateStudentAsync(studentToUpdate);
                                Console.WriteLine("Student details updated.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Student not found.");
                        }
                    }
                    break;
                case "6":
                    Console.Write("Enter student ID: ");
                    if (int.TryParse(Console.ReadLine(), out int studentIdForAddress))
                    {
                        var address = await studentService.GetStudentAddressAsync(studentIdForAddress);
                        if (address == null)
                        {
                            Console.WriteLine("Student does not have an address. Enter new address:");
                            address = new Address();
                        }
                        else
                        {
                            Console.WriteLine($"Current address: {address.City}, {address.Street}, {address.Number}");
                            Console.WriteLine("Enter new address:");
                        }

                        Console.Write("City: ");
                        address.City = Console.ReadLine();
                        Console.Write("Street: ");
                        address.Street = Console.ReadLine();
                        Console.Write("Number: ");
                        address.Number = Console.ReadLine();

                        await studentService.UpdateStudentAddressAsync(studentIdForAddress, address);
                        Console.WriteLine("Student address updated.");
                    }
                    break;
                case "7":
                    Console.Write("Enter student ID: ");
                    if (int.TryParse(Console.ReadLine(), out int studentIdForMark))
                    {
                        Console.Write("Enter mark value (1-10): ");
                        if (int.TryParse(Console.ReadLine(), out int markValue) && markValue >= 1 && markValue <= 10)
                        {
                            Console.Write("Enter subject: ");
                            string subject = Console.ReadLine();
                            await studentService.AddMarkAsync(studentIdForMark, markValue, subject);
                            Console.WriteLine("Mark added to student.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid mark value.");
                        }
                    }
                    break;
                case "8":
                    Console.Write("Enter student ID: ");
                    if (int.TryParse(Console.ReadLine(), out int studentIdForAverage))
                    {
                        var average = await studentService.GetStudentAverageAsync(studentIdForAverage);
                        if (average.HasValue)
                        {
                            Console.WriteLine($"Student's general average: {average.Value}");
                        }
                        else
                        {
                            Console.WriteLine("Student not found or no marks available.");
                        }
                    }
                    break;
                case "9":
                    Console.Write("Enter student ID: ");
                    if (int.TryParse(Console.ReadLine(), out int studentIdForSubjectAverages))
                    {
                        var subjectAverages = await studentService.GetSubjectAveragesAsync(studentIdForSubjectAverages);
                        if (subjectAverages != null)
                        {
                            Console.WriteLine("Student's subject averages:");
                            foreach (var subjectAverage in subjectAverages)
                            {
                                Console.WriteLine($"{subjectAverage.Key}: {subjectAverage.Value}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Student not found or no marks available.");
                        }
                    }
                    break;
                case "10":
                    var studentsByAverage = await studentService.GetStudentsAsync();
                    studentsByAverage = studentsByAverage
                        .OrderByDescending(s => s.Marks.Any() ? s.Marks.Average(m => m.Value) : 0) 
                        .ToList();

                    studentsByAverage.ForEach(s =>
                    {
                        var average = s.Marks.Any() ? s.Marks.Average(m => m.Value) : 0;
                        Console.WriteLine($"{s.StudentId}: {s.FirstName} {s.LastName}, Average: {average:F2}");
                    });
                    break;
                case "11":
                    return;
                default:
                    Console.WriteLine("\nInvalid option\n");
                    break;
            }
        }
    }
}
catch (Exception e)
{
    Console.WriteLine("An error occurred:");
    Console.WriteLine(e.Message);
    Console.WriteLine(e.InnerException?.Message);
}