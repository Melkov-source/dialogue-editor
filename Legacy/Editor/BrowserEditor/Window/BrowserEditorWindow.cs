using System;
using System.Collections.Generic;
using History.Editor.BrowserEditor.Window.Enums;
using History.Editor.UIToolkitExtensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace History.Editor.BrowserEditor.Window
{
    public class BrowserEditorWindow : EditorWindow
    {
        public event Action<CategoryChildInfo> OnSelectCategoryChildInfo;
        
        private static BrowserMode _mode;

        private const string CATEGORY_LIST_CLASS = "category-list";
        private const string GRID_CLASS = "grid";
        private const string BUTTON_NEXT_CLASS = "button-next";

        private CategoriesController _categoriesController;
        private ScrollView _gridElement;

        private Button _nextButton;

        private CategoryChildInfo _selectedCategoryChildInfo;
        
        
        public static BrowserEditorWindow Open(BrowserMode mode, List<CategoryInfo> categories)
        {
            _mode = mode;
            
            var window = OpenInternal();
            
            window.SetupCategories(categories);

            return window;
        }

        private static BrowserEditorWindow OpenInternal()
        {
            var window = GetWindow<BrowserEditorWindow>();
            
            window.titleContent = new GUIContent("Browser");

            window.maxSize = new Vector2(500,  500);
            window.minSize = new Vector2(500,  500);
            
            window.Show();

            return window;
        }
        
        private void CreateGUI()
        {
            var view = TemplateLoader.Get(TemplateType.BrowserWindow);
            var style = StyleLoader.Get(StyleType.BrowserWindowStyles);

            var categoriesScrollView = view.Q<ScrollView>(className: CATEGORY_LIST_CLASS);
            _categoriesController = new CategoriesController(categoriesScrollView);

            _categoriesController.OnSelect += OnSelectCategoryChildInfoHandle;

            _gridElement = view.Q<ScrollView>(className: GRID_CLASS);

            _nextButton = view.Q<Button>(className: BUTTON_NEXT_CLASS);

            _nextButton.clicked += OnNextHandle;
            
            switch (_mode)
            {
                case BrowserMode.Grid:
                    _gridElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _categoriesController.Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    break;
                case BrowserMode.Categories:
                    _gridElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _categoriesController.Root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    break;
            }
            
            rootVisualElement.styleSheets.Add(style);
            rootVisualElement.Add(view);

            _selectedCategoryChildInfo = null;
        }

        private void OnSelectCategoryChildInfoHandle(CategoryChildInfo info)
        {
            _selectedCategoryChildInfo = info;
        }

        private void OnNextHandle()
        {
            if (_selectedCategoryChildInfo == null)
            {
                return;
            }
            
            OnSelectCategoryChildInfo?.Invoke(_selectedCategoryChildInfo);
        }

        private void SetupCategories(List<CategoryInfo> categories)
        {
            _categoriesController.Setup(categories);
        }

        private void SetupGrid()
        {
            
        }
    }
}