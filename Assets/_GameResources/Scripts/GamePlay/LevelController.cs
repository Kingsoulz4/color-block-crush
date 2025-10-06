using Geckout.Data;
using Geckout.PathFinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Geckout
{
    public partial class LevelController : MonoBehaviour
    {
        [SerializeField] protected GameLevelData m_gameLevelData;
        [SerializeField] protected GameMap m_gameMap;
        [SerializeField] protected Transform m_bodyParent;

        
        [SerializeField] private TutorialHandGuide m_tutorialHandGuide;
       

        public GameLevelData GameLevelData => m_gameLevelData;

        public GameMap GameMap => m_gameMap;



        public void SetLevelData(GameLevelData gameLevelData)
        {
            m_gameLevelData = gameLevelData;
        }

        public void StartLevel()
        {
        }

        private void WinLevel()
        {
            LevelEvent.OnWin?.Invoke(m_gameLevelData.levelNum);
        }

        private void LoseLevel()
        {
            LevelEvent.OnLose?.Invoke(m_gameLevelData.levelNum);
        }

        private void ReviveLevel()
        {
        }

        #region Boosters

        public void Suffle()
        {
        }

        public void MoveGun(float timeFreeze)
        {
        }

        public void ClearBlock(float timeFreeze)
        {
        }

        #endregion


        #region Tutorial

        public void HideAllTuts()
        {
            m_tutorialHandGuide.gameObject.SetActive(false);
        }    

        public void ShowTurialHandGuide(List<Vector3> path)
        {
            m_tutorialHandGuide.ShowGuidePath(path);
        }

        public void ActiveTutLevel1()
        {
        }
        #endregion

    }
}
