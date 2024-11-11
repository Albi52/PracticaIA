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
using UnityEngine;

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
            CP = new List<CellNode>();
            visitados = new List<CellNode>();

            CellNode current = new CellNode(startNode);

            //Como el curret tiene que ser visitado siempre lo añadimos a visitados.
            this.visitados.Add(current);

            int count = 0;

            //En caso de que nuestro objetivo sea un zombie no vamos a hacer todo el camino, haremos una búsqueda por horizontes (con A*)
            int horizonte = 1000;

            if(targetNode.Type == CellInfo.CellType.Enemy)
            {
                //Definimos cual es el horizonte, dependiendo de lo intricado que sea el laberinto.
                horizonte = 220;
            }

            //En nuestro caso primero miramos los vecinos y luego accedemos al nodo.
            //De esta forma no expandimos el nodo final pero sí llegamos a él y, además, expandimos el primero.
            while (current.getCellInfo()!=targetNode && count < horizonte)
            {
                if(current.getCellInfo()==null)
                {
                    int i = 0; 
                }
                this.AddNegighbours(current, targetNode);

                current = this.GetNext();
                count++;
                if (count == 999)
                {
                    Debug.Log("No sale del bucle Expansion");
                }

            }

            Stack<CellInfo> pathInverse = new Stack<CellInfo>();

            count = 0;

            //Obtenemos el camino accediendo al padre del nodo final
            //y al padre de este así hasta llegar al nodo inicial.
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

            //Invertimos el camino para que vaya de inicio a fin.
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

        //Expandimos los vecinos del nodo
        public CellNode[] GetNeighbours(CellNode current, CellInfo finish)
        {
            CellNode[] neighbours = new CellNode[4]; //Sabemos que cada nodo tiene 4 vecinos

            for( int i = 0;i < neighbours.Length; i++)
            {
                neighbours[i] = new CellNode();
            }
            int count = 0;

            neighbours[0].setCellInfo(_world[current.getCellInfo().x, current.getCellInfo().y - 1]);
            neighbours[1].setCellInfo(_world[current.getCellInfo().x + 1, current.getCellInfo().y]);
            neighbours[2].setCellInfo(_world[current.getCellInfo().x, current.getCellInfo().y + 1]);
            neighbours[3].setCellInfo(_world[current.getCellInfo().x - 1, current.getCellInfo().y]);

            //Asignamos a los atributos de los nodos los valores correspondientes.
            for (int i = 0; i < neighbours.Length && count < 4; i++, count++)
            {
                neighbours[i].setParent(current);

                neighbours[i].setAcumulate(current.getAcumulate() + 1);

                neighbours[i].setHeuristic(Math.Abs(finish.x - neighbours[i].getCellInfo().x)
                    + Math.Abs(finish.y - neighbours[i].getCellInfo().y));
            }

            return neighbours;
        }

        //Añadimos los vecinos válidos a la cola de prioridad.
        private void AddNegighbours(CellNode current, CellInfo finish)
        {
            int count = 0;
            CellNode[] neighbours = this.GetNeighbours(current, finish);

            for(int i = 0; i<neighbours.Length && count < 4; i++)
            {
                if (neighbours[i].getCellInfo().Walkable && !visitados.Contains(neighbours[i]))
                {
                    CP.Add(neighbours[i]);
                }
            }

            CP.Sort();
        }

        //Sacamos el primer nodo de la cola de prioridad y lo metemos en la lista de visitados.
        private CellNode GetNext()
        {
            CellNode next = new CellNode();
            if(CP.Count > 0)
            {
                next = CP[0];
                CP.RemoveAt(0);
                visitados.Add(next);
            }


            return next;
        }

    }
}