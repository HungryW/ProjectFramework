using GameFramework.DataTable;
using GameFrameworkPackage;
using LitJson;
namespace Defines
{
	public class DRIapId : IDataRow
	{
		public int Id
		{
			get;
			protected set;
		}
		
		public string name
		{
			get;
			private set;
		}
		
		public string googleLink
		{
			get;
			private set;
		}
		
		public string iosLink
		{
			get;
			private set;
		}
		
		public string testLink
		{
			get;
			private set;
		}
		
		public float cost
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
			name = text[index++];
			googleLink = text[index++];
			iosLink = text[index++];
			testLink = text[index++];
			cost= float.Parse(text[index++]);
			return true;
		}
	}
}
