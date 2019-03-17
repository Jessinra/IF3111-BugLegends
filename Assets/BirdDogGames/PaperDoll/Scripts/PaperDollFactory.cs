using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BirdDogGames.PaperDoll
{
    public static class PaperDollFactory
    {
        public static DollInstance CreateDollInstance(DollPrototype proto)
        {
            var pdGameObject = new GameObject(proto.name, typeof(DollInstance));
            var instance = pdGameObject.GetComponent<DollInstance>();

            ChangeDollInstance(instance, proto);

            return instance;
        }

        public static void ChangeDollInstance(DollInstance instance, DollPrototype proto)
        {
            // Clear out children on this object
            for (var i = instance.transform.childCount - 1; i >= 0; --i) {
                var child = instance.transform.GetChild(i).gameObject;

                Object.DestroyImmediate(child);
            }

            instance.prototype = proto;

            if (proto == null) {
                return;
            }

            var skeletonDataAsset = proto.spineModelObject;

            var data = skeletonDataAsset.GetSkeletonData(true);
            if (data == null) {
                Debug.LogWarning(
                    "InstantiateSkeletonAnimation tried to instantiate a skeleton from an invalid SkeletonDataAsset.");
                return;
            }

            var skGameObject = new GameObject(string.Format("{0}_view", proto.name), typeof(MeshFilter),
                typeof(MeshRenderer), typeof(SkeletonAnimation));
            var newSkeletonAnimation = skGameObject.GetComponent<SkeletonAnimation>();
            newSkeletonAnimation.skeletonDataAsset = skeletonDataAsset;

            newSkeletonAnimation.Initialize(false);

            var skin = data.DefaultSkin ?? data.Skins.Items[0];
            var skeleton = instance.skeleton = newSkeletonAnimation.Skeleton;

            newSkeletonAnimation.initialSkinName = skin.Name;

            newSkeletonAnimation.skeleton.Update(0);
            newSkeletonAnimation.state.Update(0);
            newSkeletonAnimation.state.Apply(newSkeletonAnimation.skeleton);
            newSkeletonAnimation.skeleton.UpdateWorldTransform();

            skGameObject.transform.SetParent(instance.transform, false);

            instance.view = newSkeletonAnimation;
            skin = instance.skin = skeleton.UnshareSkin(true, false, newSkeletonAnimation.AnimationState);
            skeleton.SetSkin(skin);
        }

        public static void RandomizeOutfit(DollInstance instance, params Wardrobe[] wardrobes)
        {
            if (wardrobes.Length == 0) return;

            instance.socketArticles.Clear();

            foreach (var socket in instance.prototype.sockets) {
                var articles = new List<WardrobeArticle>();
                foreach (var w in wardrobes) {
                    if (string.CompareOrdinal(instance.prototype.name, w.dollId) != 0) continue;
                    articles.AddRange(w.FindArticlesForSocket(socket.id));
                }
                if (articles.Count == 0) continue;

                var pick = Random.Range(0, articles.Count + (!socket.allowNude ? 0 : 1));
                if (pick == articles.Count) continue; // nude

                var article = articles[pick];
                var articleInstance = instance.SetSocketArticle(socket, article);
                articleInstance.RandomizeColors(article);
            }

            RefreshDoll(instance);
        }

        public static void RandomizeOutfitColors(DollInstance instance, params Wardrobe[] wardrobes)
        {
            foreach (var aInstance in instance.socketArticles) {
                foreach (var w in wardrobes) {
                    var article = w.FindArticle(aInstance.articleId);
                    if (article == null) continue;

                    aInstance.RandomizeColors(article);
                    break;
                }
            }

            RefreshDoll(instance);
        }

        public static void RefreshDoll(DollInstance instance)
        {
            if (instance.prototype == null) {
                return;
            }

            if (!ValidateInstance(instance)) return;

            // clear any old socket items
            foreach (var socket in instance.prototype.sockets)
                RemoveAttachments(instance, socket);

            // gather any suppressed sockets
            var hidden = new List<string>();
            foreach (var socketArticle in instance.socketArticles) {
                var article = socketArticle.Article;
                if (article != null) hidden.AddRange(article.hideSockets);
            }


            // Apply the current items
            foreach (var socketArticle in instance.socketArticles) {
                if (hidden.Contains(socketArticle.socketId)) continue;
                ApplyArticle(instance, socketArticle);
            }

            // set up our skin and tinting
            if (instance.skeleton != null) {
                instance.skeleton.SetSkin((Skin) null);
                instance.skeleton.SetSkin(instance.skin);
                ApplyTinting(instance);
            }

            // set up any attachments
            foreach (var attach in instance.prototype.attachments) {
                ApplyAttachment(instance, attach);
            }
        }

        public const string DefaultPmaShader = "Spine/Skeleton";
        public const string DefaultStraightAlphaShader = "Sprites/Default";

        private static void ApplyAttachment(DollInstance instance, AttachmentPoint point)
        {
            var applyPma = false;
            var attach = instance.FindSlotAttachment(point.id);
            var sprite = attach != null ? attach.sprite : null;

            if (sprite == null) {
                instance.skeleton.FindSlot(point.spineSlot).Attachment = null;
                return;
            }

            var skeletonComponent = instance.view.GetComponentInChildren<ISkeletonComponent>(true);
            var skeletonRenderer = skeletonComponent as SkeletonRenderer;
            if (skeletonRenderer != null)
                applyPma = skeletonRenderer.pmaVertexColors;
            else {
                var skeletonGraphic = skeletonComponent as SkeletonGraphic;
                if (skeletonGraphic != null)
                    applyPma = skeletonGraphic.MeshGenerator.settings.pmaVertexColors;
            }

            var attachmentShader = applyPma
                ? Shader.Find(DefaultPmaShader)
                : Shader.Find(DefaultStraightAlphaShader);
            var attachment = applyPma
                ? sprite.ToRegionAttachmentPMAClone(attachmentShader)
                : sprite.ToRegionAttachment(GetPageFor(sprite.texture, attachmentShader));
            instance.skeleton.FindSlot(point.spineSlot).Attachment = attachment;
        }

        static Dictionary<Texture, AtlasPage> _atlasPageCache;

        static AtlasPage GetPageFor(Texture texture, Shader shader)
        {
            if (_atlasPageCache == null) _atlasPageCache = new Dictionary<Texture, AtlasPage>();
            AtlasPage atlasPage;
            _atlasPageCache.TryGetValue(texture, out atlasPage);
            if (atlasPage != null) return atlasPage;

            var newMaterial = new Material(shader);
            atlasPage = newMaterial.ToSpineAtlasPage();
            _atlasPageCache[texture] = atlasPage;
            return atlasPage;
        }

        private static void ApplyTinting(DollInstance instance)
        {
            foreach (var slot in instance.skeleton.Slots.Items) {
                if (slot.Attachment == null) continue;

                var name = slot.Attachment.Name;
                if (name.Contains("_skin")) {
                    slot.SetColor(instance.skinColor);
                }
            }
        }

        static void ApplyArticle(DollInstance instance, DollInstanceSocketArticle socketArticle)
        {
            var socket = instance.prototype.FindSocket(socketArticle.socketId);
            if (socket == null) return;

            var article = socketArticle.wardrobe.FindArticle(socketArticle.articleId);
            if (article == null) return;

            // SOURCE INFO
            var srcDoll = article.wardrobe.spineModel;
            if (srcDoll == null) return;

            var srcSkel = srcDoll.GetSkeletonData(true);
            if (srcSkel == null) return;

            var srcSkin = srcSkel.FindSkin(article.skinName);
            if (srcSkin == null) return;
            // END SOURCE INFO

            // walk through the spine slots in the socket
            for (int i = 0, maxI = article.AttachmentCount; i < maxI; i++) {
                var attachment = article.attachments[i];
                var dollSlotIndex = instance.skeleton.FindSlotIndex(attachment.slotName);
                if (dollSlotIndex < 0) continue;

                var skinSlotIndex = srcSkel.FindSlotIndex(attachment.slotName);
                if (skinSlotIndex < 0) continue;

                // Add attachments from article's doll to instance
                var newPairs = FindAttachmentPairs(srcSkin, skinSlotIndex);
                foreach (var pair in newPairs) instance.skin.Attachments[new Skin.AttachmentKeyTuple(dollSlotIndex, pair.Key.name)] = pair.Value;

                // Apply article attachment tint
                var item = instance.skeleton.Slots.Items[dollSlotIndex];
                switch (attachment.tintMode) {
                    case WardrobeArticleAttachment.TintMode.None:
                        item.SetColor(Color.white);
                        break;
                    case WardrobeArticleAttachment.TintMode.Skin:
                        item.SetColor(instance.skinColor);
                        break;
                    case WardrobeArticleAttachment.TintMode.Generic:
                        item.SetColor(socketArticle.GetTint(attachment.slotName));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static bool ValidateInstance(DollInstance instance)
        {
            if (instance.skeleton == null && instance.view != null)
                instance.skeleton = instance.view.Skeleton;
            if (instance.skeleton == null) return false;

            if (instance.skin == null && instance.view != null)
                instance.skin = instance.skeleton.UnshareSkin(true, false, instance.view.AnimationState);
            return instance.skin != null && instance.view != null;
        }

        private static void RemoveAttachments(DollInstance instance, DollSocket socket)
        {
            if (instance == null || instance.skeleton == null || instance.skin == null || socket == null) return;

            // remove attachments for the slots in the given socket.
            for (int i = 0, maxI = socket.spineSlots.Count; i < maxI; i++) {
                var slotName = socket.spineSlots[i];
                var slotIndex = instance.skeleton.FindSlotIndex(slotName);
                if (slotIndex < 0) continue;

                var item = instance.skeleton.Slots.Items[slotIndex];
                item.Attachment = null;

                var oldPairs = FindAttachmentPairs(instance.skin, slotIndex);
                foreach (var pair in oldPairs) {
                    instance.skin.Attachments.Remove(pair.Key);
                }
            }
        }

        static List<KeyValuePair<Skin.AttachmentKeyTuple, Attachment>> FindAttachmentPairs(Skin srcSkin, int slotId)
        {
            var rVal = new List<KeyValuePair<Skin.AttachmentKeyTuple, Attachment>>();
            foreach (var pair in srcSkin.Attachments) {
                if (pair.Key.slotIndex == slotId) rVal.Add(pair);
            }
            return rVal;
        }
    }
}