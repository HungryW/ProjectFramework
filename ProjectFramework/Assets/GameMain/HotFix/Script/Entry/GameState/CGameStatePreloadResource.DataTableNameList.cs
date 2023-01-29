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
                "Particle",
            };
            return arrName;
        }
    }
}