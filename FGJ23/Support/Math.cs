using Microsoft.Xna.Framework;
using Nez;
using System;

namespace FGJ23.Ext
{
    public static class GMath
    {
        public static int Modi(int x, int m)
        {
            return (x % m + m) % m;
        }
        public static float Mod(float a, float b)
        {
            return a - b * Mathf.Floor(a / b);
        }

        public static float ModDist(float a, float b, float m)
        {
            var d = Math.Abs(b - a);
            return Math.Min(d, m - d);
        }

        // Stryker disable Equality
        public static float ClampDegree(float min, float x, float max)
        {
            if (min <= max)
            {
                if (min <= x && x <= max)
                {
                    return x;
                }
                else if (min == max)
                {
                    return min;
                }
                else if (Math.Abs(max - x) < Math.Abs(min - x))
                {
                    return max;
                }
                else
                {
                    return min;
                }
            }
            else if (min <= x && x <= 360)
            {
                return Math.Max(min, Math.Min(x, 360));
            }
            else if (0 <= x && x <= max)
            {
                return Math.Max(0, Math.Min(x, max));
            }
            else if (ModDist(x, max, 360) < ModDist(x, min, 360))
            {
                return max;
            }
            else
            {
                return min;
            }
        }
        // Stryker restore Equality

        public static float FlipDegreeX(float x)
        {
            return GMath.Mod(90 + (90 - x), 360);
        }

        public static float NormDegree(float x)
        {
            if (IsBetween(-180, x, 180))
            {
                return x;
            }
            else if (x < 0)
            {
                return NormDegree(x + 360);
            }
            else
            {
                return NormDegree(x - 360);
            }
        }

        public static bool IsBetween(float a, float x, float b)
        {
            return a <= x && x <= b;
        }

        // Returns how much b->a needs to be rotated to be in the same angle as b->c
        public static float Angle3(Vector2 a, Vector2 b, Vector2 c)
        {
            return -(a - b).Angle2(c - b);
        }

        /* Return c' so that:
        - |b - c'| = bclen
        - <abc' is between (180+maxCcwAngle) and (180 - maxCwAngle)
        - c' is as close to c' as possible
         */
        public static Vector2 ClampAngle(Vector2 a, Vector2 b, Vector2 c, float bclen, float maxCcwAngle, float maxCwAngle)
        {
            var optimalAngle = Angle3(a, b, c);

            var abUnit = Vector2.Normalize(b - a);
            var bcUnit = Vector2.Normalize(c - b);
            optimalAngle = GMath.Mod(optimalAngle, 360);

            if (IsBetween(180 - maxCcwAngle, optimalAngle, 180) || IsBetween(180, optimalAngle, 180 + maxCwAngle))
            {
                return b + bcUnit * bclen;
            }

            var resA = b + abUnit.Rotate(maxCcwAngle) * bclen;
            var resB = b + abUnit.Rotate(-maxCwAngle) * bclen;

            return c.Closest(resA, resB);
        }
    }
}
