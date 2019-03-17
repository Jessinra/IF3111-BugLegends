using System;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class WardrobeArticle
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public Wardrobe wardrobe;

        [SerializeField]
        public string socketId;

        [SerializeField]
        public string skinName;

        [SerializeField]
        public Color[] defaultColors = new Color[0];

        [SerializeField]
        public WardrobeArticleAttachment[] attachments = new WardrobeArticleAttachment[0];

        [SerializeField]
        public string[] hideSockets = new string[0];

#if UNITY_EDITOR
        public bool _viewExpanded { get; set; }
        public bool _viewAdvExpanded { get; set; }
        public bool _rename { get; set; }
#endif

        public bool AddAttachment(Attachment item, string slotName)
        {
            var attachment = new WardrobeArticleAttachment
            {
                slotName = slotName,
                spriteName = item.Name
            };
            attachment.DetermineTintTypeFromSpriteName();

            if (attachment.tintMode == WardrobeArticleAttachment.TintMode.Generic)
                attachment.defaultColors = new[] {Color.white};

            return AddAttachment(attachment);
        }

        public bool AddAttachment(WardrobeArticleAttachment item)
        {
            var index = Array.IndexOf(attachments, item);
            if (index >= 0) return false;

            var size = attachments.Length;
            Array.Resize(ref attachments, size + 1);
            attachments[size] = item;
            return true;
        }

        public bool RemoveArticle(WardrobeArticle article)
        {
            var index = Array.IndexOf(attachments, article);
            if (index < 0) return false;

            // move everything up and chop off last element
            for (var i = index + 1; i < attachments.Length; i++) attachments[i - 1] = attachments[i];
            Array.Resize(ref attachments, attachments.Length - 1);
            return true;
        }

        public void AddAttachments(List<Attachment> list, string slotName)
        {
            for (int i = 0, maxI = list.Count; i < maxI; i++) {
                var item = list[i];

                if (item != null) AddAttachment(item, slotName);
            }
        }

        public bool AddHideSocket(string sId)
        {
            var index = Array.IndexOf(hideSockets, sId);
            if (index >= 0) return false;

            var size = hideSockets.Length;
            Array.Resize(ref hideSockets, size + 1);
            hideSockets[size] = sId;
            return true;
        }

        public bool RemoveHideSocket(string sId)
        {
            var index = Array.IndexOf(hideSockets, sId);
            if (index < 0) return false;

            // move everything up and chop off last element
            for (var i = index + 1; i < hideSockets.Length; i++) hideSockets[i - 1] = hideSockets[i];
            Array.Resize(ref hideSockets, hideSockets.Length - 1);
            return true;
        }

        public int AttachmentCount {
            get { return attachments != null ? attachments.Length : 0; }
        }

        public bool RemoveHideSocketAt(int index)
        {
            if (index < 0 || index > hideSockets.Length) return false;

            // move everything up and chop off last element
            for (var i = index + 1; i < hideSockets.Length; i++) hideSockets[i - 1] = hideSockets[i];
            Array.Resize(ref hideSockets, hideSockets.Length - 1);
            return true;
        }

        public WardrobeArticleAttachment FindAttachment(string slotName)
        {
            foreach (var att in attachments) {
                if (string.CompareOrdinal(att.slotName, slotName) == 0) return att;
            }

            return null;
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

        public bool HasTintableAttachment {
            get {
                return attachments.Any(attachment => attachment.tintMode == WardrobeArticleAttachment.TintMode.Generic);
            }
        }
    }
}