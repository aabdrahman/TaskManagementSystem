namespace Infrastructure.Contracts;

public interface IInfrastructureManager
{
    IFileUtilityService FileUtilityService { get; }
}