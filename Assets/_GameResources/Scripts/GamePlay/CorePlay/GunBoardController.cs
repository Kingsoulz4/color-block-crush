using ColorBlockCrush.Tools;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

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

        [Header("References")]
        [SerializeField] private Transform gunContainer;
        [SerializeField] private ConveyorController conveyor;

        private List<List<ObjectOnGunBoardColumn>> listGunColumn = new List<List<ObjectOnGunBoardColumn>>();

        private Dictionary<int, Gun> dictGun = new();
        private int totalGunCount = 0;

        public int TotalGunCount { get => totalGunCount; }

        public void Init(LevelConfig levelConfig)
        {
            totalGunCount = 0;
            dictGun.Clear();
            SpawnGunBoard(levelConfig);
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
                }
                totalGunCount = GetGunCountInBoard();
                UpdateFrontRowFlags(col);
            }
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

            gunsToPush.Sort((a, b) => a.ColumnIndex.CompareTo(b.ColumnIndex));

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

        private void AddAllGunToPush(List<Gun> listGunToPush, Gun gun)
        {
            var stack = new Stack<Gun>();
            stack.Push(gun);
            List<Gun> visited = new();
            while (stack.Count > 0)
            {
                var gunTemp = stack.Pop();
                listGunToPush.Add(gunTemp);
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

                Vector3 currentPos = objOnColumn.transform.position;
                Vector3 newPos = new Vector3(
                    currentPos.x,
                    0,
                    currentPos.z + rowSpacing
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

        private void ShiftColumn(int column)
        {
            ShiftColumn(column, 0);
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

            List<Vector3> targetPositions = shuffleableGuns.Select(s => s.originalPosition).ToList();

            for (int i = targetPositions.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                Vector3 temp = targetPositions[i];
                targetPositions[i] = targetPositions[randomIndex];
                targetPositions[randomIndex] = temp;
            }

            float animDuration = 0.25f;

            for (int i = 0; i < shuffleableGuns.Count; i++)
            {
                Gun gun = shuffleableGuns[i].gun;
                Vector3 newPosition = targetPositions[i];

                gun.transform.DOMove(newPosition, animDuration)
                    .SetEase(DG.Tweening.Ease.InOutQuad);
            }
            GameManager.Instance.SetGameState(GameState.Paused);

            this.Wait(animDuration + 0.1f, () =>
            {
                ReorganizeInternalLists(shuffleableGuns, targetPositions);
                GameManager.Instance.SetGameState(GameState.Playing);
            });
        }

        private void ReorganizeInternalLists(List<ShuffleableGun> shuffleableGuns, List<Vector3> newPositions)
        {
            Dictionary<Vector3, Gun> positionToGunMap = new Dictionary<Vector3, Gun>();

            for (int i = 0; i < shuffleableGuns.Count; i++)
            {
                positionToGunMap[newPositions[i]] = shuffleableGuns[i].gun;
            }

            for (int col = 0; col < listGunColumn.Count; col++)
            {
                List<ObjectOnGunBoardColumn> newColumnList = new List<ObjectOnGunBoardColumn>();

                for (int row = 0; row < listGunColumn[col].Count; row++)
                {
                    ObjectOnGunBoardColumn currentObj = listGunColumn[col][row];

                    if (currentObj is LockObject ||
                        (currentObj is Gun g && (g.IsConnectedGroup() || g.GunData.isHidden)))
                    {
                        newColumnList.Add(currentObj);
                    }
                    else
                    {
                        Vector3 expectedPos = spawnOrigin + new Vector3(
                            (col * columnSpacing) + GetCenterOffset(),
                            0,
                            row * -rowSpacing
                        );

                        Gun closestGun = FindClosestGunToPosition(expectedPos, positionToGunMap);
                        if (closestGun != null)
                        {
                            newColumnList.Add(closestGun);
                            closestGun.ColumnIndex = col; 
                            positionToGunMap.Remove(closestGun.transform.position);
                        }
                    }
                }

                listGunColumn[col] = newColumnList;
                UpdateFrontRowFlags(col);
            }
        }

        private Gun FindClosestGunToPosition(Vector3 position, Dictionary<Vector3, Gun> positionToGunMap)
        {
            Gun closestGun = null;
            float minDistance = float.MaxValue;
            Vector3 closestPos = Vector3.zero;

            foreach (var kvp in positionToGunMap)
            {
                float distance = Vector3.Distance(position, kvp.Key);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestGun = kvp.Value;
                    closestPos = kvp.Key;
                }
            }

            return closestGun;
        }

        private float GetCenterOffset()
        {
            int totalColumns = listGunColumn.Count;
            float totalWidth = (totalColumns - 1) * columnSpacing;
            return -totalWidth / 2f;
        }
    }

    public class ShuffleableGun
    {
        public Gun gun;
        public int originalColumn;
        public int originalRow;
        public Vector3 originalPosition;
    }
}

