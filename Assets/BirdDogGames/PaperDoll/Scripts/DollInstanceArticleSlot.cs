using System;
using UnityEngine;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class DollInstanceArticleSlot
    {
        [SerializeField]
        public string slotName;

        [SerializeField]
        public Color tintColor;

        public DollInstanceArticleSlot() { }

        public DollInstanceArticleSlot(string slotName, Color tintColor)
        {
            this.slotName = slotName;
            this.tintColor = tintColor;
        }
    }
}