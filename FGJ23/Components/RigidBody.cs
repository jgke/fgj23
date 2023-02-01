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

        }
        void IUpdatable.DrawUpdate() { }
    }
}
