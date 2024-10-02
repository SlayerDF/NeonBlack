using System.Collections.Generic;
using System.Linq;
using NeonBlack.Systems;
using UnityEngine;

namespace NeonBlack.Utilities
{
    public class PoolObject : MonoBehaviour
    {
    }

    public class ObjectPoolManager : SceneSingleton<ObjectPoolManager>
    {
        private const int MaxCapacity = 20;

        private readonly Dictionary<int, int> gameObjectPrefabs = new();
        private readonly Dictionary<int, Stack<PoolObject>> prefabPools = new();

        private int capacity;

        #region Event Functions

        protected override void OnDestroy()
        {
            foreach (var obj in prefabPools.Values.SelectMany(pool => pool))
            {
                if (obj)
                {
                    Destroy(obj.gameObject);
                }
            }

            base.OnDestroy();
        }

        #endregion


        /// <summary>
        /// Instantiate a new GameObject or retrieve it from the pool
        /// </summary>
        /// <param name="prefab">Prefab object to spawn</param>
        /// <param name="obj">Spawned object</param>
        /// <returns>Whether a new GameObject was instantiated</returns>
        public static bool Spawn<T>(PoolObject prefab, out T obj) where T : PoolObject
        {
            return Instance.SpawnInternal(prefab, out obj);
        }

        /// <summary>
        /// Release the GameObject to the pool or destroy it
        /// </summary>
        /// <param name="obj">GameObject to despawn</param>
        public static void Despawn(PoolObject obj)
        {
            Instance.DespawnInternal(obj);
        }

        private bool SpawnInternal<T>(PoolObject prefab, out T obj) where T : PoolObject
        {
            var prefabId = prefab.GetInstanceID();

            if (!prefabPools.TryGetValue(prefabId, out var pool))
            {
                pool = new Stack<PoolObject>();
                prefabPools.Add(prefabId, pool);
            }

            if (pool.Count > 0)
            {
                capacity -= 1;

                obj = (T)pool.Pop();
                obj.gameObject.SetActive(true);
                return false;
            }

            obj = (T)Instantiate(prefab);
            gameObjectPrefabs.Add(obj.GetInstanceID(), prefabId);
            return true;
        }

        private void DespawnInternal(PoolObject obj)
        {
            var gameObjectId = obj.GetInstanceID();
            var prefabId = gameObjectPrefabs[gameObjectId];
            var pool = prefabPools[prefabId];

            if (capacity < MaxCapacity)
            {
                capacity += 1;

                obj.gameObject.SetActive(false);
                pool.Push(obj);
            }
            else
            {
                gameObjectPrefabs.Remove(gameObjectId);
                Destroy(obj.gameObject);
            }
        }
    }
}
