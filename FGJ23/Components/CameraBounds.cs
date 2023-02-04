using FGJ23.Support;
using Microsoft.Xna.Framework;
using Nez;
using System;

namespace FGJ23.Components
{
    public class CameraBounds : Component, IUpdatable, ILoggable
    {
        [Loggable]
        public Vector2 Min, Max;


        public CameraBounds()
        {
            // make sure we run last so the camera is already moved before we evaluate its position
            SetUpdateOrder(int.MaxValue);
        }


        public CameraBounds(Vector2 min, Vector2 max) : this()
        {
            Min = min;
            Max = max;
        }


        public override void OnAddedToEntity()
        {
            Entity.UpdateOrder = int.MaxValue;
        }


        void IUpdatable.FixedUpdate() { }
        void IUpdatable.DrawUpdate()
        {
            var cameraBounds = Entity.Scene.Camera.Bounds;

            if (Max.X - Min.X < cameraBounds.Width)
            {
                Entity.Scene.Camera.Position -= new Vector2(cameraBounds.Left, 0);
            }
            else
            {
                if (cameraBounds.Left < Min.X)
                {
                    Entity.Scene.Camera.Position += new Vector2(Min.X - cameraBounds.Left, 0);
                }

                if (cameraBounds.Right > Max.X)
                {
                    Entity.Scene.Camera.Position += new Vector2(Max.X - cameraBounds.Right, 0);
                }
            }

            if (Max.Y - Min.Y < cameraBounds.Height)
            {
                Entity.Scene.Camera.Position -= new Vector2(0, cameraBounds.Top);
            }
            else
            {
                if (cameraBounds.Top < Min.Y)
                {
                    Entity.Scene.Camera.Position += new Vector2(0, Min.Y - cameraBounds.Top);
                }

                if (cameraBounds.Bottom > Max.Y)
                {
                    Entity.Scene.Camera.Position += new Vector2(0, Max.Y - cameraBounds.Bottom);
                }
            }
        }
    }
}
