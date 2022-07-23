using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEmergencyFrameworkClient.Utility;
using System.Dynamic;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class VehicleService
    {
        static string FuelLevelProperty = "_Fuel_Level";
        public static VehicleInfo CurrentVehicle;
        public static InventoryItemCollection SpecialTrunkItems;

        // DANGEROUS FUNCTION! NEEDS TO BE REWORKED SOON...
        [Obsolete("DANGEROUS FUNCTION! NEEDS TO BE REWORKED SOON.")]
        public static async Task AddVehicle(ProjectEmergencyFrameworkShared.Data.Model.Vehicle veh)
        {
            await Utility.QueryService.QueryConcrete<bool>("VEH_ADD", veh);
        }

        public static async Task SpawnVehicle(string id, Vector3 location, float heading)
        {
            
            var veh = await Utility.QueryService.QueryConcrete<ProjectEmergencyFrameworkShared.Data.Model.Vehicle>("GET_VEHICLE", id);

           
            var modelId = (uint)GetHashKey(veh.SpawnName);
            RequestModel(modelId);
            DeleteCurrentSpawnedVehicle();
            var vehicle = CreateVehicle(modelId, location.X, location.Y, location.Z, heading, true, false);

            CurrentVehicle = new VehicleInfo()
            {
                Handle = vehicle,
                Id = veh.Id
            };


            var fuelTankCapacity = VehicleMaxFuelLevel(new Vehicle(vehicle));
            if (!new Vehicle(vehicle).HasDecor("_Fuel_Level"))
            {
                new Vehicle(vehicle).SetDecor(FuelLevelProperty,  .5f *fuelTankCapacity
                    );
            }

            new Vehicle(vehicle).State.Set("orgConnected", false, true);
            if (veh.BelongsToOrganization)
            {
                var plate = await Utility.QueryService.QueryConcrete<string>("GET_ORG_VEH_PLATE", new ProjectEmergencyFrameworkShared.Data.Model.OrgRequest()
                {
                    CurCharId = CharacterService.CurrentCharacter.Id,
                    OrgRequestId = OrganizationService.ConnectedOrganization.CallableId
                });
                if (veh.BelongsToOrganization && OrganizationService.ConnectedOrganization.OrgType == "FIRE")
                {
                    new Vehicle(vehicle).State.Set("ambulance", true, true);
                    SpecialTrunkItems = new InventoryItemCollection();
                    SpecialTrunkItems.AddItem(EquipmentService.ConstructEquipable("Stretcher", "", new
                    {
                        O_NAME = "",
                        O_ICON = ""
                    }));
                }
                
                if (veh.BelongsToOrganization && OrganizationService.ConnectedOrganization.OrgType == "POLICE")
                {
           
                    SpecialTrunkItems = new InventoryItemCollection();
                    dynamic props1 = new ExpandoObject();
                    SpecialTrunkItems.AddItem(EquipmentService.ConstructEquipable("Ballistics Kit", "", props1));
                    dynamic props2 = new ExpandoObject();
                    props2.name = "Pump Shotgun";
                    props2.icon = "/assets/inventory/weapon_shotgun.svg";
                    props2.weapon_hash = 0x969C3D67;
                    SpecialTrunkItems.AddItem(EquipmentService.ConstructEquipable("GUN", "__gun", props2, true));
                    dynamic props3 = new ExpandoObject();
                    props3.type = "SHOTGUN";
                    props3.amount = 45;
                    props3.icon = "/assets/inventory/ammo_tan.svg";
                    SpecialTrunkItems.AddItem(EquipmentService.ConstructEquipable("Ammunition", "", props3));
                }
                new Vehicle(vehicle).State.Set("orgType", OrganizationService.ConnectedOrganization.OrgType, true);
                new Vehicle(vehicle).State.Set("orgId", OrganizationService.ConnectedOrganization.Id, true);
                new Vehicle(vehicle).State.Set("orgConnected", true, true);
                SetVehicleNumberPlateText(vehicle, plate);


                Interact.InteractService.ConstructInteract("specialTrunk", new Vehicle(vehicle) , new
                {
                });


            }
            else
            {
                Interact.InteractService.ConstructInteract("container", new Vehicle(vehicle), new
                {
                    containerId = veh.Container
                });
                SetVehicleNumberPlateText(vehicle, veh.LicensePlate);
            }
            

            SetVehicleColours(vehicle, veh.ColorId, veh.ColorId);
            SetVehicleOnGroundProperly(vehicle);

            SetVehicleDoorsLocked(vehicle, (int)VehicleLockStatus.Unlocked);

            Interact.InteractService.ConstructInteract("door_0", new Vehicle(vehicle), new { });
            Interact.InteractService.ConstructInteract("door_1", new Vehicle(vehicle), new { });
            new Vehicle(vehicle).State.Set("vehicleOwner", CharacterService.CurrentCharacter.Id, true);
            new Vehicle(vehicle).State.Set("vehicleId", id, true);
            new Vehicle(vehicle).State.Set("trunkContainerId", veh.Container, true);
            /*     new Vehicle(vehicle).State["vehicleOwner"] = CharacterService.CurrentCharacter.Id;*/
        }


        private static float VehicleMaxFuelLevel(Vehicle veh)
        {
            return GetVehicleHandlingFloat(veh.Handle, "CHandlingData", "fPetrolTankVolume");
        }

        public static async Task SpawnVehicle(string id, bool atNearestNode)
        {
            var curPos = GetEntityCoords(Game.PlayerPed.Handle, true);
            Vector3 outPos = new Vector3();
            float outHeading = 0.0f;
            var node = GetClosestVehicleNodeWithHeading(curPos.X, curPos.Y, curPos.Z, ref outPos, ref outHeading, 1, 3.0f, 0);

            if (!atNearestNode)
            {
                outPos = curPos;
                outHeading = GetEntityHeading(Game.PlayerPed.Handle);
            }

            await SpawnVehicle(id, outPos, outHeading);
        }
        public static void DeleteCurrentSpawnedVehicle()
        {
            if (CurrentVehicle != null)
            {
                SetEntityAsMissionEntity(CurrentVehicle.Handle, true, true);
                DeleteVehicle(ref CurrentVehicle.Handle);
            }
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static void HandleMDTInCar()
        {
            if (IsPedInDriversSeatOfValidVehicle(Game.PlayerPed) && OrganizationService.IsOnDuty)
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle.State.Get("orgConnected") == null) return;
                if (vehicle.State.Get("orgConnected"))
                {
                    if (vehicle.State.Get("orgId") == OrganizationService.ConnectedOrganization.Id)
                    {
                        if (IsControlJustReleased(0, 244) || IsDisabledControlJustReleased(0, 244))
                        {
                            DispatchService.MDT.Show();
                        }
                    }
                }
            }
        }

        private static bool IsPedInDriversSeatOfValidVehicle(Ped p)
        {
            Ped playerPed = p;
            Vehicle vehicle = playerPed.CurrentVehicle;

            return playerPed.IsInVehicle() &&
              (
                vehicle.Model.IsCar ||
                vehicle.Model.IsBike ||
                vehicle.Model.IsQuadbike
              ) &&
              vehicle.GetPedOnSeat(VehicleSeat.Driver) == playerPed &&
              vehicle.IsAlive;
        }

        private static Vehicle last;

       

        public static float VehicleFuelLevel(Vehicle vehicle)
        {
            if (vehicle.HasDecor(FuelLevelProperty))
            {
                return vehicle.GetDecor<float>(FuelLevelProperty);
            }
            else
            {
                return 65f;
            }
        }

        public static void VehicleSetFuelLevel(Vehicle vehicle, float fuelLevel)
        {
            float max = VehicleMaxFuelLevel(vehicle);

            if (fuelLevel > max)
            {
                fuelLevel = max;
            }

            vehicle.FuelLevel = fuelLevel;
            vehicle.SetDecor(FuelLevelProperty, fuelLevel);
        }

        private static float refuelRate = 1f;
        private static float fuelConsumptionRate = 1f;
        private static float fuelAccelerationImpact = 0.0002f;
         private static float fuelTractionImpact = 0.0001f;
        private static float fuelRPMImpact = 0.0005f;

        [ExecuteAt(ExecutionStage.Tick)]

        public static void FuelHandler()
        {
            var localPed = new Ped(PlayerPedId());
            if (IsPedInDriversSeatOfValidVehicle(Game.PlayerPed))
            {
                /*
                 * Handle the initializing of the fueling system if needed.
                 */
                if (last != null && last != Game.PlayerPed.CurrentVehicle)
                {

                    if (!Game.PlayerPed.CurrentVehicle.HasDecor("_Fuel_Level"))
                    {
                        var fuelTankCapacity = VehicleMaxFuelLevel(Game.PlayerPed.CurrentVehicle);
                        Game.PlayerPed.CurrentVehicle.SetDecor(FuelLevelProperty, .5f
                            );
                    }
                    last = Game.PlayerPed.CurrentVehicle;
                }

                var vehicle = Game.PlayerPed.CurrentVehicle;
                var fuel = VehicleFuelLevel(vehicle);

                if (fuel > 0 && vehicle.IsEngineRunning)
                {
                    float normalizedRPMValue = (float)Math.Pow(vehicle.CurrentRPM, 1.5);
                    float consumedFuel = 0f;

                    consumedFuel += normalizedRPMValue * fuelRPMImpact;
                    consumedFuel += vehicle.Acceleration * fuelAccelerationImpact;
                    consumedFuel += vehicle.MaxTraction * fuelTractionImpact;

                    fuel -= consumedFuel * fuelConsumptionRate;
                    fuel = fuel < 0f ? 0f : fuel;
                }
                VehicleSetFuelLevel(vehicle, fuel);
            }
        }

        public static float LitersToGallons(float l)
        {
            return l / 3.785f;
        }
        public static float GallonsToLiters(float g)
        {
            return g * 3.785f;
        }
        public static Vehicle GetVehicleNearestPlayer()
        {
            var x = World.GetAllVehicles();
            float lowestDist = float.MaxValue;
            Vehicle setveh = null;
            foreach (var vehicle in x)
            {
                if (vehicle.Position.DistanceToSquared(Game.PlayerPed.Position) < lowestDist)
                {
                    if (Vector3.Distance(Game.PlayerPed.Position, vehicle.Position) < 10f)
                    {
                        setveh = vehicle;
                        lowestDist = vehicle.Position.DistanceToSquared(Game.PlayerPed.Position);
                    }
                    
                }
                
            }
            return setveh;
        }
        public static float GetVehicleCapacityDifference(Vehicle veh, int percent)
        {
            float targetCapacity = (VehicleMaxFuelLevel(veh) * percent) / 100f;
            var fuel = VehicleFuelLevel(veh);
            if (fuel >= targetCapacity)
                return 0f;
            var amountToFuel = targetCapacity - fuel;
            return amountToFuel;
        }

        public static async Task DoVehicleRefuel(Vehicle veh, int percentFuelTo)
        {
            float targetCapacity =( VehicleMaxFuelLevel(veh) * percentFuelTo ) / 100f;
            var fuel = VehicleFuelLevel(veh);
            if (fuel >= targetCapacity)
                return;
            var amountToFuel = targetCapacity - fuel;

            int fuelI = (int)Math.Floor(amountToFuel * 10);

            for (int i=0;i<fuelI;i++)
            {
                FuelingStatusUpdated?.Invoke((fuelI/10)*i, i/fuelI);
                await BaseScript.Delay(100);
            }

            SetVehicleFuelLevel(veh.Handle, targetCapacity);
        }
        public static event Action<float, float> FuelingStatusUpdated;

        static Interfaces.UI.CarDashboard dashboard;
        [ExecuteAt(ExecutionStage.Tick)]
        public static void DashboardHandler()
        {
            if (IsPedInDriversSeatOfValidVehicle(Game.PlayerPed))
            {
                if (dashboard == null)
                {
                    dashboard = new Interfaces.UI.CarDashboard();
                    dashboard.Show();
                }
                string vehGear = "N";
                int gear = GetVehicleCurrentGear(Game.PlayerPed.CurrentVehicle.Handle);
                switch (gear)
                {
                    case 0:
                        vehGear = "R"; break;
                    case 1:
                        vehGear = "N"; break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        vehGear = (gear - 1).ToString();
                        break;
                }
                dashboard.Configuration = new Interfaces.UI.DashboardConfiguration()
                {
                    mph = (int)(Game.PlayerPed.CurrentVehicle.Speed * 2.236936f),
                    rpm = Game.PlayerPed.CurrentVehicle.CurrentRPM*7000,
                    fuelPercent = GetVehicleFuelLevel(Game.PlayerPed.CurrentVehicle.Handle) / VehicleMaxFuelLevel(Game.PlayerPed.CurrentVehicle),
                    maxSpeed = GetVehicleModelEstimatedMaxSpeed((uint)Game.PlayerPed.CurrentVehicle.Model.Hash) * 2.236936f,
                    gear=vehGear
                };

            } else
            {
                if (dashboard != null)
                {
                    dashboard.Hide();
                    dashboard = null;
                }
            }

        }

    }
    public class VehicleInfo
    {
        public string Id;
        public int Handle;
    }
    public class VehiclesPool : EntitiesPool<Vehicle>
    {
        public VehiclesPool() : base(0x15e55694, 0x8839120d, 0x9227415a) { }
        protected override Vehicle Cast(int handle) => new Vehicle(handle);
    }
}
