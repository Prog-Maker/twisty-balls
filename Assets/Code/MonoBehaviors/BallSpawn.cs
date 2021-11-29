using Code.EcsComponents;
using Code.Extensions;
using Code.SO;
using UnityEngine;

namespace Code.MonoBehaviors
{
    [ExecuteInEditMode]
    public class BallSpawn : MonoBehaviour
    {
        public Vector2 direction;
        public float speed;
        public float mass;
        public BallTypeConfig type;
        public Config config;

        private void Update()
        {
            if (type)
            {
                GetComponent<Renderer>().material.color = type.color;
            }

            transform.localScale = Vector3.one * new Mass { mass = mass }.CalcBallDiameter(config);
        }
    }
}