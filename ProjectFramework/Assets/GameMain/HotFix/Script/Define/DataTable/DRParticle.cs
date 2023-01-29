using GameFramework.DataTable;
using GameFrameworkPackage;
using LitJson;
namespace Defines
{
	public class DRParticle : IDataRow
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
		
		public string ClassName
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
			AssetName = text[index++];
			GroupName = text[index++];
			ClassName = text[index++];
			return true;
		}
	}
}
