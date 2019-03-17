using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class Wardrobe : ScriptableObject
    {
        [SerializeField]
        public string dollId;

        [SerializeField]
        public WardrobeArticle[] articles = new WardrobeArticle[0];

        [SerializeField]
        public SkeletonDataAsset spineModel;

#if UNITY_EDITOR
        public bool _viewExpanded { get; set; }

        public Dictionary<DollSocket, bool> _socketMap = new Dictionary<DollSocket, bool>();

        public bool _IsSocketExpanded(DollSocket socket)
        {
            return _socketMap.ContainsKey(socket) && _socketMap[socket];
        }

        public void _SetSocketExpanded(DollSocket socket, bool expanded)
        {
            _socketMap[socket] = expanded;
        }

#endif


        public bool AddArticle(WardrobeArticle article)
        {
            int index = Array.IndexOf(articles, article);
            if (index >= 0) return false;

            int size = articles.Length;
            Array.Resize(ref articles, size + 1);
            articles[size] = article;
            return true;
        }

        public bool RemoveArticle(WardrobeArticle article)
        {
            int index = Array.IndexOf(articles, article);
            if (index < 0) return false;

            // move everything up and chop off last element
            for (int i = index + 1; i < articles.Length; i++) articles[i - 1] = articles[i];
            Array.Resize(ref articles, articles.Length - 1);
            return true;
        }

        public WardrobeArticle FindArticle(string articleId)
        {
            foreach (var article in articles) {
                if (string.CompareOrdinal(article.id, articleId) == 0) return article;
            }

            return null;
        }

        public List<WardrobeArticle> FindArticlesForSocket(string socketId)
        {
            var rVal = new List<WardrobeArticle>();
            foreach (var article in articles) {
                if (string.CompareOrdinal(article.socketId, socketId) == 0) rVal.Add(article);
            }

            return rVal;
        }
    }
}