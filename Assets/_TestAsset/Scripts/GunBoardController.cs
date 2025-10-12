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
        [SerializeField] private Vector3 _spawnOrigin = new Vector3(-2f, -5f, 0);
        [SerializeField] private float _columnSpacing = 1f;
        [SerializeField] private float _rowSpacing = 1f;

        [Header("Prefabs")]
        [SerializeField] private Gun _gunPrefab;

        [Header("References")]
        [SerializeField] private Transform _gunContainer;
        [SerializeField] private ConveyorController conveyor;

        private List<List<Gun>> listGunColumn = new List<List<Gun>>();

        public Action<Gun> OnGunTapped;

        public void Init(LevelConfig levelConfig)
        {
            SpawnGunBoard(levelConfig);
        }

        public void SpawnGunBoard(LevelConfig gunBoardData)
        {
            if (gunBoardData == null) return;

            int totalColumns = gunBoardData.tankLines.Where(x => x.tankLineElementConfigs.Count > 0).Count();

            float totalWidth = (totalColumns - 1) * _columnSpacing;
            float centerOffsetX = -totalWidth / 2f;

            for (int col = 0; col < totalColumns; col++)
            {
                if (gunBoardData.tankLines[col].tankLineElementConfigs.Count == 0)
                {
                    continue;
                }

                listGunColumn.Add(new List<Gun>());

                List<TankLineElementConfig> columnData = new List<TankLineElementConfig>(gunBoardData.tankLines[col].tankLineElementConfigs);
                columnData.Reverse();

                for (int i = 0; i < columnData.Count; i++)
                {
                    TankLineElementConfig data = columnData[i];
                    var gun = SpawnGun(col, i, data.tankConfig.colorType, data.tankConfig.bulletNumber, centerOffsetX);
                    if (gun != null)
                    {
                        listGunColumn[col].Add(gun);
                    }
                }

                UpdateFrontRowFlags(col);
            }
        }

        public Gun SpawnGun(int column, int row, ColorType color, int bulletCount, float centerOffsetX = 0f)
        {
            if (!IsValidColumn(column)) return null;

            Vector3 worldPos = _spawnOrigin + new Vector3(
                (column * _columnSpacing) + centerOffsetX,
                0,
                row * -_rowSpacing
            );

            Gun gun = Instantiate(_gunPrefab, worldPos, Quaternion.identity, _gunContainer);
            gun.Init(color, bulletCount, column);
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

        private void RemoveGunFromColumn(Gun gun)
        {
            if (!IsValidColumn(gun.ColumnIndex)) return;

            listGunColumn[gun.ColumnIndex].Remove(gun);
        }

        private void ShiftColumn(int column)
        {
            if (!IsValidColumn(column)) return;

            List<Gun> columnGuns = listGunColumn[column];

            // Update positions for all remaining guns
            for (int i = 0; i < columnGuns.Count; i++)
            {
                Gun gun = columnGuns[i];

                Vector3 newPos = new Vector3(
                     gun.transform.position.x, 0, gun.transform.position.z + _rowSpacing);

                gun.transform.position = newPos;
            }

            UpdateFrontRowFlags(column);
        }

        private void UpdateFrontRowFlags(int column)
        {
            if (!IsValidColumn(column)) return;

            List<Gun> columnGuns = listGunColumn[column];

            for (int i = 0; i < columnGuns.Count; i++)
            {
                // Front row is the last gun in the list (highest row)
                columnGuns[i].IsFrontRow = (i == 0);
            }
        }

        private bool IsValidColumn(int column)
        {
            return column >= 0 && column < listGunColumn.Count;
        }
    }
}