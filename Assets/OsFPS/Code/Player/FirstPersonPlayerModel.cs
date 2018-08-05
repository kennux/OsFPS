using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.BehaviourModel;

namespace OsFPS
{
    /// <summary>
    /// Specialized entity model implementation that defines some additional events for first person players.
    /// </summary>
    [RequireComponent(typeof(FirstPersonEntity))]
    public class FirstPersonPlayerModel : PlayerEntityModel
    {
        /// <summary>
        /// Mouse X|Y movement.
        /// </summary>
        public ModelProperty<Vector2> inputLook = new ModelProperty<Vector2>();

        /// <summary>
        /// Camera recoil event sent from <see cref="FirstPersonWeaponAnimation"/>.
        /// </summary>
        public ModelEvent<Vector2> cameraRecoil = new ModelEvent<Vector2>();

        /// <summary>
        /// This activity is used for handling weapon aiming / zoom.
        /// </summary>
        public ModelActivity zoom = new ModelActivity();
        
        
    }
}