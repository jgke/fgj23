using Google.Protobuf;
using FGJ23.Components;
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
using Microsoft.Xna.Framework.Input;

namespace FGJ23
{
    public class Level13Component : Component, IUpdatable {
        int ticks = 70 * 5;
        void IUpdatable.DrawUpdate() {}
        void IUpdatable.FixedUpdate() {
            ticks -= 1;
            if(ticks < 0 && !GameState.Instance.Transitioning) {
                GameplayScene.playerInstance.PreventActions = false;
                var end = Entity.Scene.CreateEntity("lev13hack", new Vector2(0, 0));
                end.AddComponent(Event.AreaEventFromProto(
                            new FGJ23.Levels.Proto.AreaEvent()
                            {
                            Id = FGJ23.Levels.Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                            X = 50,
                            Y = 65,
                            Width = 32,
                            Height = 32,
                            Data = ByteString.CopyFromUtf8("LN_voittolinna_BW.png;level14")
                            }
                            ));
                Entity.Destroy();
            }
        }
    }

    public class Level14Component : Component, IUpdatable {
        int ticks = 70 * 10;
        void IUpdatable.DrawUpdate() {}
        void IUpdatable.FixedUpdate() {
            ticks -= 1;
            if(ticks < 0 && !GameState.Instance.Transitioning) {
                GameplayScene.playerInstance.PreventActions = false;
                var end = Entity.Scene.CreateEntity("lev13hack", new Vector2(0, 0));
                end.AddComponent(Event.AreaEventFromProto(
                            new FGJ23.Levels.Proto.AreaEvent()
                            {
                            Id = FGJ23.Levels.Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                            X = 50,
                            Y = 1,
                            Width = 32,
                            Height = 32,
                            Data = ByteString.CopyFromUtf8("LN_voittolinna_BW.png;")
                            }
                            ));
                Entity.Destroy();
            }
        }
    }

    public class GameplayScene : SceneBase
    {
        public static Player playerInstance;

        public Level level { get; set; }
        public static FGJ23.Levels.Proto.Level NextProtoLevel { get; set; }

        private GameplayScene() { }

        public static GameplayScene construct(FGJ23.Levels.Proto.Level lev)
        {
            NextProtoLevel = lev;
            return new GameplayScene();
        }

        public static void prepare(FGJ23.Levels.Proto.Level lev)
        {
            NextProtoLevel = lev;
        }

        public static GameplayScene getPreset()
        {
            return new GameplayScene();
        }

        VirtualButton _restartLevel;

        public override void Initialize()
        {
            base.Initialize();
            ColliderSystem.Reset();

            _restartLevel = new VirtualButton();
            _restartLevel.Nodes.Add(new VirtualButton.KeyboardKey(Keys.R));
            _restartLevel.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.Y));

            Log.Information("Initializing new GameplayScene with l={A}", NextProtoLevel);
            // setup a pixel perfect screen that fits our map
            int x = 200;
            int y = 150;
            SetDesignResolution(x, y, SceneResolutionPolicy.ShowAllPixelPerfect);

            CreateEntity("forceMovementSystem")
                .AddComponent(new ForceMovementSystem());

            var realMapEntity = CreateEntity("real-map-entity");
            var levelTuple = LevelLoader.LoadLevel(NextProtoLevel, file => "Content/Files/" + file);
            this.level = levelTuple.Item1;

            GameState.SetMusic(levelTuple.Item2.Music);

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
                playerEntity = CreateEntity("player", new Vector2(50, 50));
            }
            else
            {
                var r = new System.Random();
                playerEntity = CreateEntity("player", spawns[r.Next(spawns.Count)].Transform.Position);
            }
            playerInstance = playerEntity.AddComponent(new Player(14, 14, LevelBank.getSprites(NextProtoLevel.Name)));
            var playerWidth = playerInstance.Width;
            var playerHeight = playerInstance.Height;
            playerEntity.AddComponent(new BoxCollider(-playerWidth / 2, -playerHeight / 2, playerWidth, playerHeight));
            Log.Information("PlayerSize {A} {B}", playerWidth, playerHeight);
            playerEntity.AddComponent(new Levels.Mover(this.level.SpriteLayer));

            Camera.Entity.AddComponent(new FollowCamera(playerEntity)).FollowLerp = 1;

            playerInstance.PreventActions = true;

            LevelBank.setPlayerAttributes(playerInstance, NextProtoLevel.Name);
            Log.Information("Player attributes: {A}", playerInstance.CanJump);

            var storyEntity = CreateEntity("startStory", new Vector2(0, 0));
            storyEntity.AddComponent(LevelBank.getStartStory(NextProtoLevel.Name));
            GameState.OnStoryComplete += () => {
                if(NextProtoLevel.Name == "level13") {
                    var ent = CreateEntity("level13component", new Vector2(0, 0));
                    ent.AddComponent(new Level13Component());
                } else if (NextProtoLevel.Name == "level14") {
                    var ent = CreateEntity("level13component", new Vector2(0, 0));
                    ent.AddComponent(new Level14Component());
                } else {
                    playerInstance.PreventActions = false;
                }
            };
        }

        public override void Unload() {
            base.Unload();
            _restartLevel.Deregister();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();

            if(!playerInstance.PreventActions && _restartLevel.IsPressed) {
                GameState.Instance.DoTransition(() => {
                        return new GameplayScene();
                        });
            }
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
