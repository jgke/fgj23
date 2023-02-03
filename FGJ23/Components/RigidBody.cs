using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System;

namespace FGJ23.Entities
{
    public class RigidBody : Component, IUpdatable
    {
        public float gravity = 1000;
        public float maxXSpeed;
        public float brakeMultiplier = 2;

        private float _groundAccel;
        private float _airAccel;

        public Vector2 velocity;
        public Vector2 lockedVelocity;
        public float velocityLockedFor;
        public float IgnoreCollisionsFor = 0;

        public RigidBody(float maxXSpeed, float groundAccel, float airAccel)
        {
            this.maxXSpeed = maxXSpeed;
            _groundAccel = groundAccel;
            _airAccel = airAccel;
        }

        void IUpdatable.FixedUpdate()
        {
            velocity.Y += gravity * Time.DeltaTime;

            if (velocityLockedFor > 0)
            {
                velocity = lockedVelocity;
                velocityLockedFor -= Time.DeltaTime;
                if (velocityLockedFor <= 0)
                {
                    velocityLockedFor = 0;
                    velocity /= 2;
                }
            }
        }
        void IUpdatable.DrawUpdate() { }

        public void AddXForce(float force)
        {
            if (Math.Abs(velocity.X) > maxXSpeed && Math.Sign(velocity.X) == Math.Sign(force))
            {

            }
            else
            {
                velocity.X += force * Time.DeltaTime;
                if (Math.Sign(velocity.X) == Math.Sign(force))
                {
                    velocity.X = Mathf.Clamp(velocity.X, -maxXSpeed, maxXSpeed);
                }
            }
        }

        private float AccelValue(Levels.CollisionState collisions)
        {
            if (collisions.Below)
            {
                return _groundAccel;
            }
            else
            {
                return _airAccel;
            }
        }

        public void Accel(float value)
        {
            if (velocity.X != 0 && Math.Sign(value) != Math.Sign(velocity.X))
            {
                AddXForce(brakeMultiplier * value);
            }
            else
            {
                AddXForce(value);
            }
        }

        public void AccelLeft(Levels.CollisionState collisions)
        {
            float accel = AccelValue(collisions);
            Accel(-accel);
        }

        public void AccelRight(Levels.CollisionState collisions)
        {
            float accel = AccelValue(collisions);
            Accel(accel);
        }

        public void SlowDown(Levels.CollisionState collisions)
        {
            float accel = AccelValue(collisions);
            float reduceVelocityBy = brakeMultiplier * accel * Time.DeltaTime;
            velocity.X = Math.Sign(velocity.X) * Math.Max(Math.Abs(velocity.X) - reduceVelocityBy, 0);
        }

        public void UpdateCollisions(Levels.CollisionState collisions)
        {
            if (velocity.X > 0 && collisions.Right && collisions.SlopeAngle == 0)
            {
                velocity.X = 0;
            }
            else if (velocity.X < 0 && collisions.Left && collisions.SlopeAngle == 0)
            {
                velocity.X = 0;
            }
            if (collisions.Below && velocity.Y > 0)
            {
                velocity.Y = 0;
            }
            if (collisions.Above && velocity.Y < 0)
            {
                velocity.Y = 0;
            }
        }

        public void ForceMove(Vector2 velocity, float lockTime)
        {
            lockedVelocity = velocity;
            velocityLockedFor = lockTime;
        }

        public void ForceMove(Vector2 velocity, float lockTime, float noclipTime)
        {
            lockedVelocity = velocity;
            velocityLockedFor = lockTime;
            IgnoreCollisionsFor = noclipTime;
        }
    }
}
