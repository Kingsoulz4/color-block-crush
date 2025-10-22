using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiItemHolder : MonoBehaviour
{
    [SerializeField] private string placement;

    private void OnEnable()
    {
        UiHolderManager.Instance.PushItemHolder(this);
    }

    private void OnDisable()
    {
        UiHolderManager.Instance.PopItemHolder();
    }

    public string GetPlacement() => placement;
}
