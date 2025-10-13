using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ColorBlockCrush
{
    public partial class GameMap : SingletonMono<GameMap>
    {
        private static GameMap _instance;
    }
}