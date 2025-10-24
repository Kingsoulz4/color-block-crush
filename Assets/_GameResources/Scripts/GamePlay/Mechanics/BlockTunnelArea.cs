using ColorBlockCrush.Tools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BlockTunnelArea : Block
    {
        [SerializeField] private TextMeshPro m_textHealth;

        private Vector3 defaultScale;

        private List<Block> listBlockPlace = new();

        private TunnelAreaConfig tunnelData;

        private int currentElementIndex = 0;

        private int currentElementHealthRemain = 0;

        public void Init(TunnelAreaConfig tunnelAreaConfig)
        {
            tunnelData = tunnelAreaConfig;
            hitPoint = tunnelAreaConfig.elements.Sum(x => x.health);
            maxHitPoint = hitPoint;

        }    

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);

            transform.DOKill();

            transform.DOScale(defaultScale * 1.1f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                transform.DOScale(defaultScale, 0.1f).SetEase(Ease.OutBack);
            });

            UpdateHeathText();
            if (hitPoint <= 0)
            {
                listBlockPlace.ForEach(x => x.OnBlockDestroyed?.Invoke(x));
                return;
            }

            currentElementHealthRemain -= 1;

            if (currentElementHealthRemain <= 0)
            {
                NextElement();
            }
        }

        private void NextElement()
        {
            currentElementIndex += 1;
            currentElementHealthRemain = tunnelData.elements[currentElementIndex].health;

        }

        private void UpdateHeathText()
        {
            m_textHealth.text = hitPoint.ToString();
        }
    }
}
