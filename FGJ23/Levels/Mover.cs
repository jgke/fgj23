using FGJ23.Systems;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Levels
{

    public class CollisionState
    {
        public bool Right, Left, Above, Below;
        public bool BecameGroundedThisFrame;
        public bool WasGroundedLastFrame;
        public bool IsGroundedOnOneWayPlatform;
        public float SlopeAngle;

        internal SubpixelFloat _movementRemainderX, _movementRemainderY;

        public bool HasCollision => Below || Right || Left || Above;
        public void Reset(ref Vector2 motion)
        {
            if (motion.X == 0)
                Right = Left = false;

            if (motion.Y == 0)
                Above = Below = false;

            BecameGroundedThisFrame = IsGroundedOnOneWayPlatform = false;
            SlopeAngle = 0f;

            // deal with subpixel movement, storing off any non-integar remainder for the next frame
            _movementRemainderX.FixedUpdate(ref motion.X);
            _movementRemainderY.FixedUpdate(ref motion.Y);
        }

        public override string ToString()
        {
            return string.Format(
                "[CollisionState] r: {0}, l: {1}, a: {2}, b: {3}, angle: {4}, wasGroundedLastFrame: {5}, becameGroundedThisFrame: {6}",
                Right, Left, Above, Below, SlopeAngle, WasGroundedLastFrame, BecameGroundedThisFrame);
        }
    }

    public class Mover : Component
    {
        private Layer _layer;
        private Rectangle _boxColliderBounds;

        public Mover(Layer layer)
        {
            this._layer = layer;
        }

        private static Rectangle MovementRect(Rectangle body, int motionX, int motionY)
        {
            int minx = Math.Min(body.X, body.X + motionX);
            int miny = Math.Min(body.Y, body.Y + motionY);
            int maxx = Math.Max(body.X + body.Width, body.X + body.Width + motionX);
            int maxy = Math.Max(body.Y + body.Height, body.Y + body.Height + motionY);
            return new Rectangle(minx, miny, maxx - minx, maxy - miny);
        }

        public void Move(Vector2 motion, BoxCollider boxCollider, CollisionState collisionState)
        {
            // test for collisions then move the Entity
            TestCollisions(ref motion, boxCollider.Bounds, collisionState);

            ColliderSystem.ForceMove(boxCollider, motion);
        }

        public void TestCollisions(ref Vector2 motion, Rectangle boxColliderBounds, CollisionState collisionState)
        {
            _boxColliderBounds = boxColliderBounds;

            // save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
            collisionState.WasGroundedLastFrame = collisionState.Below;

            // reset our collisions state
            collisionState.Reset(ref motion);

            // reset rounded motion for us while dealing with subpixel movement so fetch the rounded values to use for our actual detection
            var motionX = (int)motion.X;
            var motionY = (int)motion.Y;
            if (motionY == 0 && motion.Y >= 0 && collisionState.WasGroundedLastFrame)
            {
                motionY = 5;
                motion.Y = 0;
            }

            if (motionX != 0)
            {
                motion.X = HandleHorizontalCollisions(boxColliderBounds, collisionState, motionX);
                boxColliderBounds.X += (int)motion.X;
            }

            motion.Y = HandleVerticalCollisions(boxColliderBounds, collisionState, motionY);

            if (!collisionState.WasGroundedLastFrame && collisionState.Below)
                collisionState.BecameGroundedThisFrame = true;
        }

        private int HandleHorizontalCollisions(Rectangle boxColliderBounds, CollisionState collisionState, int motion)
        {
            var direction = motion >= 0 ? Edge.Right : Edge.Left;
            int? collision = _layer.GetCollision(boxColliderBounds, direction, motion, collisionState.WasGroundedLastFrame);

            collisionState.Left = false;
            collisionState.Right = false;
            if (collision != null)
            {
                collisionState.Left = direction == Edge.Left;
                collisionState.Right = direction == Edge.Right;
                collisionState._movementRemainderX.Reset();
                if (collisionState.Left)
                {
                    return collision.Value - boxColliderBounds.X + 1;
                }
                else
                {
                    return collision.Value - (boxColliderBounds.X + boxColliderBounds.Width) - 1;
                }
            }
            return motion;
        }
        private int HandleVerticalCollisions(Rectangle boxColliderBounds, CollisionState collisionState, int motion)
        {
            var direction = motion >= 0 ? Edge.Bottom : Edge.Top;
            int? collision = _layer.GetCollision(boxColliderBounds, direction, motion);

            collisionState.Above = false;
            collisionState.Below = false;
            if (collision != null)
            {
                collisionState.Below = direction == Edge.Bottom;
                collisionState.Above = direction == Edge.Top;
                collisionState._movementRemainderY.Reset();
                if (collisionState.Below)
                {
                    return collision.Value - (boxColliderBounds.Y + boxColliderBounds.Height);
                }
                else
                {
                    return collision.Value - boxColliderBounds.Y + 2;
                }
            }

            return motion;
        }
    }
}
