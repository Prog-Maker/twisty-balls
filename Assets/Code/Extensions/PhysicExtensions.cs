using Code.EcsComponents;
using Code.SO;
using UnityEngine;

namespace Code.Extensions
{
    public static class PhysicExtensions
    {
        public static float CalcBallDiameter(this Mass mass, Config config) {
            return Mathf.Pow(mass.mass, 1f / 3f) * config.Platform().radiusMultiplier;
        }
    }
}