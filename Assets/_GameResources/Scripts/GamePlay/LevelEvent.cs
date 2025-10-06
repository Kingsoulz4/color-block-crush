using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geckout
{
    public class LevelEvent : SingletonMono<LevelEvent>
    {
        public static Action<int> OnWin;
        public static Action<int> OnLose;
        public static Action<int> OnLevelStart;
    }
}
