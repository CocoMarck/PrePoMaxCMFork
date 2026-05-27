using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Punto de coordenadas.
namespace CaeMesh
{
    [Serializable]
    public class CoordPoint
    {
        // Variables
        private int _id;
        private double _x;
        private double _y;
        private double _z;

        // Constructors
        public CoordPoint()
        {
        }

        public CoordPoint(int id, double x, double y, double z)
        {
            _id = id;
            _x = x;
            _y = y;
            _z = z;
        }

        // Methods
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public double[] Coor
        {
            get { return new double[] { _x, _y, _z }; }
            set
            {
                if (value == null || value.Length != 3)
                    throw new ArgumentException();

                _x = value[0];
                _y = value[1];
                _z = value[2];
            }
        }
    }
}