using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class TunnelAreaInforEditorView : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TextMeshProUGUI tunnelAreaElementNumber;
        [SerializeField] private int tunnelAreaId;
        [SerializeField] private Action<TunnelAreaInforEditorView> selectedTunnelAreaAction;

        public TunnelAreaConfig tunnelAreaConfig;
        
        public void UpdateInfor(int id, RectTransform selectionParent, IList<RectTransform> cells, 
            Canvas rootCanvas, TunnelAreaConfig tunnelAreaConfig, Action<TunnelAreaInforEditorView> selectAction)
        {
            tunnelAreaId = id;
            UiBoundsFitter.FitFrameToTargets(rectTransform, selectionParent,
                cells, Vector2.zero, rootCanvas);
            this.tunnelAreaConfig = tunnelAreaConfig;
            this.selectedTunnelAreaAction = selectAction;
            tunnelAreaElementNumber.text = tunnelAreaConfig.colorNumber.ToString();
        }

        public void OnClick()
        {
            selectedTunnelAreaAction?.Invoke(this);
        }

        public int GetTunnelAreaId() => tunnelAreaId;
    }
}
