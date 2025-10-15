using ColorBlockCrush.Tools;
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

        public Action<Gun> OnGunTapped;

        public void Init(LevelConfig levelConfig)
        {
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

                    if (data.gunConfig.hasLock && lockPrefab != null)
                    {
                        var lockObj = SpawnLock(col, i, data.gunConfig, centerOffsetX);
                        if (lockObj != null)
                        {
                            listGunColumn[col].Add(lockObj);
                        }
                    }
                    else if(!data.gunConfig.hasLock)
                    {
                        var gun = SpawnGun(col, i, data.gunConfig, centerOffsetX);
                        if (gun != null)
                        {
                            listGunColumn[col].Add(gun);
                        }
                    }
                }

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
            //gun.Init(gunData, column);
            lockObj.name = $"Gun_{column}_{row}";

            return lockObj;
        }

        public Gun SpawnGun(int column, int row, GunConfig gunData, float centerOffsetX = 0f)
        {
            if (!IsValidColumn(column)) return null;

            Vector3 worldPos = spawnOrigin + new Vector3(
                (column * columnSpacing) + centerOffsetX,
                0,
                row * -rowSpacing
            );

            Gun gun = Instantiate(gunPrefab, worldPos, Quaternion.identity, gunContainer);
            gun.Init(gunData, column);
            gun.name = $"Gun_{column}_{row}";

            return gun;
        }

        public void OnTapGun(Gun gun)
        {
            List<Gun> gunsToPush = new List<Gun>();

            if (gun.IsConnectedGroup())
            {
                gunsToPush.Add(gun);
                gunsToPush.AddRange(gun.ConnectedGuns);
            }
            else
            {
                gunsToPush.Add(gun);
            }

            int requiredSlots = gunsToPush.Count;
            if (!conveyor.CanPlaceGuns(requiredSlots))
            {
                Debug.Log("Not enough slots available");
                return;
            }

            gunsToPush.Sort((a, b) => a.ColumnIndex.CompareTo(b.ColumnIndex));

            conveyor.MoveGunIn(gunsToPush);

            foreach (Gun g in gunsToPush)
            {
                int col = g.ColumnIndex;
                RemoveGunFromColumn(g);
                ShiftColumn(col);
            }

            OnGunTapped?.Invoke(gun);
        }

        public LockObject GetPenndingLock()
        {
            LockObject lockObject = null;

            for(int i=0; i<listGunColumn.Count; i++)
            {
                if (listGunColumn[i][0] is LockObject lockObj)
                {
                    return lockObj;
                }
            }

            return lockObject;
        }

        private void RemoveGunFromColumn(Gun gun)
        {
            if (!IsValidColumn(gun.ColumnIndex)) return;

            listGunColumn[gun.ColumnIndex].Remove(gun);
        }

        private void ShiftColumn(int column)
        {
            if (!IsValidColumn(column)) return;

            List<ObjectOnGunBoardColumn> columnGuns = listGunColumn[column];

            // Update positions for all remaining guns
            for (int i = 0; i < columnGuns.Count; i++)
            {
                ObjectOnGunBoardColumn objOnColumn = columnGuns[i];

                if (objOnColumn is Gun gun)
                {
                    Vector3 newPos = new Vector3(
                         gun.transform.position.x, 0, gun.transform.position.z + rowSpacing);

                    gun.MoveColumn(newPos, 0.2f, DG.Tweening.Ease.OutQuad);
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
    }
}