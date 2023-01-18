using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace brazenborderlands
{
    internal class Inventory
    {
        public List<Item> Items {  get; set; }
        public int Capacity { get; set; }
        public Inventory()  
        {
            Capacity = Capacity == 0 ? 10 : Capacity;
            Items = new List<Item>();
        }
        public Inventory(int capacity) 
        {
            Items = new List<Item>();
            this.Capacity = capacity;
        }
        public bool Add(Item item)
        {
            if (Items.Count < Capacity)
            {
                Items.Add(item);
                return true;
            }
            return false;
        }
        public bool Remove(int inventoryNum) 
        { 
            Items[inventoryNum].IsEquipped = false;
            Items.RemoveAt(inventoryNum);
            return true;
        }
        public IEquipment EquipmentInSlot(EquipmentSlot slot)
        {
            return Items.SingleOrDefault(e => e.IsEquipped && e.Slot == slot);
        }
        public bool SlotFull(EquipmentSlot slot)
        {
            if (slot == EquipmentSlot.BothHands)
            {
                return (EquipmentInSlot(EquipmentSlot.PrimaryHand) != null ||
                    EquipmentInSlot(EquipmentSlot.SecondaryHand) != null ||
                    EquipmentInSlot(EquipmentSlot.BothHands) != null);
            }
            if (slot == EquipmentSlot.PrimaryHand)
            {
                return (EquipmentInSlot(EquipmentSlot.PrimaryHand) != null ||
                    EquipmentInSlot(EquipmentSlot.BothHands) != null);
            }
            if (slot == EquipmentSlot.SecondaryHand)
            {
                return (EquipmentInSlot(EquipmentSlot.SecondaryHand) != null ||
                    EquipmentInSlot(EquipmentSlot.BothHands) != null);
            }
            return (EquipmentInSlot(slot) != null);
        }
        public bool Unequip(EquipmentSlot slot)
        {
            IEquipment e = EquipmentInSlot(slot);
            if (e == null) return false;
            e.IsEquipped = false;
            return true;
        }
        public bool Unequip(int inventoryNum)
        {
            Items[inventoryNum].IsEquipped = false; 
            return true;
        }
        public bool Equip(int inventoryNum)
        {
            IEquipment toEquip = Items[inventoryNum];
            if (toEquip.IsEquipped) return false;
            if (SlotFull(toEquip.Slot))
            {
                if (toEquip.Slot == EquipmentSlot.BothHands)
                {
                    Unequip(EquipmentSlot.PrimaryHand);
                    Unequip(EquipmentSlot.SecondaryHand);
                }
                if (toEquip.Slot == EquipmentSlot.PrimaryHand || toEquip.Slot == EquipmentSlot.SecondaryHand)
                {
                    Unequip(EquipmentSlot.BothHands);
                }
                Unequip(toEquip.Slot);
            }
            toEquip.IsEquipped = true;
            return true;
        }
        public Weapon EquippedWeapon()
        {
            return (Weapon)Items.SingleOrDefault(e => e is Weapon && e.IsEquipped);
        }
        public List<Armor> EquippedArmors()
        {
            List<Item> l = Items.Where(e => e is Armor && e.IsEquipped).ToList();
            return l.Cast<Armor>().ToList();
        }
        public bool IsBodyArmorEquipped()
        {
            Item a = Items.SingleOrDefault(e => e is Armor && e.IsEquipped && e.Slot == EquipmentSlot.Body);
            return (a != null);
        }
        public int NumItems()
        {
            return Items.Count;
        }
        public void Rebuild()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Item newi = Items[i].Rebuild();
                Items[i] = newi;
            }
        }
    }
}
