using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.Oop;
using Code.SO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Code.Systems
{
    public class HUDSystem : IEcsRunSystem
    {
        private readonly Lazy<GUIStyle> _style = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 32
        });

        private const int BufferSizeSeconds = 3;
        private readonly AvgBuffer _fpsBuffer = new AvgBuffer(bufferSizeSeconds: BufferSizeSeconds, cacheTtl: 1);
        private readonly AvgBuffer _speedBuffer = new AvgBuffer(bufferSizeSeconds: BufferSizeSeconds, cacheTtl: 1);

        [EcsFilter(typeof(BallType))] private EcsFilter _ballTypes;
        [EcsPool] private EcsPool<Velocity> _velocity;
        [EcsPool] private EcsPool<Mass> _mass;
        [EcsShared] private Config _config;

        public void Run(EcsSystems systems)
        {
            int ballCount = 0;
            float speedSum = 0;
            float massSum = 0;
            float diamMin = float.MaxValue;
            float diamMax = 0;
            float diamSum = 0;
            float speedMin = float.MaxValue;
            float speedMax = 0;
            foreach (int entity in _ballTypes)
            {
                ballCount++;
                float speed = _velocity.Get(entity).velocity.magnitude;
                speedMin = Mathf.Min(speedMin, speed);
                speedMax = Mathf.Max(speedMax, speed);
                speedSum += speed;
                massSum += _mass.Get(entity).mass;
                float diam = _mass.Get(entity).CalcBallDiameter(_config);
                diamMin = Mathf.Min(diamMin, diam);
                diamMax = Mathf.Max(diamMax, diam);
                diamSum += diam;
            }

            float deltaTime = Time.deltaTime;

            _fpsBuffer.Add(1f / deltaTime, deltaTime);

            GUILayout.Label($"Version: Pure Lite", _style.Value);
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