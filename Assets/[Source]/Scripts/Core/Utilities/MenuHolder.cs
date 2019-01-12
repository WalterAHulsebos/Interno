using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Core.MenuHolder
{
    public class MenuHolder : MonoBehaviour
    {
        public Menu[] menus;

        public void Awake()
        {
            NavigationListener listener;
            foreach (Menu menu in menus)
                foreach (Navigational navigational in menu.navigation)
                {
                    listener = navigational.button.gameObject.AddComponent(typeof(NavigationListener)) as NavigationListener;
                    listener.Navigational = navigational;
                    if (navigational.closesSelf)
                        listener.self = menu.self;
                }
        }
    }

    public class NavigationListener : MonoBehaviour
    {
        [NonSerialized]
        public RectTransform self;
        private Navigational navigational;
        public Navigational Navigational
        {
            private get
            {
                return navigational;
            }
            set
            {
                navigational = value;
                navigational.button.onClick.AddListener(OnNavigation);
            }
        }

        private void OnNavigation()
        {
            foreach (RectTransform menu in navigational.opens)
                menu.gameObject.SetActive(true);
            foreach (RectTransform menu in navigational.closes)
                menu.gameObject.SetActive(false);
            if (navigational.closesSelf)
                self.gameObject.SetActive(false);
        }
    }

    [Serializable]
    public struct Menu
    {
        public RectTransform self;
        public Navigational[] navigation;
    }

    [Serializable]
    public struct Navigational
    {
        public Button button;
        public RectTransform[] opens, closes;
        public bool closesSelf;
    }
}