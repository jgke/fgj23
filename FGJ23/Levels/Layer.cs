using FGJ23.Entities;
using FGJ23.Ext;
using FGJ23.Support;
using Google.Protobuf.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FGJ23.Levels
{
    public partial class Layer : ILoggable
    {
        [Loggable]
        public string Name { get; set; }
        [Loggable]
        public float Opacity { get; set; } = 1;
        [Loggable]
        public bool Visible { get; set; } = true;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        [Loggable]
        public Point Offset => new Point(OffsetX, OffsetY);

        public float XSpeed { get => LayerSpeed.X; set => LayerSpeed.X = value; }
        public float YSpeed { get => LayerSpeed.Y; set => LayerSpeed.Y = value; }

        [Loggable]
        public Vector2 LayerSpeed = new Vector2(1, 1);

        [Loggable]
        public int TilesetIndex;
        [Loggable]
        public int Width;
        [Loggable]
        public int Height;

        [Loggable]
        public int TileWidth = 32;
        [Loggable]
        public int TileHeight = 32;
        [Loggable]
        int TilesetWidth = 320;

        public int TilesPerRow => TilesetWidth / TileWidth;

        public RepeatedField<Proto.CoordinateEvent> CoordinateEvents { get; internal set; }
        public RepeatedField<Proto.AreaEvent> AreaEvents { get; internal set; }

        [Loggable]
        public bool LoopX = false;
        [Loggable]
        public bool LoopY = false;

        public int[] Tiles;
        public TileEffect[] TileEffects;

        public Texture2D Image;
        public Texture2D MaskImage;
        public Mask[] Mask;
        public Mask[] VFlipMask;
        public Mask[] HFlipMask;
        public Mask[] VAndHFlipMask;
        public CoordinateEvent[] Events;

        public static Layer CreateNew()
        {
            return new Layer()
            {
                Tiles = new int[50 * 100],
                TileEffects = new TileEffect[50 * 100],

                TilesetIndex = 0,
                Width = 50,
                Height = 100,
                LoopX = false,
                LoopY = false,
                XSpeed = 1,
                YSpeed = 1,

                CoordinateEvents = new RepeatedField<Proto.CoordinateEvent>(),
                AreaEvents = new RepeatedField<Proto.AreaEvent>(),
            };
        }

        public int? TileIndex(int x, int y)
        {
            if (LoopX)
            {
                x = GMath.Modi(x, Width);
            }
            if (LoopY)
            {
                y = GMath.Modi(y, Height);
            }
            if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
            {
                return null;
            }
            return TileIndexSafe(x, y);
        }

        public int TileIndexSafe(int x, int y)
        {
            if (LoopX)
            {
                x = GMath.Modi(x, Width);
            }
            if (LoopY)
            {
                y = GMath.Modi(y, Height);
            }
            return y * Width + x;
        }

        public int? GetTile(int x, int y)
        {
            var i = TileIndex(x, y);
            return i switch
            {
                null => null,
                _ => Tiles[i.Value]
            };
        }
        int WorldToTilePositionX(float x)
        {
            return Mathf.FloorToInt(x / TileWidth);
        }

        int WorldToTilePositionY(float y)
        {
            return Mathf.FloorToInt(y / TileHeight);
        }

        public void RenderSingleLayer(Batcher batcher, Rectangle cameraBounds, Point position, Vector2 scale, int layerDepth)
        {
            if (!this.Visible)
            {
                return;
            }

            Rectangle cameraClipBounds = new Rectangle(
                    (int)(cameraBounds.X / scale.X),
                    (int)(cameraBounds.Y / scale.Y),
                    (int)(cameraBounds.Width / scale.X),
                    (int)(cameraBounds.Height / scale.Y)
                );

            position += (this.Offset.ToVector2() / scale).ToRoundedPoint();

            cameraClipBounds.Location -= position;

            var offset = cameraClipBounds.Location - (cameraClipBounds.Location.ToVector2() * LayerSpeed).ToRoundedPoint();

            var tileWidth = Mathf.RoundToInt(TileWidth * scale.X);
            var tileHeight = Mathf.RoundToInt(TileHeight * scale.Y);

            int minX = WorldToTilePositionX(cameraClipBounds.Left - offset.X);
            int minY = WorldToTilePositionY(cameraClipBounds.Top - offset.Y);
            int maxX = WorldToTilePositionX(cameraClipBounds.Right - offset.X);
            int maxY = WorldToTilePositionY(cameraClipBounds.Bottom - offset.Y);

            if(!LoopX)
            {
                minX = Math.Max(0, minX);
                maxX = Math.Min(maxX, Width);
            }
            if(!LoopY)
            {
                minY = Math.Max(0, minY);
                maxY = Math.Min(maxY, Height);
            }

            var color = Color.White;
            color.A = (byte)(this.Opacity * 255);

            // loop through and draw all the non-culled tiles
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    var tile = this.GetTile(x, y);
                    if (tile != null)
                    {
                        RenderTile(tile.Value, x, y, batcher, position + offset, scale, tileWidth, tileHeight, color, layerDepth);
                    }
                }
            }
        }

        internal void Initialize()
        {
            System.Diagnostics.Debug.Assert(Width * Height == Tiles.Length, "Invalid level size",
                    "Expected {0}*{1}={2} but got {3}", Width, Height, Width *
                    Height, Tiles.Length);
            System.Diagnostics.Debug.Assert(TileEffects != null, "Mandatory parameter not set", "Expected TileEffects != null");

            VFlipMask = new Mask[Mask.Length];
            HFlipMask = new Mask[Mask.Length];
            VAndHFlipMask = new Mask[Mask.Length];

            for (int i = 0; i < Mask.Length; i++)
            {
                VFlipMask[i] = Mask[i].VFlip();
                HFlipMask[i] = Mask[i].HFlip();
                VAndHFlipMask[i] = Mask[i].HFlip().VFlip();
            }
        }

        public RectangleF getTilesetTile(int tileValue)
        {
            int baseTilePosition = tileValue & ((1 << 24) - 1);
            int x = (baseTilePosition % TilesPerRow) * TileWidth;
            int y = (baseTilePosition / TilesPerRow) * TileHeight;
            return new RectangleF(x, y, TileWidth, TileHeight);
        }

        private void RenderTile(int tile, int x, int y, Batcher batcher, Point position, Vector2 scale, int tileWidth, int tileHeight, Color color, int layerDepth)
        {
            var sourceRect = getTilesetTile(tile);

            var tx = x * tileWidth;
            var ty = y * tileHeight;
            var pos = new Point(tx, ty) + position;

            var effects = TileEffects[TileIndexSafe(x, y)];
            var rotation = effects.GetRotation();
            var spriteEffects = effects.GetSpriteEffects();
            color = effects.GetRenderColor(color);

            batcher.Draw(Image, pos.ToVector2(), sourceRect, color, rotation, Vector2.Zero, scale, spriteEffects, layerDepth);
        }

        public Mask GetMask(int x, int y)
        {
            int? tile = GetTile(x, y);
            if (tile != null)
            {
                var maskTile = tile.Value & ((1 << 24) - 1);

                if (maskTile >= Mask.Length || Mask[maskTile].Empty)
                {
                    return null;
                }
                var effects = TileEffects[TileIndexSafe(x, y)];

                var hflip = (effects & TileEffect.HFlip) != 0;
                var vflip = (effects & TileEffect.VFlip) != 0;

                if (hflip && vflip) return VAndHFlipMask[maskTile];
                if (hflip) return HFlipMask[maskTile];
                if (vflip) return VFlipMask[maskTile];
                return Mask[maskTile];

            }
            return null;
        }

        /* Returns the first filled point in cast */
        // TODO: this is doing every pixel, not every tile
        // TODO: this is shit code
        public int? RayCast(Point point, Edge direction, int length)
        {
            int minx = direction != Edge.Left ? point.X : point.X - length;
            int miny = direction != Edge.Top ? point.Y : point.Y - length;
            int maxx = direction != Edge.Right ? point.X : point.X + length;
            int maxy = direction != Edge.Bottom ? point.Y : point.Y + length;

            if (direction == Edge.Bottom)
            {
                for (int worldY = miny; worldY <= maxy; worldY++)
                {
                    int x = WorldToTilePositionX(point.X);
                    int y = WorldToTilePositionX(worldY);
                    Mask mask = GetMask(x, y);
                    if (mask != null && (y != WorldToTilePositionY(point.Y) || 32 - (worldY % 32) > mask.DistanceFromBottom(point.X % 32)))
                    {
                        int dist = mask.DistanceFromTop(point.X % 32);
                        if (dist != 0)
                        {
                            var contact = y * TileHeight + dist - 1;
                            if (contact <= maxy)
                                return contact;
                        }
                    }
                }
            }
            else if (direction == Edge.Top)
            {
                for (int worldY = maxy; worldY >= miny; worldY--)
                {
                    int x = WorldToTilePositionX(point.X);
                    int y = WorldToTilePositionX(worldY);

                    Mask mask = GetMask(x, y);
                    if (mask != null && (y != WorldToTilePositionY(point.Y) || worldY % 32 > mask.DistanceFromTop(point.X % 32)))
                    {
                        if ((TileEffects[TileIndexSafe(x, y)] & TileEffect.OneWay) != 0)
                        {
                            return null;
                        }
                        int dist = mask.DistanceFromBottom(point.X % 32);
                        if (dist != 0)
                        {
                            var contact = (y + 1) * TileHeight - dist + 1;
                            if (contact >= miny)
                                return contact;
                        }
                    }
                }
            }
            else if (direction == Edge.Left)
            {
                for (int worldX = maxx; worldX >= minx; worldX--)
                {
                    int x = WorldToTilePositionX(worldX);
                    int y = WorldToTilePositionX(point.Y);
                    Mask mask = GetMask(x, y);
                    if (mask != null && (x != WorldToTilePositionX(point.X) || worldX % 32 > mask.DistanceFromLeft(point.Y % 32)))
                    {
                        int dist = mask.DistanceFromRight(point.Y % 32);
                        if (dist != 0)
                        {
                            var contact = (x + 1) * TileWidth - dist + 1;
                            if (contact >= minx)
                                return contact;
                        }
                    }
                }
            }
            else if (direction == Edge.Right)
            {
                for (int worldX = minx; worldX <= maxx; worldX++)
                {
                    int x = WorldToTilePositionX(worldX);
                    int y = WorldToTilePositionX(point.Y);
                    Mask mask = GetMask(x, y);
                    if (mask != null && (x != WorldToTilePositionX(point.X) || 32 - (worldX % 32) > mask.DistanceFromRight(point.Y % 32)))
                    {
                        int dist = mask.DistanceFromLeft(point.Y % 32);
                        if (dist != 0)
                        {
                            var contact = x * TileWidth + dist - 1;
                            if (contact <= maxx)
                                return contact;
                        }
                    }
                }
            }

            return null;
        }

        private int? FirstNotNull(int? a, int? b)
        {
            if (a != null)
            {
                return a.Value;
            }
            else if (b != null)
            {
                return b.Value;
            }
            else
            {
                return null;
            }
        }

        private int? SmallerInt(int? a, int? b)
        {
            if (a != null && b != null)
            {
                return Math.Min(a.Value, b.Value);
            }
            return FirstNotNull(a, b);
        }

        private int? BiggerInt(int? a, int? b)
        {
            if (a != null && b != null)
            {
                return Math.Max(a.Value, b.Value);
            }
            return FirstNotNull(a, b);
        }

        public int? GetCollision(Rectangle rect, Edge direction, int amount, bool onGround = false)
        {
            if (direction == Edge.Bottom && onGround)
            {
                int? left = RayCast(new Point(rect.X, rect.Y + rect.Height - 2), Edge.Bottom, amount + 2);
                int? right = RayCast(new Point(rect.X + rect.Width, rect.Y + rect.Height - 2), Edge.Bottom, amount + 2);
                return SmallerInt(left, right);
            }
            else if (direction == Edge.Bottom)
            {
                int? left = RayCast(new Point(rect.X, rect.Y - 2), Edge.Bottom, amount + rect.Height + 2);
                int? right = RayCast(new Point(rect.X + rect.Width, rect.Y - 2), Edge.Bottom, amount + rect.Height + 2);
                return SmallerInt(left, right);
            }
            else if (direction == Edge.Top)
            {
                int? left = RayCast(new Point(rect.X, rect.Y), Edge.Top, Math.Abs(amount));
                int? right = RayCast(new Point(rect.X + rect.Width, rect.Y), Edge.Top, Math.Abs(amount));
                return BiggerInt(left, right);
            }
            else if (direction == Edge.Left)
            {
                int? top = RayCast(new Point(rect.X - 1, rect.Y + 1), Edge.Left, Math.Abs(amount) + 1);
                int? middle = null;// RayCast(new Point(rect.X - 1, rect.Y + rect.Height / 2), Edge.Left, Math.Abs(amount) + 1);
                return BiggerInt(top, middle);
            }
            else if (direction == Edge.Right)
            {
                int? top = RayCast(new Point(rect.X + rect.Width, rect.Y + 1), Edge.Right, amount);
                int? middle = null;// RayCast(new Point(rect.X + rect.Width, rect.Y + rect.Height / 2), Edge.Right, amount);
                return SmallerInt(top, middle);
            }
            return null;
        }
        public void DebugRender(Batcher batcher, Rectangle cameraClipBounds, Point position, Vector2 scale, float layerDepth)
        {
            if (!this.Visible)
            {
                return;
            }

            var tileWidth = Mathf.RoundToInt(TileWidth * scale.X);
            var tileHeight = Mathf.RoundToInt(TileHeight * scale.Y);

            int minX = WorldToTilePositionX(cameraClipBounds.Left);
            int minY = WorldToTilePositionY(cameraClipBounds.Top);
            int maxX = WorldToTilePositionX(cameraClipBounds.Right);
            int maxY = WorldToTilePositionY(cameraClipBounds.Bottom);

            var color = Color.White;
            color.A = (byte)(this.Opacity * 255);

            // loop through and draw all the non-culled tiles
            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    var tx = x * tileWidth;
                    var ty = y * tileHeight;

                    var pos = new Point(tx, ty) + position;
                    Mask mask = GetMask(x, y);
                    if (mask != null)
                    {
                        for (int maskY = 0; maskY < 32; maskY++)
                        {
                            int left = mask.DistanceFromLeft(maskY);
                            if (left != 0)
                            {
                                batcher.DrawPixel(pos.X + left, pos.Y + maskY, Color.White);
                            }
                            int right = mask.DistanceFromRight(maskY);
                            if (right != 0)
                            {
                                batcher.DrawPixel(pos.X + tileWidth - right, pos.Y + maskY, Color.White);
                            }
                        }

                        for (int maskX = 0; maskX < 32; maskX++)
                        {
                            int top = mask.DistanceFromTop(maskX);
                            if (top != 0)
                            {
                                batcher.DrawPixel(pos.X + maskX, pos.Y + top, Color.White);
                            }
                            int bottom = mask.DistanceFromBottom(maskX);
                            if (bottom != 0)
                            {
                                batcher.DrawPixel(pos.X + maskX, pos.Y + TileHeight - bottom, Color.White);
                            }
                        }
                    }
                }
            }
        }
    }
}
