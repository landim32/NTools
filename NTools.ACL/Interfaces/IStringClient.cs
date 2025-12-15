namespace NTools.ACL.Interfaces
{
    public interface IStringClient
    {
        Task<string> GenerateSlugAsync(string name);
        Task<string> OnlyNumbersAsync(string input);
        Task<string> GenerateShortUniqueStringAsync();
    }
}
