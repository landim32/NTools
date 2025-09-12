using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL.Interfaces
{
    public interface IStringClient
    {
        Task<string> GenerateSlugAsync(string name);
        Task<string> OnlyNumbersAsync(string input);
        Task<string> GenerateShortUniqueStringAsync();
    }
}
