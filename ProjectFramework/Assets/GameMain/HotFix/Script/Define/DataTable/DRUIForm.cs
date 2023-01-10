using GameFramework.DataTable;
using GameFrameworkPackage;
using LitJson;
namespace Defines
{
	public class DRUIForm : IDataRow
	{
		public int Id
		{
			get;
			protected set;
		}
		
		public string AssetName
		{
			get;
			private set;
		}
		
		public string GroupName
		{
			get;
			private set;
		}
		
		public bool AllowMultiInstance
		{
			get;
			private set;
		}
		
		public bool PauseCoveredUIForm
		{
			get;
			private set;
		}
		
		public bool HotFix
		{
			get;
			private set;
		}
		
		public bool NeedBlurBg
		{
			get;
			private set;
		}
		
		public bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
		{
			return false;
		}
		public bool ParseDataRow(string dataRowText, object userData)
		{
			string[] text = CDataTableExtension.SplitDataRow(dataRowText);
			int index = 1;
			Id= int.Parse(text[index++]);
			index++;
			AssetName = text[index++];
			GroupName = text[index++];
			AllowMultiInstance= bool.Parse(text[index++]);
			PauseCoveredUIForm= bool.Parse(text[index++]);
			HotFix= bool.Parse(text[index++]);
			NeedBlurBg= bool.Parse(text[index++]);
			return true;
		}
	}
}
