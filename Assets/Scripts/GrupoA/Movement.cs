#region Copyright
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
            int acumulated = 0;
            int heuristicValue = 0;
            int value;
            CellInfo parent;

            public void setCellInfo(CellInfo cel)
            {
                this.cellInfo = cel;
            }
            public CellInfo getCellInfo()
            {
                return this.cellInfo;
            }
            public void setParent(CellInfo parent)
            {
                this.parent = parent;
            }
            public int getValue()
            {
                return this.value;
            }
            public void setHeuristic(int value)
            {
                this.heuristicValue = value;
            }
            public void setAcumulate(int value)
            {
                this.acumulated = value;
            }
            public int getAcumulate()
            {
                return this.acumulated;
            }
            private void calculateValue()
            {

            }
            
        }

        private WorldInfo _world;
        //private Random _random;
        private Directions _currentDirection = Directions.None;
        private int stepCount = 0;
        private List<CellNode> CP; // Cola de prioridad
        private List<CellNode> visitados;

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
        
        public CellNode[] GetNeighbours(CellNode current) //Esta función te añade los vecinos a la cola en el orden que queremos.
        {
            CellNode[] neighbours = new CellNode[4];

            neighbours[0].setCellInfo(_world[current.getCellInfo().x, current.getCellInfo().y-1]);
            neighbours[1].setCellInfo(_world[current.getCellInfo().x + 1, current.getCellInfo().y]);
            neighbours[2].setCellInfo(_world[current.getCellInfo().x, current.getCellInfo().y + 1]);
            neighbours[3].setCellInfo(_world[current.getCellInfo().x - 1, current.getCellInfo().y]);

            neighbours[0].setParent(current.getCellInfo());
            neighbours[1].setParent(current.getCellInfo());
            neighbours[2].setParent(current.getCellInfo());
            neighbours[3].setParent(current.getCellInfo());

            neighbours[0].setAcumulate(current.getAcumulate() + 1);
            neighbours[1].setAcumulate(current.getAcumulate() + 1);
            neighbours[2].setAcumulate(current.getAcumulate() + 1);
            neighbours[3].setAcumulate(current.getAcumulate() + 1);

            return neighbours;
        }

        private void AddNegighbours(CellNode[] neighbours)
        {
            float valueNode;
            int i = 0;
            while (neighbours[i].getCellInfo().Walkable && i<=neighbours.Length)
            {

                for(int j = 0; j < CP.Count; j++)
                {
                    if (neighbours[i].getValue() < CP[j].getValue())
                    {
                        CP.Insert(j, neighbours[i]);
                    }
                }
            }
        }

    }
}