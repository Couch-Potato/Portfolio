using ProjectEmergencyFrameworkShared.Configuration.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration
{
    public class PEConfiguration
    {
        public List<ClothingShop> ClothingShops { get; set; } = new List<ClothingShop>();
        public List<ClothingShopArchetype> ClothingShopArchetypes { get; set; } = new List<ClothingShopArchetype>();

        public List<VehicleShop> VehicleShops { get; set; } = new List<VehicleShop>();
        public List<VehicleShopArchetype> VehicleShopArchetypes { get; set; } = new List<VehicleShopArchetype>();

        public List<GenericShop> GenericShops { get; set; } = new List<GenericShop>();
        public List<GenericShopArchetype> GenericShopArchetypes { get; set; } = new List<GenericShopArchetype>();

        public List<WeaponShop> WeaponShops { get; set; } = new List<WeaponShop>();
        public List<WeaponShopArchetype> WeaponShopArchetypes { get; set; } = new List<WeaponShopArchetype>();
        public List<Bank> Banks { get; set; } = new List<Bank>();
        public List<GasStation> GasStations { get; set; } = new List<GasStation>();
        public List<GameItem> GameItems { get; set; } = new List<GameItem>();
        public List<ItemIcon> ItemIcons { get; set; }
        public List<CraftingRecipe> CraftingRecipes { get; set; } = new List<CraftingRecipe>();
        public List<UniformConfig> Uniforms { get; set; } = new List<UniformConfig>();
        public List<PoliceConfig> PoliceStations { get; set; } = new List<PoliceConfig>();
        public List<DoorConfig> DoorAreas { get; set; } = new List<DoorConfig>();

        public List<Markers> Markers { get; set; } = new List<Markers>();
        public List<Holster> Holsters { get; set; } = new List<Holster>();
        public List<GenericInteractItem> GenericInteracts { get; set; } = new List<GenericInteractItem>();
        public List<HydratedPropConfig> Props { get; set; } = new List<HydratedPropConfig>();
        public List<PropPlacementConfig> PropPlacements { get; set; } = new List<PropPlacementConfig>();
        public List<TattooShop> TattooShops { get; set; } = new List<TattooShop>();
        public List<BarberShop> BarberShops { get; set; } = new List<BarberShop>();
        public List<ComputerAreaConfig> ComputerAreas { get; set; } = new List<ComputerAreaConfig>();
        public List<ApartmentConfig> Apartments { get; set; } = new List<ApartmentConfig>();
        //Added new config v 05? 
        public List<NPCPed> NPCs { get; set; } = new List<NPCPed>();
        public List<PedBarterRegion> PedBarterRegions { get; set; } = new List<PedBarterRegion>();
        public List<FreightShopArchetype> FreightShopArchetypes { get; set; } = new List<FreightShopArchetype>();
        public List<FreightShop> FreightShops { get; set; } = new List<FreightShop>();
        public List<UniverseArchetype> Universes { get; set; } = new List<UniverseArchetype>();
    }
}
