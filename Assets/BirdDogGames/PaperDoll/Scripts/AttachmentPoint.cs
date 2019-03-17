using System;
using UnityEngine;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class AttachmentPoint
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public string spineSlot;

#if UNITY_EDITOR
        public bool _rename { get; set; }
#endif
    }
}