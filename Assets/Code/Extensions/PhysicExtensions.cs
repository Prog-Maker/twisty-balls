using Code.EcsComponents;
using Code.SO;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.Extensions
{
    public static class PhysicExtensions
    {
        public static float CalcBallDiameter(this in BallBody mass, Config config) {
            return Mathf.Pow(mass.mass, 1f / 3f) * config.radiusMultiplier;
        }
    }
}