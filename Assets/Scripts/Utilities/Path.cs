using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NeonBlack.Utilities
{
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

#if UNITY_EDITOR
            Debug.Assert(children != null, nameof(children) + " != null");
            Debug.Assert(children.Length > 1, nameof(children) + " is not empty or a single waypoint path");
#endif
        }

        public Waypoint NextWaypoint(Waypoint currentWaypoint)
        {
            if (children == null)
            {
                InitializeChildren();
            }

            int nextIndex;

            if (circular)
            {
                nextIndex = (currentWaypoint.Index + 1) % children!.Length;

                return new Waypoint(children![nextIndex].position, false, nextIndex);
            }

            // Non-circular logic below
            bool moveBackwards;
            if (currentWaypoint.Index == 0)
            {
                moveBackwards = false;
                nextIndex = 1;
            }
            else if (currentWaypoint.Index == children!.Length - 1)
            {
                moveBackwards = true;
                nextIndex = children.Length - 2;
            }
            else
            {
                moveBackwards = currentWaypoint.MoveBackwards;
                nextIndex = moveBackwards ? currentWaypoint.Index - 1 : currentWaypoint.Index + 1;
            }

            return new Waypoint(children![nextIndex].position, moveBackwards, nextIndex);
        }

        public Waypoint InitialWaypoint(Vector3 position)
        {
            if (children == null)
            {
                InitializeChildren();
            }

            var sortedChildren = children!
                .Select((child, index) => (index, child.position))
                .OrderBy(child => (child.position - position).sqrMagnitude)
                .ToList();

            var closestChild = sortedChildren.First();

            if (circular)
            {
                return new Waypoint(closestChild.position, false, closestChild.index);
            }

            // Non-circular logic below
            bool moveBackwards;
            var secondClosestChild = sortedChildren.Skip(1).First();
            if (closestChild.index == 0)
            {
                moveBackwards = false;
            }
            else if (closestChild.index == children.Length - 1)
            {
                moveBackwards = true;
            }
            else
            {
                moveBackwards = closestChild.index > secondClosestChild.index;
            }

            return new Waypoint(closestChild.position, moveBackwards, closestChild.index);
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

        [Header("Debug")]
        [SerializeField]
        private bool alwaysShowGizmos;

        [ContextMenu("Reinitialize children")]
        private void ReinitializeChildren()
        {
            InitializeChildren();
        }

        private void OnDrawGizmos()
        {
            if (!alwaysShowGizmos && Selection.activeObject != gameObject)
            {
                return;
            }

            if (children == null)
            {
                InitializeChildren();
            }

            Debug.Assert(children != null, nameof(children) + " != null");

            for (var i = 0; i < children.Length; i++)
            {
                Handles.DrawWireDisc(children[i].position, Vector3.up, 0.5f);

                if (i < children.Length - 1)
                {
                    Handles.DrawLine(children[i].position, children[i + 1].position);
                }
                else if (i == children.Length - 1 && circular)
                {
                    Handles.DrawLine(children[i].position, children[0].position);
                }
            }
        }
#endif
    }
}
