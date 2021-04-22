using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using Himesyo.Collections;
using Himesyo.Runtime;

namespace Himesyo.IO
{
    /// <summary>
    /// 表示应用程序设置。
    /// </summary>
    public class AppConfig
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
    }

    /// <summary>
    /// 提供对窗体位置、大小管理并可保存到配置
    /// </summary>
    public class WinFormManager
    {
        /// <summary>
        /// 窗体信息
        /// </summary>
        public DictionaryCollection<FormInfo> FormInfos { get; set; }

        private DictionaryCollection<FormInfo> GetOrCreateInfos()
        {
            if (FormInfos == null)
            {
                FormInfos = new DictionaryCollection<FormInfo>();
            }
            return FormInfos;
        }

        /// <summary>
        /// 将指定窗体添加到管理。注意：必须有名称且添加后名称不可更改。
        /// </summary>
        /// <param name="form"></param>
        public void AddManager(Form form)
        {
            form.LocationChanged -= Form_PositionChanged;
            form.LocationChanged += Form_PositionChanged;
            form.SizeChanged -= Form_PositionChanged;
            form.SizeChanged += Form_PositionChanged;
            Form_PositionChanged(form, EventArgs.Empty);
        }
        /// <summary>
        /// 设置启动信息。
        /// </summary>
        /// <param name="form"></param>
        /// <param name="defaultPosition"></param>
        /// <param name="defaultSize"></param>
        /// <returns></returns>
        public bool SetStartInfo(Form form, StartPosition defaultPosition, StartSize defaultSize)
        {
            if (FormInfos == null)
            {
                SetSize(form, defaultSize);
                SetPosition(form, defaultPosition);
                return false;
            }
            else
            {
                var info = FormInfos[form.Name];
                if (info == null)
                {
                    SetSize(form, defaultSize);
                    SetPosition(form, defaultPosition);
                    return false;
                }
                else
                {
                    // 必须先处理大小
                    if (info.StartSize == StartSize.UpClose)
                    {
                        form.Size = info.Size;
                        form.WindowState = info.WindowState;
                    }
                    else
                    {
                        SetSize(form, info.StartSize);
                    }
                    if (info.StartPosition == StartPosition.UpClose)
                    {
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = info.Location;
                    }
                    else
                    {
                        SetPosition(form, info.StartPosition);
                    }
                    return true;
                }
            }
        }
        /// <summary>
        /// 是否存在指定窗体名称的信息。
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        public bool ContainsName(string formName)
        {
            return FormInfos?.ContainsKey(formName) ?? false;
        }

        private void Form_PositionChanged(object sender, EventArgs e)
        {
            if (sender is Form form)
            {
                var infos = GetOrCreateInfos();
                var info = infos[form.Name];
                if (info == null)
                {
                    info = new FormInfo()
                    {
                        WindowState = form.WindowState,
                        Location = form.Location,
                        Size = form.Size
                    };
                    infos[form.Name] = info;
                }
                else
                {
                    if (info.StartPosition == StartPosition.UpClose || info.StartSize == StartSize.UpClose)
                    {
                        info.WindowState = form.WindowState;
                        if (info.WindowState == FormWindowState.Normal)
                        {
                            info.Location = form.Location;
                            info.Size = form.Size;
                        }
                    }
                }
            }
        }

        private void SetPosition(Form form, StartPosition position)
        {
            if (position == StartPosition.CenterScreen)
            {
                form.StartPosition = FormStartPosition.CenterScreen;
            }
            else if (position == StartPosition.FullScreen)
            {
                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                var rect = Screen.PrimaryScreen.Bounds;
                switch (position)
                {
                    case StartPosition.LeftUp:
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = rect.Location;
                        break;
                    case StartPosition.LeftDown:
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = rect.Location + new Size(0, rect.Height - form.Height);
                        break;
                    case StartPosition.RightUp:
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = rect.Location + new Size(rect.Width - form.Width, 0);
                        break;
                    case StartPosition.RightDown:
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = rect.Location + new Size(rect.Width - form.Width, 0);
                        break;
                }
            }
        }

        private void SetSize(Form form, StartSize size)
        {
            var rect = Screen.PrimaryScreen.Bounds;
            switch (size)
            {
                case StartSize.QuarterScreen:
                    form.Size = new Size(rect.Width / 2, rect.Height / 2);
                    break;
                case StartSize.HalfScreenWidth:
                    form.Size = new Size(rect.Width / 2, rect.Height);
                    break;
                case StartSize.HalfScreenHeight:
                    form.Size = new Size(rect.Width, rect.Height / 2);
                    break;
                case StartSize.FullScreen:
                    form.Size = rect.Size;
                    break;
            }
        }
    }

    /// <summary>
    /// 窗体信息
    /// </summary>
    public class FormInfo
    {
        /// <summary>
        /// 启动位置
        /// </summary>
        [XmlAttribute]
        public StartPosition StartPosition { get; set; }
        /// <summary>
        /// 启动位置
        /// </summary>
        [XmlAttribute]
        public StartSize StartSize { get; set; }
        /// <summary>
        /// 启动状态
        /// </summary>
        public FormWindowState WindowState { get; set; }
        /// <summary>
        /// 记录的坐标
        /// </summary>
        public Point Location { get; set; }
        /// <summary>
        /// 记录的大小
        /// </summary>
        public Size Size { get; set; }
    }

    /// <summary>
    /// 启动位置
    /// </summary>
    public enum StartPosition
    {
        /// <summary>
        /// 上次关闭时的位置
        /// </summary>
        [Description("上次关闭时的位置")]
        UpClose,
        /// <summary>
        /// 默认位置
        /// </summary>
        [Description("默认位置")]
        Default,
        /// <summary>
        /// 左上
        /// </summary>
        [Description("左上")]
        LeftUp,
        /// <summary>
        /// 左下
        /// </summary>
        [Description("左下")]
        LeftDown,
        /// <summary>
        /// 右上
        /// </summary>
        [Description("右上")]
        RightUp,
        /// <summary>
        /// 右下
        /// </summary>
        [Description("右下")]
        RightDown,
        /// <summary>
        /// 屏幕中心
        /// </summary>
        [Description("屏幕中心")]
        CenterScreen,
        /// <summary>
        /// 全屏幕
        /// </summary>
        [Description("全屏幕")]
        FullScreen
    }


    /// <summary>
    /// 启动大小
    /// </summary>
    public enum StartSize
    {
        /// <summary>
        /// 上次关闭时的大小
        /// </summary>
        [Description("上次关闭时的大小")]
        UpClose,
        /// <summary>
        /// 默认大小
        /// </summary>
        [Description("默认大小")]
        Default,
        /// <summary>
        /// 四分之一屏幕
        /// </summary>
        [Description("四分之一屏幕")]
        QuarterScreen,
        /// <summary>
        /// 半屏幕宽
        /// </summary>
        [Description("半屏幕宽")]
        HalfScreenWidth,
        /// <summary>
        /// 半屏幕高
        /// </summary>
        [Description("半屏幕高")]
        HalfScreenHeight,
        /// <summary>
        /// 全屏幕
        /// </summary>
        [Description("全屏幕")]
        FullScreen
    }
}
