using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FGJ23.Core;
using FGJ23.Support;
using FGJ23.Entities;

namespace FGJ23.Levels
{
    public static class LevelBank
    {
        public static Proto.Level GetLevel(string name)
        {
            Log.Information("GetLevel {name}", name);
            return name switch
            {
                "level1" => Level1(),
                "level2" => Level2(),
                "level3" => Level3(),
                "level4" => Level4(),
                "level5" => Level5(),
                "level6" => Level6(),
                "level7" => Level7(),
                "level8" => Level8(),
                "level9" => Level9(),
                "level10" => Level10(),
                "level11" => Level11(),
                "level12" => Level12(),
                "" => null,
                _ => throw new Exception("Unknown level name: " + name),
            };
        }

#pragma warning disable format
        public static Proto.Level Level1()
        {
            var lev = new Proto.Level
            {
                Title = "Level 1",
                Name = "level1",
                SpriteLayer = 0
            };
            var protoLayer = new Proto.Layer()
            {
                TilesetIndex = 0,
                Height = 15,
                Width = 20,
                LoopX = false,
                LoopY = false,
                XSpeed = 1,
                YSpeed = 1,
            };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11,  4,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14,  0,  1,  1,  1,  2, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14,  5,  6,  6,  6,  7, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    7, 14, 14, 14, 14, 14,  5,  6,  6,  6,  7, 14, 14, 14, 14, 14, 14, 14, 14,  5,
                    8,  1,  1,  1,  1,  1,  9,  6,  6,  6,  8,  1,  1,  1,  1,  1,  1,  1,  1,  9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent()
            {
                Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                X = 17 * 16,
                Y = 13 * 16,
                Width = 64,
                Height = 64,
                Data = ByteString.CopyFromUtf8("level2")
            });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent()
            {
                Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy,
                X = 10 * 16,
                Y = 10 * 16,
            });

