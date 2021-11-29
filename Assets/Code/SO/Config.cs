using Code.Dtos;
using UnityEngine;

namespace Code.SO
{
    public class Config : ScriptableObject
    {
        public InitialSpawn initialSpawn;
        public GameObject ballPrefab;
        public BallTypeConfig[] ballTypes;
        public int movementStepCount;
        public float gravity;
        public float criticalMass;
        public float criticalExplosionSpeed;
        public float cameraSize;
        public float radiusMultiplier;
    }
}