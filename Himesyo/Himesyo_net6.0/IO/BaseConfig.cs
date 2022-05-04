using System.Text.Json;
using System.Text.Json.Serialization;

namespace Himesyo.IO
{
    /// <summary>
    /// 基本配置。
    /// </summary>
    public abstract class BaseConfig
    {
        /// <summary>
        /// 加载与保存时使用到默认配置。
        /// </summary>
        public static JsonSerializerOptions? DefaultSerializerOptions { get; set; }

        static BaseConfig()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                IgnoreReadOnlyFields = true,
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new JsonDateTimeConverter());
            DefaultSerializerOptions = options;
        }

        /// <summary>
        /// 从文件中加载指定类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T? Load<T>(string path, JsonSerializerOptions? options = default)
        {
            if (!File.Exists(path))
                return default;

            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return JsonSerializer.Deserialize<T>(path, options ?? DefaultSerializerOptions);
        }

        /// <summary>
        /// 将对象保存到指定位置。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Save<T>(string path, T obj, JsonSerializerOptions? options = default)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            JsonSerializer.Serialize(obj, options ?? DefaultSerializerOptions);
            stream.SetLength(stream.Position);
        }

        /// <summary>
        /// 将对象保存到指定位置。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Save(string path, object obj, JsonSerializerOptions? options = default)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            JsonSerializer.Serialize(stream, obj, obj.GetType(), options ?? DefaultSerializerOptions);
            stream.SetLength(stream.Position);
        }

        /// <summary>
        /// 将当前对象保存到指定位置。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        public void Save(string path, JsonSerializerOptions? options = default)
        {
            Save(path, (object)this, options);
        }
    }

    /// <summary>
    /// 将 <see cref="DateTime"/> 类型与字符串之间进行转换。
    /// </summary>
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        /// <inheritdoc/>
        public override bool HandleNull => false;
        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTime);
        }

        /// <inheritdoc/>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDateTime();
        }
        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
