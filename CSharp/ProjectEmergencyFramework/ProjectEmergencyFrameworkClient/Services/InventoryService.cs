using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static CitizenFX.Core.Native.API;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using System.Dynamic;
using ProjectEmergencyFrameworkClient.Equipables;

namespace ProjectEmergencyFrameworkClient.Services
{
    public class InventoryItemCollection : List<InventoryItem>
    {
        public bool HasItem(string name, string icon)
        {
            foreach (var item in this)
            {
                if (item.name == name && item.icon == icon)
                    return true;
            }
            return false;
        }
        public InventoryItemCollection(List<InventoryItem> x):base(x)
        {
        }
        public InventoryItemCollection() : base()
        {
        }
        public void AddItem(Equipables.Equipable e, int qty = 1)
        {
            var dynInst = RandomUtil.RandomString(16);
            e.Modifiers.DynamicInstanceId = dynInst;
            AddItemWithNewSlot(e.Name, e.Icon, qty, e.Stackable, e.Modifiers);
        }
        public void AddItem(string name, string icon, int qty = 1, bool isStackable = false, dynamic mods = null)
        {
            bool isFull = true;
            bool isInAnyHotbarSlot = false;
            var setQty = qty;
            foreach (var item in this)
            {

                if (item.name == name && item.qty < 64 & qty > 0 && item.isStackable && isStackable && mods == item.modifiers)
                {
                    var qtyAvailForSlot = (64 - item.qty);
                    if (qty > qtyAvailForSlot)
                    {
                        var i = this.IndexOf(item);
                        isInAnyHotbarSlot = i < 4 || isInAnyHotbarSlot;
                        qty = qty - qtyAvailForSlot;
                        this[i].qty = 64;
                        isFull = false;
                    }
                    else
                    {
                        var i = this.IndexOf(item);
                        isInAnyHotbarSlot = i < 4 || isInAnyHotbarSlot;
                        this[i].qty += qty;
                        isFull = false;
                    }
                }
                if (item.name == null)
                {
                    var i = this.IndexOf(item);
                    isInAnyHotbarSlot = i < 4 || isInAnyHotbarSlot;
                    this[i].name = name;
                    this[i].icon = icon;
                    this[i].qty = qty;
                    this[i].isStackable = isStackable;
                    this[i].modifiers = mods;
                    if (mods != null)
                    {
                        this[i].transportString = JsonConvert.SerializeObject(mods);
                    }
                    qty = 0;
                    isFull = false;
                    break;
                }
            }
        

        }
        public void AddItemWithNewSlot(string name, string icon, int qty = 1, bool isStackable = false, dynamic mods = null)
        {
            Add(new InventoryItem()
            {
                name = name,
                icon = icon,
                qty = qty,
                isStackable = isStackable,
                modifiers = mods
            });


        }
    }
    public static class InventoryService
    {
        public static InventoryNotification InventoryNotification;
        public static List<InventoryItem> InventoryItems = new List<InventoryItem>();
        public static List<Equipables.Equipable> Equipables = new List<Equipables.Equipable>();
        public static Equipables.Equipable CurrentlyEquipped;
        public static Hotbar Hotbar;
        public static event Action InventoryUpdated;
        public static bool WeaponWheelEnabled = false;
        public static bool ShowingInventory = false;
        public static void ShowInventory()
        {
            
            Interfaces.InterfaceController.ShowInterface("inventory", new { items = InventoryItems });
            ShowingInventory = true;
        }
        public static bool IsItemEquipped(string name)
        {
            if (GetEquippedItem() != null)
            {
                if (GetEquippedItem().name == name) return true;
            }
            return false;
        }
        public static void HideInventory()
        {
            Interfaces.InterfaceController.HideInterface("inventory");
            ShowingInventory = false;
        }

        private static void SendInventoryNotification(string name, string icon, int qty)
        {
            InventoryNotification.Notify(icon, name, qty);
        }

