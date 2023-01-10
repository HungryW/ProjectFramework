using GameFramework.DataTable;
using GameFrameworkPackage;
using LitJson;
namespace Defines
{
	public class DRUISound : IDataRow
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
		
		public int Priority
		{
			get;
			private set;
		}
		
		public float Volume
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
			Priority= int.Parse(text[index++]);
			Volume= float.Parse(text[index++]);
			return true;
		}
	}
}
