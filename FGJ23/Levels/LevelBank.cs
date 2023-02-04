using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 2, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 2, 2, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 2, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 1, 1, 1, 1, 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 2, 2, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 2, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    });
#pragma warning restore format

            protoLayer.AreaEvents.Add(new Proto.AreaEvent()
            {
                Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                X = 17 * 32,
                Y = 13 * 32,
                Width = 64,
                Height = 64,
                Data = ByteString.CopyFromUtf8("level2")
            });

            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent()
            {
                Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy,
                X = 10 * 32,
                Y = 10 * 32,
            });

            lev.Layers.Add(protoLayer);
            var protoTileset = new Proto.Tileset()
            {
                Image = "test.png",
                Mask = "testmask.png",
            };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.Level Level2()
        {
            var lev = new Proto.Level
            {
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
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                    1, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 1,
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                    });
#pragma warning restore format

            protoLayer.AreaEvents.Add(new Proto.AreaEvent()
            {
                Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                X = 17 * 32,
                Y = 13 * 32,
                Width = 64,
                Height = 64,
                Data = ByteString.CopyFromUtf8("level1")
            });


            protoLayer.CoordinateEvents.Add(new Proto.CoordinateEvent()
            {
                Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy,
                X = 10 * 32,
                Y = 10 * 32,
            });

            lev.Layers.Add(protoLayer);
            var protoTileset = new Proto.Tileset()
            {
                Image = "test.png",
                Mask = "testmask.png",
            };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }
    }
}
