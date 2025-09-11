using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL.Interfaces
{
    public interface IStringClient
    {
        Task<string> GenerateSlug(string name);
        Task<string> OnlyNumbers(string input);
        Task<string> GenerateShortUniqueString();
    }
}
