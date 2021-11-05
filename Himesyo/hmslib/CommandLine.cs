using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Himesyo
{
    /// <summary>
    /// 标准命令行信息
    /// </summary>
    public class CommandLines
    {
        /// <summary>
        /// 将单行命令分析为字符串数组。
        /// </summary>
        /// <param name="cmdLine"></param>
        /// <returns></returns>
        public static string[] AnalysisLines(string cmdLine)
        {
            Regex regexChar = new Regex(@"[""\s]");
            List<string> args = new List<string>();
            int indexUp = -1;
            int indexUpQuotation = -1;
            int indexUpSpace = -1;
            bool quotation = false;
            bool waitend = false;
            StringBuilder argValue = new StringBuilder();
            foreach (Match item in regexChar.Matches(cmdLine))
            {
                int index = item.Index;
                if (item.Value == "\"")
                {
                    if (index == 0)
                    {
                        argValue.Append("\"");
                        quotation = true;
                    }
                    else
                    {
                        if (quotation)
                        {
                            if (waitend)
                            {
                                if (index == indexUpQuotation + 1)
                                {
                                    argValue.Append("\"");
                                    waitend = false;
                                }
                                else
                                {
                                    argValue.Append("\"");
                                    argValue.Append(cmdLine.Substring(indexUp + 1, index - indexUp - 1));
                                    waitend = true;
                                }
                            }
                            else
                            {
                                argValue.Append(cmdLine.Substring(indexUp + 1, index - indexUp - 1));
                                waitend = true;
                            }
                        }
                        else
                        {
                            if (indexUpSpace == index - 1)
                            {
                                argValue.Append("\"");
                                quotation = true;
                            }
                            else
                            {
                                argValue.Append(cmdLine.Substring(indexUp + 1, index - indexUp));
                            }
                        }
                    }
                    indexUpQuotation = index;
                }
                else
                {
                    if (waitend)
                    {
                        if (indexUpQuotation == index - 1)
                        {
                            argValue.Remove(0, 1);
                            waitend = false;
                            quotation = false;
                            args.Add(argValue.ToString());
                            argValue.Clear();
                            indexUpSpace = index;
                        }
                        else
                        {
                            argValue.Append("\"");
                            waitend = false;
                            argValue.Append(cmdLine.Substring(indexUp + 1, index - indexUp));
                        }
                    }
                    else if (quotation)
                    {
                        argValue.Append(cmdLine.Substring(indexUp + 1, index - indexUp));
                        //argValue.Append(item.Value);
                    }
                    else
                    {
                        if (index == indexUpSpace + 1)
                        {
                            indexUpSpace = index;
                        }
                        else
                        {
                            argValue.Append(cmdLine.Substring(indexUp + 1, index - indexUp - 1));
                            args.Add(argValue.ToString());
                            argValue.Clear();
                            indexUpSpace = index;
                        }
                    }
                }
                indexUp = index;
            }
            if (waitend && indexUpQuotation == cmdLine.Length - 1)
            {
                argValue.Remove(0, 1);
            }
            argValue.Append(cmdLine.Substring(indexUp + 1));
            if (argValue.Length != 0)
            {
                args.Add(argValue.ToString());
            }
            return args.ToArray();
        }

        private readonly Dictionary<string, string> globalArgValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 是否含有命令行
        /// </summary>
        public bool HasCommandLine
        {
            get
            {
                return globalArgValue.Count > 0 || Values.Count > 0;
            }
        }
        /// <summary>
        /// 全局参数外的其他参数值。
        /// </summary>
        public ReadOnlyCollection<ArgValue> Values { get; }

        /// <summary>
        /// 使用指定命令行和全局参数名称初始化 <see cref="CommandLines"/> 类型的新实例。
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="globalArgs"></param>
        public CommandLines(string arg, params string[] globalArgs) : this(AnalysisLines(arg), globalArgs)
        {

        }
        /// <summary>
        /// 使用指定命令行和全局参数名称初始化 <see cref="CommandLines"/> 类型的新实例。
        /// </summary>
        /// <param name="args"></param>
        /// <param name="globalArgs"></param>
        public CommandLines(string[] args, params string[] globalArgs)
        {
            Regex regexSwitch = new Regex(@"^\s*[-/]\s*(?<name>[^\s=:]+)\s*$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            Regex regexCommand = new Regex(@"^\s*[-/]\s*(?<name>[^\s=:]+)\s*[=:](?<value>.*)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            Regex regexInput = new Regex(@"^(?<value>.*)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            List<ArgValue> values = new List<ArgValue>();
            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                    continue;
                Match match = regexSwitch.Match(arg);
                if (!match.Success)
                {
                    match = regexCommand.Match(arg);
                }
                if (!match.Success)
                {
                    match = regexInput.Match(arg);
                }
                if (match.Success)
                {
                    var nameGroup = match.Groups["name"];
                    var valueGroup = match.Groups["value"];
                    string name = nameGroup.Success ? nameGroup.Value : null;
                    string value = valueGroup.Success ? valueGroup.Value : null;
                    if (globalArgs.Contains(name, StringComparer.OrdinalIgnoreCase))
                    {
                        if (globalArgValue.TryGetValue(name, out string old))
                        {
                            if (value != old)
                            {
                                throw new ArgumentException($"存在同名称的全局开关 {name} 并具有不同的值。");
                            }
                        }
                        else
                        {
                            globalArgValue.Add(name, value);
                        }
                    }
                    else
                    {
                        values.Add(new ArgValue(name, value));
                    }
                }
            }
            Values = new ReadOnlyCollection<ArgValue>(values);
        }

        /// <summary>
        /// 获取指定名称参数的值。输入 <see langword="null"/> 获取所有无名称的参数值。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string[] this[string name]
        {
            get
            {
                if (name != null && globalArgValue.TryGetValue(name, out string value))
                {
                    return new string[] { value };
                }
                else
                {
                    var select = from arg in Values
                                 where arg.Name == name
                                 select arg.Value;
                    return select.ToArray();
                }
            }
        }

        /// <summary>
        /// 是否存在指定全局参数。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasGlobalValue(string name)
        {
            return globalArgValue.ContainsKey(name);
        }

        /// <summary>
        /// 是否存在指定非全局参数。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasValue(string name)
        {
            var select = from arg in Values
                         where arg.Name == name
                         select arg;
            var result = select.FirstOrDefault();
            return result != null;
        }

        /// <summary>
        /// 按用户输入顺序枚举所有非全局参数。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ArgValue> EnumerableValues()
        {
            foreach (var value in Values)
            {
                yield return value;
            }
        }
    }

    /// <summary>
    /// 表示一个参数值。
    /// </summary>
    public class ArgValue
    {
        /// <summary>
        /// 参数名称。可为 <see langword="null"/> 。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 参数值。不可为 <see langword="null"/> 。
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 使用指定名称和值初始化 <see cref="ArgValue"/> 类型的新实例。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ArgValue(string name, string value)
        {
            Name = name;
            Value = value ?? string.Empty;
        }
    }
}
