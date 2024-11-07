using UnityEngine;

namespace NeonBlack.Interfaces
{
    public interface IDistractible
    {
        void Distract(GameObject distractor, float maxTime);
    }
}
