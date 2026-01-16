namespace Infrastructure.Contracts;

public interface IInfrastructureManager
{
    IFileUtilityService FileUtilityService { get; }
    ICacheService CacheService { get; }
}