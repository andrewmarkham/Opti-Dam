namespace OptiDAM.Services
{
    public interface IOptiDamAuthService
    {
        Task<string?> GetBearerToken();
    }
}