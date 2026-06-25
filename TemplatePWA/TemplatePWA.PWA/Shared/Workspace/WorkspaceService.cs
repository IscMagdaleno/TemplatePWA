using TemplatePWA.PWA.Shared.Common;
using System;
using System.Collections.Generic;

namespace TemplatePWA.PWA.Shared.Workspace
{
    public class WorkspaceService
    {
        public event Action<WorkspaceTab> OnMenuTabSelected;
        public void AddComponenteTab(WorkspaceTab tab)
        {
            OnMenuTabSelected?.Invoke(tab);
        }

        public event Action MenuChanged;
        private List<MenuItemModel> _currentMenuItems = new List<MenuItemModel>();
        public IReadOnlyList<MenuItemModel> CurrentMenuItems => _currentMenuItems;
        
        public void SetMenuItems(List<MenuItemModel> menuItems)
        {
            _currentMenuItems = menuItems;
            MenuChanged?.Invoke();
        }

        public event Action<string, string> TabNameChanged;
        public void SetCurrentTabName(string tabName, string iconText)
        {
            TabNameChanged?.Invoke(tabName, iconText);
        }

        public event Action<TipoEstadoControl> TabStateChanged;
        public void SetCurrentTabState(TipoEstadoControl tabState)
        {
            TabStateChanged?.Invoke(tabState);
        }

        public event Action<bool> CloseCurrentTab;
        public void SetCloseCurrentTab(bool withWarning = false)
        {
            CloseCurrentTab?.Invoke(withWarning);
        }
    }
}
