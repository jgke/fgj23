using FGJ23.Ext;
using FGJ23.Levels;
using FGJ23.PNG;
using FGJ23.Support;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.IO;

namespace FGJ23.Levels
{
    public class Level : RenderableComponent, IUpdatable, ILoggable
    {
        public List<Layer> Layers;

        public List<Texture2D> Images = new List<Texture2D>();
        public List<Texture2D> MaskImages = new List<Texture2D>();
        private List<Mask[]> _masks = new List<Mask[]>();

        [Loggable]
        public override float Width => tileWidth * SpriteLayerWidthInTiles;
        [Loggable]
        public override float Height => tileHeight * SpriteLayerHeightInTiles;

        [Loggable]
        public uint SpriteLayerNum = 3;
        public Layer SpriteLayer { get => Layers[(int)SpriteLayerNum]; }

        [Loggable]
        private int tileWidth = 32;
        [Loggable]
        private int tileHeight = 32;
        public Point TileSize { get => new Point(tileWidth, tileHeight); }
        [Loggable]
        private int SpriteLayerWidthInTiles = 100;
        [Loggable]
        private int SpriteLayerHeightInTiles = 100;

        public List<string> ImageFileNames;
        public List<MemoryStream> Imagefiles;
        public List<string> MaskFileNames;
        public List<MemoryStream> Maskfiles;

        public Color ResetColor = Color.Pink;

        public static Level CreateNew()
        {
            return new Level
            {
                Layers = new List<Layer>(),
                SpriteLayerNum = 0,
                Imagefiles = new List<MemoryStream>(),
                ImageFileNames = new List<string>(),
                Maskfiles = new List<MemoryStream>(),
                MaskFileNames = new List<string>()
            };
        }

        private Texture2D ConvertTilesetImage(MemoryStream stream)
        {
            var data = PngDecoder.ReadImageToRgbaPng(stream.GetBuffer());
            var convertedStream = new MemoryStream();
            convertedStream.Write(data, 0, data.Length);
            return Nez.Core.GraphicsDevice == null ? null : Texture2D.FromStream(Nez.Core.GraphicsDevice, convertedStream);
        }

        private void CalculateMask(MemoryStream stream)
        {
            bool[,] mask = PngDecoder.DecodeMaskFromPng(stream.ToArray());

            var mapHeight = mask.GetLength(0) / tileWidth;
            var mapWidth = mask.GetLength(1) / tileHeight;

            Mask[] masks = new Mask[mapWidth * mapHeight];

            for (int ty = 0; ty < mapHeight; ty++)
            {
                for (int tx = 0; tx < mapWidth; tx++)
                {
                    bool[,] tileMask = new bool[tileWidth, tileHeight];
                    for (int y = 0; y < tileHeight; y++)
                    {
                        for (int x = 0; x < tileWidth; x++)
                        {
                            tileMask[x, y] = mask[ty * tileHeight + y, tx * tileWidth + x];
                        }
                    }
                    masks[ty * mapWidth + tx] = new Mask(tileMask);
                }
            }

            _masks.Add(masks);
        }

        public new void Initialize()
        {
            foreach (var stream in Imagefiles)
            {
                Images.Add(ConvertTilesetImage(stream));
            }
            foreach (var stream in Maskfiles)
            {
                MaskImages.Add(ConvertTilesetImage(stream));
                CalculateMask(stream);
            }
            Imagefiles = new List<MemoryStream>();
            Maskfiles = new List<MemoryStream>();

            Log.Information("Sprite layer: {A}", SpriteLayerNum);
            foreach (var layer in Layers)
            {
                layer.Image = Images[layer.TilesetIndex];
                layer.MaskImage = MaskImages[layer.TilesetIndex];
                layer.Mask = _masks[layer.TilesetIndex];
                layer.Initialize();

                Log.Information("Layer inititialized {@layer}", layer);
            }

            SpriteLayerWidthInTiles = SpriteLayer.Width;
            SpriteLayerHeightInTiles = SpriteLayer.Height;

            Log.Information("Level loaded {@level}", this);
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            Point position = (Entity.GraphicsTransform.Position + _localOffset).ToRoundedPoint();
            Rectangle cameraBounds = camera.Bounds.ToIntRect();
            batcher.DrawRect(Entity.GraphicsTransform.Position, Width, Height, ResetColor);
            for (int layerNum = Layers.Count - 1; layerNum >= 0; layerNum--)
            {
                Layers[layerNum].RenderSingleLayer(batcher, cameraBounds, position, Entity.GraphicsTransform.Scale, (int)(LayerDepth * 65535));
            }
        }
        public override void DebugRender(Batcher batcher)
        {
            Point position = (Entity.GraphicsTransform.Position + _localOffset).ToRoundedPoint();
            Rectangle cameraBounds = Entity.Scene.Camera.Bounds.ToIntRect();
            Layers[3].DebugRender(batcher, cameraBounds, position, Entity.GraphicsTransform.Scale, (int)(LayerDepth * 65535));
        }

        void IUpdatable.FixedUpdate()
        {
        }
        void IUpdatable.DrawUpdate() { }

        public Level IntoLayer(uint layer)
        {
            return new Level
            {
                Layers = Layers.Count > 0 ? new List<Layer>(new Layer[] { Layers[(int)layer] }) : new List<Layer>(),
                Images = Images,
                MaskImages = MaskImages,
                _masks = _masks,
                SpriteLayerNum = 0,
                tileWidth = tileWidth,
                tileHeight = tileHeight,
                SpriteLayerWidthInTiles = SpriteLayerWidthInTiles,
                SpriteLayerHeightInTiles = SpriteLayerHeightInTiles,

                ImageFileNames = ImageFileNames,
                Imagefiles = new List<MemoryStream>(),
                MaskFileNames = MaskFileNames,
                Maskfiles = new List<MemoryStream>()
            };
        }

        public Level CloneNew()
        {
            return new Level
            {
                Layers = Layers,
                Images = Images,
                MaskImages = MaskImages,
                _masks = _masks,
                SpriteLayerNum = SpriteLayerNum,
                tileWidth = tileWidth,
                tileHeight = tileHeight,
                SpriteLayerWidthInTiles = SpriteLayerWidthInTiles,
                SpriteLayerHeightInTiles = SpriteLayerHeightInTiles,

                ImageFileNames = ImageFileNames,
                MaskFileNames = MaskFileNames,
                Imagefiles = new List<MemoryStream>(),
                Maskfiles = new List<MemoryStream>()
            };
        }

        public void AddTileset(string tilesetFile, string maskName)
        {
            ImageFileNames.Add(tilesetFile.Split("\\").LastItem());
            MaskFileNames.Add(maskName.Split("\\").LastItem());

            using (var stream = LevelLoader.ReadFromDisk(tilesetFile))
            {
                Images.Add(ConvertTilesetImage(stream));
            }
            using (var stream = LevelLoader.ReadFromDisk(maskName))
            {
                MaskImages.Add(ConvertTilesetImage(stream));
                CalculateMask(stream);
            }
        }
    }
}
