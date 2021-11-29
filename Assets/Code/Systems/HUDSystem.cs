using System;
using Code.EcsComponents;
using Code.Oop;
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
        private readonly FpsBuffer _fpsBuffer = new FpsBuffer(bufferSizeSeconds: BufferSizeSeconds, cacheTtl: 1);

        [EcsFilter(typeof(BallType))] private EcsFilter _ballTypes;

        public void Run(EcsSystems systems)
        {
            int ballCount = 0;
            foreach (int _ in _ballTypes)
            {
                ballCount++;
            }
            
            _fpsBuffer.AddDeltaTime(Time.deltaTime);

            GUILayout.Label($"Version: Pure Lite", _style.Value);
            GUILayout.Label($"FPS: {_fpsBuffer.GetFps()} (smoothing: {BufferSizeSeconds}s)", _style.Value);
            GUILayout.Label($"Balls:            {ballCount}", _style.Value);
            GUILayout.Label($"Frames:           {Time.frameCount}", _style.Value);
        }
    }
}