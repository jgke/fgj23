using FGJ23.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Nez;
using FGJ23.Support;

namespace FGJ23
{
    public class Game1 : Nez.Core
    {
        public static int TotalFrames = 0;
        private int _drawFrames = 0;
        private int _physicsFrames = 0;
        Stopwatch _elapsedTime = Stopwatch.StartNew();

        public Game1()
        {
            float physicsFps = 70;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1 / physicsFps);
            this.IsFixedTimeStep = true;
            Nez.Core.PauseOnFocusLost = false;
        }

        protected override void Initialize()
        {
            Log.Information("Loading FMOD libraries");
            new FmodWrapper().Init();

            base.Initialize();

            DebugRenderEnabled = false;

            Window.AllowUserResizing = true;
            Scene = new MenuScene();
            Nez.Input.Touch.EnableTouchSupport();
        }

        protected override void FixedUpdate(GameTime gameTime)
        {
            base.FixedUpdate(gameTime);
            FmodWrapper.FixedUpdate(gameTime);
            _physicsFrames += 1;
        }

        protected override void DrawUpdate(GameTime gameTime)
        {
            base.DrawUpdate(gameTime);
            _drawFrames += 1;
            TotalFrames += 1;

            if (_elapsedTime.ElapsedMilliseconds >= 1000)
            {
                Log.Information("draw fps: {0}, physics fps: {1}, target: {2}, Running slowly: {3}", _drawFrames, _physicsFrames, (int)(1000 / this.TargetElapsedTime.TotalMilliseconds), gameTime.IsRunningSlowly);
                _drawFrames = 0;
                _physicsFrames = 0;
                _elapsedTime.Restart();
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            FmodWrapper.Unload();
        }
    }
}
