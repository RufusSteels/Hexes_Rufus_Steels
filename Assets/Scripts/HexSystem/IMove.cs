using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem
{
    interface IMove <TPosition>
        where TPosition : IPosition
    {
        bool CanExecute(Character<TPosition> piece);
        void Execute(Character<TPosition> piece, List<TPosition> position);
        List<TPosition> Positions(Character<TPosition> piece);
    }
}
