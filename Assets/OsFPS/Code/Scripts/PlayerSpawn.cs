using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Helper script used to mark transforms as player spawns.
    /// </summary>
    public class PlayerSpawn : MonoBehaviour
    {
        public static List<PlayerSpawn> spawns = new List<PlayerSpawn>();

        public void Awake()
        {
            spawns.Add(this);
        }

        public void OnDestroy()
        {
            spawns.Remove(this);
        }
    }
}