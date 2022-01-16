using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAE.ReplaySystem
{
    public interface IReplayCommand
    {
        void Forward();
        void Backward();
    }
}
