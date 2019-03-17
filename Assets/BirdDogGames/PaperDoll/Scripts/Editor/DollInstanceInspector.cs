using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BirdDogGames.PaperDoll
{
	/// <summary>
	/// Custom inspector for the DollInstance object
	/// </summary>
    [CustomEditor(typeof(DollInstance))]
    public class DollInstanceInspector : Editor
    {
		/// <summary>
		/// The current doll instance that this inspector is modifying (the target)
		/// </summary>
        DollInstance doll;

		/// <summary>
		/// Cached dictionary of doll paths found in the asset database
		/// </summary>
        Dictionary<string, string> idToPath;

		/// <summary>
		/// Cached list of all doll prototype ids found in the asset database
		/// </summary>
        List<string> dollPrototypeIds;

		/// <summary>
		/// Cached list of all wardrobes found for the currently selected doll
		/// </summary>
        readonly List<Wardrobe> wardrobes = new List<Wardrobe>();

		/// <summary>
		/// Cached dictionary of all the tags and their associated skins based on the doll and all the wardrobes loaded into memory
		/// </summary>
        readonly Dictionary<string, List<string>> taggedSkins = new Dictionary<string, List<string>>();

		/// <summary>
		/// Called when drawing the inspector GUI
		/// </summary>
        public override void OnInspectorGUI()
        {
            doll = (DollInstance) target;

			// Load in all the DollPrototypes found in the asset database and cache them
            if (idToPath == null) {
                idToPath = new Dictionary<string, string>();
                dollPrototypeIds = new List<string>();

				// Look for DollPrototype objects
                var guids = AssetDatabase.FindAssets("t:DollPrototype");

				// Loop through them and cache their path and ids for later use
                for (int i = 0, maxI = guids.Length; i < maxI; i++) {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var id = Path.GetFileNameWithoutExtension(path);
                    if (id == null) continue;

                    idToPath.Add(id, path);
                    dollPrototypeIds.Add(id);
                }

				// Look for wardrobes for the currently selected doll
                ReloadWardrobes();
            }

			// Actually render the inspector
            RenderDollInstanceInspector();
        }



		/// <summary>
		/// Reloads the wardrobe into cache for use later
		/// </summary>
		/// <param name="refreshActiveWardrobes">If set to <c>true</c> refresh active wardrobes 
		/// (used when loading new doll and their respective wardrobe.</param>
        void ReloadWardrobes(bool refreshActiveWardrobes = false)
        {
			// Clear out necessary objects
            wardrobes.Clear();
            taggedSkins.Clear();

            if (refreshActiveWardrobes) {
                doll.activeWardrobes.Clear();
            }

			// Validate that we have a doll selected
            if (doll.prototype == null || string.IsNullOrEmpty(doll.prototype.name)) {
                return;
            }

            // Load the wardrobes for this doll
            var wardrobeGuids = AssetDatabase.FindAssets("t:Wardrobe");

			// Loop through all the wardrobes from asset database
            for (int i = 0, maxI = wardrobeGuids.Length; i < maxI; i++) {
                var wardrobePath = AssetDatabase.GUIDToAssetPath(wardrobeGuids[i]);
                var wardrobe = AssetDatabase.LoadAssetAtPath<Wardrobe>(wardrobePath);

				// Ignore wardrobes not for the currently loaded doll
                if (string.CompareOrdinal(wardrobe.dollId, doll.prototype.name) != 0) continue;

                wardrobes.Add(wardrobe);

                // Go through all the articles to find the skins
                for (int j = 0, maxJ = wardrobe.articles.Length; j < maxJ; j++) {
                    var wardrobeArticle = wardrobe.articles[j];
                    var socket = doll.prototype.FindSocket(wardrobeArticle.socketId);

                    var socketTag = socket.tag;

					// Default socket tag is Other
                    if (string.IsNullOrEmpty(socketTag)) socketTag = "Other";

					// Initialize a list with Custom and Nothing
                    if (!taggedSkins.ContainsKey(socketTag)) {
                        taggedSkins.Add(socketTag, new List<string> {"Custom", "Nothing"});
                    }

                    var skins = taggedSkins[socketTag];

					// Add new skins to the list of tagged skins
                    if (!skins.Contains(wardrobeArticle.skinName)) skins.Add(wardrobeArticle.skinName);

					// Only allow the Nothing flag if a socket allows nude
                    if (!socket.allowNude) skins.Remove("Nothing");
                }

				// Add this to the list of active wardrobes
                if (refreshActiveWardrobes) {
                    doll.activeWardrobes.Add(wardrobe);
                }
            }
        }

        private const int SmallButtonWidth = 80;

        static GUIStyle _colorButtonStyle;

		/// <summary>
		/// Render a list of colors as buttons in the GUI so that they can be selected
		/// </summary>
		/// <returns>The current color for the selection</returns>
		/// <param name="colors">A list of colors to render as buttons</param>
		/// <param name="currentColor">The current color selected for this button selection</param>
        static Color RenderColorListButtons(Color[] colors, Color currentColor)
        {
			// Create a button style if we have not done so yet
            if (_colorButtonStyle == null || _colorButtonStyle.normal.background == null ||
                _colorButtonStyle.active.background == null) {
                var transp = new Texture2D(1, 1) {wrapMode = TextureWrapMode.Repeat};
                transp.SetPixel(0, 0, new Color(1, 1, 1, 0.5f));
                transp.Apply();

                var white = new Texture2D(1, 1) {wrapMode = TextureWrapMode.Repeat};
                white.SetPixel(0, 0, new Color(1, 1, 1, 1));
                white.Apply();

                _colorButtonStyle = new GUIStyle
                {
                    normal = {background = white},
                    margin = new RectOffset(2, 2, 2, 2),
                    active = {background = transp}
                };
            }

			// Loop through the colors and generate a GUI button for them with our custom layout
            foreach (var color in colors) {
                GUI.backgroundColor = color;

				// If the button is pressed, then return the new color that was selected
                if (GUILayout.Button(new GUIContent(" "), _colorButtonStyle, GUILayout.Width(22),
                    GUILayout.MaxWidth(20), GUILayout.Height(16), GUILayout.MaxHeight(16))) {
                    return color;
                }

                GUI.backgroundColor = Color.white;
            }

			// Just return back the current color, nothing was selected
            return currentColor;
        }

		/// <summary>
		/// Main function used to render the doll instance inspector
		/// </summary>
        void RenderDollInstanceInspector()
        {
            EditorGUILayout.Space();

			// Render the doll dropdown list, which is populated with all the currently found dolls within the asset database
            if (dollPrototypeIds != null) {
				// Find the current doll index
                var currentSelectedDoll = doll.prototype != null
                    ? dollPrototypeIds.IndexOf(doll.prototype.name)
                    : -1;

				if(DollInstance.helpBoxes)
				{
					EditorGUILayout.HelpBox("Select a doll from the available dolls in your current project.", MessageType.None);
				}

				// Render the popup menu to select new doll, selecting the current doll
                var newSelectedDoll =
                    EditorGUILayout.Popup("Doll: ", currentSelectedDoll, dollPrototypeIds.ToArray());

				// If a new doll has been selected, load the new prototype in and reload the wardrobe for it
                if (newSelectedDoll != currentSelectedDoll) {
					// Get the prototype by it's path in the asset database
                    var id = dollPrototypeIds[newSelectedDoll];
                    var path = idToPath[id];
                    var proto = AssetDatabase.LoadAssetAtPath<DollPrototype>(path);

					// Clear out the doll's selected outfit
                    doll.socketArticles.Clear();

					// And the animation list
					doll._viewAnimations.Clear();

					// Tell the factory that the doll has changed, rendering the default doll
                    PaperDollFactory.ChangeDollInstance(doll, proto);

					// Find this doll's wardrobes and put them into the cache
                    ReloadWardrobes(true);

                    // Give the doll a random tint
                    doll.RandomizeSkinColor();

					// Initialize the doll's sockets.  This sets doll's attachments based on the selections on the doll
                    PaperDollFactory.RefreshDoll(doll);

					// Flag the editor as dirty so we get a redraw
                    MarkDirty(doll);
                }
            }

			// No doll has been selected, therefore no reason to continue
            if (doll.prototype == null) return;

            EditorGUILayout.Space();

			if(DollInstance.helpBoxes)
			{
				EditorGUILayout.HelpBox("You can change the doll's skin tone from the preselection of colors below or choose your own by clicking the current color.", MessageType.None);
			}

			// Render the buttons for skin tone selection
            GUILayout.Label(new GUIContent("Skin Tone"), EditorStyles.whiteLargeLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);

			// Show the current skin color and provide a color field to change the skin to any color
            GUILayout.Label("Current: ", GUILayout.ExpandWidth(false));

            var newSkinColor =
                EditorGUILayout.ColorField(new GUIContent(), doll.skinColor, true, true, false, GUILayout.Width(50), GUILayout.MaxWidth(50));

			// If the skin color has changed, update the color and redraw the doll
            if (doll.skinColor != newSkinColor) {
                doll.skinColor = newSkinColor;
                PaperDollFactory.RefreshDoll(doll);
                MarkDirty(target);
            }

            // Simple button for randomly picking
            if (GUILayout.Button(new GUIContent("Random", "Randomize Skin Color"), EditorStyles.miniButton,
                GUILayout.ExpandHeight(false), GUILayout.Width(SmallButtonWidth))) {
                doll.RandomizeSkinColor();
                PaperDollFactory.RefreshDoll(doll);
                MarkDirty(doll);
                return;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(60);

			// Render the predefined set of color default buttons for skin on this doll
            GUILayout.Label("Defaults: ", GUILayout.ExpandWidth(false));
            var newSelectedSkinColor =
                RenderColorListButtons(doll.prototype.defaultSkinColors, doll.skinColor);

			// If a default skin color has been selected, update the color and redraw the doll
            if (doll.skinColor != newSelectedSkinColor) {
                doll.skinColor = newSelectedSkinColor;
                PaperDollFactory.RefreshDoll(doll);
                MarkDirty(target);
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

			// During play mode, create animations drop down so that you can change the animation through the inspector
            if (EditorApplication.isPlaying) {
				if(DollInstance.helpBoxes)
				{
					EditorGUILayout.HelpBox("Change the doll's animation from the following selection.", MessageType.None);
				}

                GUILayout.Label(new GUIContent("Animations"), EditorStyles.whiteLargeLabel);

                // Cache animations list if it is empty
				if(doll._viewAnimations.Count == 0)
				{
					doll._viewAnimations = doll.prototype.spineModelObject.GetSkeletonData(true)
						.Animations.Items.ToList()
						.ConvertAll(a => a.Name);
				}

				// Create the dropdown for selecting the animation on the doll
				var oldIndex = doll._viewAnimations.IndexOf(doll.view.AnimationName);
				var aIndex = EditorGUILayout.Popup("Current Anim", oldIndex, doll._viewAnimations.ToArray());
				if (oldIndex != aIndex) doll.view.AnimationName = doll._viewAnimations[aIndex];

				// Loop toggle button
                EditorGUI.BeginChangeCheck();
                doll.view.loop = EditorGUILayout.Toggle("Loop", doll.view.loop);

				// If the loop toggle has changed, then refresh the animation
                if (EditorGUI.EndChangeCheck()) {
                    var old = doll.view.AnimationName;
                    doll.view.AnimationName = null;
                    doll.view.AnimationName = old;
                }

                EditorGUILayout.Space();
            }

			// Render attachment points (like weapons) for the doll
            if (doll.prototype.attachments.Length > 0) {
				if(DollInstance.helpBoxes)
				{
					EditorGUILayout.HelpBox("Attachment points can be used to attach any sprite to predefined sections of the doll (eg. weapons).", MessageType.None);
				}

                GUILayout.Label(new GUIContent("Attachment Points"), EditorStyles.whiteLargeLabel);

				// Loop through all the attachments
                foreach (var point in doll.prototype.attachments) {
                    // grab the matching attachment in the instance
                    var attach = doll.FindSlotAttachment(point.id);
                    EditorGUI.BeginChangeCheck();

					// Create a Sprite selection to select a new sprite object to attach
                    var sprite = EditorGUILayout.ObjectField(point.id, attach != null ? attach.sprite : null,
                        typeof(Sprite), false) as Sprite;
                    if (!EditorGUI.EndChangeCheck()) continue;

					// If it changed, then set the attachment to the new sprite and refresh the doll
                    doll.SetSlotAttachment(point.id, sprite);
                    MarkDirty(doll);
                    PaperDollFactory.RefreshDoll(doll);
                }

                EditorGUILayout.Space();
            }

			if(DollInstance.helpBoxes)
			{
				EditorGUILayout.HelpBox("This is a list of the current wardrobes for your selected doll.  You can turn wardrobes on and off if you do not want to see them in the available wardrobe selection below.", MessageType.None);
			}

			// List out all the active wardrobes loaded, and create a way of toggling them on and off.  Useful if you don't want to see ALL the wardrobes
            GUILayout.Label(new GUIContent("Active Wardrobes"), EditorStyles.whiteLargeLabel);

			// Loop through all the loaded wardrobes
            for (int i = 0, maxI = wardrobes.Count; i < maxI; i++) {
                var wardrobe = wardrobes[i];
                var isActive = doll.activeWardrobes.Contains(wardrobe);

				// Create a toggle for the wardrobe
                var newActive = EditorGUILayout.Toggle(wardrobe.name, isActive);

                if (newActive == isActive) continue;

				// Add or remove the wardrobe from the list of active wardrobes when the toggle is changed
                if (newActive) {
                    doll.activeWardrobes.Add(wardrobe);
                }
                else {
                    doll.activeWardrobes.Remove(wardrobe);
                }

				// Redraw the UI
                Repaint();
            }

            EditorGUILayout.Space();

			if(DollInstance.helpBoxes)
			{
				EditorGUILayout.HelpBox("Customize your doll's look by selecting predefined costumes from the dropdowns below or by expanding the items to tint them or select from any wardrobe article for individual slots.", MessageType.None);
			}

            GUILayout.BeginHorizontal();

			// Create the current wardrobe section
            GUILayout.Label(new GUIContent("Current Wardrobe"), EditorStyles.whiteLargeLabel);
            GUILayout.FlexibleSpace();

			// Randomize outfit sets the doll's wardrobe to a random selection of pieces
            if (GUILayout.Button(new GUIContent("Rand Outfit", "Randomize Outfit"), EditorStyles.miniButtonLeft,
                GUILayout.ExpandHeight(false), GUILayout.Width(SmallButtonWidth))) {
                doll.RandomizeOutfit();
                MarkDirty(doll);
                return;
            }

			// Randomize outfit color goes through the doll's outfit and randomly selects colors for any of the pieces
            if (GUILayout.Button(new GUIContent("Rand Color", "Randomize Outfit Colors"), EditorStyles.miniButtonMid,
                GUILayout.ExpandHeight(false), GUILayout.Width(SmallButtonWidth))) {
                doll.RandomizeColors();
                MarkDirty(doll);
                return;
            }

			// Force the doll to refresh
            if (GUILayout.Button(new GUIContent("Refresh", "Refresh"), EditorStyles.miniButtonRight,
                GUILayout.ExpandHeight(false), GUILayout.Width(SmallButtonWidth))) {
                PaperDollFactory.RefreshDoll(doll);
                MarkDirty(doll);
                return;
            }
            GUILayout.EndHorizontal();

            // Go through all the tagged skins and create a dropdown for them in the inspector
            foreach (var pair in taggedSkins) {
                var tagSkinIndex = 0;
                if (doll._viewTagSkinSelection.ContainsKey(pair.Key)) {
                    tagSkinIndex = doll._viewTagSkinSelection[pair.Key];
                }

                var options = pair.Value.ToArray();

				// Draw the dropdown list of tags and skins within those tags
                var newTagSkinIndex = EditorGUILayout.Popup(pair.Key + ":", tagSkinIndex, options);
                if (newTagSkinIndex == tagSkinIndex) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    GUILayout.BeginVertical();
                    DrawCustomOptions(pair.Key, null);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    continue;
                }

                // We have a new skin selected from a particular tag.
                doll._viewTagSkinSelection[pair.Key] = newTagSkinIndex;

                string newSkin = null;
                if (options[newTagSkinIndex] != "Custom") {
                    newSkin = options[newTagSkinIndex];
                }

				// New skin has been selected from the dropdown, so we need to change all the sockets that use this tag to the new skin
                DrawCustomOptions(pair.Key, newSkin);
            }
        }

        private static void MarkDirty(Object obj)
        {
            EditorUtility.SetDirty(obj);
	        if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
		/// Draw out the custom foldout section of the inspector for a particular tag
		/// </summary>
		/// <param name="tag">The current tag that should be rendered</param>
		/// <param name="newSkin">If a new skin has been selected, this will be the new skin, otherwise it will be null</param>
        private void DrawCustomOptions(string tag, string newSkin)
        {
            var expand = false;
            doll._viewTagSkinExpanded.TryGetValue(tag, out expand);

			// Collapse the individual items to clean up the interface
            expand = doll._viewTagSkinExpanded[tag] = EditorGUILayout.Foldout(expand, new GUIContent("Items:"));

			// If we don't need to render the rest of the items, then return
            if (!expand && string.IsNullOrEmpty(newSkin)) return;

			// Go through all the sockets on the prototype
            var sockets = doll.prototype.sockets;
            foreach (var socket in sockets) {
				// If this socket is not tagged with the passed in tag, then loop to the next socket
                if ((socket.tag == null && string.CompareOrdinal(tag, "Other") != 0) ||
                    string.CompareOrdinal(tag, socket.tag) != 0) continue;

				// Find all articles for this socket
                var articles = doll.prototype.FindArticles(socket, doll.activeWardrobes.ToArray());
                var currentArticleInstance = doll.FindSocketArticle(socket);
                var index = -1;
                var articleStrings = new string[articles.Count + (socket.allowNude ? 1 : 0)];

				// Generate a list of articles as strings
                for (int j = 0, maxJ = articles.Count; j < maxJ; j++) {
                    articleStrings[j] = articles[j].id;
                }

				// If this is the article currently set on the doll, then store the index
                if (currentArticleInstance != null) {
                    for (int j = 0, maxJ = articles.Count; j < maxJ; j++) {
                        if (string.CompareOrdinal(articles[j].id, currentArticleInstance.articleId) == 0) {
                            index = j;
                        }
                    }
                }

				// Allow nude allows "nothing" to be set on this socket
                if (socket.allowNude) {
                    if (index < 0) {
                        index = articles.Count;
                    }

                    articleStrings[articles.Count] = "Nothing";
                }

				// Create the dropdown with all the articles
                var newIndex = EditorGUILayout.Popup(socket.id + ":", index, articleStrings);
                var foundTaggedSkin = false;

                // Switch us to the new skin if it's set.
                if (!string.IsNullOrEmpty(newSkin)) {
					// Loop through the articles to find the one associated with the new skin
					for (int j = 0, maxJ = articles.Count; j < maxJ && !foundTaggedSkin; j++) {
                        if (string.CompareOrdinal(articles[j].skinName, newSkin) == 0) {
                            newIndex = j;
                            foundTaggedSkin = true;
                        }
                    }

					// If nothing is associated with this tag, then set to nothing
                    if (!foundTaggedSkin && socket.allowNude) {
                        newIndex = articles.Count;
						foundTaggedSkin = true;
                    }
                }

				// If a new article has been selected, we change it here
                if (currentArticleInstance != null && newIndex >= 0 && newIndex < articles.Count) {
                    var article = articles[newIndex];

					// If the article is tintable, render the color selection for it
                    if (article.HasTintableAttachment) {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(40);

						// Create an override section to override individual slots on this article with different colors
                        currentArticleInstance._viewArticlesExpanded = EditorGUILayout.Foldout(
                            currentArticleInstance._viewArticlesExpanded, new GUIContent("Overrides"));
                        GUILayout.FlexibleSpace();

						// Create a color field with the tint for this socket
                        GUILayout.Label(new GUIContent("Tint:", "Socket Tint"),
                            GUILayout.ExpandWidth(false));

                        var colorA = currentArticleInstance.slots.Count > 0
                            ? currentArticleInstance.slots[0].tintColor
                            : Color.white;
                        var colorB = EditorGUILayout.ColorField(new GUIContent(), colorA, false, true, false,
                            GUILayout.Width(30), GUILayout.MaxWidth(30));

						// Change the color of this slot and refresh the doll
                        if (colorA != colorB) {
                            currentArticleInstance.SetSlotsTint(colorB);
                            PaperDollFactory.RefreshDoll(doll);
                            MarkDirty(target);
                        }

						// Create the list of default colors for this article
                        if (article.defaultColors.Length > 0) {
                            GUILayout.Label(new GUIContent("Def:", "Default Palette"),
                                GUILayout.ExpandWidth(false));

                            colorA = new Color(-1, -1, -1, 1);
                            colorB = RenderColorListButtons(article.defaultColors, colorA);

							// Changed the color to a default color, so change and refresh
                            if (colorA != colorB) {
                                currentArticleInstance.SetSlotsTint(colorB);
                                PaperDollFactory.RefreshDoll(doll);
                                MarkDirty(target);
                            }
                        }
                        GUILayout.EndHorizontal();

						// Render the override section, listing out the individual attachments
                        if (currentArticleInstance._viewArticlesExpanded) {
                            for (int j = 0, maxJ = article.attachments.Length; j < maxJ; j++) {
                                var attachment = article.attachments[j];

                                if (attachment.tintMode != WardrobeArticleAttachment.TintMode.Generic) continue;

                                GUILayout.BeginHorizontal();
                                GUILayout.Space(40);
                                GUILayout.Label(attachment.slotName, GUILayout.ExpandWidth(false));

                                GUILayout.FlexibleSpace();

								// Override a particular attachment's tint color
                                GUILayout.Label(new GUIContent("Tint:", "Article Tint"),
                                    GUILayout.ExpandWidth(false));

                                colorA = currentArticleInstance.GetTint(attachment.slotName);
                                colorB = EditorGUILayout.ColorField(new GUIContent(), colorA, false, true, false,
                                    GUILayout.Width(30), GUILayout.MaxWidth(30));

								// if the color changed on this slot attachment, change it and refresh
                                if (colorA != colorB) {
                                    currentArticleInstance.SetSlotTint(attachment.slotName, colorB);
                                    PaperDollFactory.RefreshDoll(doll);
                                    MarkDirty(target);
                                }

								// Check the default palette for this particular attachment and generate color buttons
                                if (attachment.defaultColors.Length > 0) {
                                    GUILayout.Label(new GUIContent("Def:", "Default Palette"),
                                        GUILayout.ExpandWidth(false));

                                    colorA = RenderColorListButtons(attachment.defaultColors, colorB);

									// Color changed to a default color for this attachment.  Change it and refresh the doll
                                    if (colorA != colorB) {
                                        currentArticleInstance.SetSlotTint(attachment.slotName, colorA);
                                        PaperDollFactory.RefreshDoll(doll);
                                        MarkDirty(target);
                                    }
                                }

                                GUILayout.EndHorizontal();

                                EditorGUILayout.Space();
                            }
                        }
                    }
                }

                EditorGUILayout.Space();

				// If a new wardrobe article has not been selected, then continue.  No reason to continue
                if (newIndex == index) continue;

                var art = newIndex == articles.Count ? null : articles[newIndex];
                var sockData = doll.SetSocketArticle(socket, art);

				// If we've changed articles and we've selected a tag before, lets change us back to Custom
				if(!foundTaggedSkin && !string.IsNullOrEmpty(socket.tag) && doll._viewTagSkinSelection.ContainsKey(socket.tag))
				{
					doll._viewTagSkinSelection[socket.tag] = 0;
				}

				// When a new article has been selected, randomize it's color
                if (art != null)
                    sockData.RandomizeColors(art);

				// Refresh the doll
                PaperDollFactory.RefreshDoll(doll);
                MarkDirty(target);
            }
        }
    }
}