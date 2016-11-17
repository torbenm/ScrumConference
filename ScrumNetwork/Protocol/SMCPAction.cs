namespace ScrumNetwork.Protocol
{
    /// <summary>
    /// Diese Klasse listet alle ActionIDs für das SMCP Protokoll auf
    /// 0 - 9 für Connection Information
    /// 10+ für Informationen zum Meeting selbst
    /// </summary>
    public enum SMCPAction : byte
    {
        ASSIGN_CLIENT_ID = 1,
        CLIENT_INFO = 2,
        ONLINE_CLIENTS = 3,

        REQUEST_ITEM_ID = 10,
        ASSIGN_ITEM_ID = 11,
        ADD_ITEM = 12,
        REMOVE_ITEM = 13,
        UPDATE_ITEM = 14,
        ALL_ITEMS = 15,
        START_EDITING = 16, //
        END_EDITING = 17, //
        FOCUS_ON_ITEM = 18, //
        VIEW_CHANGED = 19
    }
}
