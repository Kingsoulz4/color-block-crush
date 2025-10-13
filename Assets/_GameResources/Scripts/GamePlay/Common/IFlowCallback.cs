using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public interface IFlowCallback
    {
        void Execute(Action callback);
    }
}
