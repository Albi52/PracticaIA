using Navigation.Interfaces;
using Navigation.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Unity;
using UnityEngine.UIElements;
using GrupoA;
using UnityEditor;

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
            
            CellInfo[] wordlZombies = _worldInfo.Enemies;
            List<CellInfo> zombies = new List<CellInfo>();

            for (int i = 0; i < wordlZombies.Length; i++)
            {
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
                this.GetCompletePathEnemies(currentPosition);

                //Debug.Log(enemy[1] + ", " + enemy[0]);
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

                    Debug.Log(_objectives.Count);
                    _objectives = _objectives.OrderByDescending(p => (Math.Abs(p.x - currentPosition.x) + Math.Abs(p.y - currentPosition.y))).ToList();
                    
                    this.GetCompletePath(currentPosition);
                }
            }            

            if (_path.Count > 0)
            {
                CellInfo destination = _path.Dequeue();
                CurrentDestination = _worldInfo.ToWorldPosition(destination);
            }

            return CurrentDestination;
        }

        private CellInfo[] GetDestinations(CellInfo position)
        {
            List<CellInfo> targets = new List<CellInfo>();
            for (int i = 0; i < _worldInfo.Targets.Length; i++)
            {
                targets.Add(_worldInfo.Targets[i]);
            }
            List<CellInfo> targetOrder = targets.OrderByDescending(p => (Math.Abs(p.x - position.x) + Math.Abs(p.y - position.y))).ToList();
            //targetOrder.Insert(0,_worldInfo.Exit);
            return targetOrder.ToArray();
        }

        private CellInfo[] GetEnemies (CellInfo position)
        {
            List<CellInfo> enemies = new List<CellInfo>();
            for (int i = 0; i < _worldInfo.Enemies.Length; i++)
            {
                enemies.Add(_worldInfo.Enemies[i]);
            }
            List<CellInfo> targetOrder = enemies.OrderByDescending(p => (Math.Abs(p.x - position.x) + Math.Abs(p.y - position.y))).ToList();

            return targetOrder.ToArray();
        }

        private void GetObjetives(Vector3 position)
        {
            CellInfo currentPosition = _worldInfo.FromVector3(position);
            _objectives = GetDestinations(currentPosition).ToList<CellInfo>();
            CurrentObjective = _objectives[_objectives.Count - 1];
            NumberOfDestinations = _objectives.Count;
        }

        private void GetCompletePath(CellInfo currentPosition)
        {
            CellInfo[] path = _navigationAlgorithm.GetPath(currentPosition, CurrentObjective);
            _path = new Queue<CellInfo>(path);

            if (_objectives.Count > 0)
            {
                CurrentObjective = _objectives[_objectives.Count - 1];
                NumberOfDestinations = _objectives.Count;
                _objectives.Remove(_objectives.Last<CellInfo>());
            }
        }

        private void GetCompletePathEnemies (CellInfo currentPosition)
        {
            CellInfo[] path = _navigationAlgorithm.GetPath(currentPosition, CurrentObjective);
            _path = new Queue<CellInfo>(path);
        }

    }
}