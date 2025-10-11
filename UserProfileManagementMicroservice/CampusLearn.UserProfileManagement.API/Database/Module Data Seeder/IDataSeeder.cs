namespace CampusLearn.UserProfileManagement.API.Database.Module_Data_Seeder;

public interface IDataSeeder
{
    Task SeedModulesAsync();
    Task SeedUsersAsync();
}
