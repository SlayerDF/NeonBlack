using System.Collections.Generic;
using NeonBlack.Systems;
using NeonBlack.Utilities.ObjectPool;
using UnityEngine;

namespace NeonBlack.Utilities
{
    public abstract class PoolObject : MonoBehaviour
    {
    }

    public class SceneObjectPool : SceneSingleton<SceneObjectPool>
    {
        private const int MaxCapacity = 20;

        private readonly Dictionary<int, int> gameObjectPrefabs = new();
        private readonly Dictionary<int, ObjectPool<PoolObject>> prefabPools = new();

        private int capacity;

        #region Event Functions

        protected override void OnDestroy()
        {
            foreach (var pool in prefabPools.Values)
            {
                foreach (var obj in pool)
                {
                    if (obj)
                    {
                        Destroy(obj.gameObject);
                    }
                }

                pool.Dispose();
            }

            base.OnDestroy();
        }

        #endregion


        /// <summary>
        /// Instantiate a new GameObject or retrieve it from the pool
        /// </summary>
        /// <param name="prefab">Prefab object to spawn</param>
        /// <param name="obj">Spawned object</param>
        /// <param name="spawnDeactivated">Don't auto-activate the object</param>
        /// <returns>Whether a new GameObject was instantiated</returns>
        public static bool Spawn<T>(PoolObject prefab, out T obj, bool spawnDeactivated = false) where T : PoolObject
        {
            return Instance.SpawnInternal(prefab, out obj, spawnDeactivated);
        }

        /// <summary>
        /// Release the GameObject to the pool or destroy it
        /// </summary>
        /// <param name="obj">GameObject to despawn</param>
        public static void Despawn(PoolObject obj)
        {
            if (Instance)
            {
                Instance.DespawnInternal(obj);
            }
        }

        private bool SpawnInternal<T>(PoolObject prefab, out T obj, bool spawnDeactivated = false) where T : PoolObject
        {
            var prefabId = prefab.GetInstanceID();

            if (!prefabPools.TryGetValue(prefabId, out var pool))
            {
                pool = new ObjectPool<PoolObject>(MaxCapacity);
                prefabPools.Add(prefabId, pool);
            }

            var spawned = false;
            var result = pool.Get(out var gotObject);
            obj = (T)gotObject;

            if (result == GetResult.Missing)
            {
                obj = (T)Instantiate(prefab);

                gameObjectPrefabs.Add(obj.GetInstanceID(), prefabId);

                spawned = true;

# if UNITY_EDITOR
                if (obj.gameObject.scene != gameObject.scene)
                {
                    Debug.LogWarning("ObjectPoolManager spawns objects in the wrong scene");
                }
#endif
            }

            obj.gameObject.SetActive(!spawnDeactivated);

            return spawned;
        }

        private void DespawnInternal(PoolObject obj)
        {
            var gameObjectId = obj.GetInstanceID();
            var prefabId = gameObjectPrefabs[gameObjectId];
            var pool = prefabPools[prefabId];

            obj.gameObject.SetActive(false);

            if (pool.Return(obj) == ReturnResult.Discarded)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}
