using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Can be used to define a set of objects being spawned for a decal spawn in <see cref="DebrisManager.TrySpawnDebris(UnityEngine.GameObject, UnityEngine.Vector3, UnityEngine.Vector3, DebrisSpawn)"/>
    /// </summary>
    [System.Flags]
    public enum DebrisSpawn : uint
    {
        None = 0,
        DebrisParticles = 1u,
        BulletDecal = (1u << 1),
    }

    /// <summary>
    /// Manager singleton that is used to spawn debris on bullet hits or other damage events.
    /// Used in <see cref="DamageHelper.SendDamage(UnityEngine.GameObject, DamageEventArgs)"/> to spawn decal and debri.
    /// </summary>
    public class DebrisManager : MonoBehaviour
    {
        public static DebrisManager instance { get { return UnitySingleton<DebrisManager>.Get(); } }

        [System.Serializable]
        public class ConfigEntry
        {
            public string tag;
            public GameObject[] debris;
            public GameObject[] bulletDecal;
        }

        /// <summary>
        /// The configuration settings from the unity editor, in runtime filled into <see cref="configs"/> for more convenient access.
        /// </summary>
        [SerializeField]
        private ConfigEntry[] config;

        private Dictionary<string, ConfigEntry> configs = new Dictionary<string, ConfigEntry>();

        public void Awake()
        {
            for (int i = 0; i < this.config.Length; i++)
            {
                this.configs.Add(this.config[i].tag, this.config[i]);
            }

            UnitySingleton<DebrisManager>.Register(this);
        }

        /// <summary>
        /// Tries spawning debris for the specified gameobject at the specified position.
        /// Forward is being set to the spawned gameobject transform.
        /// 
        /// Usually position and forward is hitpoint / hitnormal.
        /// 
        /// The spawn might fail is there is no config set up for the go's tag.
        /// </summary>
        /// <param name="debrisSpawn">The debris object that will get spawned.</param>
        public bool TrySpawnDebris(GameObject go, Vector3 position, Vector3 forward, DebrisSpawn debrisSpawn)
        {
            ConfigEntry ce;
            if (this.configs.TryGetValue(go.tag, out ce))
            {
                // Grab list for prefabs to spawn
                List<GameObject> prefabsToSpawn = ListPool<GameObject>.Get();

                // Determine prefabs to spawn
                if ((debrisSpawn & DebrisSpawn.DebrisParticles) != 0 && ce.debris.Length > 0)
                    prefabsToSpawn.Add(ce.debris.RandomItem());
                if ((debrisSpawn & DebrisSpawn.BulletDecal) != 0 && ce.bulletDecal.Length > 0)
                    prefabsToSpawn.Add(ce.bulletDecal.RandomItem());

                // Spawn gameobjects
                for (int i = 0; i < prefabsToSpawn.Count; i++)
                {
                    var prefab = prefabsToSpawn[i];

                    // Spawn at hit position with some padding and attach to hit gameobject by parenting to it.
                    var g = PrefabPool.instance.GetInstance(prefab);
                    g.transform.position = position + (forward * 0.01f);
                    g.transform.rotation = Quaternion.LookRotation(forward);
                    g.transform.parent = go.transform;

                    // Apply prefab scale
                    var prefabScale = prefab.transform.lossyScale;
                    var realScale = g.transform.lossyScale;

                    g.transform.localScale = new Vector3(prefabScale.x / realScale.x, prefabScale.y / realScale.y, prefabScale.z / realScale.z);
                }

                // Return list to pool
                ListPool<GameObject>.Return(prefabsToSpawn);

                return true;
            }
            else
                return false;
        }
    }
}