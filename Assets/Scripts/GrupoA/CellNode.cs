using Navigation.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrupoA
{
    public class CellNode : IComparable<CellNode>, IEquatable<CellNode>
    {
        CellInfo cellInfo;
        int acumulated;
        int heuristicValue;
        int value;
        CellNode parent;

        public CellNode(CellInfo nod)
        {
            this.acumulated = 0;
            this.heuristicValue = 0;
            this.value = 0;
            this.cellInfo = nod;
            this.parent = null;
        }
        public CellNode()
        {
            this.acumulated = 0;
            this.heuristicValue = 0;
            this.value = 0;
            this.cellInfo = null;
            this.parent = null;
        }

        public void setCellInfo(CellInfo cel)
        {
            this.cellInfo = cel;
        }
        public CellInfo getCellInfo()
        {
            return this.cellInfo;
        }
        public void setParent(CellNode parent)
        {
            this.parent = parent;
        }
        public CellNode getParent()
        {
            return this.parent;
        }
        public int getValue()
        {
            return this.value;
        }
        public void setHeuristic(int value)
        {
            this.heuristicValue = value;
            calculateValue();

        }
        public void setAcumulate(int value)
        {
            this.acumulated = value;
            calculateValue();
        }
        public int getAcumulate()
        {
            return this.acumulated;
        }
        private void calculateValue()
        {
            this.value = this.acumulated + this.heuristicValue;
        }

        //Esta función nos permite ver mejor al hacer 
        public override string ToString()
        {
            return this.cellInfo.ToString();
        }

        public int CompareTo(CellNode other)
        {
            if (this.value == other.value) return 0;
            else if (this.value > other.value) return 1;
            else if (this.value < other.value) return -1;
            else return 1;
        }

        public bool Equals(CellNode other)
        {
            return this.getCellInfo() == other.getCellInfo();
        }
    }

}