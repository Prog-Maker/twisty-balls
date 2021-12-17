using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.Oop;
using Code.Phases;
using Code.SO;
using Kk.BusyEcs;
using UnityEngine;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class HUDSystem
    {
        private readonly Lazy<GUIStyle> _style = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 32
        });

        private const int BufferSizeSeconds = 3;
        private readonly AvgBuffer _fpsBuffer = new AvgBuffer(bufferSizeSeconds: BufferSizeSeconds, cacheTtl: 1);

        [Inject]
        public IEnv env;

        [Inject]
        public Config config;

        [OnGUI]
        public void Act()
        {
            int ballCount = 0;
            float speedSum = 0;
            float massSum = 0;
            float diamMin = float.MaxValue;
            float diamMax = 0;
            float diamSum = 0;
            float speedMin = float.MaxValue;
            float speedMax = 0;
            env.Query((ref BallType _, ref Velocity velocity, ref Mass mass) =>
            {
                ballCount++;
                float speed = velocity.velocity.magnitude;
                speedMin = Mathf.Min(speedMin, speed);
                speedMax = Mathf.Max(speedMax, speed);
                speedSum += speed;
                massSum += mass.mass;
                float diam = mass.CalcBallDiameter(config);
                diamMin = Mathf.Min(diamMin, diam);
                diamMax = Mathf.Max(diamMax, diam);
                diamSum += diam;
            });

            float deltaTime = Time.deltaTime;

            _fpsBuffer.Add(1f / deltaTime, deltaTime);

            GUILayout.Label($"Version: BusyECS", _style.Value);
            GUILayout.Label($"FPS: {_fpsBuffer.GetAvg()} (smoothing: {BufferSizeSeconds}s)", _style.Value);
            GUILayout.Label($"Balls:            {ballCount}", _style.Value);
            GUILayout.Label($"Frames:           {Time.frameCount}", _style.Value);
            GUILayout.Label($"Bounces:          {Stats.Instance.bounces}", _style.Value);
            GUILayout.Label($"Explosions:       {Stats.Instance.explosions}", _style.Value);
            GUILayout.Label($"Merges:           {Stats.Instance.merges}", _style.Value);
            GUILayout.Label($"D min:           {diamMin}", _style.Value);
            GUILayout.Label($"D avg:           {diamSum / ballCount}", _style.Value);
            GUILayout.Label($"D max:           {diamMax}", _style.Value);
            GUILayout.Label($"V min:           {speedMin}", _style.Value);
            GUILayout.Label($"V avg:           {speedSum / ballCount}", _style.Value);
            GUILayout.Label($"V max:           {speedMax}", _style.Value);
        }
    }
}