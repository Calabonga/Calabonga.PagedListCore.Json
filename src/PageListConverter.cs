using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Calabonga.PagedListCore.Json
{
    /// <summary>
    /// Custom PagedList converter for serialization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageListConverter<T> : JsonConverter<IPagedList<T>>
    {
        public override IPagedList<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new PagedList<T>(new List<T>(), 0, 10, 0, 20);
            }

            var pageIndex = 0;
            var pageSize = 20;
            var totalCount = 0;
            var indexFrom = 0;

            var list = new List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();

                    reader.Read();

                    switch (propertyName)
                    {
                        case "pageIndex":
                            pageIndex = reader.GetInt32();
                            break;

                        case "pageSize":
                            pageSize = reader.GetInt32();
                            break;

                        case "totalCount":
                            totalCount = reader.GetInt32();
                            break;

                        case "indexFrom":
                            indexFrom = reader.GetInt32();
                            break;

                        case "items":

                            if (reader.TokenType != JsonTokenType.StartArray)
                            {
                                throw new JsonException();
                            }

                            if (reader.TokenType == JsonTokenType.StartArray)
                            {
                                var converter = (JsonConverter<T>)options.GetConverter(typeof(T));
                                if (converter.CanConvert(typeof(T)))
                                {
                                    reader.Read();
                                    while (reader.TokenType != JsonTokenType.EndArray)
                                    {
                                        var value = converter.Read(ref reader, typeof(T), options)!;
                                        list.Add(value);
                                        reader.Read();
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            var result = new PagedList<T>(list, pageIndex, pageSize, indexFrom, totalCount);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, IPagedList<T> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
