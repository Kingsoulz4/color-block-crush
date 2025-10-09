using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace ColorBlockCrush
{
    public class TrayItem : MonoBehaviour
    {
        [SerializeField] private Transform gunParent;
        [SerializeField] private Transform model;
        [SerializeField] private SplineAnimate splineAnimate;
        private Gun myGun;
        public SplineAnimate SplineAnimate { get => splineAnimate; set => splineAnimate = value; }
        public Gun MyGun { get => myGun; set => myGun = value; }

        public void Init()
        {
            model.transform.Rotate(originRotation);
            myGun = null;
            SplineAnimate = null;
            DOTween.Kill(this);
        }

        public void SetChild(Gun gun)
        {
            myGun = gun;
            gun.transform.SetParent(gunParent);
        }

        public void SetSplineContainer(SplineContainer splineContainer)
        {
            splineAnimate.Container = splineContainer;
        }

        private Vector3 originRotation = new Vector3(0, -90, 0);
        private Vector3 targetRotation = Vector3.zero;
        Sequence moveToConveyorSq;
        public void MoveToConeyor(Vector3 endPos, Action callback = null)
        {
            if (moveToConveyorSq != null && moveToConveyorSq.IsPlaying())
            {
                moveToConveyorSq.Kill();
            }
            else
            {
                moveToConveyorSq = DOTween.Sequence();
            }

            moveToConveyorSq.Append(transform.DOMove(endPos, 0.2f)).OnComplete(() =>
            {
                callback?.Invoke();
            });

            moveToConveyorSq.Join(model.DORotate(targetRotation, 0.2f));
            moveToConveyorSq.SetId(this);
            moveToConveyorSq.Play();
        }

        public void Move()
        {
            splineAnimate.Play();
        }
    }
}
