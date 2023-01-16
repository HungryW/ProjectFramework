namespace HotFixEntry
{
    public partial class CGameStatePreloadResource : CGameStateBase
    {
        private string[] _GetDataTableNameList()
        {
            string[] arrName =
            {
                "GameState",
                "UIForm",
                "Music",
                "Sound",
                "UISound",
                "IapId",
            };
            return arrName;
        }
    }
}