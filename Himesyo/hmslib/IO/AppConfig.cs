using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

using Himesyo.Collections;
using Himesyo.Runtime;

namespace Himesyo.IO
{
    /// <summary>
    /// 表示应用程序设置。
    /// </summary>
    [Serializable]
    public class AppConfig : ICloneable
    {
        /// <summary>
        /// 使用默认的 XML 序列化器加载指定文件。如果文件不存在，则返回 <see langword="default"/> 。
        /// </summary>
        /// <typeparam name="T">要反序列化的类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="extraTypes">数组类型集合</param>
        /// <returns></returns>
        public static T Load<T>(string path, params Type[] extraTypes)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            XmlSerializer xml = new XmlSerializer(typeof(T), extraTypes);
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (T)xml.Deserialize(stream);
            }
        }
        /// <summary>
        /// 使用指定的格式化器反序列化指定的文件。如果文件不存在，则返回 <see langword="default"/> 。
        /// </summary>
        /// <typeparam name="T">要反序列化的类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="formatter">格式化器</param>
        /// <returns></returns>
        public static T Load<T>(string path, IFormatter formatter)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (T)formatter.Deserialize(stream);
            }
        }
        /// <summary>
        /// 使用默认的 XML 序列化器保存到指定文件。覆盖已有文件。
        /// </summary>
        /// <typeparam name="T">要反序列化的类型</typeparam>
        /// <param name="obj">要保存的文件</param>
        /// <param name="path">文件路径</param>
        /// <param name="extraTypes">数组类型集合</param>
        /// <returns></returns>
        public static void Save<T>(T obj, string path, params Type[] extraTypes)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T), extraTypes);
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(stream, obj);
                stream.SetLength(stream.Position);
            }
        }
        /// <summary>
        /// 使用指定的格式化器序列化保存到指定文件。覆盖已有文件。
        /// </summary>
        /// <typeparam name="T">要反序列化的类型</typeparam>
        /// <param name="obj">要保存的文件</param>
        /// <param name="path">文件路径</param>
        /// <param name="formatter">格式化器</param>
        /// <returns></returns>
        public static void Save<T>(T obj, string path, IFormatter formatter)
        {
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
                stream.SetLength(stream.Position);
            }
        }
        /// <summary>
        /// 使用默认的 XML 序列化器保存到指定文件。覆盖已有文件。
        /// </summary>
        /// <param name="obj">要保存的文件</param>
        /// <param name="path">文件路径</param>
        /// <param name="extraTypes">数组类型集合</param>
        /// <returns></returns>
        public static void Save(object obj, string path, params Type[] extraTypes)
        {
            XmlSerializer xml = new XmlSerializer(obj.GetType(), extraTypes);
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(stream, obj);
                stream.SetLength(stream.Position);
            }
        }
        /// <summary>
        /// 使用指定的格式化器序列化保存到指定文件。覆盖已有文件。
        /// </summary>
        /// <param name="obj">要保存的文件</param>
        /// <param name="path">文件路径</param>
        /// <param name="formatter">格式化器</param>
        /// <returns></returns>
        public static void Save(object obj, string path, IFormatter formatter)
        {
            using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
                stream.SetLength(stream.Position);
            }
        }

        /// <summary>
        /// 将当前配置保存到指定文件。默认为 XML 序列化。
        /// </summary>
        /// <param name="path"></param>
        public virtual void Save(string path)
        {
            Save((object)this, path);
        }

        /// <summary>
        /// 使用二进制反序列化的方式创建副本。
        /// </summary>
        /// <returns></returns>
        public virtual AppConfig Clone()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return (AppConfig)formatter.Deserialize(stream);
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
