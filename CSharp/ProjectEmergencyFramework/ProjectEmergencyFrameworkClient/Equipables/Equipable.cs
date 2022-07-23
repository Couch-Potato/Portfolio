using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectEmergencyFrameworkClient.Equipables
{
    public abstract class Equipable
    {
        public Equipable()
        {
            
            _name = this.GetType().GetCustomAttribute<EquipableAttribute>().Name;
            _icon = this.GetType().GetCustomAttribute<EquipableAttribute>().Icon;
            OriginalIcon = _icon;
            OriginalName = _name;
        }
        public void CreateInstance(dynamic mod, bool discrete = false)
        {
            _modifiers = mod;
            IsInDiscreteMode = discrete;
            OnInstanced();
        }
        protected bool IsInDiscreteMode = false;
        protected virtual void OnEquip() { }
        protected virtual void OnInstanced() { }

        protected virtual void OnDeInstanced() { }
        protected virtual void OnUnEquip() { }

        protected virtual void OnPrimaryUp()
        {

        }

        public void PrimaryUp()
        {

            OnPrimaryUp();
        }

        private string OriginalName;
        private string OriginalIcon;

        public virtual bool DisabledPrimary { get; } = false;

        protected string _name;
        protected string _icon;
        private bool _stackable;
        private dynamic _modifiers;

        public string Name { get => _name; }
        public string Icon { get => _icon; }
        public bool Stackable { get => _stackable; }
        public dynamic Modifiers { get => _modifiers; }

        public InventoryItem ToInventoryData()
        {
            return new InventoryItem()
            {
                name = Name,
                icon = Icon,
                isStackable = Stackable,
                modifiers = Modifiers,
                transportString = JsonConvert.SerializeObject(Modifiers)
            };
        } 
        public void Unequip()
        {
            OnUnEquip();
        }
        public void Equip()
        {
            OnEquip();
        }
        public bool IsInstanced = true;
        public void DeInstance()
        {
            if (IsInstanced)
            {
                IsInstanced = false;
                OnDeInstanced();
            }
        }
    }
}
