using System;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Character.AccountComponent;
using RP_Server_Scripts.Component;

namespace RP_Server_Scripts.CharacterManagement
{
    public static class CharacterAccountExtensions
    {
        public static CharacterList GetCharacterList(this Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (!account.TryGetComponent<CharacterList>(out CharacterList charList))
            {
                throw  new ComponentNotFoundException($"The component '{nameof(CharacterList)}' was not found for {nameof(Account)} '{account.UserName}'");
            }

            return charList;
        }
    }
}
