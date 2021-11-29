using System;
using Code.SO;
using UnityEngine;

namespace Code.EcsComponents
{
    [Serializable]
    public struct BallBody
    {
        public BallTypeConfig config;
        public Vector2 position;
        public float mass;
        public Vector2 velocity;
        public bool collisionApplied;
    }
}