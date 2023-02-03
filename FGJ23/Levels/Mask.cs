using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Levels
{
    public class Mask
    {
        private int[] top;
        private int[] bottom;
        private int[] left;
        private int[] right;

        public readonly bool[,] Data;

        public readonly bool Empty = true;

        public Mask(bool[,] tile)
        {
            int height = tile.GetLength(0);
            int width = tile.GetLength(1);
            top = new int[width];
            bottom = new int[width];
            left = new int[height];
            right = new int[height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tile[x, y])
                    {
                        right[y] = width - x;
                        Empty = false;
                    }
                }
                for (int x = width - 1; x >= 0; x--)
                {
                    if (tile[x, y])
                    {
                        left[y] = x + 1;
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tile[x, y])
                    {
                        bottom[x] = height - y;
                    }
                }
                for (int y = height - 1; y >= 0; y--)
                {
                    if (tile[x, y])
                    {
                        top[x] = y + 1;
                    }
                }
            }

            this.Data = tile;
        }

        /*
         * How many pixels away from the top border is the first masked pixel?
         * 0 = no collision
         * 1 = first pixel etc
         */
        public int DistanceFromTop(int column)
        {
            return top[column];
        }
        public int DistanceFromBottom(int column)
        {
            return bottom[column];
        }

        public int DistanceFromLeft(int row)
        {
            return left[row];
        }
        public int DistanceFromRight(int row)
        {
            return right[row];
        }

        internal Mask VFlip()
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            bool[,] flippedData = new bool[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    flippedData[x, y] = Data[x, height - y - 1];
                }
            }

            return new Mask(flippedData);
        }

        internal Mask HFlip()
        {
            int width = Data.GetLength(0);
            int height = Data.GetLength(1);

            bool[,] flippedData = new bool[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    flippedData[x, y] = Data[width - x - 1, y];
                }
            }

            return new Mask(flippedData);
        }
    }
}
