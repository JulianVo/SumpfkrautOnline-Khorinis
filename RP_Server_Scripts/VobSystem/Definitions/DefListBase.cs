using System;
using System.Collections.Generic;
using System.Linq;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    /// <summary>
    /// Base class of all vob definitions lists.
    /// </summary>
    /// <typeparam name="TDefType">The type of <see cref="BaseVobDef"/> that is handled by the concrete implementation.</typeparam>
    internal abstract class DefListBase<TDefType> where TDefType : BaseVobDef
    {
        /// <summary>
        /// The dictionary that is used for storing all registered definitions with the type <see cref="TDefType"/>
        /// </summary>
        protected readonly Dictionary<string, TDefType> DefDict = new Dictionary<string, TDefType>();

        /// <summary>
        /// Lock object for locking the internals of the current <see cref="DefListBase{TDefType}"/> instance.
        /// </summary>
        protected readonly object Lock = new object();

        public TDefType GetByCode(string codeName)
        {
            //Lock the dictionary while accessing it. Should not add much overhead but adds to the preparations for multi threading support.
            lock (Lock)
            {
                if (string.IsNullOrWhiteSpace(codeName))
                {
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(codeName));
                }

                if (!DefDict.TryGetValue(codeName, out TDefType reVal))
                {
                    throw new ArgumentException($"A {nameof(TDefType)} with the given code name '{codeName}' could not be found",
                        nameof(codeName));
                }

                return reVal;
            }
        }

        public void AddDef(TDefType npcDef)
        {
            //Lock the dictionary while accessing it. Should not add much overhead but adds to the preparations for multi threading support.
            lock (Lock)
            {
                if (npcDef == null)
                {
                    throw new ArgumentNullException(nameof(npcDef));
                }

                if (DefDict.ContainsKey(npcDef.CodeName))
                {
                    throw new ArgumentException($"A {nameof(TDefType)} with the given code name was already registered.");
                }

                DefDict.Add(npcDef.CodeName, npcDef);
            }
        }

        public void RemoveDef(TDefType npcDef)
        {
            //Lock the dictionary while accessing it. Should not add much overhead but adds to the preparations for multi threading support.
            lock (Lock)
            {
                if (npcDef == null)
                {
                    throw new ArgumentNullException(nameof(npcDef));
                }

                if (!DefDict.ContainsKey(npcDef.CodeName))
                {
                    throw new ArgumentException($"A {nameof(TDefType)} with the given code name could not be found",
                        nameof(npcDef));
                }

                DefDict.Remove(npcDef.CodeName);
            }
        }

        public IEnumerable<TDefType> AllDefinitions
        {
            get
            {
                TDefType[] listCopy;
                lock (Lock)
                {
                    listCopy = DefDict.Values.ToArray();
                }
                return listCopy;
            }
        }
    }
}