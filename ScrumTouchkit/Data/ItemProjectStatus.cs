namespace ScrumTouchkit.Data
{
    /// <summary>
    /// Gibt an, zu welchem Backlog ein Item gehört
    /// </summary>
    public enum ItemBacklogStatus : int
    {
        /// <summary>
        /// Das Item gehört zum Product Backlog
        /// </summary>
        PRODUCT_BACKLOG = 0,
        /// <summary>
        /// Das Item gehört zum Sprint Backlog
        /// </summary>
        SPRINT_BACKLOG = 1
    }
}
