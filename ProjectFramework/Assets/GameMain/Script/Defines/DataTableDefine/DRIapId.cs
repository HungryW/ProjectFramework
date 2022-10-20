using GameFramework.DataTable;
using GameFrameworkPackage;
using LitJson;
namespace Defines.DataTable
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


        public bool ParseDataRow(string dataRowString, object userData)
        {
            string[] text = CDataTableExtension.SplitDataRow(dataRowString);
            int index = 1;
            Id = int.Parse(text[index++]);
            name = text[index++];
            googleLink = text[index++];
            iosLink = text[index++];
            testLink = text[index++];
            cost = float.Parse(text[index++]);
            return true;
        }

        public bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            throw new System.NotImplementedException();
        }
    }
}
