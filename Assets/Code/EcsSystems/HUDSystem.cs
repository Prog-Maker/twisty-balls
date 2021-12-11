using System;
using Code.EcsComponents;
using Code.Oop;
using Code.Phases;
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
        private readonly FpsBuffer _fpsBuffer = new FpsBuffer(bufferSizeSeconds: BufferSizeSeconds, cacheTtl: 1);

        [Inject]
        public IEnv env;

        [OnGUI]
        public void Act()
        {
            int ballCount = 0;
            env.Query((ref BallType _) => ballCount++);
            
            _fpsBuffer.AddDeltaTime(Time.deltaTime);

            GUILayout.Label($"Version: BusyECS", _style.Value);
            GUILayout.Label($"FPS: {_fpsBuffer.GetFps()} (smoothing: {BufferSizeSeconds}s)", _style.Value);
            GUILayout.Label($"Balls:            {ballCount}", _style.Value);
            GUILayout.Label($"Frames:           {Time.frameCount}", _style.Value);
        }
    }
}