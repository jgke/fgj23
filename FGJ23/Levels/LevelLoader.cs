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
    public static class LevelLoader
    {
        private static byte[] Magic = new byte[] { 0x42, 0x06, 0x91, 0x01 };

        public static MemoryStream ReadFromDisk(string path)
        {
            var data = ResourceLoader.ReadResource(path);
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            return stream;
        }

        public static Tuple<Level, Proto.Level> LoadLevel(Func<string, string> resolve)
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
#pragma warning enable format
            lev.Layers.Add(protoLayer);
                var protoTileset = new Proto.Tileset()
                {
                    Image = "test.png",
                        Mask = "testmask.png",
                };
            lev.Tilesets.Add(protoTileset);

        var level = new Level()
        {
            ImageFileNames = lev.Tilesets.Select(tileset => tileset.Image).ToList(),
                Imagefiles = lev.Tilesets.Select(tileset => ReadFromDisk(resolve(tileset.Image))).ToList(),
                MaskFileNames = lev.Tilesets.Select(tileset => tileset.Mask).ToList(),
                Maskfiles = lev.Tilesets.Select(tileset => ReadFromDisk(resolve(tileset.Mask))).ToList(),
                SpriteLayerNum = lev.SpriteLayer,
                Layers = lev.Layers
                .Select(layer => new Layer()
                        {
                        Tiles = layer.Tiles.ToArray(),
                        TileEffects = TileEffectExt.FromProtoLayer(layer),
                        
                        TilesetIndex = layer.TilesetIndex,
                        Width = layer.Width,
                        Height = layer.Height,
                        LoopX = layer.LoopX,
                        LoopY = layer.LoopY,
                        XSpeed = layer.XSpeed,
                        YSpeed = layer.YSpeed,
                        
                        CoordinateEvents = layer.CoordinateEvents,
                        AreaEvents = layer.AreaEvents,
                        })
            .ToList()
        };

        level.Initialize();
        return new(level, lev);
    }
}
}
