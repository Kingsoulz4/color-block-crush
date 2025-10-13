using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public interface IGameState
    {
        void OnStartGame(int level);
        void OnPauseGame(int level);
        void OnPlayingGame(int level);
        void OnLoseGame(int level);
        void OnWinGame(int level);
        
    }
}
