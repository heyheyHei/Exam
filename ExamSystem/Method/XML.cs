using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ExamSystem.Method
{
    public class XML
    {

    }
    public class Serialize<T>
    {
        /// <summary>
        /// 序列化List集合
        /// </summary>
        /// <param name="list"></param>
        /// <param name="path">保存路径</param>
        public static void SerializeMethod(List<T> list, string path)
        {
            string tempStr = "";
            string[] tempStrs = path.Split('\\');
            tempStrs[tempStrs.Count() - 1] = string.Empty;
            for (int i = 0; i < tempStrs.Count() - 1; i++)
            {
                tempStrs[tempStrs.Count() - 1] += tempStrs[i] + tempStr;
                tempStr = "\\";
            }
            if (!Directory.Exists(tempStrs.Last()))
            {
                Directory.CreateDirectory(tempStrs.Last());
            }
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static List<T> ReserializeMethod(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                {
                    return null;
                }
                BinaryFormatter bf = new BinaryFormatter();
                List<T> list = (List<T>)bf.Deserialize(fs);
                return list;
            }
        }
    }
}
