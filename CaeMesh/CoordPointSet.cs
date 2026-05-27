using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;

// Set de puntos de coordenadas
namespace CaeMesh
{
    [Serializable]
    public class CoordPointSet : FeGroup
    {
        // Variables
        private List<CoordPoint> _points;

        // Constructors
        public CoordPointSet(string name)
            : base(name, new int[0])
        {
            _points = new List<CoordPoint>();
        }

        // Methods
        public List<CoordPoint> Points
        {
            get { return _points; }
            set { _points = value; }
        }

        public void AddPoint(int id, double x, double y, double z)
        {
            _points.Add(
                new CoordPoint(id, x, y, z)
            );
        }
    }
}