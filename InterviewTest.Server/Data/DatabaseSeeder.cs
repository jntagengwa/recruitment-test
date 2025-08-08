using InterviewTest.Server.Model;

namespace InterviewTest.Server.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Employees.Any()) return;

            db.Employees.AddRange(new List<Employee>
            {
                new() { Name = "Abul", Value = 1357 },
                new() { Name = "Adolfo", Value = 1224 },
                new() { Name = "Alexander", Value = 2296 },
                new() { Name = "Amber", Value = 1145 },
                new() { Name = "Amy", Value = 4359 },
                new() { Name = "Andy", Value = 1966 },
                new() { Name = "Anna", Value = 4040 },
                new() { Name = "Antony", Value = 449 },
                new() { Name = "Ashley", Value = 8151 },
                new() { Name = "Borja", Value = 9428 },
                new() { Name = "Cecilia", Value = 2136 },
                new() { Name = "Christopher", Value = 9035 },
                new() { Name = "Dan", Value = 1475 },
                new() { Name = "Dario", Value = 284 },
                new() { Name = "David", Value = 948 },
                new() { Name = "Elike", Value = 1860 },
                new() { Name = "Ella", Value = 4549 },
                new() { Name = "Ellie", Value = 5736 },
                new() { Name = "Elliot", Value = 1020 },
                new() { Name = "Emily", Value = 7658 },
                new() { Name = "Faye", Value = 7399 },
                new() { Name = "Fern", Value = 1422 },
                new() { Name = "Francisco", Value = 5028 },
                new() { Name = "Frank", Value = 3281 },
                new() { Name = "Gary", Value = 9190 },
                new() { Name = "Germaine", Value = 6437 },
                new() { Name = "Greg", Value = 5929 },
                new() { Name = "Harvey", Value = 8471 },
                new() { Name = "Helen", Value = 963 },
                new() { Name = "Huzairi", Value = 9491 },
                new() { Name = "Izmi", Value = 8324 },
                new() { Name = "James", Value = 6994 },
                new() { Name = "Jarek", Value = 6581 },
                new() { Name = "Jim", Value = 202 },
                new() { Name = "John", Value = 261 },
                new() { Name = "Jose", Value = 1605 },
                new() { Name = "Josef", Value = 3714 },
                new() { Name = "Karthik", Value = 4828 },
                new() { Name = "Katrin", Value = 5393 },
                new() { Name = "Lee", Value = 269 },
                new() { Name = "Luke", Value = 5926 },
                new() { Name = "Madiha", Value = 2329 },
                new() { Name = "Marc", Value = 3651 },
                new() { Name = "Marina", Value = 6903 },
                new() { Name = "Mark", Value = 3368 },
                new() { Name = "Marzena", Value = 7515 },
                new() { Name = "Mohamed", Value = 1080 },
                new() { Name = "Nichole", Value = 1221 },
                new() { Name = "Nikita", Value = 8520 },
                new() { Name = "Oliver", Value = 2868 },
                new() { Name = "Patryk", Value = 1418 },
                new() { Name = "Paul", Value = 4332 },
                new() { Name = "Ralph", Value = 1581 },
                new() { Name = "Raymond", Value = 7393 },
                new() { Name = "Roman", Value = 4056 },
                new() { Name = "Ryan", Value = 252 },
                new() { Name = "Sara", Value = 2618 },
                new() { Name = "Sean", Value = 691 },
                new() { Name = "Seb", Value = 5395 },
                new() { Name = "Sergey", Value = 8282 },
                new() { Name = "Shaheen", Value = 3721 },
                new() { Name = "Sharni", Value = 7737 },
                new() { Name = "Sinu", Value = 3349 },
                new() { Name = "Stephen", Value = 8105 },
                new() { Name = "Tim", Value = 8386 },
                new() { Name = "Tina", Value = 5133 },
                new() { Name = "Tom", Value = 7553 },
                new() { Name = "Tony", Value = 4432 },
                new() { Name = "Tracy", Value = 1771 },
                new() { Name = "Tristan", Value = 2030 },
                new() { Name = "Victor", Value = 1046 },
                new() { Name = "Yury", Value = 1854 },
            });

            db.SaveChanges();
        }
    }
}