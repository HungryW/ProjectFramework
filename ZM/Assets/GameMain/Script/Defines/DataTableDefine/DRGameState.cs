using GameFramework.DataTable;
using GameFrameworkPackage;
namespace Defines.DataTable
{
	public class DRGameState : IDataRow
	{
		public int Id
		{
			get;
			protected set;
		}
		
		public string GameStateName
		{
			get;
			private set;
		}
		
		public string SceneAssetName
		{
			get;
			private set;
		}
		
		public string BackgroundMusicId
		{
			get;
			private set;
		}
		
		public string CameraPos
		{
			get;
			private set;
		}
		
		public float CameraSize
		{
			get;
			private set;
		}
		
		public int IsCanDrag
		{
			get;
			private set;
		}
		
		public float CameraMinSize
		{
			get;
			private set;
		}
		
		public float CameraMaxSize
		{
			get;
			private set;
		}
		
        public bool ParseDataRow(string dataRowString, object userData)
        {
            string[] text = CDataTableExtension.SplitDataRow(dataRowString);
            int index = 1;
            Id = int.Parse(text[index++]);
            index++;
            GameStateName = text[index++];
            SceneAssetName = text[index++];
            BackgroundMusicId = text[index++];
            CameraPos = text[index++];
            CameraSize = float.Parse(text[index++]);
            IsCanDrag = int.Parse(text[index++]);
            CameraMinSize = float.Parse(text[index++]);
            CameraMaxSize = float.Parse(text[index++]);
            return true;
        }

        public bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            throw new System.NotImplementedException();
        }
    }
}
