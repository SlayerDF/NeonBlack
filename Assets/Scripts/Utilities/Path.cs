using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    public readonly struct Waypoint
    {
        public Vector3 Position { get; }
        public bool MoveBackwards { get; }
        public int Index { get; }

        public Waypoint(Vector3 position, bool moveBackwards, int index)
        {
            Position = position;
            MoveBackwards = direction;
            Index = index;
        }
    }

    #region Serialized Fields

    [SerializeField]
    bool circular;

    #endregion

    #region Event Functions

    private void Awake()
    {
        InitializeChildren();
    }

    #endregion

    private Transform[] children;

    private void InitializeChildren()
    {
        children = new Transform[transform.childCount];

        for (var i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }

    public Waypoint NextWaypoint(Waypoint waypoint = null)
    {
        bool moveBackwards;
        int nextIndex;

        if (circular)
        {
            moveBackwards = false;
            nextIndex = waypoint.Index + 1;

            if (nextIndex >= children.Length)
            {
                nextIndex = 0;
            }
        }
        else
        {
            moveBackwards = waypoint.MoveBackwards;
            nextIndex = waypoint.MoveBackwards ? waypoint.Index - 1 : waypoint.Index + 1;

            if (nextIndex < 0)
            {
                moveBackwards = false;
                nextIndex = 1;
            }
            else if (nextIndex >= children.Length)
            {
                moveBackwards = true;
                nextIndex = children.Length - 2;
            }
        }

        nextIndex = Mathf.Clamp(nextIndex, 0, transform.childCount - 1);

        return new Waypoint(children[nextIndex].position, moveBackwards, nextIndex);
    }

#if UNITY_EDITOR

    [ContextMenu("Reinitialize children")]
    void DoSomething()
    {
        InitializeChildren();
    }

    private void OnDrawGizmos()
    {
        if (children == null) InitializeChildren();

        for (var i = 0; i < children.Length; i++)
        {
            Handles.DrawWireDisc(children[i].position, Vector3.up, 0.5f);

            if (i < children.Length - 1)
            {
                Handles.DrawLine(children[i].position, children[i + 1].position);

                if (circular)
                {
                    Handles.DrawLine(children[i + 1].position, children[0].position);
                }
            }
        }
    }
#endif
}
