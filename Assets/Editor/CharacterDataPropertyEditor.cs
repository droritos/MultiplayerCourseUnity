using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor
{
    [CustomPropertyDrawer(typeof(CharacterData))]
    public class CharacterDataPropertyDrawer : PropertyDrawer
    {
        private const int PreviewRetryDelayMS = 100;
        private const string StyleSheetSearchFilter = "CharacterDataPropertyDrawer t:StyleSheet";

        // CSS class names
        private const string RootClass = "character-data-root";
        private const string MainContainerClass = "character-data-main-container";
        private const string IconContainerClass = "character-data-icon-container";
        private const string IconPreviewClass = "character-data-icon-preview";
        private const string PrefabContainerClass = "character-data-prefab-container";
        private const string PrefabPreviewClass = "character-data-prefab-preview";
        private const string FieldsContainerClass = "character-data-fields-container";
        private const string PropertyFieldClass = "character-data-property-field";
        private const string BaseFieldClass = "unity-base-field__aligned";

        // Element names
        private const string IconPreviewName = "character-icon-preview";
        private const string PrefabPreviewName = "prefab-preview";

        private static StyleSheet _cachedStyleSheet;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = CreateRootElement();
            var mainContainer = CreateMainContainer();

            mainContainer.Add(CreateIconPreviewSection());
            mainContainer.Add(CreatePrefabPreviewSection());
            mainContainer.Add(CreateFieldsSection(root, property));

            root.Add(mainContainer);
            return root;
        }

        private VisualElement CreateRootElement()
        {
            var root = new VisualElement();

            var styleSheet = LoadStyleSheet();
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }

            root.AddToClassList(RootClass);
            return root;
        }

        private VisualElement CreateMainContainer()
        {
            var container = new VisualElement();
            container.AddToClassList(MainContainerClass);
            return container;
        }

        private VisualElement CreateIconPreviewSection()
        {
            var container = new VisualElement();
            container.AddToClassList(IconContainerClass);

            var preview = new VisualElement { name = IconPreviewName };
            preview.AddToClassList(IconPreviewClass);

            container.Add(preview);
            return container;
        }

        private VisualElement CreatePrefabPreviewSection()
        {
            var container = new VisualElement();
            container.AddToClassList(PrefabContainerClass);

            var preview = new VisualElement { name = PrefabPreviewName };
            preview.AddToClassList(PrefabPreviewClass);

            container.Add(preview);
            return container;
        }

        private VisualElement CreateFieldsSection(VisualElement root, SerializedProperty property)
        {
            var container = new VisualElement();
            container.AddToClassList(FieldsContainerClass);

            var nameProperty = property.FindPropertyRelative("name");
            var prefabProperty = property.FindPropertyRelative("prefab");
            var iconProperty = property.FindPropertyRelative("characterIcon");

            var nameField = CreateNameField(nameProperty);
            var prefabField = CreatePrefabField(root, prefabProperty);
            var iconField = CreateIconField(root, iconProperty);

            container.Add(nameField);
            container.Add(prefabField);
            container.Add(iconField);

            UpdateIconPreview(root, iconProperty.objectReferenceValue as Sprite);
            UpdatePrefabPreview(root, prefabProperty.objectReferenceValue as GameObject);

            return container;
        }

        private PropertyField CreateNameField(SerializedProperty nameProperty)
        {
            var field = new PropertyField { name = "name-field", label = "Name" };

            field.AddToClassList(BaseFieldClass);
            field.AddToClassList(PropertyFieldClass);
            field.BindProperty(nameProperty);

            return field;
        }

        private ObjectField CreatePrefabField(VisualElement root, SerializedProperty prefabProperty)
        {
            var field = new ObjectField { name = "prefab-field", label = "Prefab", objectType = typeof(GameObject) };

            field.AddToClassList(BaseFieldClass);
            field.AddToClassList(PropertyFieldClass);
            field.BindProperty(prefabProperty);
            field.RegisterValueChangedCallback(evt =>
                UpdatePrefabPreview(root, evt.newValue as GameObject));

            return field;
        }

        private ObjectField CreateIconField(VisualElement root, SerializedProperty iconProperty)
        {
            var field = new ObjectField
            {
                name = "character-icon-field", label = "Character Icon", objectType = typeof(Sprite)
            };

            field.AddToClassList(BaseFieldClass);
            field.AddToClassList(PropertyFieldClass);
            field.BindProperty(iconProperty);
            field.RegisterValueChangedCallback(evt =>
                UpdateIconPreview(root, evt.newValue as Sprite));

            return field;
        }

        private void UpdateIconPreview(VisualElement root, Sprite sprite)
        {
            var iconPreview = root.Q<VisualElement>(IconPreviewName);
            if (iconPreview == null)
            {
                return;
            }

            iconPreview.style.backgroundImage = sprite != null
                ? Background.FromSprite(sprite)
                : StyleKeyword.Null;
        }

        private void UpdatePrefabPreview(VisualElement root, GameObject prefab)
        {
            var prefabPreview = root.Q<VisualElement>(PrefabPreviewName);
            if (prefabPreview == null)
            {
                return;
            }

            if (prefab == null)
            {
                prefabPreview.style.backgroundImage = StyleKeyword.Null;
                return;
            }

            SetPrefabPreviewTexture(prefabPreview, prefab);
        }

        private void SetPrefabPreviewTexture(VisualElement previewElement, GameObject prefab)
        {
            var previewTexture = AssetPreview.GetAssetPreview(prefab);

            if (previewTexture != null)
            {
                previewElement.style.backgroundImage = Background.FromTexture2D(previewTexture);
                return;
            }

            // Asset preview might not be ready yet, retry after a short delay
            previewElement.schedule
                .Execute(() => RetrySetPreviewTexture(previewElement, prefab))
                .StartingIn(PreviewRetryDelayMS);
        }

        private void RetrySetPreviewTexture(VisualElement previewElement, GameObject prefab)
        {
            var previewTexture = AssetPreview.GetAssetPreview(prefab);
            if (previewTexture != null)
            {
                previewElement.style.backgroundImage = Background.FromTexture2D(previewTexture);
            }
        }

        private static StyleSheet LoadStyleSheet()
        {
            if (_cachedStyleSheet != null)
            {
                return _cachedStyleSheet;
            }

            string[] guids = AssetDatabase.FindAssets(StyleSheetSearchFilter);
            if (guids.Length == 0)
            {
                Debug.LogWarning("CharacterDataPropertyDrawer.uss stylesheet not found in project.");
                return null;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            _cachedStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath);

            return _cachedStyleSheet;
        }
    }
}