        public static InventoryItem[] GetInventoryItemsOfName(string name)
        {
            var invenOfType = new List<InventoryItem>();
            foreach(var itx in InventoryItems)
            {
                if (itx.name == name)
                    invenOfType.Add(itx);
            }
            return invenOfType.ToArray();
        }

        public static bool IsItemEquipped(string name, string icon)
        {
            if (Hotbar != null)
            {
                if (Hotbar.Selected != -1)
                {
                    if (InventoryItems[Hotbar.Selected].name == name && InventoryItems[Hotbar.Selected].icon == icon)
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }
        public static InventoryItem GetEquippedItem()
        {
            if (Hotbar != null)
            {
                if (Hotbar.Selected != -1)
                {
                    return InventoryItems[Hotbar.Selected];
                }
            }
            
            return null;
        }
        public static void AddItem(string name, string icon, int qty = 1, bool isStackable = false, dynamic mods = null)
        {
            bool isFull = true;
            bool isInAnyHotbarSlot = false;
            var setQty = qty;
            foreach (var item in InventoryItems)
            {
                
                if (item.name == name && item.qty < 64 & qty > 0 && item.isStackable && isStackable && mods == item.modifiers)
                {
                    var qtyAvailForSlot = (64 - item.qty);
                    if (qty > qtyAvailForSlot)
                    {
                        var i = InventoryItems.IndexOf(item);
                        isInAnyHotbarSlot = i < 4 || isInAnyHotbarSlot;
                        qty = qty - qtyAvailForSlot;
                        InventoryItems[i].qty = 64;
                        isFull = false;
                    }else
                    {
                        var i = InventoryItems.IndexOf(item);
                        isInAnyHotbarSlot = i < 4 || isInAnyHotbarSlot;
                        InventoryItems[i].qty += qty;
                        isFull = false;
                    }
                }
                if (item.name == null)
                {
                    var i = InventoryItems.IndexOf(item);
                    isInAnyHotbarSlot = i < 4 || isInAnyHotbarSlot;
                    InventoryItems[i].name = name;
                    InventoryItems[i].icon = icon;
                    InventoryItems[i].qty = qty;
                    InventoryItems[i].isStackable = isStackable;
                    InventoryItems[i].modifiers = mods;
                    if (mods != null)
                    {
                        InventoryItems[i].transportString = JsonConvert.SerializeObject(mods);
                    }
                    qty = 0;
                    isFull = false;
                    break;
                }
            }
            if (!isFull)
            {
                if (isInAnyHotbarSlot)
                {
                    ShowHotbarForInterval(3000);
                }
                SendInventoryNotification(name, icon, setQty);
                InvokeInventoryUpdate();
            }
           
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static void TickPrimary()
        {
            
            if (CurrentlyEquipped != null)
            {
                if (CurrentlyEquipped.DisabledPrimary)
                    DisableControlAction(0, 24, true);
                if (IsDisabledControlJustPressed(0, 24) || IsControlJustPressed(0, 24))
                {
                    CurrentlyEquipped.PrimaryUp();
                }
            }
        }
        public static void ForceUpdateEquipable(Equipables.Equipable e)
        {
            foreach (var item in InventoryItems)
            {
                if (item.modifiers == null) continue;
                if (CrappyWorkarounds.HasProperty(item.modifiers, "DynamicInstanceId"))
                {
                    if (item.modifiers.DynamicInstanceId == e.Modifiers.DynamicInstanceId)
                    {
                        item.name = e.Name;
                        item.icon = e.Icon;
                        item.modifiers = e.Modifiers;
                        InvokeInventoryUpdate();
                    }
                    
                }
            }
        }
        public static void AddItem(Equipables.Equipable e, int qty = 1)
        {
            
            AddItem(e.Name, e.Icon, qty, e.Stackable, e.Modifiers);
            Equipables.Add(e);
        }
        public static void RemoveItem(Equipables.Equipable e, int qty = 1)
        {
            e.DeInstance();
            RemoveItem(e.Name, e.Icon, qty, e.Modifiers);
            Equipables.Remove(e);
        }

        public static bool RemoveItem(string name, string icon, int qty = 1, dynamic modifiers = null)
        {
            int oqty = qty;
            if (!HasItem(name, icon, qty))
                return false;
            List<InventoryItem> expired = new List<InventoryItem>();
            foreach (var item in InventoryItems)
            {
                if (item.name != null)
                {
                    if (item.name == name)
                    {
                        var i = InventoryItems.IndexOf(item);
                        item.qty -= qty;
                        if (item.qty > 0)
                        {
                            
                        }
                        if (item.qty <= 0)
                        {
                            qty = qty - item.qty;

                            if (CurrentlyEquipped != null)
                            {
                                if (CurrentlyEquipped.Name == name)
                                    CurrentlyEquipped.Unequip();
                            }
                            if (CrappyWorkarounds.HasProperty(item, "DynamicInstanceId"))
                            {
                                Equipables.Equipable eqp = InventoryService.GetEquipableById(item.modifiers.DynamicInstanceId);
                                eqp.DeInstance();
                                InventoryService.Equipables.Remove(eqp);
                            }

                            InventoryItems[i].name = null;
                            InventoryItems[i].icon = null;
                            InventoryItems[i].isStackable = false;
                            InventoryItems[i].modifiers = null;
                            break;
                        }
                    }
                }

            }
            InvokeInventoryUpdate();
            SendInventoryNotification(name, icon, -1 *oqty);
            return true;
        }

        public static bool HasItem(string name, string icon, int qty = 1)
        {
            foreach (var item in InventoryItems)
            {
                if (item.name != null)
                {
                    if (item.name == name)
                    {
                        var i = InventoryItems.IndexOf(item);
                        item.qty -= qty;
                        if (item.qty >= 0)
                        {
                            return true;
                        }
                        if (item.qty < 0)
                        {
                            qty = qty - item.qty;
                        }
                    }
                }
                
            }
            return false;
        }

        public static void ShowHotbar()
        {
            Hotbar.ShowPopup();
        }

        public static void SetHotbarSelected(int id)
        {
            if (CharacterService.CharacterCuffed) return;
            Hotbar.Selected = id;
            EnableControlAction(0, 24, true);
            if (id == -1)
            {
                RemoveAllPedWeapons(Game.PlayerPed.Handle, false);
                ShowHotbarForInterval(3000);
                return;
            }
            if (CurrentlyEquipped != null)
            {
                CurrentlyEquipped.Unequip();
                CurrentlyEquipped = null;
            }
            foreach (var item in Equipables)
            {
                if (InventoryItems[id].name != null && InventoryItems[id].modifiers != null)
                {
                    if (CrappyWorkarounds.HasProperty(InventoryItems[id].modifiers, "DynamicInstanceId"))
                    {
                        var inst = InventoryItems[id].modifiers.DynamicInstanceId;
                        if (item.Modifiers.DynamicInstanceId == inst)
                        {
                            item.Equip();
                           
                            CurrentlyEquipped = item;
                        }
                    }
                }
            }

            ShowHotbarForInterval(3000);

        }

        public static async void ShowHotbarForInterval(int ms)
        {
            if (Hotbar.Showing) return;
            ShowHotbar();
            await CitizenFX.Core.BaseScript.Delay(ms);
            HideHotbar();
        }

        public static void HideHotbar()
        {
            Hotbar.HidePopup();
        }


        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void StartInventoryService()
        {
            CharacterService.CharacterChanged += CharacterService_CharacterChanged;
        }

        private static void CharacterService_CharacterChanged(Character newCharacter)
        {
            InventoryNotification = new InventoryNotification();
            
            InventoryItems = new List<InventoryItem>();
            InventoryUpdated += InventoryUpdatedHandler;



            HandleAsyncLoad();
            
            
        }

        private static async void HandleAsyncLoad()
        {
            await InventoryNotification.Show();
            await ConfigureInventory();
            Hotbar = new Hotbar();
            //await Hotbar.Show();
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static void HideWeaponWheel()
        {
            if (!WeaponWheelEnabled)
            {
                HideHudComponentThisFrame(19);
                HideHudComponentThisFrame(20);
            }
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static void HandleInventoryKeybinds()
        {
            try
            {

               // DisableControlAction(0, 243, true); // `
                DisableControlAction(0, 194, true); // 177

                DisableControlAction(0, 157, true); // 1
                DisableControlAction(0, 158, true); // 2
                DisableControlAction(0, 160, true); // 3
                DisableControlAction(0, 164, true); // 4
                DisableControlAction(0, 37, true); // TAB
                if (IsDisabledControlJustReleased(0, 194))
                {
                    SetHotbarSelected(-1);
                }
                if (IsDisabledControlJustReleased(0, 157))
                {
                    SetHotbarSelected(0);
                }
                if (IsDisabledControlJustReleased(0, 158))
                {
                    SetHotbarSelected(1);
                }
                if (IsDisabledControlJustReleased(0, 160))
                {
                    SetHotbarSelected(2);
                }
                if (IsDisabledControlJustReleased(0, 164))
                {
                    SetHotbarSelected(3);
                }
                if (IsDisabledControlJustReleased(0, 37))
                {
                    if (ShowingInventory)
                        HideInventory();
                    else
                        ShowInventory();
                }
            }catch (Exception ex)
            {
                DebugService.UnhandledException(ex);
            }
            
        }
        public static Equipables.Equipable GetEquipableById(string id)
        {
            foreach (var inst in Equipables)
            {
                if (inst.Modifiers.DynamicInstanceId == id)
                    return inst;
            }
            return null;
        }
        public static async Task ConfigureInventory()
        {
            DebugService.DebugCall("INVENTORY", "Player Inventory Begin Configure");
            InventoryItems = await QueryService.QueryList<InventoryItem>("GET_C_INVENTORY");
            Equipables.Clear();
            foreach (var item in InventoryItems)
            {
                
                if (item.transportString != null)
                {
                    item.modifiers = new ExpandoObject();
                    item.modifiers = JsonConvert.DeserializeAnonymousType(item.transportString, item.modifiers);
                }
                if (item.modifiers != null)
                {
                    if (CrappyWorkarounds.HasProperty(item.modifiers, "O_NAME"))
                    {
                        var ix = EquipmentService.ConstructEquipable(item.modifiers.O_NAME, item.modifiers.O_ICON, item.modifiers);
                        item.modifiers.DynamicInstanceId = ix.Modifiers.DynamicInstanceId;
                        Equipables.Add(ix);
                    }
                }
            }
            
        }
        public static async Task<List<InventoryItem>> GetContainerInventory(string ContainerId)
        {
            var items = await QueryService.QueryList<InventoryItem>("GET_CONTAINER", ContainerId);
            foreach (var item in items)
            {
                if (item.transportString != null)
                {
                    item.modifiers = new ExpandoObject();
                    item.modifiers = JsonConvert.DeserializeAnonymousType(item.transportString, item.modifiers);
                }
                if (item.modifiers != null)
                {
                    if (CrappyWorkarounds.HasProperty(item.modifiers, "O_NAME"))
                    {
                      //  var ix = EquipmentService.ConstructEquipable(item.modifiers.O_NAME, item.modifiers.O_ICON, item.modifiers);
                        //Equipables.Add(ix);
                    }
                }
            }
            return items;
        }
        public static List<InventoryItem> InvDiff(List<InventoryItem> a, List<InventoryItem> b)
        {
            List<InventoryItem> items = new List<InventoryItem>();
            items.AddRange(a);
            //Loop through the "a" array
            foreach (var i_a in a)
            {
                if (i_a.name == null)
                    items.Remove(i_a);
                // Loop through the "b" array
                foreach (var i_b in b)
                {

                    if (i_a.name == i_b.name && i_b.icon == i_a.icon)
                    {
                        if (i_b.modifiers != null && i_a.modifiers != null)
                        {
                            if (CrappyWorkarounds.HasProperty(i_b.modifiers, "DynamicInstanceId") && CrappyWorkarounds.HasProperty(i_a.modifiers, "DynamicInstanceId"))
                            {
                                if (i_b.modifiers.DynamicInstanceId == i_a.modifiers.DynamicInstanceId)
                                    items.Remove(i_a);
                            }
                        }
                        
                    }
                }
            }

            return items;
        }
        public static List<InventoryItem> DeferModifiers(List<InventoryItem> a, List<InventoryItem> b)
        {
            foreach (var ixa in a)
            {
                if (ixa.modifiers == null) continue;
                    foreach (var ixb in b)
                {
                    if (ixb.modifiers == null) continue;
                        if (CrappyWorkarounds.HasProperty(ixa.modifiers, "DynamicInstanceId") && CrappyWorkarounds.HasProperty(ixb.modifiers, "DynamicInstanceId"))
                    {
                        if (ixb.modifiers.DynamicInstanceId == ixa.modifiers.DynamicInstanceId)
                            ixa.modifiers = ixb.modifiers;
                    }
                }
            }
            return a;
        }
        public static List<InventoryItem> PrepareInventoryList(List<InventoryItem> items)
        {
            foreach (var item in items)
            {
                if (item.transportString != null)
                {
                    item.modifiers = new ExpandoObject();
                    item.modifiers = JsonConvert.DeserializeAnonymousType(item.transportString, item.modifiers);
                }
                if (item.modifiers != null)
                {
                    if (CrappyWorkarounds.HasProperty(item.modifiers, "O_NAME"))
                    {
                       // var ix = EquipmentService.ConstructEquipable(item.modifiers.O_NAME, item.modifiers.O_ICON, item.modifiers);
                      //  Equipables.Add(ix);
                    }
                }
                if (item.transportString == null)
                {
                    item.transportString = JsonConvert.SerializeObject(item.modifiers);
                }
            }
            return items;
        }
        /// <summary>
        /// Returns the container metadata
        /// </summary>
        /// <param name="ContainerId">The container id</param>
        /// <returns>The metadata</returns>
        /// <remarks>The inventory information is not specified. Use GetContainerInventory to get properly transported inventory data.</remarks>
        public static async Task<Container> GetContainerMetaData(string ContainerId)
        {
            return await QueryService.QueryConcrete<Container>("GET_CONTAINER_DATA", ContainerId);
        }
        public static void InvokeInventoryUpdate()
        {
            InventoryUpdated?.Invoke();
        }

        public static void OpenCustomInventory(Container c, Action<List<InventoryItem>> onInventorySet)
        {
            ShowingInventory = true;
            Interfaces.InterfaceController.ShowInterface("inventory", new
            {
                custom = c,
                customSetHandler = onInventorySet
            });
        }
        public static event Action OnInventoryEnd;

        public static void OpenNetworkedContainer(string id)
        {
            ShowingInventory = true;
            Interfaces.InterfaceController.ShowInterface("inventory", new
            {
                containerId = id
            });
        }
        public static void InvokeInventoryEnd()
        {
            OnInventoryEnd?.Invoke();
        }
        public static async Task<string> CreateNetworkedContainer(int maxItems, string name, string type)
        {
            return await QueryService.QueryConcrete<string>("CREATE_CONTAINER", new
            {
                max=maxItems,
                name=name,
                type=type
            });
        }
        private static void InventoryUpdatedHandler()
        {
            QueryService.Query<object>("SET_C_INVENTORY", InventoryItems);
        }
        public static void SetContainerInventory(string ContainerId, List<InventoryItem> items)
        {
            QueryService.Query<object>("SET_CONTAINER", new
            {
                ContainerId = ContainerId,
                ItemSet = items
            });
        }
    }
}
