using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance.Milestone3
{
    public static class Extensions
    {
        public static IEnumerable<(int X, int Y)> ForEach(this (int MaxX, int MaxY) dimation)
        {
            for (int cx = 0; cx < dimation.MaxX; cx++)
            {
                for (int cy = 0; cy < dimation.MaxY; cy++)
                {
                    yield return (cx, cy);
                }
            }
        }
    }
}
