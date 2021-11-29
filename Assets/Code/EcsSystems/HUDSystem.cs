using System;
using Code.EcsComponents;
using Code.Oop;
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

        private const int BufferSizeSeconds = 3;
        private readonly FpsBuffer _fpsBuffer = new FpsBuffer(bufferSizeSeconds: BufferSizeSeconds, cacheTtl: 1);

        public void Act(IEntityStorage storage)
        {
            int ballCount = 0;
            foreach (Entity<BallType> _ in storage.Query<BallType>())
            {
                ballCount++;
            }
            
            _fpsBuffer.AddDeltaTime(Time.deltaTime);

            GUILayout.Label($"Version: Full Sugar", _style.Value);
            GUILayout.Label($"FPS: {_fpsBuffer.GetFps()} (smoothing: {BufferSizeSeconds}s)", _style.Value);
            GUILayout.Label($"Balls:            {ballCount}", _style.Value);
            GUILayout.Label($"Frames:           {Time.frameCount}", _style.Value);
        }
    }
}