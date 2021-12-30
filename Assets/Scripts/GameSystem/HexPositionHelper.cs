using DAE.BoardSystem;
using DAE.HexSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DAE.GameSystem
{
    [CreateAssetMenu(menuName = "DAE/HexPositionHelper")]
    class HexPositionHelper : ScriptableObject
    {
        [SerializeField]
        private float _tileSize = 1;
        public float TileSize
        {
            get 
            {
                return _tileSize; 
            }
        }

        private Layout _layout;// = new Layout(Layout.pointy, Vector2.one, Vector2.zero);

        private void OnValidate()
        {
            _layout = new Layout(Layout.pointy, new Vector2(_tileSize, _tileSize), Vector2.zero);
        }

        public Vector3 AxialToWorldPosition(int q, int r)
        {
            Vector3 worldPos = new Vector3(_tileSize * (Mathf.Sqrt(3) * q + Mathf.Sqrt(3) / 2 * r), 0, _tileSize * (3f/2f * r));
            return worldPos;
        }
        public (int q, int r) WorldToAxialPosition(/*Board<Character<Tile>, Tile> board, Grid<Tile> grid, */Vector3 position)
        {
            FractionalHex hex = _layout.PixelToHex(new Vector2(position.x, position.z));
            Hex hexR = hex.HexRound();
            return (hexR.q, hexR.r);
        }
    }
    struct Hex
    {
        public Hex(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }
        public readonly int q;
        public readonly int r;
        public readonly int s;

        public Hex Add(Hex b)
        {
            return new Hex(q + b.q, r + b.r, s + b.s);
        }


        public Hex Subtract(Hex b)
        {
            return new Hex(q - b.q, r - b.r, s - b.s);
        }


        public Hex Scale(int k)
        {
            return new Hex(q * k, r * k, s * k);
        }


        public Hex RotateLeft()
        {
            return new Hex(-s, -q, -r);
        }


        public Hex RotateRight()
        {
            return new Hex(-r, -s, -q);
        }

        static public List<Hex> directions = new List<Hex> 
        { 
            new Hex(1, 0, -1), 
            new Hex(1, -1, 0), 
            new Hex(0, -1, 1), 
            new Hex(-1, 0, 1), 
            new Hex(-1, 1, 0), 
            new Hex(0, 1, -1) 
        };

        static public Hex Direction(int direction)
        {
            return Hex.directions[direction];
        }


        public Hex Neighbor(int direction)
        {
            return Add(Hex.Direction(direction));
        }

        static public List<Hex> diagonals = new List<Hex> { new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1), new Hex(1, 1, -2) };

        public Hex DiagonalNeighbor(int direction)
        {
            return Add(Hex.diagonals[direction]);
        }


        public int Length()
        {
            return (int)((Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2);
        }


        public int Distance(Hex b)
        {
            return Subtract(b).Length();
        }

    }

    struct FractionalHex
    {
        public FractionalHex(float q, float r, float s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (Mathf.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
        }
        public readonly float q;
        public readonly float r;
        public readonly float s;

        public Hex HexRound()
        {
            int qi = (int)(Mathf.Round(q));
            int ri = (int)(Mathf.Round(r));
            int si = (int)(Mathf.Round(s));
            float q_diff = Mathf.Abs(qi - q);
            float r_diff = Mathf.Abs(ri - r);
            float s_diff = Mathf.Abs(si - s);
            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else
                if (r_diff > s_diff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }
            return new Hex(qi, ri, si);
        }


        public FractionalHex HexLerp(FractionalHex b, float t)
        {
            return new FractionalHex(q * (1.0f - t) + b.q * t, r * (1.0f - t) + b.r * t, s * (1.0f - t) + b.s * t);
        }


        static public List<Hex> HexLinedraw(Hex a, Hex b)
        {
            int N = a.Distance(b);
            FractionalHex a_nudge = new FractionalHex(a.q + 1e-06f, a.r + 1e-06f, a.s - 2e-06f);
            FractionalHex b_nudge = new FractionalHex(b.q + 1e-06f, b.r + 1e-06f, b.s - 2e-06f);
            List<Hex> results = new List<Hex> { };
            float step = 1.0f / Mathf.Max(N, 1);
            for (int i = 0; i <= N; i++)
            {
                results.Add(a_nudge.HexLerp(b_nudge, step * i).HexRound());
            }
            return results;
        }

    }

    struct OffsetCoord
    {
        public OffsetCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        public readonly int col;
        public readonly int row;
        static public int EVEN = 1;
        static public int ODD = -1;

        static public OffsetCoord QoffsetFromCube(int offset, Hex h)
        {
            int col = h.q;
            int row = h.r + (int)((h.q + offset * (h.q & 1)) / 2);
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new OffsetCoord(col, row);
        }


        static public Hex QoffsetToCube(int offset, OffsetCoord h)
        {
            int q = h.col;
            int r = h.row - (int)((h.col + offset * (h.col & 1)) / 2);
            int s = -q - r;
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new Hex(q, r, s);
        }


        static public OffsetCoord RoffsetFromCube(int offset, Hex h)
        {
            int col = h.q + (int)((h.r + offset * (h.r & 1)) / 2);
            int row = h.r;
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new OffsetCoord(col, row);
        }


        static public Hex RoffsetToCube(int offset, OffsetCoord h)
        {
            int q = h.col - (int)((h.row + offset * (h.row & 1)) / 2);
            int r = h.row;
            int s = -q - r;
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new Hex(q, r, s);
        }

    }

    struct DoubledCoord
    {
        public DoubledCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        public readonly int col;
        public readonly int row;

        static public DoubledCoord QfloatdFromCube(Hex h)
        {
            int col = h.q;
            int row = 2 * h.r + h.q;
            return new DoubledCoord(col, row);
        }


        public Hex QfloatdToCube()
        {
            int q = col;
            int r = (int)((row - col) / 2);
            int s = -q - r;
            return new Hex(q, r, s);
        }


        static public DoubledCoord RfloatdFromCube(Hex h)
        {
            int col = 2 * h.q + h.r;
            int row = h.r;
            return new DoubledCoord(col, row);
        }


        public Hex RfloatdToCube()
        {
            int q = (int)((col - row) / 2);
            int r = row;
            int s = -q - r;
            return new Hex(q, r, s);
        }

    }

    struct Orientation
    {
        public Orientation(float f0, float f1, float f2, float f3, float b0, float b1, float b2, float b3, float start_angle)
        {
            this.f0 = f0;
            this.f1 = f1;
            this.f2 = f2;
            this.f3 = f3;
            this.b0 = b0;
            this.b1 = b1;
            this.b2 = b2;
            this.b3 = b3;
            this.start_angle = start_angle;
        }
        public readonly float f0;
        public readonly float f1;
        public readonly float f2;
        public readonly float f3;
        public readonly float b0;
        public readonly float b1;
        public readonly float b2;
        public readonly float b3;
        public readonly float start_angle;
    }

    struct Layout
    {
        public Layout(Orientation orientation, Vector2 size, Vector2 origin)
        {
            this.orientation = orientation;
            this.size = size;
            this.origin = origin;
        }
        public readonly Orientation orientation;
        public readonly Vector2 size;
        public readonly Vector2 origin;
        static public Orientation pointy = new Orientation(Mathf.Sqrt(3.0f), Mathf.Sqrt(3.0f) / 2.0f, 0.0f, 3.0f / 2.0f, Mathf.Sqrt(3.0f) / 3.0f, -1.0f / 3.0f, 0.0f, 2.0f / 3.0f, 0.5f);
        static public Orientation flat = new Orientation(3.0f / 2.0f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f, Mathf.Sqrt(3.0f), 2.0f / 3.0f, 0.0f, -1.0f / 3.0f, Mathf.Sqrt(3.0f) / 3.0f, 0.0f);

        public Vector2 HexToPixel(Hex h)
        {
            Orientation M = orientation;
            float x = (M.f0 * h.q + M.f1 * h.r) * size.x;
            float y = (M.f2 * h.q + M.f3 * h.r) * size.y;
            return new Vector2(x + origin.x, y + origin.y);
        }


        public FractionalHex PixelToHex(Vector2 p)
        {
            Orientation M = orientation;
            Vector2 pt = new Vector2((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
            float q = M.b0 * pt.x + M.b1 * pt.y;
            float r = M.b2 * pt.x + M.b3 * pt.y;
            return new FractionalHex(q, r, -q - r);
        }


        public Vector2 HexCornerOffset(int corner)
        {
            Orientation M = orientation;
            float angle = 2.0f * Mathf.PI * (M.start_angle - corner) / 6.0f;
            return new Vector2(size.x * Mathf.Cos(angle), size.y * Mathf.Sin(angle));
        }


        public List<Vector2> PolygonCorners(Hex h)
        {
            List<Vector2> corners = new List<Vector2> { };
            Vector2 center = HexToPixel(h);
            for (int i = 0; i < 6; i++)
            {
                Vector2 offset = HexCornerOffset(i);
                corners.Add(new Vector2(center.x + offset.x, center.y + offset.y));
            }
            return corners;
        }

    }
}
