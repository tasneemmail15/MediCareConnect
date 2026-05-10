using MediCareConnect.Models;

namespace MediCareConnect.Services
{
    public static class Seeder
    {
        // use this class to seed the database with dummy test data using an IUserService 
        public static void Seed(IUserService svc)
        {
            // seeder destroys and recreates the database - NOT to be called in production!!!
            svc.Initialise();

            // add users
            svc.AddUser("Administrator", "admin@mail.com", "admin", Role.admin);
            svc.AddUser("Doctor", "doctor@mail.com", "manager", Role.doctor);
            svc.AddUser("Patient", "patient@mail.com", "guest", Role.patient);
            svc.AddUser("Guest", "guest@mail.com", "guest", Role.guest); 
        
            // optionally add some fake users
            // var faker = new Faker();
            // for(int i=1; i<=20; i++)
            // {
            //     var s = svc.AddUser(
            //         faker.Name.FullName(),
            //         faker.Internet.Email(),
            //         "password",
            //         Role.guest
            //     );
            // }
        }
    }

}