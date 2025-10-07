using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class GunBoardController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _rows = 10;
        [SerializeField] private int _columns = 5;
        [SerializeField] private Vector3 _spawnOrigin = new Vector3(-2f, -5f, 0);
        [SerializeField] private Vector3 _gridNodeScale = Vector3.one;
        [SerializeField] private Vector3 _gridNodeOffset = Vector3.one;
        [SerializeField] private GridAxisTypes _axisType = GridAxisTypes.XY;
        [SerializeField] private GridAnchorTypes _anchorType = GridAnchorTypes.BottomLeft;

        [Header("Prefabs")]
        [SerializeField] private Gun _gunPrefab;

        [Header("References")]
        [SerializeField] private GridController _gridController;
        [SerializeField] private Transform _gunContainer;
        [SerializeField] private SlotController _slotController;

        private Gun[,] _guns;

        public Action<Gun> OnGunTapped;

        public void Initialize()
        {
            // Initialize grid using GridController
            _gridController.PrepareGrid(
                _rows,
                _columns,
                _spawnOrigin,
                _axisType,
                _anchorType,
                _gridNodeScale,
                _gridNodeOffset
            );

            _guns = new Gun[_rows, _columns];
        }

        public Gun SpawnGun(int row, int col, ColorType color, int bulletCount, float fireRate)
        {
            if (!IsValidPosition(row, col)) return null;
            if (_guns[row, col] != null) return null;

            GridNode node = _gridController.GridNodes[row, col];

            Gun gun = Instantiate(_gunPrefab, node.transform.position, Quaternion.identity, _gunContainer);
            gun.name = $"Gun_{row}_{col}";
            gun.Initialize(color, bulletCount, fireRate);
            gun.GridPosition = new Vector2Int(row, col);

            _guns[row, col] = gun;

            return gun;
        }

        public void OnTapGun(Gun gun)
        {
            if (gun == null) return;
            if (!CanTapGun(gun)) return;

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
            //if (!_slotController.CanPlaceGuns(requiredSlots))
            //{
            //    Debug.Log("Not enough slots available");
            //    return;
            //}

            //gunsToPush.Sort((a, b) => a.GridPosition.y.CompareTo(b.GridPosition.y));

            //_slotController.PushGuns(gunsToPush);

            foreach (Gun g in gunsToPush)
            {
                int col = g.GridPosition.y;
                RemoveGun(g);
                ShiftColumn(col);
            }

            OnGunTapped?.Invoke(gun);
        }

        private bool CanTapGun(Gun gun)
        {
            if (gun == null) return false;
            return gun.CanPushToSlot();
        }

        private void ShiftColumn(int column)
        {
            for (int row = 0; row < _rows - 1; row++)
            {
                Gun gunBelow = _guns[row + 1, column];
                _guns[row, column] = gunBelow;

                if (gunBelow != null)
                {
                    gunBelow.GridPosition = new Vector2Int(row, column);

                    // Move gun to new grid node position
                    GridNode targetNode = _gridController.GridNodes[row, column];
                    gunBelow.transform.position = targetNode.transform.position;
                }
            }

            _guns[_rows - 1, column] = null;
        }

        private void RemoveGun(Gun gun)
        {
            if (gun == null) return;

            int row = gun.GridPosition.x;
            int col = gun.GridPosition.y;

            if (IsValidPosition(row, col))
            {
                _guns[row, col] = null;
            }
        }

        public List<Gun> GetFrontRowGuns()
        {
            List<Gun> frontGuns = new List<Gun>();

            for (int c = 0; c < _columns; c++)
            {
                if (_guns[0, c] != null)
                {
                    frontGuns.Add(_guns[0, c]);
                }
            }

            return frontGuns;
        }

        public Gun GetGun(int row, int col)
        {
            if (IsValidPosition(row, col))
                return _guns[row, col];
            return null;
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < _rows && col >= 0 && col < _columns;
        }
    }
}