using System;
using System.Collections.Generic;
using System.Linq;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace BirdDogGames.PaperDoll
{
    [ExecuteInEditMode]
    public class DollInstance : MonoBehaviour
    {
        public DollPrototype prototype;
        public List<Wardrobe> activeWardrobes = new List<Wardrobe>();
        public SkeletonAnimation view;
        public List<DollInstanceSocketArticle> socketArticles = new List<DollInstanceSocketArticle>();
        public List<DollInstanceAttachment> attachments = new List<DollInstanceAttachment>();
        public Skeleton skeleton;
        public Skin skin;
        public Color skinColor = Color.white;

#if UNITY_EDITOR
        public bool _viewWardrobeExpanded { get; set; }
        public Dictionary<string, int> _viewTagSkinSelection = new Dictionary<string, int>();
        public Dictionary<string, bool> _viewTagSkinExpanded = new Dictionary<string, bool>();
		public List<string> _viewAnimations = new List<string>();

		/// <summary>
		/// Flag for help boxes in inspector
		/// </summary>
		public static bool helpBoxes = false;
#endif

        void Start()
        {
            PaperDollFactory.RefreshDoll(this);
        }

        public DollInstanceSocketArticle FindSocketArticle(DollSocket socket)
        {
            return socketArticles.FirstOrDefault(
                socketArticle => string.CompareOrdinal(socketArticle.socketId, socket.id) == 0);
        }

        public DollInstanceSocketArticle FindSocketArticle(string socketId)
        {
            return socketArticles.FirstOrDefault(
                socketArticle => string.CompareOrdinal(socketArticle.socketId, socketId) == 0);
        }

        public DollInstanceSocketArticle SetSocketArticle(DollSocket socket, WardrobeArticle article)
        {
            DollInstanceSocketArticle socketArticle;
            for (int i = socketArticles.Count - 1; i >= 0; i--) {
                socketArticle = socketArticles[i];

                if (string.CompareOrdinal(socketArticle.socketId, socket.id) != 0) continue;

                if (article == null) {
                    socketArticles.RemoveAt(i);
                    return null;
                }

                socketArticle.wardrobe = article.wardrobe;
                socketArticle.articleId = article.id;
                return socketArticle;
            }

            if (article == null) return null;

            socketArticle = new DollInstanceSocketArticle(socket, article.wardrobe, article.id);
            socketArticles.Add(socketArticle);
            return socketArticle;
        }

        public DollInstanceAttachment FindSlotAttachment(string id)
        {
            return attachments.FirstOrDefault(att => string.CompareOrdinal(att.attachId, id) == 0);
        }

        public DollInstanceAttachment SetSlotAttachment(string id, Sprite sprite)
        {
            DollInstanceAttachment att;
            for (int i = attachments.Count - 1; i >= 0; i--) {
                att = attachments[i];

                if (string.CompareOrdinal(att.attachId, id) != 0) continue;

                if (sprite == null) {
                    attachments.RemoveAt(i);
                    return null;
                }

                att.attachId = id;
                att.sprite = sprite;
                return att;
            }

            if (sprite == null) return null;

            att = new DollInstanceAttachment() {attachId = id, sprite = sprite};
            attachments.Add(att);
            return att;
        }

        [ContextMenu("Randomize Outfit")]
        public void RandomizeOutfit()
        {
            PaperDollFactory.RandomizeOutfit(this,
                activeWardrobes != null ? activeWardrobes.ToArray() : new Wardrobe[0]);
        }

        [ContextMenu("Randomize Colors")]
        public void RandomizeColors()
        {
            PaperDollFactory.RandomizeOutfitColors(this,
                activeWardrobes != null ? activeWardrobes.ToArray() : new Wardrobe[0]);
        }

        [ContextMenu("Randomize Skin Color")]
        public void RandomizeSkinColor()
        {
            skinColor = prototype.RandomSkinColor;
        }

		#if UNITY_EDITOR
		[ContextMenu("Toggle Help")]
		public void ToggleHelp()
		{
			helpBoxes = !helpBoxes;
		}
		#endif

    }

    [Serializable]
    public class DollInstanceAttachment
    {
        public string attachId;
        public Sprite sprite;
    }
}