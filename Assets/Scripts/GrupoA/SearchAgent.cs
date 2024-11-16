using Navigation.Interfaces;
using Navigation.World;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace GrupoA
{
    public class SearchAgent : INavigationAgent
    {
        public CellInfo CurrentObjective { get; private set; }
        public Vector3 CurrentDestination { get; private set; }
        public int NumberOfDestinations { get; private set; }

        private WorldInfo _worldInfo;
        private INavigationAlgorithm _navigationAlgorithm;

        private List<CellInfo> _objectives;
        private Queue<CellInfo> _path;

        private bool cofresRecogidos = false;

        public void Initialize(WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm)
        {
            _worldInfo = worldInfo;
            _navigationAlgorithm = navigationAlgorithm;
        }

        public Vector3? GetNextDestination(Vector3 position)
        {
            //Buscamos los zombies que quedan en el entorno
            CellInfo[] wordlZombies = _worldInfo.Enemies;
            List<CellInfo> zombies = new List<CellInfo>();

            for (int i = 0; i < wordlZombies.Length; i++)
            {
                //Comprobamos que los zombies son zombies y no el lugar donde ha perecido uno
                if (wordlZombies[i].Type == CellInfo.CellType.Enemy)
                {
                    zombies.Add(wordlZombies[i]);
                }
            }

            if (zombies.Count() != 0) 
            {
                CellInfo enemy1 = zombies[zombies.Count() - 1];
                CurrentObjective = enemy1;

                CellInfo currentPosition = _worldInfo.FromVector3(position);

                this.GetCompletePath(currentPosition);       
            }
            else
            {

                if (_objectives == null && !cofresRecogidos)
                {
                    this.GetObjetives(position);
                    cofresRecogidos = true;
                }
                else if (_objectives.Count == 0)
                {
                    _objectives.Add(_worldInfo.Exit);
                }

                if (_path == null || _path.Count == 0)
                {
                    CellInfo currentPosition = _worldInfo.FromVector3(position);

                    //Ordenamos los objetivos según la distancia con el objetivo actual, así irá al más cercano.
                    _objectives = _objectives.OrderByDescending(p => (Math.Abs(p.x - currentPosition.x) + Math.Abs(p.y - currentPosition.y))).ToList();
                    
                    this.GetCompletePath(currentPosition);

                    //Quitamos el objetivo al del cual ya tenemos el camino
                    if (_objectives.Count > 0)
                    {
                        CurrentObjective = _objectives[_objectives.Count - 1];
                        NumberOfDestinations = _objectives.Count;
                        _objectives.Remove(_objectives.Last<CellInfo>());
                    }
                }
            }            

            if (_path.Count > 0)
            {
                CellInfo destination = _path.Dequeue();
                CurrentDestination = _worldInfo.ToWorldPosition(destination);
            }

            return CurrentDestination;
        }

        //Esta función busca los cofres que tiene que recoger el agente y los mete en la lista de objetivos.
        private void GetObjetives(Vector3 position)
        {
            CellInfo currentPosition = _worldInfo.FromVector3(position);

            List<CellInfo> targets = new List<CellInfo>();
            for (int i = 0; i < _worldInfo.Targets.Length; i++)
            {
                if (_worldInfo.Targets[i].Type == CellInfo.CellType.Treasure)
                {
                    targets.Add(_worldInfo.Targets[i]);
                }
            }

            //Ordenamos los objetivos para que vaya siempre al más cercano.
            _objectives = targets.OrderByDescending(p => (Math.Max(Math.Abs(p.x - currentPosition.x), Math.Abs(p.y - currentPosition.y)))).ToList();

            CurrentObjective = _objectives[_objectives.Count - 1];
            NumberOfDestinations = _objectives.Count;
        }

        //Llama a la función de GetPath y convierte la lista en en una cola para que sea el camino a seguir.
        private void GetCompletePath(CellInfo currentPosition)
        {
            CellInfo[] path = _navigationAlgorithm.GetPath(currentPosition, CurrentObjective);
            _path = new Queue<CellInfo>(path);
        }

    }
}