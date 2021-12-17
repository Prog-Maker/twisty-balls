using System;
using UnityEngine;

namespace Code.Dtos
{
    [Serializable]
    public struct InitialSpawn
    {
        public float maxCenterDistance;
        public int ballNumber;
        public float ballSpeed;
        public float ballMass;
        public AnimationCurve ballMassDistribution;
        public int typesCount;
    }
}