using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Helper script that destroys the gameobject after the specified amount of seconds time.
    /// </summary>
    public class DestroyAfterTime : MonoBehaviour
    {
        public float time;

        public void Awake()
        {
            Destroy(this.gameObject, this.time);
        }
    }
}