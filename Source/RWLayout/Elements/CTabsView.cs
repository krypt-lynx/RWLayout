using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Tab page for tab view
    /// </summary>
    public class CTabPage : CElement
    {
        bool guiNotReady = true;
        public void ConstructGUIIfNeeded()
        {
            if (guiNotReady)
            {
                ConstructGUI();
                guiNotReady = false;
                SetNeedsUpdateLayout();
            }
        }

        /// <summary>
        /// Tab title
        /// </summary>
        /// <returns>tab title</returns>
        public virtual string Title()
        {
            return NamePrefix();
        }

        /// <summary>
        /// Put gui constuction code in override of this method to create UI on demand
        /// </summary>
        protected virtual void ConstructGUI()
        {

        }
    }

    /// <summary>
    /// Primitive tab panel implementation (likely have wrong geometry)
    /// </summary>
    public class CTabsPanel : CElement
    {
        public List<TabRecord> Tabs = new List<TabRecord>();
        Rect tabsRegion;

        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();

            tabsRegion = Rect.MinMaxRect(Bounds.xMin, Bounds.yMax, Bounds.xMax, Bounds.yMax).GUIRounded(); // yMax is intentional
        }

        public override Vector2 tryFit(Vector2 size)
        {
            return new Vector2(0, 32);
        }

        public override void DoContent()
        {
            base.DoContent();

            TabDrawer.DrawTabs(tabsRegion, Tabs);
        }
    }

    /// <summary>
    /// Tab View
    /// </summary>
    public class CTabsView : CElement
    {
        List<CTabPage> tabs = new List<CTabPage>();
        CTabsPanel headersPanel;
        CElement tabsHost;

        int tabIndex = 0;
        /// <summary>
        /// visisble tab index
        /// </summary>
        public int TabIndex
        {
            get => tabIndex;
            set
            {
                tabIndex = value;
                for (int i = 0; i < tabs.Count; i++)
                {
                    bool selected = tabIndex == i;

                    tabs[i].Hidden = !selected;
                    headersPanel.Tabs[i].selected = selected;
                    if (selected)
                    {
                        tabs[i].ConstructGUIIfNeeded();
                    }
                }
            }
        }

        public CTabsView()
        {
            tabsHost = AddElement(new CElement());
            headersPanel = AddElement(new CTabsPanel()); 

            this.StackTop((headersPanel, headersPanel.intrinsicHeight), tabsHost);

        }

        /// <summary>
        /// Add a tab to tab view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tab">the tab</param>
        /// <returns></returns>
        public T AddTab<T>(T tab) where T : CTabPage
        {
            var newIndex = tabs.Count;

            var selected = TabIndex == newIndex;

            headersPanel.Tabs.Add(new TabRecord(tab.Title(), () =>
            {
                TabIndex = newIndex;
            }, selected));

            tabs.Add(tab);
            tabsHost.AddElement(tab);
            tabsHost.Embed(tab);
            tab.Hidden = !selected;
            if (selected)
            {
                tab.ConstructGUIIfNeeded();
            }
                

            return tab;
        }

        public int RemoveTab(CTabPage tab)  
        {
            var index = tabs.IndexOf(tab);
            if (index != -1)
            {
                RemoveTabAt(index);
            }
            return index;
        }

        private void RemoveTabAt(int index)
        {
            var tab = tabs[index];
            headersPanel.Tabs.RemoveAt(index);
            tabs.RemoveAt(index);
            tabsHost.RemoveElement(tab);

            if (tabIndex >= index)
            {
                TabIndex = Mathf.Max(tabIndex - 1, 0);
            }
        }
    }

}
