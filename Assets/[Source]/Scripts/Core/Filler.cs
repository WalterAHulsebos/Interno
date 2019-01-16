using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Pathfinding;
using Sirenix.OdinInspector;

namespace Core
{
    public class Filler : MonoBehaviour, IMoveable<Node>
    {
        [InlineEditor]
        [SerializeField] private MoveSet moveSet;
        
        public MoveSet MoveSet { get { return moveSet; } }

        public Node Node { get; protected set; }
        public bool HorizontalMovement { get; private set; }
        public bool VerticalMovement { get; private set; }

        protected virtual void Awake()
        {
            NotifyExistence();
        }

        public virtual bool Walkable(Node node)
        {
            return false;
        }

        protected virtual void NotifyExistence()
        {
            GameManager.instance.filler.Add(this);
        }

        protected virtual void NotifyDestruction()
        {
            GameManager.instance.filler.Remove(this);
        }
    }
}