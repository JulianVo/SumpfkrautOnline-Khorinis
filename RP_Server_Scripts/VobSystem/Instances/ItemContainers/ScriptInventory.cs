using System;
using GUC.Network;
using GUC.WorldObjects;
using GUC.WorldObjects.ItemContainers;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.VobSystem.Instances.ItemContainers
{
    public class ScriptInventory : ItemInventory.IScriptItemInventory
    {



        public event RemoveItemHandler OnRemoveItem;



        public event AddItemHandler OnAddItem;

        public ScriptInventory(IContainer owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public IContainer Owner { get; }

        public ItemInventory BaseInst => Owner.BaseInventory;

        public void ForEachItem(Action<ItemInst> action)
        {
            BaseInst.ForEach(i => action((ItemInst) i.ScriptObject));
        }

        public void ForEachItemPredicate(Predicate<ItemInst> predicate)
        {
            BaseInst.ForEachPredicate(i => predicate((ItemInst) i.ScriptObject));
        }

        public void AddItem(Item item)
        {
            AddItem((ItemInst) item.ScriptObject);
        }

        public void RemoveItem(Item item)
        {
            RemoveItem((ItemInst) item.ScriptObject);
        }

        public void AddItem(ItemInst item)
        {
            BaseInst.Add(item.BaseInst);
            item.OnSetAmount += Item_OnSetAmount;
            OnAddItem?.Invoke(item);
        }


    
        public event OnItemAmountChangeHandler OnItemAmountChange;

        private void Item_OnSetAmount(ItemInst item)
        {
            OnItemAmountChange?.Invoke(item);
        }

        public void RemoveItem(ItemInst item)
        {
            BaseInst.Remove(item.BaseInst);
            item.OnSetAmount -= Item_OnSetAmount;
            OnRemoveItem?.Invoke(item);
        }

        public ItemInst GetItem(int id)
        {
            if (BaseInst.TryGetItem(id, out Item item))
            {
                return (ItemInst) item.ScriptObject;
            }

            return null;
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        public void OnReadProperties(PacketReader stream)
        {
        }

        public bool Contains(ItemDef def)
        {
            bool result = false;
            ForEachItemPredicate(i =>
            {
                if (i.Definition == def)
                {
                    result = true;
                    return false;
                }

                return true;
            });
            return result;
        }
    }
}