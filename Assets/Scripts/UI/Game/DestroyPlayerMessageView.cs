using System;
using UI.Common;
using UnityEngine;

namespace UI.Game
{
    public sealed class DestroyPlayerMessageView : MonoBehaviour
    {
        [field: SerializeField] public TextView LevelsNumber { get; private set; }
        [field: SerializeField] public ButtonView DestroyPlayerButton { get; private set; }

        public void Init(float levelsNumber, Action onClickAction)
        {
            LevelsNumber.Init(levelsNumber.ToString());
            DestroyPlayerButton.Init(onClickAction);
        }
    } 
}
