namespace NTools.ACL.Interfaces
{
    public interface IDocumentClient
    {
        Task<bool> validarCpfOuCnpjAsync(string cpfCnpj);
    }
}
