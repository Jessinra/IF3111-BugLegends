using System;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class DollPrototype : ScriptableObject
    {
        [SerializeField]
        public DollSocket[] sockets = new DollSocket[0];

        [SerializeField]
        public AttachmentPoint[] attachments = new AttachmentPoint[0];

        [SerializeField]
        public SkeletonDataAsset spineModelObject;

        [SerializeField]
        public Color[] defaultSkinColors = new Color[0];

#if UNITY_EDITOR
        public bool _viewExpanded { get; set; }
        public bool _viewInfoExpanded { get; set; }
        public bool _viewSocketsExpanded { get; set; }
        public bool _viewAttachExpanded { get; set; }
        public bool _viewUnassExpanded { get; set; }
        public bool _viewWardrobeExpanded { get; set; }
        public Vector2 _colorScroll { get; set; }
#endif

        public bool Valid {
            get { return spineModelObject != null; }
        }

        public Color RandomSkinColor {
            get {
                if (defaultSkinColors == null || defaultSkinColors.Length == 0) return Color.white;
                return defaultSkinColors[Random.Range(0, defaultSkinColors.Length)];
            }
        }

        public bool AddSocket(DollSocket socket)
        {
            var index = Array.IndexOf(sockets, socket);
            if (index >= 0) return false;

            var size = sockets.Length;
            Array.Resize(ref sockets, size + 1);
            sockets[size] = socket;
            return true;
        }

        public bool RemoveSocket(DollSocket socket)
        {
            var index = Array.IndexOf(sockets, socket);
            if (index < 0) return false;

            // move everything up and chop off last element
            for (var i = index + 1; i < sockets.Length; i++) sockets[i - 1] = sockets[i];
            Array.Resize(ref sockets, sockets.Length - 1);
            return true;
        }

        public DollSocket FindSocket(string socketId)
        {
            return sockets.FirstOrDefault(socket => string.CompareOrdinal(socket.id, socketId) == 0);
        }

        public DollSocket FindSocketBySlot(string slotName)
        {
            return sockets.FirstOrDefault(socket => socket.spineSlots.Contains(slotName));
        }

        public void EnsureUniqueSlots(DollSocket updatedSlot)
        {
            foreach (var socket in sockets) {
                if (socket == updatedSlot) continue;

                var slots = socket.spineSlots;
                for (var j = slots.Count; --j >= 0;) {
                    if (updatedSlot.spineSlots.Contains(slots[j])) slots.RemoveAt(j);
                }
            }
        }

        public List<WardrobeArticle> FindArticles(DollSocket socket, params Wardrobe[] wardrobes)
        {
            var rVal = new List<WardrobeArticle>();
            foreach (var wardrobe in wardrobes) {
                var items = wardrobe.FindArticlesForSocket(socket.id);
                if (items != null) rVal.AddRange(items);
            }

            return rVal;
        }

        public void AddSkinColor(Color c)
        {
            if (defaultSkinColors == null) {
                defaultSkinColors = new[] {c};
                return;
            }

            var size = defaultSkinColors.Length;
            Array.Resize(ref defaultSkinColors, size + 1);
            defaultSkinColors[size] = c;
        }

        public bool AddAttachmentPoint(AttachmentPoint att)
        {
            var index = Array.IndexOf(attachments, att);
            if (index >= 0) return false;

            var size = attachments.Length;
            Array.Resize(ref attachments, size + 1);
            attachments[size] = att;
            return true;
        }

        public bool RemoveAttachmentPoint(AttachmentPoint att)
        {
            var index = Array.IndexOf(attachments, att);
            if (index < 0) return false;

            // move everything up and chop off last element
            for (var i = index + 1; i < attachments.Length; i++) attachments[i - 1] = attachments[i];
            Array.Resize(ref attachments, attachments.Length - 1);
            return true;
        }

        public AttachmentPoint FindAttachmentPoint(string id)
        {
            return attachments.FirstOrDefault(att => string.CompareOrdinal(att.id, id) == 0);
        }

        public AttachmentPoint FindAttachmentPointBySlot(string slotName)
        {
            return attachments.FirstOrDefault(att => string.CompareOrdinal(att.spineSlot, slotName) == 0);
        }
    }
}