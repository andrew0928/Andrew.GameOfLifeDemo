using System;

namespace GameHost1.Roles
{
    public class World
    {
        private Zerg[,] _currentMap;
        public World()
        {

        }

        public void Init(bool[,] init_map)
        {
            if(_currentMap != null) throw new InvalidOperationException("you can only init onece.");

            _currentMap = new Zerg[init_map.GetLength(0), init_map.GetLength(1)];

            for (int y = 0; y < init_map.GetLength(1); y++)
            {
                for (int x = 0; x < init_map.GetLength(0); x++)
                {
                    _currentMap[x,y] = new Zerg(init_map[x,y]);
                }
            }
        }

        public bool[,] NextGen()
        {
            var nextWorld = new bool[ _currentMap.GetLength(0) ,_currentMap.GetLength(1)];
            for(int y = 0; y < _currentMap.GetLength(1); y++)
            {
                 for(int x = 0; x < _currentMap.GetLength(0); x++)
                {
                    nextWorld[x,y] = _currentMap[x,y].IsAlive;
                }
            }

            return nextWorld;
        }
    }
}