using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;

namespace UnityGameFramework.Editor
{
    public class ByteTool
    {
        public static byte[] Object2Bytes(object obj)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }
            return buff;
        }

        public static void WriteStringByFile(string path, string content)
        {
            byte[] dataByte = Encoding.GetEncoding("UTF-8").GetBytes(content);

            CreateFile(path, dataByte);
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
            }
        }


        public static void CreateFile(string path, byte[] byt)
        {
            try
            {
                FileTool.CreatFilePath(path);
                File.WriteAllBytes(path, byt);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

}

