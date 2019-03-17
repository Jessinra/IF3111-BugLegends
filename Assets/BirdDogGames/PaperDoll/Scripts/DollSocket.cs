using System;
using System.Collections.Generic;
using UnityEngine;

namespace BirdDogGames.PaperDoll
{
    [Serializable]
    public class DollSocket
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public bool allowNude;

        [SerializeField]
        public List<string> spineSlots = new List<string>();

        [SerializeField]
        public string tag;

#if UNITY_EDITOR
        public bool _viewAttExpanded { get; set; }
        public bool _viewSlotExpanded { get; set; }
        public bool _rename { get; set; }
#endif

        public DollSocket Clone()
        {
            var clone = new DollSocket
            {
                id = id,
                allowNude = allowNude,
                spineSlots = new List<string>(spineSlots)
            };
            return clone;
        }
    }
}