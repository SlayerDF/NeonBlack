using JetBrains.Annotations;
using UnityEngine;

namespace NeonBlack.Interactables
{
    public class TerrainController : MonoBehaviour
    {
        private Terrain terrain;

        #region Event Functions

        private void Awake()
        {
            terrain = GetComponent<Terrain>();
        }

        #endregion

        [CanBeNull]
        public TerrainLayer LayerAt(Vector3 position)
        {
            var terrainPosition = position - terrain.transform.position;
            var terrainData = terrain.terrainData;

            var mapPosition = new Vector3(terrainPosition.x / terrainData.size.x, 0,
                terrainPosition.z / terrainData.size.z);

            if (mapPosition.x < 0 || mapPosition.x > 1 || mapPosition.z < 0 || mapPosition.z > 1)
            {
                return null;
            }

            var xCoord = mapPosition.x * terrainData.alphamapWidth;
            var zCoord = mapPosition.z * terrainData.alphamapHeight;
            var posX = (int)xCoord;
            var posZ = (int)zCoord;

            var mapData = terrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);
            var layersCollection = new float[mapData.GetUpperBound(2) + 1];

            for (var i = 0; i < layersCollection.Length; i++)
            {
                layersCollection[i] = mapData[0, 0, i];
            }

            var highest = 0f;
            var maxIndex = 0;
            for (var i = 0; i < layersCollection.Length; i++)
            {
                if (!(layersCollection[i] > highest))
                {
                    continue;
                }

                maxIndex = i;
                highest = layersCollection[i];
            }

            return terrain.terrainData.terrainLayers[maxIndex];
        }
    }
}
