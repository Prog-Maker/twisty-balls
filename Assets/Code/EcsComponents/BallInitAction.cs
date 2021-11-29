using System;
using Code.SO;
using UnityEngine;

namespace Code.EcsComponents
{
    [Serializable]
    public struct BallInitAction
    {
        public string name;
        public Vector2 position;
        public Vector2 direction;
        public float speed;
        public float mass;
        public BallTypeConfig config;

        public BallInitAction(string name, Vector2 position, Vector2 direction, float speed, float mass, BallTypeConfig config)
        {
            this.name = name;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.mass = mass;
            this.config = config;
        }
    }
}