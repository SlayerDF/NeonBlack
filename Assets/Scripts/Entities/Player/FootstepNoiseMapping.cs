using System;
using UnityEngine;

namespace NeonBlack.Entities.Player
{
    [Serializable]
    public class FootstepNoiseMapping
    {
        #region Serialized Fields

        [SerializeField]
        private TerrainLayer terrainLayer;

        [SerializeField]
        [Range(0f, 1f)]
        private float noiseLevel;

        #endregion

        public TerrainLayer TerrainLayer => terrainLayer;
        public float NoiseLevel => noiseLevel;
    }
}
