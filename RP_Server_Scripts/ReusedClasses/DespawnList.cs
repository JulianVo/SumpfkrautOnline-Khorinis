using System.Collections.Generic;
using RP_Server_Scripts.ReusedClasses.Utilities;
using RP_Server_Scripts.VobSystem.Instances;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.ReusedClasses
{
    // Fixme: use timer and capacity together?
    public class DespawnList<T> where T : BaseVobInst
    {
        Dictionary<T, int> indices;
        T[] vobs;

        public int Capacity => vobs.Length;
        public int Count => indices.Count;

        public DespawnList(int capacity)
        {
            vobs = new T[capacity];
            indices = new Dictionary<T, int>(capacity);
        }
        
        int currentIndex = 0;
        public void AddVob(T vob)
        {
            ExceptionCheck.ArgumentNull(vob);

            if (indices.ContainsKey(vob))
                return;

            // Despawn old vob
            T otherVob = vobs[currentIndex];
            if (otherVob != null)
            {
                indices.Remove(otherVob);
                otherVob.OnDespawn -= RemoveVobHandler;
                if (otherVob.IsSpawned)
                {
                    otherVob.Despawn();
                }
            }

            // add new vob
            vobs[currentIndex] = vob;
            indices.Add(vob, currentIndex);
            vob.OnDespawn += RemoveVobHandler;
            
            // change current index
            if (++currentIndex >= Capacity)
                currentIndex = 0;
        }

        void RemoveVobHandler(BaseVobInst vob, WorldInst world)
        {
            RemoveVob((T)vob);
        }

        public void RemoveVob(T vob)
        {
            ExceptionCheck.ArgumentNull(vob);

            if (!indices.TryGetValue(vob, out int index))
                return;

            vobs[index] = null;
            indices.Remove(vob);
            vob.OnDespawn -= RemoveVobHandler;
        }
    }
}
