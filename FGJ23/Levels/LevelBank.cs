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
                "" => null,
                _ => throw new Exception("Unknown level name: " + name),
            };
        }

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
#pragma warning disable format
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
#pragma warning restore format

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
#pragma warning disable format
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
#pragma warning restore format

            protoLayer.AreaEvents.Add(new Proto.AreaEvent()
            {
                Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                X = 17 * 16,
                Y = 13 * 16,
                Width = 64,
                Height = 64,
                Data = ByteString.CopyFromUtf8("level1")
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


        public static StoryComponent getStartStory(string name)
        {
            return name switch
            {
                "level1" =>
                    new StoryBuilder()
                    .Exposition("Only expo here")
                    .Exposition("No characters")
                    .GoToLevel(),

                "level2" =>
                    new StoryBuilder()
                    .Exposition("lev2 start story")
                    .Exposition("blah")
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
                    .Exposition("lev1 end story")
                    .Exposition("blah")
                    .GoToLevel(),

                "level2" =>
                    new StoryBuilder()
                    .Exposition("lev2 end story")
                    .Exposition("blah")
                    .GoToLevel(),

                _ => throw new Exception("Unknown level name: " + name),
            };
        }
    }
}
