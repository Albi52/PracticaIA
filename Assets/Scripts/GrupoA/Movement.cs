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
using Unity.VisualScripting;
using UnityEngine.Rendering;
using Unity;
using UnityEngine;
using GrupoA;

namespace GrupoA
{
    public class Movement : INavigationAlgorithm
    {
        private WorldInfo _world;
        private List<CellNode> CP; // Cola de prioridad
        private List<CellNode> visitados;

        public void Initialize(WorldInfo worldInfo)
        {
            _world = worldInfo;
            CP = new List<CellNode>();
            visitados = new List<CellNode>();
        }

        public CellInfo[] GetPath(CellInfo startNode, CellInfo targetNode)
        {
            Debug.Log("hola");
            CellNode current = new CellNode(startNode);
            this.visitados.Add(current);

            int count = 0;

            

            while (current.getCellInfo()!=targetNode && count < 1000)
            {
                if(current.getCellInfo()==null)
                {
                    int i = 0; 
                }
                this.AddNegighbours(current, targetNode);

                Debug.Log(CP.Count);
                current = this.GetNext();
                count++;
                if (count == 999)
                {
                    Debug.Log("No sale del bucle Expansion");
                }

            }

            Stack<CellInfo> pathInverse = new Stack<CellInfo>();

            count = 0;

            while (current.getParent()!=null && count<1000)
            {
                pathInverse.Push(current.getCellInfo());
                current = current.getParent();

                count++;
                if (count == 999)
                {
                    Debug.Log("No sale del bucle Parent");
                }
            }

            count = 0;

            CellInfo[] path = new CellInfo[pathInverse.Count];

            int nodeNumber = pathInverse.Count;

            for(int i = 0; i < nodeNumber && count < 1000; i++, count ++)
            {
                path[i] = pathInverse.Pop();
                if (count == 999)
                {
                    Debug.Log("No sale del bucle Inverse");
                }
            }

            return path;
        }
        
        public CellNode[] GetNeighbours(CellNode current, CellInfo finish) //Esta función inicializa los nodos.
        {
            CellNode[] neighbours = new CellNode[4];

            for( int i = 0;i < neighbours.Length; i++)
            {
                neighbours[i] = new CellNode();
            }
            int count = 0;

            neighbours[0].setCellInfo(_world[current.getCellInfo().x, current.getCellInfo().y - 1]);
            neighbours[1].setCellInfo(_world[current.getCellInfo().x + 1, current.getCellInfo().y]);
            neighbours[2].setCellInfo(_world[current.getCellInfo().x, current.getCellInfo().y + 1]);
            neighbours[3].setCellInfo(_world[current.getCellInfo().x - 1, current.getCellInfo().y]);

            for (int i = 0; i < neighbours.Length && count < 4; i++, count++)
            {
                neighbours[i].setParent(current);

                neighbours[i].setAcumulate(current.getAcumulate() + 1);

                neighbours[i].setHeuristic(Math.Abs(finish.x - neighbours[i].getCellInfo().x)
                    + Math.Abs(finish.y - neighbours[i].getCellInfo().y));
                Debug.Log("Vecino añadido");
            }
            Debug.Log("Vecinos encontrados ~.~");

            return neighbours;
        }

        private void AddNegighbours(CellNode current, CellInfo finish)
        {
            int count = 0;
            CellNode[] neighbours = this.GetNeighbours(current, finish);

            Debug.Log("Ahora voy a añadir los nodos vecinos");

            for(int i = 0; i<neighbours.Length && count < 4; i++)
            {
                if (neighbours[i].getCellInfo().Walkable && !visitados.Contains(neighbours[i]))
                {
                    CP.Add(neighbours[i]);
                }
            }

            CP.Sort();
        }

        private CellNode GetNext()
        {
            CellNode next = new CellNode();
            if(CP.Count > 0)
            {
                next = CP[0];
                CP.RemoveAt(0);
                visitados.Add(next);
            }

            Debug.Log("Nuevo nodo en visitados, nodos en visitados: ");
            Debug.Log(visitados.Count);

            return next;
        }

    }
}