using Code.SO;
using UnityEngine;

namespace Code.MonoBehaviors
{
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour
    {
        public Config config;

        private void Update()
        {
            if (!config)
            {
                return;
            }

            Camera c = GetComponent<Camera>();
            c.orthographicSize = config.cameraSize;
            c.farClipPlane = config.cameraSize * 1.2f;
            c.transform.localPosition = new Vector3(0,0, -config.cameraSize);
            
        }
    }
}