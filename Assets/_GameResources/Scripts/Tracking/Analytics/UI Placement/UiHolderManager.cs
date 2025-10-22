using System.Collections.Generic;
using UnityEngine;
using Yoolax.Framework;

public class UiHolderManager : SingletonDontDestroyMono<UiHolderManager>
{
    [SerializeField]
    private Stack<UiItemHolder> itemsHolderStack = new Stack<UiItemHolder>();

    public void PushItemHolder(UiItemHolder itemHolder)
    {
        itemsHolderStack.Push(itemHolder);
    }

    public void PopItemHolder()
    {
        itemsHolderStack.Pop();
    }

    public string GetCurrentPlacement()
    {
        if(itemsHolderStack.Count > 0)
            return itemsHolderStack.Peek().GetPlacement();
        return "unknown";
    }
}
