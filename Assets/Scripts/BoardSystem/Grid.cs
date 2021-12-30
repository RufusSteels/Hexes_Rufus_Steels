using DAE.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.BoardSystem
{
    public class Grid<TPosition>
    {
        private BidirectionalDictionary<TPosition, (int x, int y)> _positions 
            = new BidirectionalDictionary<TPosition, (int x, int y)>();
        public int Rows { get; }
        public int Columns { get; }

        public Grid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        public bool TryGetPositionAt(int x, int y, out TPosition position)
        {
            return _positions.TryGetKey((x, y), out position);
        }

        public bool TryGetCoordinateAt(TPosition position, out (int x, int y) coordinate) //(int x, int y) is een tuple, kan bijv ook (string a, int b, double c) zijn.
        {
            return _positions.TryGetValue(position, out coordinate);
        }

        public void Register(TPosition position, int x, int y)
        {
            if (x < 0 || x >= Columns)
                throw new ArgumentException(nameof(x));

            if (y < 0 || y >= Rows)
                throw new ArgumentException(nameof(y));

            _positions.Add(position, (x, y));
        }
    }
}
