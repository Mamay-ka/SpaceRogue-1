using UI.Common;
using UnityEngine;
using UI.Abstracts;

namespace UI.Game
{

    public sealed class PlayerExperienceBarView : MonoBehaviour
    {
        [field: SerializeField] public TextView ExperienceTextView { get; private set; }
        [field: SerializeField] public BarView BarView { get; private set; }//!!!!

        public void Init(string text) => ExperienceTextView.Init(text);
        public void UpdateText(string text) => ExperienceTextView.UpdateText(text);
    }
}
