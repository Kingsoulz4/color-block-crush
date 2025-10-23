using ColorBlockCrush.Tools;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace ColorBlockCrush
{
    public class GunBoardController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector3 spawnOrigin = new Vector3(0f, 0f, -10f);
        [SerializeField] private float columnSpacing = 3f;
        [SerializeField] private float rowSpacing = 2f;

        [Header("Prefabs")]
        [SerializeField] private Gun gunPrefab;
        [SerializeField] private LockObject lockPrefab;
        [SerializeField] private TunnelController tunnelPrefab;

        [Header("References")]
        [SerializeField] private Transform gunContainer;
        [SerializeField] private ConveyorController conveyor;

        private List<List<ObjectOnGunBoardColumn>> listGunColumn = new List<List<ObjectOnGunBoardColumn>>();

        private Dictionary<int, Gun> dictGun = new();
        private int totalGunCount = 0;
        private LevelConfig levelConfig;

        public int TotalGunCount { get => totalGunCount; }

        public void Init(LevelConfig levelConfig)
        {
            totalGunCount = 0;
            dictGun.Clear();
            this.levelConfig = levelConfig;
            SpawnGunBoard(levelConfig);
            this.Wait(0.1f, () => // gun connect init take 1 frame
            {
                InitConnectGun();
            });
        }

        public void SpawnGunBoard(LevelConfig gunBoardData)
        {
            if (gunBoardData == null) return;

            int totalColumns = gunBoardData.gunLines.Where(x => x.gunLineElementConfigs.Count > 0).Count();

            float totalWidth = (totalColumns - 1) * columnSpacing;
            float centerOffsetX = -totalWidth / 2f;

            for (int col = 0; col < totalColumns; col++)
            {
                if (gunBoardData.gunLines[col].gunLineElementConfigs.Count == 0)
                {
                    continue;
                }

                listGunColumn.Add(new List<ObjectOnGunBoardColumn>());

                List<GunLineElementConfig> columnData = new List<GunLineElementConfig>(gunBoardData.gunLines[col].gunLineElementConfigs);

                for (int i = 0; i < columnData.Count; i++)
                {
                    GunLineElementConfig data = columnData[i];

                    if (data.elementType == GunLineElementType.Lock && lockPrefab != null)
                    {
                        var lockObj = SpawnLock(col, i, data.gunConfig, centerOffsetX);
                        if (lockObj != null)
                        {
                            listGunColumn[col].Add(lockObj);
                        }
                    }
                    else if (data.elementType == GunLineElementType.Tank)
                    {
                        var gun = SpawnGun(col, i, data.gunConfig, data.elementId, centerOffsetX);
                        if (gun != null)
                        {
                            listGunColumn[col].Add(gun);
                            dictGun[data.elementId] = gun;
                        }
                    }
                    else if (data.elementType == GunLineElementType.Tunnel)
                    {
                        var gun = SpawnTunnel(col, i, data.tunnelConfig, data.elementId, centerOffsetX);
                        if (gun != null)
                        {
                            listGunColumn[col].Add(gun);
                            //dictGun[data.elementId] = gun;
                        }
                    }

                }
                totalGunCount = GetGunCountInBoard();
                UpdateFrontRowFlags(col);
            }
        }

        public void InitConnectGun()
        {
            foreach (var c in listGunColumn)
            {
                foreach (var obj in c)
                {
                    if (obj is Gun gun)
                    {
                        gun.GetConnectedCountAll(listGunColumn);
                    }
                }
            }
        }

        public void RemoveGunByColor(ColorType colorType)
        {
            Dictionary<int, List<int>> removedIndicesByColumn = new Dictionary<int, List<int>>();

            for (int col = 0; col < listGunColumn.Count; col++)
            {
                removedIndicesByColumn[col] = new List<int>();

                for (int row = listGunColumn[col].Count - 1; row >= 0; row--)
                {
                    if (listGunColumn[col][row] is Gun gun && gun.ColorType == colorType)
                    {
                        gun.RemoveAllConnection();
                        gun.ForceDisappear();
                        RemoveObjectFromColumn(gun);
                        removedIndicesByColumn[col].Add(row);
                    }
                }
            }

            foreach (var kvp in removedIndicesByColumn)
            {
                ShiftColumn(kvp.Key, 0);
                if (kvp.Value.Count > 0)
                {
                    //ShiftColumnAll(kvp.Key);
                    
                }
            }
        }

        private void ShiftColumnAll(int column)
        {
            if (!IsValidColumn(column)) return;

            List<ObjectOnGunBoardColumn> columnObjects = listGunColumn[column];

            int totalColumns = levelConfig.gunLines.Where(x => x.gunLineElementConfigs.Count > 0).Count();
            float totalWidth = (totalColumns - 1) * columnSpacing;
            float centerOffsetX = -totalWidth / 2f;

            for (int i = 0; i < columnObjects.Count; i++)
            {
                ObjectOnGunBoardColumn objOnColumn = columnObjects[i];

                Vector3 targetPos = spawnOrigin + new Vector3(
                    (column * columnSpacing) + centerOffsetX,
                    0,
                    i * -rowSpacing
                );

                Vector3 currentPos = objOnColumn.transform.position;

                if (Vector3.Distance(currentPos, targetPos) > 0.01f)
                {
                    if (objOnColumn is Gun gun)
                    {
                        gun.MoveColumn(targetPos, 0.2f, DG.Tweening.Ease.OutQuad);
                    }
                    else if (objOnColumn is LockObject lockObject)
                    {
                        lockObject.MoveColumn(targetPos, 0.2f, DG.Tweening.Ease.OutQuad);
                    }
                    else if (objOnColumn is TunnelController tunnel)
                    {
                        tunnel.MoveColumn(targetPos, 0.2f, DG.Tweening.Ease.OutQuad);
                    }
                }

                objOnColumn.SetIndex(i);
            }

            UpdateFrontRowFlags(column);
        }
        public LockObject SpawnLock(int column, int row, GunConfig gunData, float centerOffsetX = 0f)
        {
            if (!IsValidColumn(column)) return null;

            Vector3 worldPos = spawnOrigin + new Vector3(
                (column * columnSpacing) + centerOffsetX,
                0,
                row * -rowSpacing
            );

            LockObject lockObj = Instantiate(lockPrefab, worldPos, Quaternion.identity, gunContainer);
            lockObj.Init(column);
            lockObj.name = $"Gun_{column}_{row}";

            return lockObj;
        }

        public TunnelController SpawnTunnel(int column, int row, TunnelConfig gunData, int id, float centerOffsetX = 0f)
        {
            if (!IsValidColumn(column)) return null;

            Vector3 worldPos = spawnOrigin + new Vector3(
                (column * columnSpacing) + centerOffsetX,
                0,
                row * -rowSpacing
            );

            TunnelController lockObj = Instantiate(tunnelPrefab, worldPos, Quaternion.identity, gunContainer);
            lockObj.Init(gunData, column, id);
            lockObj.name = $"Gun_{column}_{row}";

            return lockObj;
        }

        public Gun SpawnGun(int column, int row, GunConfig gunData, int id, float centerOffsetX = 0f)
        {
            if (!IsValidColumn(column)) return null;

            Vector3 worldPos = spawnOrigin + new Vector3(
                (column * columnSpacing) + centerOffsetX,
                0,
                row * -rowSpacing
            );

            Gun gun = Instantiate(gunPrefab, worldPos, Quaternion.identity, gunContainer);
            gun.Init(gunData, column, id);
            gun.OnGunEmpty += OnGunEmty;
            gun.OnGunDissapear += OnGunDissapear;
            gun.name = $"Gun_{column}_{row}";

            return gun;
        }

        public Gun SpawnNewGun(int column, int row, GunConfig gunData, int id)
        {
            int totalColumns = levelConfig.gunLines.Where(x => x.gunLineElementConfigs.Count > 0).Count();

            float totalWidth = (totalColumns - 1) * columnSpacing;
            float centerOffsetX = -totalWidth / 2f;
            var gunn = SpawnGun(column, row, gunData, id, centerOffsetX);
            listGunColumn[column].Insert(row - 1, gunn);
            gunn.SetIndex(row - 1);

            Vector3 currentPos = gunn.transform.position;
            Vector3 newPos = new Vector3(
                currentPos.x,
                0,
                currentPos.z + rowSpacing
            );

            gunn.MoveColumn(newPos, 0.2f, Ease.OutQuad);


            //ShiftColumn(column);
            return gunn;
        }

        public void OnTapGun(Gun gun)
        {
            if (gun.IsFrontRow)
            {
                this.Wait(0.15f, () =>
                {
                    gun.PlayAnim(Constant.GunAnimation.IDLE);
                });
            }

            List<Gun> gunsToPush = new List<Gun>();

            if (gun.IsConnectedGroup())
            {
                AddAllGunToPush(gunsToPush, gun);
            }
            else
            {
                gunsToPush.Add(gun);
            }

            int requiredSlots = gunsToPush.Count;
            if (!conveyor.CanPlaceGuns(requiredSlots))
            {
                Debug.Log("Not enough slots available");
                conveyor.WarnTrayText();
                return;
            }

            //gunsToPush.Sort((a, b) =>
            //{
            //    int result = a.ColumnIndex.CompareTo(b.ColumnIndex);
            //    if (result == 0)
            //        result = a.ConnectedGuns.Count.CompareTo(b.ConnectedGuns.Count); // second field

            //    return result;
            //});

            conveyor.MoveGunIn(gunsToPush);

            foreach (Gun g in gunsToPush)
            {
                int col = g.ColumnIndex;
                RemoveObjectFromColumn(g);
                ShiftColumn(col, g.Index);
            }
        }

        public void ResolveLock(LockObject lockObject)
        {
            RemoveObjectFromColumn(lockObject);
            totalGunCount--;
            ShiftColumn(lockObject.ColumnIndex, lockObject.Index);
        }

        public void ResolveTunnel(TunnelController tunnel)
        {
            RemoveObjectFromColumn(tunnel);
            ShiftColumn(tunnel.ColumnIndex, tunnel.Index);
        }

        private void AddAllGunToPush(List<Gun> listGunToPush, Gun gun)
        {
            var stack = new Stack<Gun>();
            stack.Push(gun);
            List<Gun> visited = new();
            while (stack.Count > 0)
            {
                var gunTemp = stack.Pop();
                
                if(listGunToPush.Count < 2)
                {
                    listGunToPush.Add(gunTemp);
                }
                else if (listGunToPush[0].ConnectedGuns.Count <= 1 && listGunToPush[^1].ConnectedGuns.Count <=1)
                {
                    listGunToPush.Insert(listGunToPush.Count -2, gunTemp);
                }
                else if(listGunToPush[0].ConnectedGuns.Count <= 1)
                {
                    listGunToPush.Add(gunTemp);
                }
                else if(listGunToPush[^1].ConnectedGuns.Count <= 1)
                {
                    listGunToPush.Insert(0, gunTemp);
                }

                for (int i = 0; i < gunTemp.ConnectedGuns.Count; i++)
                {
                    var linkGun = gunTemp.ConnectedGuns[i];
                    if (!listGunToPush.Contains(linkGun))
                    {
                        stack.Push(linkGun);
                    }
                }
            }
        }

        public LockObject GetPenndingLock()
        {
            LockObject lockObject = null;

            for (int i = 0; i < listGunColumn.Count; i++)
            {
                if (listGunColumn[i].Count > 0 && listGunColumn[i][0] is LockObject lockObj && !lockObj.IsResolved)
                {
                    return lockObj;
                }
            }

            return lockObject;
        }

        private void RemoveObjectFromColumn(ObjectOnGunBoardColumn gun)
        {
            if (!IsValidColumn(gun.ColumnIndex)) return;

            listGunColumn[gun.ColumnIndex].Remove(gun);
        }

        private void ShiftColumn(int column, int removedIndex)
        {
            if (!IsValidColumn(column)) return;

            List<ObjectOnGunBoardColumn> columnObjects = listGunColumn[column];

            for (int i = removedIndex; i < columnObjects.Count; i++)
            {
                ObjectOnGunBoardColumn objOnColumn = columnObjects[i];
                

                if (!objOnColumn.CanShift)
                {
                    objOnColumn.UpdateWhenColumnChange();
                    return;
                }

                objOnColumn.SetIndex(i);
                objOnColumn.UpdateWhenColumnChange();


                Vector3 currentPos = objOnColumn.transform.position;

                Vector3 newPos = spawnOrigin + new Vector3(
                    currentPos.x,
                    0,
                     i * -rowSpacing
                );

                if (objOnColumn is Gun gun)
                {
                    gun.MoveColumn(newPos, 0.2f, DG.Tweening.Ease.OutQuad);
                }
                else if (objOnColumn is LockObject lockObject)
                {
                    lockObject.MoveColumn(newPos, 0.2f, DG.Tweening.Ease.OutQuad);
                }
            }

            UpdateFrontRowFlags(column);
        }

        private void UpdateFrontRowFlags(int column)
        {
            if (!IsValidColumn(column)) return;

            List<ObjectOnGunBoardColumn> columnGuns = listGunColumn[column];

            for (int i = 0; i < columnGuns.Count; i++)
            {
                // Front row is the last gun in the list (highest row)
                columnGuns[i].SetIndex(i);
                columnGuns[i].UpdateWhenColumnChange();
            }
        }

        private bool IsValidColumn(int column)
        {
            return column >= 0 && column < listGunColumn.Count;
        }

        public Gun GetGunByID(int id)
        {
            return dictGun[id];
        }

        private void OnGunEmty(Gun gun)
        {
            gun.transform.SetParent(gunContainer);
        }

        private void OnGunDissapear(Gun gun)
        {
            totalGunCount -= 1;

            if (totalGunCount <= 5)
            {
                LevelEvent.OnFastMode?.Invoke();
            }

            if (totalGunCount <= 0)
            {
                this.Wait(0.2f, () =>
                {
                    LevelController.Instance.WinLevel();
                });
            }
        }

        public int GetGunCountInBoard()
        {
            int count = 0;
            for (int i = 0; i < listGunColumn.Count; i++)
            {
                count += listGunColumn[i].Count;
            }
            return count;
        }

        public float GetPercentGunCleared()
        {
            return totalGunCount;
        }    

        public void ShuffleBoard()
        {
            List<ShuffleableGun> shuffleableGuns = new List<ShuffleableGun>();

            for (int col = 0; col < listGunColumn.Count; col++)
            {
                for (int row = 0; row < listGunColumn[col].Count; row++)
                {
                    if (listGunColumn[col][row] is Gun gun)
                    {
                        if (gun.IsConnectedGroup())
                        {
                            continue;
                        }

                        shuffleableGuns.Add(new ShuffleableGun
                        {
                            gun = gun,
                            originalColumn = col,
                            originalRow = row,
                            originalPosition = gun.transform.position
                        });
                    }
                }
            }

            if (shuffleableGuns.Count <= 1)
            {
                Debug.Log("Not enough guns to shuffle");
                return;
            }

            List<Gun> shuffledGuns = shuffleableGuns.Select(s => s.gun).ToList();

            for (int i = shuffledGuns.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                Gun temp = shuffledGuns[i];
                shuffledGuns[i] = shuffledGuns[randomIndex];
                shuffledGuns[randomIndex] = temp;
            }

            float animDuration = 0.25f;

            for (int i = 0; i < shuffleableGuns.Count; i++)
            {
                Gun gun = shuffledGuns[i];
                Vector3 targetPosition = shuffleableGuns[i].originalPosition;

                gun.transform.DOMove(targetPosition, animDuration)
                    .SetEase(DG.Tweening.Ease.InOutQuad);
            }

            GameManager.Instance.SetGameState(GameState.Paused);

            this.Wait(animDuration + 0.1f, () =>
            {
                ReorganizeInternalLists(shuffleableGuns, shuffledGuns);
            });

            this.Wait(animDuration + 0.8f, () =>
            {
                GameManager.Instance.SetGameState(GameState.Playing);
            });
        }

        private void ReorganizeInternalLists(List<ShuffleableGun> originalSlots, List<Gun> shuffledGuns)
        {
            for (int i = 0; i < originalSlots.Count; i++)
            {
                ShuffleableGun slot = originalSlots[i];
                Gun newGun = shuffledGuns[i];

                //Debug.Log($"Attempting to place gun at col={slot.originalColumn}, row={slot.originalRow}");
                //Debug.Log($"listGunColumn[{slot.originalColumn}].Count = {listGunColumn[slot.originalColumn].Count}");

                if (slot.originalRow >= listGunColumn[slot.originalColumn].Count)
                {
                    Debug.LogError($"OUT OF RANGE: row {slot.originalRow} >= count {listGunColumn[slot.originalColumn].Count}");
                    continue;
                }

                newGun.ColumnIndex = slot.originalColumn;
                listGunColumn[slot.originalColumn][slot.originalRow] = newGun;
            }

            for (int col = 0; col < listGunColumn.Count; col++)
            {
                UpdateFrontRowFlags(col);
            }
        }

        private class ShuffleableGun
        {
            public Gun gun;
            public int originalColumn;
            public int originalRow;
            public Vector3 originalPosition;
        }
    }
}

