using Code.Dtos;
using Code.EcsComponents;
using UnityEngine;

namespace Code.SO
{
    public class Config : ScriptableObject
    {
        public InitialSpawn initialSpawn;
        public GameObject ballPrefab;
        public GameObject ballPrefabNoCollider;
        public BallTypeConfig[] ballTypes;
        public float stepFactor;
        public int movementStepCount;
        public float gravity;
        public float criticalMass;
        public float criticalExplosionSpeed;
        public float cameraSize;
        public float radiusMultiplier;
        public float visualRadiusMultiplier;
        public CollisionStrategy collisionStrategy;

        public enum CollisionStrategy
        {
            Unity2D,
            CustomRegularGrid
        }
    }
}