using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Monobehaviour used for levels.
    /// </summary>
    public class Level : MonoBehaviour
    {
        public Vector3 gravity = new Vector3(0, -9.81f, 0f);

        public void Awake()
        {
            Physics.gravity = this.gravity;
        }
    }
}