namespace Mediaful.Shared.Layout
{
    /// <summary>
    /// Helper class to manage the state of the layout.
    /// </summary>
    public class LayoutState
    {
        /// <summary>
        /// OnChange event used to notify state changes.
        /// </summary>
        public event Action OnChange;

        /// <summary>
        /// Flag to determine if the sidebar layout is shown.
        /// </summary>
        public bool IsSidebarExpanded { get; set; } = true;

        /// <summary>
        /// Toggles the layout sidebar.
        /// </summary>
        public void SidebarToggle()
        {
            IsSidebarExpanded = !IsSidebarExpanded;
            NotifyStateChanged();
        }

        /// <summary>
        /// Collapse layout sidebar.
        /// </summary>
        public void SidebarCollapse()
        {
            IsSidebarExpanded = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// Expand layout sidebar.
        /// </summary>
        public void SidebarExpand()
        {
            IsSidebarExpanded = true;
            NotifyStateChanged();
        }

        /// <summary>
        /// Invokes OnChange event to notify state changes.
        /// </summary>
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
