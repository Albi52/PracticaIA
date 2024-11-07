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

        public void Initialize(WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm)
        {
            _worldInfo = worldInfo;
            _navigationAlgorithm = navigationAlgorithm;
        }

        public Vector3? GetNextDestination(Vector3 position)
        {
            if (_objectives == null)
            {
                CellInfo currentPosition = _worldInfo.FromVector3(position);
                _objectives = GetDestinations(currentPosition).ToList<CellInfo>();
                CurrentObjective = _objectives[_objectives.Count - 1];
                NumberOfDestinations = _objectives.Count;
            }

            if (_path == null || _path.Count == 0)
            {
                CellInfo currentPosition = _worldInfo.FromVector3(position);
                CellInfo[] path = _navigationAlgorithm.GetPath(currentPosition, CurrentObjective);
                _path = new Queue<CellInfo>(path);

                if(_objectives.Count > 1)
                {
                    _objectives.Remove(_objectives.Last<CellInfo>());
                    CurrentObjective = _objectives[_objectives.Count - 1];
                    NumberOfDestinations = _objectives.Count;
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
            targetOrder.Insert(0,_worldInfo.Exit);
            return targetOrder.ToArray();
        }

    }
}