using System;
using Code.EcsComponents;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class HUDSystem : ISystem
    {
        private readonly Lazy<GUIStyle> _style = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 32
        });

        public void Act(IEntityStorage storage)
        {
            int ballCount = 0;
            foreach (Entity<BallType> _ in storage.Query<BallType>())
            {
                ballCount++;
            }

            GUILayout.Label($"Balls:            {ballCount}", _style.Value);
            GUILayout.Label($"Collisions:       {(storage.TrySingle(out Entity<CollisionStats> stats) ? (object)stats.Get1().collisions : "unknown")}", _style.Value);
            GUILayout.Label($"Frames:           {Time.frameCount}", _style.Value);
            GUILayout.Label($"Collisions/frame: {(storage.TrySingle(out Entity<CollisionStats> stats1) ? (object)(1f * stats1.Get1().collisions / Time.frameCount) : "unknown")}", _style.Value);
        }
    }
}