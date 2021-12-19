using System;
using Code.Dtos;
using UnityEngine;

namespace Code.SO
{
    public class Config : ScriptableObject
    {
        public GameObject ballPrefab;
        public GameObject ballPrefabNoCollider;
        public BallTypeConfig[] ballTypes;
        public Preset pc;
        public Preset webgl;
        public bool editorIsWebgl;
        public float pushAwaySpeed;

        public Preset Platform()
        {
#if UNITY_EDITOR
            return editorIsWebgl ? webgl : pc;
#elif UNITY_WEBGL
            return webgl;
#else
            return pc;
#endif
        }

        public enum CollisionStrategy
        {
            Unity2D,
            CustomRegularGrid
        }

        [Serializable]
        public struct Preset
        {
            public InitialSpawn initialSpawn;
            public float stepFactor;
            public int movementStepCount;
            public float gravity;
            public float criticalMass;
            public float criticalExplosionSpeed;
            public float cameraSize;
            public float radiusMultiplier;
            public float visualRadiusMultiplier;
            public CollisionStrategy collisionStrategy;
        }
    }
}