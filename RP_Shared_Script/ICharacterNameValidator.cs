using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP_Shared_Script
{
    public interface ICharacterNameValidator
    {
        bool IsValid(string characterName);
    }
}
