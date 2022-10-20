using GameFramework.DataTable;
using GameFrameworkPackage;
namespace Defines.DataTable
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


        public bool ParseDataRow(string dataRowString, object userData)
        {
            string[] text = CDataTableExtension.SplitDataRow(dataRowString);
            int index = 1;
            Id = int.Parse(text[index++]);
            index++;
            AssetName = text[index++];
            GroupName = text[index++];
            AllowMultiInstance = bool.Parse(text[index++]);
            PauseCoveredUIForm = bool.Parse(text[index++]);
            HotFix = bool.Parse(text[index++]);
            NeedBlurBg = bool.Parse(text[index++]);
            return true;
        }

        public bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            throw new System.NotImplementedException();
        }
    }
}
