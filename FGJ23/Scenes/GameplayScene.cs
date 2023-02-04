﻿using FGJ23.Components;
using FGJ23.Core;
using FGJ23.Entities;
using FGJ23.Entities.CoordinateEvents;
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
        public static FGJ23.Levels.Proto.Level NextProtoLevel { get; set; }

        private GameplayScene() { }

        public static GameplayScene construct(FGJ23.Levels.Proto.Level lev)
        {
            NextProtoLevel = lev;
            return new GameplayScene();
        }

        public override void Initialize()
        {
            base.Initialize();
            ColliderSystem.Reset();

            Log.Information("Initializing new GameplayScene with l={A}", NextProtoLevel);
            // setup a pixel perfect screen that fits our map
            int x = 800;
            int y = 600;
            SetDesignResolution(x, y, SceneResolutionPolicy.ShowAllPixelPerfect);
            if (OperatingSystem.IsAndroid())
            {
                Nez.Screen.IsFullscreen = true;
                Nez.Screen.SetSize(
                        Screen.MonitorWidth,
                        Screen.MonitorHeight
                );
                Nez.Screen.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            }
            else
            {
                Screen.SetSize(x * 2, y * 2);
            }

            CreateEntity("forceMovementSystem")
                .AddComponent(new ForceMovementSystem());

            var realMapEntity = CreateEntity("real-map-entity");
            var levelTuple = LevelLoader.LoadLevel(NextProtoLevel, file => "Content/Files/" + file);
            this.level = levelTuple.Item1;

            foreach (var layer in level.Layers)
            {
                foreach (var e in layer.CoordinateEvents)
                {
                    var eventComponent = Event.CoordinateEventFromProto(e);
                    if (eventComponent != null)
                    {
                        var eventEntity = CreateEntity("event");
                        eventEntity.AddComponent(eventComponent);
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
            playerInstance = playerEntity.AddComponent(new Player(20, 24));
            var playerWidth = playerInstance.Width;
            var playerHeight = playerInstance.Height;
            playerEntity.AddComponent(new BoxCollider(-playerWidth / 2, -playerHeight / 2, playerWidth, playerHeight));
            Log.Information("PlayerSize {A} {B}", playerWidth, playerHeight);
            playerEntity.AddComponent(new Levels.Mover(this.level.SpriteLayer));

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
