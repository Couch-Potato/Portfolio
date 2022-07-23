using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    public enum ChunkingMethod
    {
        PositionCalculated,
        Manual
    }
    public interface IInteractable
    {
        /// <summary>
        /// If the interactable element can be shown at the current time
        /// </summary>
        /// <returns>If the interact is to be shown.</returns>
        Task<bool> CanShow();

        /// <summary>
        /// Invokes the interactable elements main code
        /// </summary>
        void Interact();

        /// <summary>
        /// The properties of the interact
        /// </summary>
        dynamic Properties { get; set; }

        /// <summary>
        /// The position of the interact
        /// </summary>
        Vector3 Position {get;set;}

        /// <summary>
        /// The chunk id of the interact.
        /// </summary>
        int ChunkId { get; set; }

        /// <summary>
        /// The chunking method of the interactable element.
        /// </summary>
        ChunkingMethod ChunkingMethod { get; }

        Entity Entity { get; set; }

        DoorObject AttachedDoor { get; set; }
    }
}