            lev.Layers.Add(protoLayer);
            var protoTileset = new Proto.Tileset()
            {
                Image = "MB_maa.png",
                Mask = "MB_maa.png",
            };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level2()
        {
            var lev = new Proto.Level
            {
                Title = "Level 2",
                Name = "level2",
                SpriteLayer = 0
            };
            var protoLayer = new Proto.Layer()
            {
                TilesetIndex = 0,
                Height = 15,
                Width = 20,
                LoopX = false,
                LoopY = false,
                XSpeed = 1,
                YSpeed = 1,
            };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 4,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 5,
                    7, 14, 14, 14, 14, 14, 2, 2, 2, 2, 2, 2, 2, 14, 14, 14, 14, 14, 14, 5,
                    8, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent()
            {
                Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                X = 17 * 16,
                Y = 13 * 16,
                Width = 64,
                Height = 64,
                Data = ByteString.CopyFromUtf8("level3")
            });


            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent()
            {
                Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy,
                X = 10 * 16,
                Y = 10 * 16,
            });

            lev.Layers.Add(protoLayer);
            var protoTileset = new Proto.Tileset()
            {
                Image = "MB_maa.png",
                Mask = "MB_maa.png",
            };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level3()
        {
            var lev = new Proto.Level { Title = "level3", Name = "level3", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level4") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level4()
        {
            var lev = new Proto.Level { Title = "level4", Name = "level4", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level5") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level5()
        {
            var lev = new Proto.Level { Title = "level5", Name = "level5", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level6") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }


        public static Proto.Level Level6()
        {
            var lev = new Proto.Level { Title = "level6", Name = "level6", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level7") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }


        public static Proto.Level Level7()
        {
            var lev = new Proto.Level { Title = "level6", Name = "level7", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level8") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level8()
        {
            var lev = new Proto.Level { Title = "level8", Name = "level8", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level9") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level9()
        {
            var lev = new Proto.Level { Title = "level9", Name = "level9", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level10") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }


        public static Proto.Level Level10()
        {
            var lev = new Proto.Level { Title = "level10", Name = "level10", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level11") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level11()
        {
            var lev = new Proto.Level { Title = "level11", Name = "level11", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("level12") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level12()
        {
            var lev = new Proto.Level { Title = "level12", Name = "level12", SpriteLayer = 0 };
            var protoLayer = new Proto.Layer() { TilesetIndex = 0, Height = 5, Width = 5, LoopX = false, LoopY = false, XSpeed = 1, YSpeed = 1, };
            protoLayer.Tiles.AddRange(new List<int>() {
                    3, 11, 11, 11,   4,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    7, 14, 14, 14,   5,
                    8,  1,  1,  1,   9,
                    });

            protoLayer.AreaEvents.Add(new Proto.AreaEvent() { Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd, X = 2 * 16, Y = 2 * 16, Width = 64, Height = 64, Data = ByteString.CopyFromUtf8("") });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent() { Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy, X = 10 * 16, Y = 10 * 16, });

            lev.Layers.Add(protoLayer);
            
            var protoTileset = new Proto.Tileset() { Image = "MB_maa.png", Mask = "MB_maa.png", };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

#pragma warning restore format
        public static StoryComponent getStartStory(string name)
        {
            return name switch
            {
                "level1" =>
                    new StoryBuilder()
                    .Exposition("I was born in 1982. Back in my day, games were about CONTENT. You'd walk around with the arrow keys. Jump by pressing up.")
                    .Exposition("Maybe shoot some bad guys with the space bar. Double jump by jumping again in the air and wall jump by pointing towards a wall in mid-air and jumping. The good old days")
                    .Exposition("Nowadays the kids can’t stay focused for more than two seconds unless it’s some virtual reality thirst trap.")
                    .GoToLevel(),

                "level2" =>
                    new StoryBuilder()
                    .Exposition("2082. What a year this could've been for games. A century of evolution from the legends of yore.")
                    .Exposition("Every first level should be easy, but showcase the main features without hand-holding.")
                    .Exposition("There's an enemy. You can shoot it. You may need to jump over things to progress.")
                    .GoToLevel(),

                "level3" =>
                    new StoryBuilder()
                    .Exposition("Sometimes just jumping is not enough. Sometimes an enemy is in your way. Sometimes, you have to make the right choices")
                    .Exposition("And a grand finale, a true puzzle! Do I jump here or there?")
                    .GoToLevel(),

                "level4" =>
                    new StoryBuilder()
                    .Exposition("That didn't feel right. It has to be rewarding! You have to use all your jumps to clear it!")
                    .GoToLevel(),

                "level5" =>
                    new StoryBuilder()
                    .Exposition("THERE we go! A proper, pure challenge! Hit it just right. Special jumps are for whippersnappers.")
                    .GoToLevel(),

                "level6" =>
                    new StoryBuilder()
                    .Exposition("Graphics became frivolous much too early. You don’'t need more than a suggestion. A hint to jump-start your own imagination. No spoon-fed high-definition dreamscapes")
                    .Exposition("Change of pace. Enemies in the way. More of them. A challenge of a different kind. A satisfying contrast.")
                    .GoToLevel(),

                "level7" =>
                    new StoryBuilder()
                    .Exposition("Shooting from afar is cowardly. You have to make it personal. Look losing your only life right in the eye to clear every single monster. ")
                    .GoToLevel(),

                "level8" =>
                    new StoryBuilder()
                    .Exposition("Back in my day, every mechanic was necessary. No useless frills.\r\n")
                    .Exposition("You jump higher if you jump on an enemy. That's a clue. You need it somewhere. Use it.")
                    .GoToLevel(),

                "level9" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level10" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level11" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level12" =>
                    new StoryBuilder()
                    .GoToLevel(),

                _ => throw new Exception("Unknown level name: " + name),

            };
        }

        public static StoryComponent getEndStory(string name)
        {
            return name switch
            {
                "level1" =>
                    new StoryBuilder()
                    .Exposition("Ever since the 2050's, kids have been unable to function without constantly tending to their retinal and cochlear implants.")
                    .Exposition("Sharing four-dimensional memes. Living third lives on some D-Corporation Hyperverse.")
                    .Exposition("I want none of that. I just want a challenge and to reach that castle on the far right.")
                    .Exposition("I retired this year at the ripe old age of one hundred, and I intend to spend the rest of my days enjoying what games should always have been about!")
                    .GoToLevel(),

                "level2" =>
                    new StoryBuilder()
                    .Exposition("Simple. Pure. Beautiful.")
                    .GoToLevel(),

                "level3" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level4" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level5" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level6" =>
                    new StoryBuilder()
                    .Exposition("You really gave it to them!")
                    .GoToLevel(),
                    
                "level7" =>
                    new StoryBuilder()
                    .Exposition("THIS is true skill. THIS is how you feel accomplishment. ")
                    .GoToLevel(),

                "level8" =>
                     new StoryBuilder()
                    .Exposition("Chekhov’s guns are sloppy writing.")
                    .Exposition("This is perfect form, that requires perfect execution of the player. Exquisite.")
                    .GoToLevel(),

                "level9" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level10" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level11" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level12" =>
                    new StoryBuilder()
                    .GoToLevel(),

                _ => throw new Exception("Unknown level name: " + name),
            };
        }

        public static void setPlayerAttributes(Player player, string name)
        {
            switch (name) {
                case "level1":
                    player.CanJump = true;
                    player.CanWalljump = true;
                    player.CanShoot = true;
                    player.ExtraJumps = 1;
                    break;

                case "level2":
                    player.CanJump = true;
                    player.CanWalljump = false;
                    player.CanShoot = false;
                    player.ExtraJumps = 0;
                    break;


                case "level3":
                case "level4":
                case "level5":
                case "level6":
                case "level7":
                case "level8":
                case "level9":
                case "level10":
                case "level11":
                case "level12":
                    player.CanJump = true;
                    player.CanWalljump = true;
                    player.CanShoot = true;
                    player.ExtraJumps = 1;
                    break;

                default:
                    throw new Exception("Unknown level name: " + name);
            }
        }
    }
}
