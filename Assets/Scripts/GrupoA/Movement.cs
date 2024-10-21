﻿#region Copyright
// MIT License
// 
// Copyright (c) 2023 David María Arribas
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;

namespace GrupoA
{
    public class Movement : INavigationAlgorithm
    {
        public enum Directions
        {
            None,
            Up,
            Right,
            Down,
            Left
        }
        
        public class CellNode
        {
            CellInfo cellInfo;
            int acumulated;
            int value;
            CellInfo parent;
        }

        private WorldInfo _world;
        //private Random _random;
        private Directions _currentDirection = Directions.None;
        private int stepCount = 0;
        private Queue<CellNode> CP; // Cola de prioridad
        private Queue<CellNode> visitados;

        public void Initialize(WorldInfo worldInfo)
        {
            _world = worldInfo;
            //_random = new Random();
        }

        public CellInfo[] GetPath(CellInfo startNode, CellInfo targetNode)
        {
            CellInfo[] path = new CellInfo[1];
            
            if(_currentDirection == Directions.None || stepCount==0)
            {
                _currentDirection = GetRandomDirection();
                stepCount = UnityEngine.Random.Range(3, 8);
            }
            
            CellInfo nextCell = GetNeighbour(startNode, _currentDirection);
            while(!nextCell.Walkable)
            {
                _currentDirection = GetRandomDirection();
                nextCell = GetNeighbour(startNode, _currentDirection);
                stepCount = UnityEngine.Random.Range(3, 8);
            }

            stepCount--;
            path[0] = nextCell;
            return path;
        }
        
        public CellInfo[] GetNeighbours(CellInfo current) //Esta función te añade los vecinos a la cola en el orden que queremos.
        {
            CellNode[] neighbours = new CellNode[4];

            neighbours[0].
            neighbours[1] = _world[current.x + 1, current.y];
            neighbours[2] = _world[current.x, current.y + 1];
            neighbours[3] = _world[current.x - 1, current.y];

            return neighbours;
        }

        private void AddNegighbours(CellInfo[] neighbours)
        {
            float valueNode;
            int i = 0;
            while (neighbours[i].Walkable && i<=3)
            {

                for(int j = 0; j < CP.Count; j++)
                {

                    if()
                }
            }
        }

    }
}