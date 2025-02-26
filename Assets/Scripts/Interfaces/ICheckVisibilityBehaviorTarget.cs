using NeonBlack.Enums;
using UnityEngine;

namespace NeonBlack.Interfaces
{
    public interface ICheckVisibilityBehaviorTarget
    {
        Transform VisibilityChecker { get; }
        bool IsVisible { get; }
        Layer VisibilityLayer { get; }
    }
}
