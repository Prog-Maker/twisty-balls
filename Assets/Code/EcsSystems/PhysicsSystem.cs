using System;
using Code.EcsComponents;
using Code.Extensions;
using Code.MonoBehaviors;
using Code.SO;
using Kk.LeoQuery;
using UnityEngine;

namespace Code.EcsSystems
{
    public class PhysicsSystem : ISystem
    {
        [Inject]
        public Config config;

        private static Collider2D[] _results = new Collider2D[1024];

        public void Act(IEntityStorage storage)
        {
            int stepCount = config.movementStepCount;
            float dt = Time.deltaTime / stepCount;
            for (int i = 0; i < stepCount; i++)
            {
                int count = 0;
                Vector2 center = Vector2.zero;
                float totalMass = 0;
                foreach (var entity in storage.Query<BallBody>())
                {
                    ref BallBody body = ref entity.Get1();
                    totalMass += body.mass;
                    center += body.position;
                    count++;
                }

                center /= count;

                foreach (var entity in storage.Query<BallBody>())
                {
                    // float a = config.gravity * (totalMass - entity.Get2().mass) / (entity.Get<BallBody>().position - center).sqrMagnitude;
                    float a = config.gravity;
                    ref BallBody body = ref entity.Get1();
                    body.velocity -= body.position.normalized * (a * dt);
                }

                foreach (var entity in storage.Query<BallBody>())
                {
                    ref BallBody body = ref entity.Get1();
                    body.position += body.velocity * dt;
                }

                foreach (var entity in storage.Query<BallBody>())
                {
                    if (!entity.Has1())
                    {
                        // because this loop can delete these components by reference from this loop
                        continue;
                    }
                    
                    ref BallBody body = ref entity.Get1();

                    if (body.collisionApplied)
                    {
                        continue;
                    }

                    int collisionCount = Physics2D.OverlapCircleNonAlloc(body.position, body.CalcBallDiameter(config) / 2, _results);
                    if (collisionCount >= _results.Length)
                    {
                        Debug.LogWarning($"too many collisions: {collisionCount}");
                    }
                    for (int j = 0; j < collisionCount; j++)
                    {
                        if (!_results[j].TryGetComponent(out EntityLink link)) continue;
                        if (!link.entity.TryGet(out Entity<BallBody> another)) continue;
                        if (another == entity || another.Has<BallDestroyAction>()) continue;
                        ref BallBody anotherBody = ref another.Get1();
                        if (anotherBody.collisionApplied) continue;
                        if (DoDistanceGrow(body, anotherBody, dt)) continue;
                        if (anotherBody.config == body.config)
                        {
                            Merge(entity, another);
                        }
                        else
                        {
                            Bounce(ref body, ref anotherBody);
                        }

                        body.collisionApplied = true;
                        break;
                    }
                }

                foreach (Entity<BallBody> another in storage.Query<BallBody>())
                {
                    another.Get1().collisionApplied = false;
                }
            }
        }

        private static void Merge(Entity<BallBody> b1, Entity<BallBody> b2)
        {
            ref BallBody body1 = ref b1.Get<BallBody>();
            ref BallBody body2 = ref b2.Get<BallBody>();
            
            Vector2 myImpulse = body1.velocity * body1.mass;
            Vector2 anotherImpulse = body2.velocity * body2.mass;

            float totalMass = body1.mass + body2.mass;

            body1.position = Vector2.Lerp(
                body1.position,
                body2.position,
                Mathf.InverseLerp(
                    b1.Get1().mass,
                    b2.Get1().mass,
                    totalMass / 2
                )
            );
            body1.velocity = (myImpulse + anotherImpulse) / totalMass;
            body1.mass = body1.mass + body2.mass;
            b1.Get<PushToScene>().requestCount++;

            b2.Add<BallDestroyAction>();
            b2.Del<BallBody>();
            b2.Del<BallBody>();
        }

        private void Bounce(ref BallBody body1, ref BallBody body2)
        {
            body1.velocity = ElasticImpactSpeed(
                body1.position, body1.velocity, body1.mass,
                body2.position, body2.velocity, body2.mass
            );
            body2.velocity = ElasticImpactSpeed(
                body2.position, body2.velocity, body2.mass,
                body1.position, body1.velocity, body1.mass
            );
        }

        private static bool DoDistanceGrow(in BallBody b1, in BallBody b2, float dt)
        {
            Vector2 p1 = b1.position;
            Vector2 p2 = b2.position;
            Vector2 v1 = b1.velocity;
            Vector2 v2 = b2.velocity;
            
            return (p1 - p2).sqrMagnitude > (p1 - v1 * dt - (p2 - v2 * dt)).sqrMagnitude;
        }

        // private Vector2 ElasticImpactSpeed(Vector3 v1, float m1, Vector3 v2, float m2)
        // {
        //     return ((m1 - m2) * v1 + 2 * m2 * v2) / (m1 + m2);
        // }

        private Vector2 ElasticImpactSpeed(
            Vector2 x1,
            Vector2 v1,
            float m1,
            Vector2 x2,
            Vector2 v2,
            float m2
        ) => v1 - ((2 * m2) / (m1 + m2)) * (Vector2.Dot(v1 - v2, x1 - x2) / (x1 - x2).sqrMagnitude) * (x1 - x2);
    }
}