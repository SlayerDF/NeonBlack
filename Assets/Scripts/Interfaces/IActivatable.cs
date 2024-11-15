using System;

namespace NeonBlack.Interfaces
{
    public interface IActivatable
    {
        bool IsActivated { get; }
        event Action Activated;
    }
}
