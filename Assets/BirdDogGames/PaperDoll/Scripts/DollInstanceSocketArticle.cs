using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class DollInstanceSocketArticle
    {
        [SerializeField]
        public string socketId;

        [SerializeField]
        public Wardrobe wardrobe;

        [SerializeField]
        public string articleId;

        [SerializeField]
        public List<DollInstanceArticleSlot> slots = new List<DollInstanceArticleSlot>();

#if UNITY_EDITOR
        public bool _viewArticlesExpanded { get; set; }
#endif

        public WardrobeArticle Article {
            get { return wardrobe == null ? null : wardrobe.FindArticle(articleId); }
        }

        public DollInstanceSocketArticle(DollSocket socket, Wardrobe wardrobe, string articleId)
        {
            this.socketId = socket != null ? socket.id : null;
            this.wardrobe = wardrobe;
            this.articleId = articleId;
            InitSlots(socket);
        }

        private void InitSlots(DollSocket socket)
        {
            if (socket == null) return;

            foreach (var slot in socket.spineSlots)
                slots.Add(new DollInstanceArticleSlot() {slotName = slot, tintColor = Color.white});
        }

        public void SetSlotTint(string slotName, Color tintColor)
        {
            var slot = FindSlot(slotName);
            if (slot != null) {
                slot.tintColor = tintColor;
            }
        }

        public void SetSlotsTint(Color color)
        {
            var article = Article;
            if (article == null) return;

            foreach (var slot in slots) {
                var att = article.FindAttachment(slot.slotName);
                if (att != null && att.tintMode == WardrobeArticleAttachment.TintMode.Generic)
                    slot.tintColor = color;
            }
        }

        public DollInstanceArticleSlot FindSlot(string slotName)
        {
            return slots.FirstOrDefault(s => string.CompareOrdinal(slotName, s.slotName) == 0);
        }

        public Color GetTint(string slotName)
        {
            var slot = FindSlot(slotName);
            return slot == null ? Color.white : slot.tintColor;
        }

        public void RandomizeColors(WardrobeArticle article)
        {
            if (article == null) return;

            // use default article palette if we have one
            if (article.defaultColors.Length > 0) {
                var color = article.RandomPaletteColor;
                foreach (var att in article.attachments) {
                    if (att.tintMode == WardrobeArticleAttachment.TintMode.Generic) {
                        SetSlotTint(att.slotName, color);
                    }
                }
                return;
            }

            // No default palette, so try using colors from the attachments themselves.
            foreach (var att in article.attachments) {
                if (att.tintMode == WardrobeArticleAttachment.TintMode.Generic) {
                    SetSlotTint(att.slotName, att.RandomPaletteColor);
                }
            }
        }
    }
}