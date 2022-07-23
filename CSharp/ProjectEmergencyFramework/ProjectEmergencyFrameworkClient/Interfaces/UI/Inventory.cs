using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    public class InventoryContainerConfiguration
    {
        public string name { get; set; }
        public int items { get; set; }
        public bool isActive { get; set; }
        public string containerId { get; set; }
        public List<InventoryItem> invItems { get; set; }
    }

    public enum CraftInterface
    {
        Inactive = -1,
        ChemicalCrafter = 0,
        CarCrafter = 1,
        FoodCrafter = 2,
        WeaponCrafter = 3,
        Manufactory =4
    }
    public delegate List<InventoryItem> InventoryHandlerDelegate(List<InventoryItem> item);
    public class CraftingConfiguration
    {
        private CraftInterface craftInterface = CraftInterface.Inactive;
        public CraftInterface CraftInterfaceType { 
            get => craftInterface; 
            set => craftInterface = value; 
        }
        public CInterfaceConfiguration GetCInterface()
        {
            return new CInterfaceConfiguration()
            {
                type = (int)CraftInterfaceType,
                isActive = CraftInterfaceType != CraftInterface.Inactive,
                craftSpots = CraftInterfaceType != CraftInterface.Inactive ? 4 : 0,
            };
        }
        public Action<InventoryItemCollection, Action> CraftPressed { get; set; }
        public InventoryHandlerDelegate HandlerDelegate { get; set; }


    }

    public class CInterfaceConfiguration
    {
        public int type { get; set; }
        public bool isActive { get; set; }
        public int craftSpots { get; set; }
    }

    [UserInterface("inventory", true)]
    public class Inventory : UserInterface
    {
        private List<InventoryItem> items = new List<InventoryItem>();

        private bool _useCustomSetter = false;
        private dynamic setHandler;

        protected override bool AllowHiddenConfiguration => false;

        [Configuration("items")]
        public List<InventoryItem> Items { get
            {

                if (ContainerConfiguration.isActive)
                {
                    var localRange = InventoryService.InventoryItems.ToList();
                    localRange.AddRange(ContainerConfiguration.invItems);
                    return localRange;
                }
                if (CInterfaceConfiguration.isActive)
                {
                   
                    var localRange = InventoryService.InventoryItems.ToList();
                    /*var invx = new List<InventoryItem>();
                    for (int i=0; i < CInterfaceConfiguration.craftSpots; i++) {
                        if (i>= t_craft.Count)
                            invx.Add(new InventoryItem());
                        else
                            invx.Add()
                    }*/
                    localRange.AddRange(t_craft);
                    return localRange;
                }
                return InventoryService.InventoryItems;
            } set {

            }
        }

        private InventoryContainerConfiguration containerData = new InventoryContainerConfiguration() { isActive = false };


        public CraftingConfiguration CraftConfigure = new CraftingConfiguration();

        [Configuration("craft")]
        public CInterfaceConfiguration CInterfaceConfiguration { get => CraftConfigure.GetCInterface(); }

        [Configuration("container")] 
        public InventoryContainerConfiguration ContainerConfiguration { get => containerData; set =>containerData = value; }


        private InventoryItemCollection t_craft = new InventoryItemCollection();

        [Reactive("itemsX")]
        public string ItemsX { get => ""; set
            {
                try
                {

                    var ixl = new List<InventoryItem>();

                    var inBetween = JsonConvert.DeserializeObject<List<InventoryItem>>(value);
                    for (int i = 0; i < 19; i++)
                    {
                        var x = inBetween[i];
                        if (x.name != null)
                        {
                            x.modifiers = new ExpandoObject();
                            x.modifiers = JsonConvert.DeserializeAnonymousType(x.transportString, x.modifiers);
                        }
                        ixl.Add(x);

                    }

                    var ItemsAdded = InventoryService.InvDiff(ixl, InventoryService.InventoryItems);
                    
                    var ItemsRemoved = InventoryService.InvDiff(InventoryService.InventoryItems, ixl);


                    foreach (var item in ItemsRemoved)
                    {
                        if (CrappyWorkarounds.HasProperty(item.modifiers, "DynamicInstanceId"))
                        {
                            Equipables.Equipable eqp = InventoryService.GetEquipableById(item.modifiers.DynamicInstanceId);
                            eqp.DeInstance();
                            InventoryService.Equipables.Remove(eqp);
                        }
                    }
                    foreach (var item in ItemsAdded)
                    {
                        if (CrappyWorkarounds.HasProperty(item.modifiers, "O_NAME"))
                        {
                            Equipables.Equipable eqp = EquipmentService.ConstructEquipable(item.modifiers.O_NAME, item.modifiers.O_ICON, item.modifiers);
                            InventoryService.Equipables.Add(eqp);
                        }
                    }



                    InventoryService.InventoryItems = InventoryService.DeferModifiers(ixl, InventoryService.InventoryItems);


                    /*UpdateAsync();*/
                    if (ContainerConfiguration.isActive && !_useCustomSetter)
                    {
                        var containerItems = new List<InventoryItem>();
                        for (int i = 19; i < inBetween.Count; i++)
                        {
                            var x = inBetween[i];
                            if (x.name != null)
                            {
                                x.modifiers = new ExpandoObject();
                                x.modifiers = JsonConvert.DeserializeAnonymousType(x.transportString, x.modifiers);
                            }
                            containerItems.Add(x);
                        }
                        
                        InventoryService.SetContainerInventory(ContainerConfiguration.containerId, containerItems);


                    }
                    if (_useCustomSetter)
                    {
                        var containerItems = new List<InventoryItem>();
                        for (int i = 19; i < inBetween.Count; i++)
                        {
                            var x = inBetween[i];
                            if (x.name != null)
                            {
                                x.modifiers = new ExpandoObject();
                                x.modifiers = JsonConvert.DeserializeAnonymousType(x.transportString, x.modifiers);
                            }
                            containerItems.Add(x);
                        }
                        setHandler(containerItems);
                    }


                    if (CInterfaceConfiguration.isActive)
                    {
                        var containerItems = new List<InventoryItem>();
                        for (int i = 19; i < inBetween.Count; i++)
                        {
                            var x = inBetween[i];
                            if (x.name != null)
                            {
                                x.modifiers = new ExpandoObject();
                                x.modifiers = JsonConvert.DeserializeAnonymousType(x.transportString, x.modifiers);
                            }
                            containerItems.Add(x);
                        }
                        
                        t_craft = new InventoryItemCollection(containerItems);
                        /*CraftConfigure.CraftPressed(new InventoryItemCollection(containerItems), () =>
                        {
                            UpdateAsync();
                        });*/
                    }
                    
                    InventoryService.InvokeInventoryUpdate();
                }
                catch(Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                

                
            } }


        [Reactive("_hide")]
        public void OnHide()
        {
            if (!InventoryService.ShowingInventory) return;
            InventoryService.ShowingInventory = false;
            InventoryService.HideInventory();
        }
        [Reactive("hide")]
        public void OnHide2()
        {
            if (!InventoryService.ShowingInventory) return;
            InventoryService.ShowingInventory = false;
            InventoryService.HideInventory();
        }

        [Reactive("craft_b")]
        public void Craft()
        {
            if (CInterfaceConfiguration.isActive)
            {
                CraftConfigure.CraftPressed(new InventoryItemCollection(t_craft), () =>
                {
                    t_craft.Clear();
                    for (int i = 0; i < CInterfaceConfiguration.craftSpots; i++)
                    {
                        t_craft.Add(new InventoryItem());
                    }
                    UpdateAsync();
                });
            }
        }

        protected override void Cleanup()
        {
            //InventoryService.InvokeInventoryUpdate();
            //InventoryService.InvokeInventoryEnd();
            //base.Cleanup();
        }

       /* protected override async Task ConfigureAsync()
        {
            items = (List<InventoryItem>)Properties.items;
            InventoryService.InventoryUpdated += InventoryService_InventoryUpdated;
            await base.ConfigureAsync();
        }*/
        protected override async Task ConfigureAsync()
        {


            if (CrappyWorkarounds.HasProperty(Properties, "containerId"))
            {
                DebugService.Watchpoint("CONTAINERID", "FUCKING KILL ME");
                Container metaData = await InventoryService.GetContainerMetaData(Properties.containerId);
                ContainerConfiguration = new InventoryContainerConfiguration()
                {
                    isActive = true,
                    items = metaData.MaxItems,
                    name=metaData.Name,
                    containerId = metaData.Id,
                    invItems = await InventoryService.GetContainerInventory(metaData.Id)
                };

            }
            if (CrappyWorkarounds.HasProperty(Properties, "custom"))
            {
                var c = Properties.custom;
                ContainerConfiguration = new InventoryContainerConfiguration()
                {
                    isActive = true,
                    items = c.MaxItems,
                    name = c.Name,
                    containerId = c.Id,
                    invItems = InventoryService.PrepareInventoryList(c.Inventory)
                };
            }
            if (CrappyWorkarounds.HasProperty(Properties, "customSetHandler"))
            {
                _useCustomSetter = true;
                setHandler = Properties.customSetHandler;
            }
            if (CrappyWorkarounds.HasProperty(Properties, "craftConfigure"))
            {
                
                if (!CInterfaceConfiguration.isActive)
                {
                    CraftConfigure = Properties.craftConfigure;
                    t_craft.Clear();
                    for (int i = 0; i < CInterfaceConfiguration.craftSpots; i++)
                    {
                        t_craft.Add(new InventoryItem());
                    }
                }
                CraftConfigure = Properties.craftConfigure;
            }
            await base.ConfigureAsync();
        }

        private void InventoryService_InventoryUpdated()
        {
            UpdateAsync();
        }
        protected override async Task BeforeShow()
        {
            InventoryService.InventoryUpdated += InventoryService_InventoryUpdated;
            await base.BeforeShow();
        }
    }
}
