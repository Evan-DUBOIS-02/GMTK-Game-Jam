using UnityEngine;

namespace Player
{
    public class PlayerState
    {
        private Vector3 _position;
        public Vector3 Position{get{return _position;} set{_position=value;}}
    }
}