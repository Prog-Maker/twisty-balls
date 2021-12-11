using System;
using Code.EcsComponents;
using Code.Oop;
using Code.Phases;
using Kk.BusyEcs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class UserControlsSystem
    {
        private readonly Lazy<GUIStyle> _style = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 32
        });

        private const int BufferSizeSeconds = 3;
        private readonly FpsBuffer _fpsBuffer = new FpsBuffer(bufferSizeSeconds: BufferSizeSeconds, cacheTtl: 1);
        
        [Inject]
        public IEnv env;
        
        [EarlyUpdate]
        public void Act()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                env.NewEntity(new RestartGameCommand());
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}