using FGJ23.Components;
using FGJ23.Core;
using FGJ23.Entities;
using FGJ23.Entities.CoordinateEvents;
using FGJ23.Entities.Enemies;
using FGJ23.Levels;
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

        public Level level { get; set; }

        public GameplayScene(Level level)
        {
            this.level = level;
        }

        public override void Initialize()
        {
            base.Initialize();
            ColliderSystem.Reset();

            Log.Information("Initializing new GameplayScene with l={A}", this.level);
            // setup a pixel perfect screen that fits our map
            SetDesignResolution(400, 300, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(400 * 2, 300 * 2);

            CreateEntity("forceMovementSystem")
                .AddComponent(new ForceMovementSystem());

            var realMapEntity = CreateEntity("real-map-entity");
            if (this.level == null)
            {
                var levelTuple = LevelLoader.LoadLevel(file => "Content/Files/" + file);
                this.level = levelTuple.Item1;
            }

            foreach (var layer in level.Layers)
            {
                foreach (var e in layer.CoordinateEvents)
                {
                    var eventComponent = Event.CoordinateEventFromProto(e);
                    if (eventComponent != null)
                    {
                        var eventEntity = CreateEntity("event");
                        eventEntity.AddComponent(eventComponent);
                        Log.Information("{@A}", eventComponent);
                    }
                }
            }

            foreach (var layer in level.Layers)
            {
                foreach (var e in layer.AreaEvents)
                {
                    var eventComponent = Event.AreaEventFromProto(e);
                    if (eventComponent != null)
                    {
                        var eventEntity = CreateEntity("event");
                        eventEntity.AddComponent(eventComponent);
                        Log.Information("{@A}", eventComponent);
                    }
                }
            }
            realMapEntity.AddComponent(this.level);

            var topLeft = new Vector2(0, 0);
            var bottomRight = new Vector2(this.level.Width, this.level.Height);

            var bounds = new CameraBounds(topLeft, bottomRight);
            Log.Information("Camera bounds: {A}", bounds);
            realMapEntity.AddComponent(bounds);

            var spawns = FindComponentsOfType<Spawn>();
            Entity playerEntity;
            foreach (var spawn in spawns)
            {
                Log.Information("Spawn: {A} {B}", spawn, spawn.Entity.Transform.Position);
            }
            if (spawns.Count == 0)
            {
                playerEntity = CreateEntity("player", new Vector2(100, 100));
            }
            else
            {
                var r = new System.Random();
                playerEntity = CreateEntity("player", spawns[r.Next(spawns.Count)].Transform.Position);
            }
            playerInstance = playerEntity.AddComponent(new Player());
            var playerWidth = 20;
            var playerHeight = 24;
            playerEntity.AddComponent(new BoxCollider(-playerWidth / 2, -playerHeight / 2, playerWidth, playerHeight));
            playerEntity.AddComponent(new Levels.Mover(this.level.SpriteLayer));
            CreateEntity("blob", new Vector2(1000, 1000)).AddComponent(new FloatingBlob());

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
