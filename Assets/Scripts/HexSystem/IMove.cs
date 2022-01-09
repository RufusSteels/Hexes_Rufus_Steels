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
        bool CanExecute(Character<TPosition> character);
        void Execute(Character<TPosition> character, List<TPosition> position);
        List<TPosition> ValidPositions(Character<TPosition> character);
        List<TPosition> AffectedPositions(Character<TPosition> character, TPosition position);
    }
}
