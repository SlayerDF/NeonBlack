using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class Path : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField]
    private bool circular;

    #endregion

    private Transform[] children;

    #region Event Functions

    private void Awake()
    {
        if (children == null)
        {
            InitializeChildren();
        }
    }

    #endregion

    private void InitializeChildren()
    {
        children = new Transform[transform.childCount];

        for (var i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }

    public Waypoint NextWaypoint(Waypoint? currentWaypoint = null)
    {
        if (children == null)
        {
            InitializeChildren();

#if UNITY_EDITOR
            Debug.Assert(children != null, nameof(children) + " != null");
#endif
        }

        var waypoint = currentWaypoint ?? new Waypoint(Vector3.zero, false, -1);

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

    #region Nested type: ${0}

    public readonly struct Waypoint
    {
        public Vector3 Position { get; }
        public bool MoveBackwards { get; }
        public int Index { get; }

        public Waypoint(Vector3 position, bool moveBackwards, int index)
        {
            Position = position;
            MoveBackwards = moveBackwards;
            Index = index;
        }
    }

    #endregion

#if UNITY_EDITOR

    [ContextMenu("Reinitialize children")]
    private void DoSomething()
    {
        InitializeChildren();
    }

    private void OnDrawGizmos()
    {
        if (children == null)
        {
            InitializeChildren();
        }

#if UNITY_EDITOR
        Debug.Assert(children != null, nameof(children) + " != null");
#endif

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
