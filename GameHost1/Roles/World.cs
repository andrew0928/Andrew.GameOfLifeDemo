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
            if(_currentMap != null) throw new InvalidOperationException("you can only init once.");

            _currentMap = new Zerg[init_map.GetLength(0), init_map.GetLength(1)];

            for (int y = 0; y < init_map.GetLength(1); y++)
            {
                for (int x = 0; x < init_map.GetLength(0); x++)
                {
                    var zerg = new Zerg(init_map[x,y]);
                    _currentMap[x,y] = zerg;

                    var hasTop = false; 
                    if(y - 1 >= 0)
                    {
                        hasTop = true;
                        this.Connect(zerg, _currentMap[x,y-1]);
                    }

                    //Stupid
                    if(x-1 >= 0)
                    {
                        this.Connect(zerg, _currentMap[x-1, y]);
                        if(hasTop)
                        {
                            this.Connect(zerg, _currentMap[x-1, y-1]);
                        } 
                    }

                    if(x + 1 < _currentMap.GetLength(0) && hasTop)
                    {
                        this.Connect(zerg, _currentMap[x+1, y-1]);
                    }
                }
            }
        }

        private void Connect(Zerg a, Zerg b)
        {
            a.SendSingal += b.OnSignalReceived;
            b.SendSingal += a.OnSignalReceived;
        }

        public bool[,] NextGen()
        {
            var nextWorld = new bool[ _currentMap.GetLength(0) ,_currentMap.GetLength(1)];
            for(int y = 0; y < _currentMap.GetLength(1); y++)
            {
                 for(int x = 0; x < _currentMap.GetLength(0); x++)
                {
                    nextWorld[x,y] = _currentMap[x,y].IsAlive();
                }
            }

            return nextWorld;
        }
    }
}