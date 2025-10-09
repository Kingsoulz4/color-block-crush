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
        [SerializeField] private float moveOutDuration = 0.2f;
        [SerializeField] private float moveInDuration = 0.2f;
        [SerializeField] private Ease shiftEase = Ease.OutQuad;

        private Gun myGun;
        private Vector3 originRotation = new Vector3(0, 0, -90);
        private Vector3 targetRotation = Vector3.zero;
        private Sequence moveToConveyorSq;

        public SplineAnimate SplineAnimate { get => splineAnimate; set => splineAnimate = value; }
        public Gun MyGun { get => myGun; set => myGun = value; }

        public void Init()
        {
            model.transform.Rotate(originRotation);
            DOTween.Kill(this);
        }

        public void ResetTray(Vector3 endPos, Action callback = null)
        {
            splineAnimate.Pause();
            myGun = null;
            SplineAnimate.Container = null;

            if (moveToConveyorSq != null && moveToConveyorSq.IsPlaying())
            {
                moveToConveyorSq.Kill();
            }
            else
            {
                moveToConveyorSq = DOTween.Sequence();
            }

            moveToConveyorSq.Append(transform.DOLocalMove(endPos, moveInDuration).SetEase(shiftEase));
            moveToConveyorSq.Join(model.DORotate(originRotation, moveInDuration)).OnComplete(() =>
            {
                transform.DORotate(targetRotation, 0).SetId(this);
                model.DORotate(originRotation, 0).SetId(this);
                callback?.Invoke();
            });
            moveToConveyorSq.Play();
        }

        public void SetChild(Gun gun)
        {
            myGun = gun;
            gun.transform.SetParent(gunParent);
        }

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

            moveToConveyorSq.Append(transform.DOMove(endPos, moveOutDuration)).OnComplete(() =>
            {
                callback?.Invoke();
            });

            moveToConveyorSq.Join(model.DORotate(targetRotation, moveOutDuration));
            moveToConveyorSq.SetId(this);
            moveToConveyorSq.Play();
        }

        public void Move()
        {
            splineAnimate.Play();
        }
    }
}
