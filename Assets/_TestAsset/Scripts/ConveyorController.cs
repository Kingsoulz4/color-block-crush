using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class ConveyorController : SingletonMono<ConveyorController>
    {
        [SerializeField] private Transform[] wayPoints;

        public Transform[] WayPoints { get => wayPoints; }
    }
