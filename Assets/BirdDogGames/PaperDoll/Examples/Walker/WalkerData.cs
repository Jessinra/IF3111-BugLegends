using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirdDogGames.PaperDoll.Examples.Walker
{
	public class WalkerData : ScriptableObject
	{
		[SerializeField] public DollPrototype[] dolls = new DollPrototype[0];
		[SerializeField] public Wardrobe[] wardrobes = new Wardrobe[0];
	}
}