using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CitizenFX.Core.Native.API;
using System.Threading.Tasks;
using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Utility;
using System.Dynamic;
using ProjectEmergencyFrameworkClient.Equipables;
using ProjectEmergencyFrameworkClient.Effects;
using Newtonsoft.Json;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class CharacterService
    {
        public static uint Timestamp()
        {
            return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static double TimestampF()
        {
            return (double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public delegate void CharacterChangedDelegate(Character newCharacter);
        public static event CharacterChangedDelegate CharacterChanged;
        public static IncarcerationRecord IncarcerationRecord;
        public static Arrests CurrentArrest;
        public static bool CharacterSelected = false;
        public static Character CurrentCharacter;
        public static string CurrentPlayerId;
        public static bool CharacterCuffed = false;
        public static bool IsChracterGrabbed = false;
        public static int PedGrabbingCharacter = 0;
        public static bool IsCharacterGrabbing = false;
        public static int CharacterBeingGrabbed = 0;
        public static uint LastShotTime = 0;
        public static uint LastWelfareTime = Timestamp();
        public static bool IsIncarcerated { get => CurrentCharacter.IsIncarcerated; }
        
        public static async Task<string> CreateCharacter(CharacterInit init) {
            return await Utility.QueryService.Query<string>("CREATE_CHARACTER", init);
        }
        /// <summary>
        /// Downloads a character and spawns them in game and releases all resources.
        /// </summary>
        /// <param name="characterId">The id of the character</param>
        /// <returns></returns>
        public static async Task SetCharacterAndSpawn(string characterId) {
            await RoutingService.RouterSetMainBucket();
            HealthEffectService.StopAllHealthEffects();
            //CurrentCharacter = await Utility.QueryService.Query<Character>("GET_CHARACTER", characterId);
            var xx = await Utility.QueryService.Query<object>("GET_CHARACTER", characterId);
            await Utility.QueryService.Query<object>("SET_CHARACTER", characterId);
            var aptId = await Utility.QueryService.QueryConcrete<Apartment>("GET_CHARACTER_PRIMARY_APARTMENT", characterId);
            CharacterSelected = true;
            CurrentCharacter = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<Character>(xx);
            Interfaces.InterfaceController.HideInterface("characterbuilder");
            Interfaces.InterfaceController.HideInterface("characterselector");
            DoScreenFadeOut(1000);
            await BaseScript.Delay(1000);
            RenderScriptCams(false, false, 0, true, true);
            EnableAllControlActions(0);
            FreezeEntityPosition(Game.PlayerPed.Handle, false);
            LastWelfareTime = Timestamp();
            LastShotTime = 0;
            uint model = (uint)GetHashKey("mp_m_freemode_01");

            if (!HasModelLoaded(model))
            {
                RequestModel(model);
                while (!HasModelLoaded(model))
                {
                    await BaseScript.Delay(0);
                }
            }

            await BaseScript.Delay(2000);

            SetPlayerModel(PlayerId(), model);

            SetPedDefaultComponentVariation(Game.PlayerPed.Handle);

            SetPedComponentVariation(Game.PlayerPed.Handle, 4, CurrentCharacter.Clothing.Pants.Drawable, CurrentCharacter.Clothing.Pants.Texture, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 0, CurrentCharacter.Physical.HeadId, 0, 0);
            SetPedHeadBlendData(Game.PlayerPed.Handle, CurrentCharacter.Physical.HeadId, 0, CurrentCharacter.Physical.HeadId, CurrentCharacter.Physical.HeadId, 0, CurrentCharacter.Physical.HeadId, 0f, 0f, 1f, true);

            //SetPedComponentVariation(Game.PlayerPed.Handle, 2, CurrentCharacter.Physical.HairId, CurrentCharacter.Physical.HairColorId, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 8, CurrentCharacter.Clothing.Undershirt.Drawable, CurrentCharacter.Clothing.Undershirt.Texture, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 3, CurrentCharacter.Physical.Torso, 0, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 11, CurrentCharacter.Clothing.Shirt.Drawable, CurrentCharacter.Clothing.Shirt.Texture, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 6, CurrentCharacter.Clothing.Shoes.Drawable, CurrentCharacter.Clothing.Shoes.Texture, 0);

            //EYEBROW
            SetPedHeadOverlay(Game.PlayerPed.Handle, 2, CurrentCharacter.Physical.EyebrowStyleId, CurrentCharacter.Physical.EyebrowOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, CurrentCharacter.Physical.EyebrowColor, CurrentCharacter.Physical.EyebrowColor);

            //MOLE
            SetPedHeadOverlay(Game.PlayerPed.Handle, 9, CurrentCharacter.Physical.MoleStyleId, CurrentCharacter.Physical.MoleOpacity);

            //SUN DAMAGE
            SetPedHeadOverlay(Game.PlayerPed.Handle, 7, CurrentCharacter.Physical.SunDamageStyleId, CurrentCharacter.Physical.SunDamageOpacity);

            // AGEING
            SetPedHeadOverlay(Game.PlayerPed.Handle, 3, CurrentCharacter.Physical.AgeingStyleId, CurrentCharacter.Physical.AgeingStyleOpacity);

            // BEARD
            SetPedHeadOverlay(Game.PlayerPed.Handle, 1, CurrentCharacter.Physical.BeardId, CurrentCharacter.Physical.BeardOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, CurrentCharacter.Physical.BeardColorId, CurrentCharacter.Physical.BeardColorId);

            //HAIR
            SetPedComponentVariation(Game.PlayerPed.Handle, 2, CurrentCharacter.Physical.HairId, 0, 0);
            SetPedHairColor(Game.PlayerPed.Handle, CurrentCharacter.Physical.HairColorId, 0);

            // Face Features

            if (CurrentCharacter.Physical.FaceFeaturesByIndex != null)
            {
                if (CurrentCharacter.Physical.FaceFeaturesByIndex.Count == 20)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        SetPedFaceFeature(Game.PlayerPed.Handle, i, CurrentCharacter.Physical.FaceFeaturesByIndex[i]);
                    }
                }else
                {
                    DebugService.DebugWarning("CHARACTER_SPAWN", "The face features for the specified character is not complete.");
                }
            }else
            {
                DebugService.DebugWarning("CHARACTER_SPAWN", "The specified character has no facial features.");

            }

            ClearPedDecorations(Game.PlayerPed.Handle);
            if (CurrentCharacter.Physical.Tattoos != null)
            {
                foreach (var tattoo in CurrentCharacter.Physical.Tattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.CollectionName), (uint)GetHashKey(tattoo.Name));
                }

            }


            SetEntityVisible(Game.PlayerPed.Handle, true, true);
            SetEntityCoords(Game.PlayerPed.Handle, -1042.635f, -2745.828f, 21.358f, true, false, false, false);

            foreach (var apt in ConfigurationService.CurrentConfiguration.Apartments)
            {
                if (apt.Id == aptId.ApartmentConfigName)
                {
                    var vx = ConfigurationService._loc_to_vector_3(apt.SpawnLocation);
                    Game.PlayerPed.Position = vx;
                }
            }
          
            if (IsIncarcerated)
            {
                bool shouldActuallyBeArrested = true;
                IncarcerationRecord = await QueryService.QueryConcrete<IncarcerationRecord>("GET_CHARACTER_INCARCERATION", CurrentCharacter.Id);
                CurrentArrest = await QueryService.QueryConcrete<Arrests>("GET_CHARACTER_ARREST", CurrentCharacter.Id);

                if (CurrentArrest.Plea == "NOT GUILTY")
                {
                    var curTime = Timestamp();
                    if (curTime > IncarcerationRecord.ArrestTimestap + (60 * 60 * 24 * 3) || IncarcerationRecord.BailPaid >= IncarcerationRecord.Bail)
                    {
                        await QueryService.QueryConcrete<Arrests>("MARK_CHARACTER_FREE", CurrentCharacter.Id);
                        shouldActuallyBeArrested = false;
                        CurrentCharacter.IsIncarcerated = false;
                    }
                    else
                    {
                        Interact.InteractService.ConstructInteract("cj@bail", new Vector3(1840.37f, 2579.55f, 45.01f), new
                        {
                            arrestId = CurrentArrest.Id,
                            bailAmount = IncarcerationRecord.Bail
                        });
                    }
                }

                if (CurrentArrest.Plea == "GUILTY" || CurrentArrest.Plea=="NO CONTEST")
                {
                    var curTime = Timestamp();
                    if (curTime > IncarcerationRecord.TimeOfRelease)
                    {
                        await QueryService.QueryConcrete<Arrests>("MARK_CHARACTER_FREE", CurrentCharacter.Id);
                        shouldActuallyBeArrested = false;
                        CurrentCharacter.IsIncarcerated = false;
                    }
                }

                if (!shouldActuallyBeArrested)
                {
                    SetEntityCoords(Game.PlayerPed.Handle, 1855.807f, 2601.949f, 45.323f, true, false, false, false);
                }
                else
                {
                   
                    SetEntityCoords(Game.PlayerPed.Handle, 1756.56f, 2483.01f, 44.74f, true, false, false, false);


                    SetPedComponentVariation(Game.PlayerPed.Handle, 10, 25, 0, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 11, 32, 0, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 4, 45, 4, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 9, 0, 0, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 6, 42, 1, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 1, 0, 0, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 7, 0, 0, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 5, 0, 0, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 8, 15, 0, 0);
                    SetPedComponentVariation(Game.PlayerPed.Handle, 3, 0, 0, 0);
                    SetPedPropIndex(Game.PlayerPed.Handle, 0, -1, -1, true);

                    TaskService.InvokeUntilExpire(async () =>
                    {
                        if (CurrentArrest.Plea == "NOT GUILTY")
                        {
                            var curTime = Timestamp();
                            if (curTime > IncarcerationRecord.ArrestTimestap + (60 * 60 * 24 * 3))
                            {
                                await QueryService.QueryConcrete<Arrests>("MARK_CHARACTER_FREE", CurrentCharacter.Id);
                                DoBeginIncarceration();
                                return true;
                            }
                        }

                        if (CurrentArrest.Plea == "GUILTY" || CurrentArrest.Plea == "NO CONTEST")
                        {
                            var curTime = Timestamp();
                            if (curTime > IncarcerationRecord.TimeOfRelease)
                            {
                                await QueryService.QueryConcrete<Arrests>("MARK_CHARACTER_FREE", CurrentCharacter.Id);
                                DoBeginIncarceration();
                                return true;
                            }
                        }
                        return false;
                    });

                }

                

            }

            TaskService.InvokeUntilExpire(async () =>
            {
                if (CurrentCharacter.Id != characterId) return true;
                if (Timestamp() - LastWelfareTime > (60 * 10))
                {
                    LastWelfareTime = Timestamp();
                    await BankService.AttemptWelfare();
                }
                return false;
            });

            // TODO: CHANGE SPAWN POSITION!!!
            
            SetEntityHeading(Game.PlayerPed.Handle, -30.0f);

            if (!IsPlayerSwitchInProgress())
            {
                SwitchOutPlayer(Game.PlayerPed.Handle, 1, 1);
            }

            
            await BaseScript.Delay(5000);
            DoScreenFadeIn(1000);

            SwitchInPlayer(Game.PlayerPed.Handle);

            while (GetPlayerSwitchState() != 12)
            {
                await BaseScript.Delay(0);
            }
           

            DisplayRadar(true);

            CharacterChanged?.Invoke(CurrentCharacter);
        }
        public static void DoBeginIncarceration()
        {
            SetCharacterAndSpawn(CurrentCharacter.Id);
        }
        public static async Task RespawnCharacter()
        {
            DisplayRadar(false);
            SwitchOutPlayer(Game.PlayerPed.Handle, 0, 1);
            HealthService.IsBeingTreated = false;
            HealthService.IsAlive = true;
            Game.PlayerPed.IsInvincible = false;
            Game.PlayerPed.Health = 100;
            SetEnableHandcuffs(Game.PlayerPed.Handle, false);
            if (HealthEffectService.IsEffectRunning("health@dying"))
            {
                HealthEffectService.StopHealthEffect("health@dying");
            }
            if (HealthEffectService.IsEffectRunning("health@injured"))
            {
                HealthEffectService.StopHealthEffect("health@injured");
            }
            HealthEffectService.StopAllHealthEffects();
            await BaseScript.Delay(5000);
            ClearPedBloodDamage(Game.PlayerPed.Handle);
            SetEntityCoords(Game.PlayerPed.Handle, -1042.635f, -2745.828f, 21.358f, true, false, false, false);
            SetEntityHeading(Game.PlayerPed.Handle, -30.0f);
            SwitchInPlayer(Game.PlayerPed.Handle);
          
            while (GetPlayerSwitchState() != 12)
            {
                await BaseScript.Delay(0);
            }
            DisplayRadar(true);
        }
        public static Equipable MintClothingItem(ClothingItemType type, int drawable, int texture)
        {
            dynamic props = new ExpandoObject();
            props.drawable = drawable;
            props.variant = texture;
            props.clothType = (int)type;
            props.icon = "";
            return EquipmentService.ConstructEquipable("_CLOTHING_ITEM", "_CLOTH", props);
        }
        public static Equipable MintClothingItem(ClothingItemType type, int drawable, int texture, int badge, int badgeTexture)
        {
            dynamic props = new ExpandoObject();
            props.drawable = drawable;
            props.variant = texture;
            props.clothType = (int)type;
            props.badge = badge;
            props.badgeVariant = badgeTexture;
            return EquipmentService.ConstructEquipable("_CLOTHING_ITEM", "_CLOTH", props);
        }


        public static async Task SetCharacterQuietly(string characterId, string setOrd = null)
        {
            await RoutingService.RouterSetMainBucket();
            //CurrentCharacter = await Utility.QueryService.Query<Character>("GET_CHARACTER", characterId);
            var xx = await Utility.QueryService.Query<object>("GET_CHARACTER", characterId);
            await Utility.QueryService.Query<object>("SET_CHARACTER", characterId);
            CharacterSelected = true;
            CurrentCharacter = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<Character>(xx);
            Interfaces.InterfaceController.HideInterface("characterbuilder");
            Interfaces.InterfaceController.HideInterface("characterselector");
       
            EnableAllControlActions(0);
            FreezeEntityPosition(Game.PlayerPed.Handle, false);

            uint model = (uint)GetHashKey("mp_m_freemode_01");

            if (!HasModelLoaded(model))
            {
                RequestModel(model);
                while (!HasModelLoaded(model))
                {
                    await BaseScript.Delay(0);
                }
            }

            await BaseScript.Delay(2000);

            SetPlayerModel(PlayerId(), model);


            SetPedDefaultComponentVariation(Game.PlayerPed.Handle);

            SetPedComponentVariation(Game.PlayerPed.Handle, 4, CurrentCharacter.Clothing.Pants.Drawable, CurrentCharacter.Clothing.Pants.Texture, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 0, CurrentCharacter.Physical.HeadId, 0, 0);
            SetPedHeadBlendData(Game.PlayerPed.Handle, CurrentCharacter.Physical.HeadId, 0, CurrentCharacter.Physical.HeadId, CurrentCharacter.Physical.HeadId, 0, CurrentCharacter.Physical.HeadId, 0f, 0f, 1f, true);

            //SetPedComponentVariation(Game.PlayerPed.Handle, 2, CurrentCharacter.Physical.HairId, CurrentCharacter.Physical.HairColorId, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 8, CurrentCharacter.Clothing.Undershirt.Drawable, CurrentCharacter.Clothing.Undershirt.Texture, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 3, CurrentCharacter.Physical.Torso, 0, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 11, CurrentCharacter.Clothing.Shirt.Drawable, CurrentCharacter.Clothing.Shirt.Texture, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 6, CurrentCharacter.Clothing.Shoes.Drawable, CurrentCharacter.Clothing.Shoes.Texture, 0);

            //EYEBROW
            SetPedHeadOverlay(Game.PlayerPed.Handle, 2, CurrentCharacter.Physical.EyebrowStyleId, CurrentCharacter.Physical.EyebrowOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, CurrentCharacter.Physical.EyebrowColor, CurrentCharacter.Physical.EyebrowColor);

            //MOLE
            SetPedHeadOverlay(Game.PlayerPed.Handle, 9, CurrentCharacter.Physical.MoleStyleId, CurrentCharacter.Physical.MoleOpacity);

            //SUN DAMAGE
            SetPedHeadOverlay(Game.PlayerPed.Handle, 7, CurrentCharacter.Physical.SunDamageStyleId, CurrentCharacter.Physical.SunDamageOpacity);

            // AGEING
            SetPedHeadOverlay(Game.PlayerPed.Handle, 3, CurrentCharacter.Physical.AgeingStyleId, CurrentCharacter.Physical.AgeingStyleOpacity);

            // BEARD
            SetPedHeadOverlay(Game.PlayerPed.Handle, 1, CurrentCharacter.Physical.BeardId, CurrentCharacter.Physical.BeardOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, CurrentCharacter.Physical.BeardColorId, CurrentCharacter.Physical.BeardColorId);

            //HAIR
            SetPedComponentVariation(Game.PlayerPed.Handle, 2, CurrentCharacter.Physical.HairId, 0, 0);
            SetPedHairColor(Game.PlayerPed.Handle, CurrentCharacter.Physical.HairColorId, 0);

            if (CurrentCharacter.Physical.FaceFeaturesByIndex != null)
            {
                if (CurrentCharacter.Physical.FaceFeaturesByIndex.Count == 20)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        SetPedFaceFeature(Game.PlayerPed.Handle, i, CurrentCharacter.Physical.FaceFeaturesByIndex[i]);
                    }
                }
                else
                {
                    DebugService.DebugWarning("CHARACTER_SPAWN", "The face features for the specified character is not complete.");
                }
            }
            else
            {
                DebugService.DebugWarning("CHARACTER_SPAWN", "The specified character has no facial features.");

            }

            ClearPedDecorations(Game.PlayerPed.Handle);
            if (CurrentCharacter.Physical.Tattoos != null)
            {
                foreach (var tattoo in CurrentCharacter.Physical.Tattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.CollectionName), (uint)GetHashKey(tattoo.Name));
                }

            }


            // TODO: CHANGE SPAWN POSITION!!!



            DisplayRadar(true);

            CharacterChanged?.Invoke(CurrentCharacter);

            if (setOrd != null)
                OrganizationService.GoOnDuty(setOrd, false);
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static void CuffHandler()
        {
            if (CharacterCuffed)
            {
                DisablePlayerFiring(Game.PlayerPed.Handle, true);
                DisableControlAction(0, 25, true);
                DisableControlAction(1, 140, true);
                DisableControlAction(1, 141, true);
                DisableControlAction(1, 142, true);
                SetPedPathCanUseLadders(GetPlayerPed(PlayerId()), false);

                if (IsPedInAnyVehicle(Game.PlayerPed.Handle, false))
                {
                    DisableControlAction(0, 59, true);
                }

            }
        }

        public static CharacterSnapshot GetCharacterSnapshot()
        {
            CharacterSnapshot snapshot = new CharacterSnapshot();

            snapshot.character = JsonConvert.DeserializeObject<Character>(JsonConvert.SerializeObject(CurrentCharacter)); // Way to do a deep copy. 

            for (int i = 0; i < 12; i++)
            {
                var draw = GetPedDrawableVariation(Game.PlayerPed.Handle, i);
                var texture = GetPedTextureVariation(Game.PlayerPed.Handle, i);
                snapshot.clothes.Add(new Tuple<int, int>(draw, texture));
            }
            for (int i = 0; i < 7;)
            {
                var draw = GetPedPropIndex(Game.PlayerPed.Handle, i);
                var texture = GetPedPropTextureIndex(Game.PlayerPed.Handle, i);
                snapshot.props.Add(new Tuple<int, int>(draw, texture));
            }

            return snapshot;
        }

        /// <summary>
        /// Gets all of a players characters
        /// </summary>
        /// <param name="playerId">The Player Id</param>
        /// <returns>A list of their alive characters</returns>
        public static async Task<List<object>> GetCharacters()
        {
            return await Utility.QueryService.Query<List<object>>("GET_CHARACTERS");
        }
        
    }
    public class CharacterSnapshot
    {
        public List<Tuple<int, int>> clothes = new List<Tuple<int, int>>();
        public List<Tuple<int, int>> props = new List<Tuple<int, int>>();
        public Character character;
        public void Restore()
        {
            RestoreClothes();
            RestoreProps();
            RestoreTattoo();
            RestorePhysical();
        }
        
        public void RestorePhysical()
        {
            //EYEBROW
            SetPedHeadOverlay(Game.PlayerPed.Handle, 2, character.Physical.EyebrowStyleId, character.Physical.EyebrowOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, character.Physical.EyebrowColor, character.Physical.EyebrowColor);

            //MOLE
            SetPedHeadOverlay(Game.PlayerPed.Handle, 9, character.Physical.MoleStyleId, character.Physical.MoleOpacity);

            //SUN DAMAGE
            SetPedHeadOverlay(Game.PlayerPed.Handle, 7, character.Physical.SunDamageStyleId, character.Physical.SunDamageOpacity);

            // AGEING
            SetPedHeadOverlay(Game.PlayerPed.Handle, 3, character.Physical.AgeingStyleId, character.Physical.AgeingStyleOpacity);

            // BEARD
            SetPedHeadOverlay(Game.PlayerPed.Handle, 1, character.Physical.BeardId, character.Physical.BeardOpacity);
            SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, character.Physical.BeardColorId, character.Physical.BeardColorId);

            //HAIR
            SetPedComponentVariation(Game.PlayerPed.Handle, 2, character.Physical.HairId, 0, 0);
            SetPedHairColor(Game.PlayerPed.Handle, character.Physical.HairColorId, 0);

            if (character.Physical.FaceFeaturesByIndex != null)
            {
                if (character.Physical.FaceFeaturesByIndex.Count == 20)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        SetPedFaceFeature(Game.PlayerPed.Handle, i, character.Physical.FaceFeaturesByIndex[i]);
                    }
                }
                else
                {
                    DebugService.DebugWarning("CHARACTER_SPAWN", "The face features for the specified character is not complete.");
                }
            }
            else
            {
                DebugService.DebugWarning("CHARACTER_SPAWN", "The specified character has no facial features.");

            }
        }
        public void RestoreClothes()
        {
            for (int i = 0; i < 12; i++)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, i, clothes[i].Item1, clothes[i].Item2, 0);
            }
        }
        public void RestoreProps()
        {
            for (int i = 0; i < 7;)
            {
                SetPedPropIndex(Game.PlayerPed.Handle, i, props[i].Item1, props[i].Item2, true);
            }
        }
        public void ClearProps()
        {
            for (int i = 0; i < 7;)
            {
                SetPedPropIndex(Game.PlayerPed.Handle, i, 0, 0, true);
            }
        }
        public void RestoreTattoo()
        {
            ClearPedDecorations(Game.PlayerPed.Handle);
            if (character.Physical.Tattoos != null)
            {
                foreach (var tattoo in character.Physical.Tattoos)
                {
                    SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.CollectionName), (uint)GetHashKey(tattoo.Name));
                }

            }
        }
    }

}
