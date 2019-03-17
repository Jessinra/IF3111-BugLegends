using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class WardrobeArticleAttachment
    {
        [SerializeField]
        public string slotName;

        [SerializeField]
        public string spriteName;

        [SerializeField]
        public TintMode tintMode;

        [SerializeField]
        public Color[] defaultColors = new Color[0];

        public enum TintMode
        {
            None,
            Skin,
            Generic
        }

#if UNITY_EDITOR
        public Vector2 _scroll { get; set; }
#endif

        public void DetermineTintTypeFromSpriteName()
        {
            if (string.IsNullOrEmpty(spriteName)) {
                tintMode = TintMode.None;
                return;
            }

            if (spriteName.Contains("_skin")) tintMode = TintMode.Skin;
            else if (spriteName.Contains("_tint")) tintMode = TintMode.Generic;
            else tintMode = TintMode.None;
        }

        public void AddColor(Color c)
        {
            if (defaultColors == null) {
                defaultColors = new[] {c};
                return;
            }

            var size = defaultColors.Length;
            Array.Resize(ref defaultColors, size + 1);
            defaultColors[size] = c;
        }

        public Color RandomPaletteColor {
            get {
                if (defaultColors == null || defaultColors.Length == 0) return Color.white;
                return defaultColors[Random.Range(0, defaultColors.Length)];
            }
        }
    }
}