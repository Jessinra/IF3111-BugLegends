using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine.Unity;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BirdDogGames.PaperDoll.Examples.Walker
{
	public class Walker : MonoBehaviour {
		/// <summary>
		/// Data for the platformer
		/// </summary>
		public WalkerData data;

		/// <summary>
		/// Link to the camera on the scene
		/// </summary>
		public Camera mainCamera;

		/// <summary>
		/// The minimum number of seconds until an item spawns in the background
		/// </summary>
		public float minimumItemSpawn = 1f;

		/// <summary>
		/// The maximum number of seconds until an item spawns in the background
		/// </summary>
		public float maximumItemSpawn = 4f;

		/// <summary>
		/// The minimum Y that an item should spawn at
		/// </summary>
		public float minimumSpawnY = .1f;

		/// <summary>
		/// The maximum Y that an item should spawn at
		/// </summary>
		public float maximumSpawnY = .3f;

		/// <summary>
		/// List of background items that randomly spawn off screen
		/// </summary>
		public Sprite[] backgroundItems;

		/// <summary>
		/// The sprite designated as the path
		/// </summary>
		public Sprite pathSprite;

		/// <summary>
		/// The main doll actor's transform
		/// </summary>
		Transform mainActorTransform;

		/// <summary>
		/// The next time an item will spawn in the background
		/// </summary>
		float nextSpawnTime;

		/// <summary>
		/// Cached quaternion for flipping the character
		/// </summary>
		readonly Quaternion flippedRotation = Quaternion.Euler(0, 180, 0);

		/// <summary>
		/// Next path position
		/// </summary>
		Vector3 nextPathPosition;

		float backgroundActorZ = .1f;
		float basePathY;

		// Use this for initialization
		void Awake() {
			// Only do this if we're in the editor.  Otherwise, just use the currently linked data
			#if UNITY_EDITOR
			ReloadData();
			#endif

			// Choose our main actor
			var proto = ChooseDoll();

			if(proto != null)
			{
				var go = new GameObject("Main Actor");
				var doll = go.AddComponent<DollInstance>();

				// Set up position
				go.transform.position = mainCamera.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, 10.0f));

				// Initialize the doll
				PaperDollFactory.ChangeDollInstance(doll, proto);

				// Set a random skin color from our defaults
				doll.skinColor = doll.prototype.RandomSkinColor;

				// Randomize the outfit based on the doll and the warddrobes specified
				PaperDollFactory.RandomizeOutfit(doll, data.wardrobes);

				// Make sure to setup looping animations
				doll.view.loop = true;

				var skeletonAnim = doll.GetComponentInChildren<SkeletonAnimation>();

				skeletonAnim.AnimationName = FindWalkAnimation(doll.prototype);

				// Have camera follow the main actor
				mainCamera.transform.SetParent(go.transform, true);

				mainActorTransform = go.transform;

				doll.GetComponentInChildren<Renderer>().sortingOrder = 1000000;
			}

			SpawnBackgroundItem();
			SpawnPath();
		}

		void Update()
		{
			if(mainActorTransform == null) return;

			mainActorTransform.Translate(new Vector3(3f, 0f) * Time.deltaTime);

			if(Time.time > nextSpawnTime)
			{
				SpawnBackgroundItem();
			}

			if(mainCamera.WorldToViewportPoint(nextPathPosition).x < 1.2)
			{
				SpawnIndividualPath(nextPathPosition);
			}
		}

		void SpawnPath()
		{
			var position = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.05f, 0f));

			nextPathPosition = new Vector3(position.x, position.y, 0f); //Random.value * (maximumSpawnPathY - minimumSpawnPathY) + minimumSpawnPathY
			basePathY = position.y;

			while(mainCamera.WorldToViewportPoint(nextPathPosition).x < 1.2f)
			{
				SpawnIndividualPath(nextPathPosition);
			}
		}

		void SpawnIndividualPath(Vector3 position)
		{
			var pathGO = new GameObject("Path");
			var renderer = pathGO.AddComponent<SpriteRenderer>();

			renderer.sprite = pathSprite;
			pathGO.transform.position = position;
			pathGO.AddComponent<DestroyWhenOffScreen>();

			pathGO = new GameObject("Path");
			renderer = pathGO.AddComponent<SpriteRenderer>();

			renderer.sprite = pathSprite;
			pathGO.transform.position = new Vector3(position.x - .8f, position.y + .8f, 0f);
			pathGO.AddComponent<DestroyWhenOffScreen>();

			nextPathPosition = new Vector3(position.x + 1.5f, basePathY + Random.Range(-.2f, .2f), 0f);//Random.value * (maximumSpawnPathY - minimumSpawnPathY) + minimumSpawnPathY
		}

		void SpawnBackgroundItem()
		{
			if(backgroundItems.Length <= 0) return;

			// Set next spawn time
			nextSpawnTime = Time.time + Random.value * (maximumItemSpawn - minimumItemSpawn) + minimumItemSpawn;

			var index = Random.Range(0, backgroundItems.Length + 1);

			if(index < backgroundItems.Length)
			{
				// Choose item to spawn
				var item = backgroundItems[Random.Range(0, backgroundItems.Length)];
				var width = item.bounds.size.x;
				var position = mainCamera.ViewportToWorldPoint(new Vector3(1f, Random.value * (maximumSpawnY - minimumSpawnY) + minimumSpawnY, 0f));

				position = position - new Vector3(-width, 0f, -10.5f);

				var backgroundGO = new GameObject("Background Item");
				var renderer = backgroundGO.AddComponent<SpriteRenderer>();

				renderer.sprite = item;
				backgroundGO.transform.position = position;
				renderer.sortingOrder = (int)(backgroundGO.transform.localPosition.y * -100);
				backgroundGO.AddComponent<DestroyWhenOffScreen>();
			}
			else
			{
				// Choose our main actor
				var proto = ChooseDoll();

				if(proto != null)
				{
					var go = new GameObject("Background Actor");
					var doll = go.AddComponent<DollInstance>();
					var position = mainCamera.ViewportToWorldPoint(new Vector3(1f, Random.value * (maximumSpawnY - minimumSpawnY) + minimumSpawnY, 0f));

					position = position - new Vector3(-3f, 0f, -10f -  + backgroundActorZ);
					backgroundActorZ += .1f;

					// Set up position
					go.transform.position = position;
					go.transform.rotation = flippedRotation;
					go.AddComponent<DestroyWhenOffScreen>();

					// Initialize the doll
					PaperDollFactory.ChangeDollInstance(doll, proto);

					// Set a random skin color from our defaults
					doll.skinColor = doll.prototype.RandomSkinColor;

					// Randomize the outfit based on the doll and the warddrobes specified
					PaperDollFactory.RandomizeOutfit(doll, data.wardrobes);

					// Make sure to setup looping animations
					doll.view.loop = true;

					var skeletonAnim = doll.GetComponentInChildren<SkeletonAnimation>();

					skeletonAnim.AnimationName = FindRandomAnimation(doll.prototype);

					doll.GetComponentInChildren<Renderer>().sortingOrder = (int)(go.transform.localPosition.y * -100);
				}
			}
		}

		string FindRandomAnimation(DollPrototype proto)
		{
			var animations = proto.spineModelObject.GetSkeletonData(true)
				.Animations.Items.ToList()
				.ConvertAll(a => a.Name);

			return animations[Random.Range(0, animations.Count)];
		}

		string FindWalkAnimation(DollPrototype proto)
		{
			var animations = proto.spineModelObject.GetSkeletonData(true)
				.Animations.Items.ToList()
				.ConvertAll(a => a.Name);

			if(animations.Contains("walk_med_001"))
			{
				return "walk_med_001";
			}
			else if(animations.Contains("dummy_walk_001"))
			{
				return "dummy_walk_001";
			}

			for(int i = 0, maxI = animations.Count;i < maxI;i++)
			{
				if (animations[i].ToLower().StartsWith("walk_"))
					return animations[i];
			}

			return "walk";
		}

		#if UNITY_EDITOR
		void ReloadData()
		{
			// Look for WalkerData objects
			if(data == null)
			{
				var datas = LoadAssets<WalkerData>("t:WalkerData");

				if(datas.Length > 0)
				{
					data = datas[0];
				}
			}

			// Still can't find the data, so lets create a new asset
			if(data == null)
			{
				data = ScriptableObject.CreateInstance<WalkerData>();

				var assetPathAndName =
					AssetDatabase.GenerateUniqueAssetPath("Assets/WalkerData.asset");

				AssetDatabase.CreateAsset(data, assetPathAndName);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			// Look for DollPrototype objects
			data.dolls = LoadAssets<DollPrototype>("t:DollPrototype");
			data.wardrobes = LoadAssets<Wardrobe>("t:Wardrobe");
		}

		static T[] LoadAssets<T>(string filter) where T : Object
		{
			var guids = AssetDatabase.FindAssets(filter);
			var rtnList = new List<T>();

			// Loop through them and cache their path and ids for later use
			for (int i = 0, maxI = guids.Length; i < maxI; i++) {
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);
				var asset = AssetDatabase.LoadAssetAtPath<T>(path);

				rtnList.Add(asset);
			}

			return rtnList.ToArray();
		}
		#endif

		private DollPrototype ChooseDoll()
		{
			// Randomly select an available doll from the data asset
			var dolls = data.dolls;
			if (dolls == null || dolls.Length == 0) return null;

			return dolls[Random.Range(0, dolls.Length)];
		}
	}
}