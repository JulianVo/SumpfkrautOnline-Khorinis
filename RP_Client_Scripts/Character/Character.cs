using System;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;

namespace GUC.Scripts.Character
{
    public abstract class Character
    {
        protected Character(int characterId, string name,NPCDef template)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(name));
            }

            CharacterId = characterId;
            Name = name;
            Template = template ?? throw new ArgumentNullException(nameof(template));
        }

        public int CharacterId { get; }
        public string Name { get; }
        public NPCDef Template { get; }
    }
}
