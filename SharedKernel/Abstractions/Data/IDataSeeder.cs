namespace SharedKernel.Abstractions.Data;

public interface IDataSeeder
{
    Task SeedAllAsync();
}