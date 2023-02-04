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

        public static Tuple<Level, Proto.Level> LoadLevel(Proto.Level lev, Func<string, string> resolve)
        {
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
