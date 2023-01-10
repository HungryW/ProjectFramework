using GameFramework.DataTable;
using GameFrameworkPackage;
using LitJson;
namespace Defines
{
	public class DRSound : IDataRow
	{
		public int Id
		{
			get;
			protected set;
		}
		
		public string LogicName
		{
			get;
			private set;
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
		
		public bool Loop
		{
			get;
			private set;
		}
		
		public float Volume
		{
			get;
			private set;
		}
		
		public float SpatialBlend
		{
			get;
			private set;
		}
		
		public float MaxDistance
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
			LogicName = text[index++];
			AssetName = text[index++];
			index++;
			index++;
			Priority= int.Parse(text[index++]);
			Loop= bool.Parse(text[index++]);
			Volume= float.Parse(text[index++]);
			SpatialBlend= float.Parse(text[index++]);
			MaxDistance= float.Parse(text[index++]);
			return true;
		}
	}
}
