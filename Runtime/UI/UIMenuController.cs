using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UnityUtility.Ui
{
    public class UIMenuController<T> : GenericSingleton<UIMenuController<T>> where T : Enum
    {
        [Serializable]
        public struct MenuPanelEntry
        {
            public T pageType;
            public GameObject panelObject;
        }

        [Header("Buttons")]
        [SerializeField] private Button backButton;

        [Header("Panel Mapping")]
        [SerializeField] private List<MenuPanelEntry> menuPanelsList = new();

        private T currentPage;
        private bool isInitialized = false;
        private Dictionary<T, GameObject> menuPanels;
        private readonly Stack<T> navigationStack = new();

        public event Action<T> OnPageChanged;

        protected override void Awake()
        {
            base.Awake();

            SetupMenuDictionary();
            SetBackButton();

            isInitialized = true;
        }

        private void SetupMenuDictionary()
        {
            menuPanels = new();
            foreach (var entry in menuPanelsList)
            {
                if (entry.panelObject != null)
                    menuPanels[entry.pageType] = entry.panelObject;
            }
        }

        private void SetBackButton()
        {
            if (backButton != null)
                backButton.onClick.AddListener(OnBackPressed);
        }

        public virtual void NavigateTo(T newPage, bool clearHistory = false)
        {
            if (!isInitialized) SetupMenuDictionary();

            if (EqualityComparer<T>.Default.Equals(currentPage, newPage))
                return;

            if (clearHistory)
                navigationStack.Clear();
            else if (!EqualityComparer<T>.Default.Equals(currentPage, default))
                navigationStack.Push(currentPage);

            SwitchToPage(newPage);
        }

        public void NavigaToLastPanel(bool clearHistory = false)
        {
            if (!isInitialized) SetupMenuDictionary();

            if (navigationStack.Count == 0)
            {
                Debug.LogWarning($"{nameof(UIMenuController<T>)}: No previous panel to navigate to.");
                return;
            }

            T toNavigate = navigationStack.Pop();

            if (clearHistory)
                navigationStack.Clear();

            SwitchToPage(toNavigate);
        }

        private void SwitchToPage(T newPage)
        {
            foreach (var kvp in menuPanels)
            {
                bool isActive = EqualityComparer<T>.Default.Equals(kvp.Key, newPage);
                var panel = kvp.Value;

                if (panel == null) continue;

                if (panel.activeSelf != isActive)
                {
                    if (panel.TryGetComponent<IMenuPage>(out var menuPage))
                    {
                        if (isActive) menuPage.OnPageEnter();
                        else menuPage.OnPageExit();
                    }

                    panel.SetActive(isActive);
                }
            }

            currentPage = newPage;
            OnPageChanged?.Invoke(newPage);
            UpdateBackButtonState();
        }

        private void UpdateBackButtonState()
        {
            if (backButton != null)
                backButton.gameObject.SetActive(navigationStack.Count > 0);
        }

        public void OnBackPressed()
        {
            if (navigationStack.Count > 0)
            {
                var previousPage = navigationStack.Pop();
                SwitchToPage(previousPage);
            }
        }

        /// <summary>
        /// Ritorna la pagina attuale.
        /// </summary>
        public T GetCurrentPanelIndex() => currentPage;

        public GameObject GetCurrentPanelObject()
        {
            if (menuPanels.TryGetValue(currentPage, out var panel))
                return panel;

            return null;
        }

        /// <summary>
        /// Mostra o nasconde un pannello specifico (senza cambiare la pagina corrente).
        /// </summary>
        public void SetPageActive(T page, bool isActive)
        {
            if (menuPanels.TryGetValue(page, out var panel))
                panel.SetActive(isActive);
        }

#if UNITY_EDITOR
        [ContextMenu("Reset Navigation")]
        private void ResetNavigation()
        {
            navigationStack.Clear();
            Debug.Log($"{nameof(UIMenuController<T>)}: navigation reset.");
        }

        [ContextMenu("Fill Navigation")]
        public void FillNavigation()
        {
            menuPanels.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                T test = child.GetComponent<T>();
                menuPanels[test] = child.gameObject;
            }
        }
#endif
    }
}
