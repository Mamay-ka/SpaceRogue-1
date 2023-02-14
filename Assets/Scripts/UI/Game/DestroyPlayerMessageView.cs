using System;
using UI.Common;
using UnityEngine;

public sealed class DestroyPlayerMessageView : MonoBehaviour
{
    [field: SerializeField] public ButtonView DestroyPlayerButton;

    public void Init(Action onClickAction)
    {
        DestroyPlayerButton.Init(onClickAction);
    }
}
