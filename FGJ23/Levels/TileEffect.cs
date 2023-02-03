using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace FGJ23.Levels
{
    [Flags]
    public enum TileEffect
    {
        None = 0,
        VFlip = 1 << 0,
        HFlip = 1 << 1,
        Transparent = 1 << 2,
        OneWay = 1 << 3,
    }

    static class TileEffectExt
    {
        public static TileEffect[] FromProtoLayer(Proto.Layer layer)
        {
            var arr = layer.Tiles.Select(tile =>
            {
                var effects = TileEffect.None;
                if ((tile & (int)Proto.Layer.Types.TileEffect.HFlip) != 0)
                {
                    effects |= TileEffect.HFlip;
                }
                if ((tile & (int)Proto.Layer.Types.TileEffect.VFlip) != 0)
                {
                    effects |= TileEffect.VFlip;
                }
                if ((tile & (int)Proto.Layer.Types.TileEffect.Transparent) != 0)
                {
                    effects |= TileEffect.Transparent;
                }
                if ((tile & (int)Proto.Layer.Types.TileEffect.OneWay) != 0)
                {
                    effects |= TileEffect.OneWay;
                }

                return effects;
            }).ToArray();
            return arr;
        }

        public static int[] ToPackedFormat(int[] Tiles, TileEffect[] effects)
        {
            return Tiles.Zip(effects).Select(pair =>
            {
                var tile = pair.First;
                var effect = pair.Second;
                if ((effect & TileEffect.HFlip) != 0)
                {
                    tile |= (int)Proto.Layer.Types.TileEffect.HFlip;
                }
                if ((effect & TileEffect.VFlip) != 0)
                {
                    tile |= (int)Proto.Layer.Types.TileEffect.VFlip;
                }
                if ((effect & TileEffect.Transparent) != 0)
                {
                    tile |= (int)Proto.Layer.Types.TileEffect.Transparent;
                }
                if ((effect & TileEffect.OneWay) != 0)
                {
                    tile |= (int)Proto.Layer.Types.TileEffect.OneWay;
                }

                return tile;
            }).ToArray();
        }

        public static float GetRotation(this TileEffect tile)
        {
            return 0f;
        }

        public static SpriteEffects GetSpriteEffects(this TileEffect tile)
        {
            var effects = SpriteEffects.None;
            if ((tile & TileEffect.HFlip) != 0)
            {
                effects |= SpriteEffects.FlipHorizontally;
            }
            if ((tile & TileEffect.VFlip) != 0)
            {
                effects |= SpriteEffects.FlipVertically;
            }
            return effects;
        }

        public static Color GetRenderColor(this TileEffect tile, Color color)
        {
            if ((tile & TileEffect.Transparent) != 0)
            {
                return new Color(color, color.A / 2);
            }
            return color;
        }
    }
}
