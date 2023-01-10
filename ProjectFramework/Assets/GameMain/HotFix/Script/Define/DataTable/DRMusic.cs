using GameFramework.DataTable;
using GameFrameworkPackage;
using LitJson;
namespace Defines
{
	public class DRMusic : IDataRow
	{
		public int Id
		{
			get;
			protected set;
		}
		
		public string Name
		{
			get;
			private set;
		}
		
		public float VolumeRate
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
			Name = text[index++];
			VolumeRate= float.Parse(text[index++]);
			return true;
		}
	}
}
