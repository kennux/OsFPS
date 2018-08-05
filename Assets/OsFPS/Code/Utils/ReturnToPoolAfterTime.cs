using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Helper script that destroys the gameobject after the specified amount of seconds time.
    /// </summary>
    public class ReturnToPoolAfterTime : MonoBehaviour
    {
        public float time;

        public void OnEnable()
        {
            Invoke("Return", this.time);
        }

        private void Return()
        {
            PrefabPool.instance.Return(this.gameObject);
        }
    }
}