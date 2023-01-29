using FGJ23.Components;
using FGJ23.Core;
using FGJ23.Entities;
using FGJ23.Support;
using FGJ23.Systems;
using Microsoft.Xna.Framework;
using Nez.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace FGJ23
{
    public class GameplayScene : SceneBase
    {
        static Player playerInstance;

        public override void Initialize()
        {
            base.Initialize();
            ColliderSystem.Reset();

            Log.Information("Initializing new GameplayScene");
            // setup a pixel perfect screen that fits our map
            SetDesignResolution(800, 600, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(800 * 2, 600 * 2);

            var realMapEntity = CreateEntity("real-map-entity");

            var topLeft = new Vector2(0, 0);
            //var bottomRight = new Vector2(this.level.Width, this.level.Height);
            var bottomRight = new Vector2(1000, 1000);

            var bounds = new CameraBounds(topLeft, bottomRight);
            Log.Information("Camera bounds: {A}", bounds);
            realMapEntity.AddComponent(bounds);

            var playerEntity = CreateEntity("player", new Vector2(32, 0));
            playerInstance = playerEntity.AddComponent(new Player());
            var playerWidth = 20;
            var playerHeight = 24;
            playerEntity.AddComponent(new BoxCollider(-playerWidth / 2, -playerHeight / 2, playerWidth, playerHeight));

            Camera.Entity.AddComponent(new FollowCamera(playerEntity)).FollowLerp = 1;
        }

#if !ANDROID
        [Command("warp", "Warp player to (x, y)")]
        static void WarpPlayer(int x = 0, int y = 0)
        {
            ColliderSystem.SetPosition(playerInstance.GetComponent<Collider>(), new Vector2(x, y));
        }
#endif
    }
}
