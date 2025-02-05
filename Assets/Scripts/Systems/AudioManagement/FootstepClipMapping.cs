using System;
using UnityEngine;

namespace NeonBlack.Systems.AudioManagement
{
    [Serializable]
    public class FootstepClipMapping
    {
        #region Serialized Fields

        [SerializeField]
        private TerrainLayer terrainLayer;

        [SerializeField]
        private AudioClip clip;

        #endregion

        public TerrainLayer TerrainLayer => terrainLayer;
        public AudioClip Clip => clip;
    }
}
