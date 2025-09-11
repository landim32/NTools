using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL.Interfaces
{
    public interface IDocumentClient
    {
        Task<bool> validarCpfOuCnpjAsync(string cpfCnpj);
    }
}
