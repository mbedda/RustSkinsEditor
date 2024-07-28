using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RustSkinsEditor.Helpers
{
    public class Common
    {
        public static string FormatJSON(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        public static string GetJsonString<T>(T theobject)
        {
            string json = "";
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer
                    (typeof(T), new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true });
                    serializer.WriteObject(ms, theobject);
                    Encoding enc = Encoding.UTF8;
                    json = enc.GetString(ms.ToArray());
                }

                return json;
            }
            catch (Exception e)
            {
                return json;
            }
        }

        public static bool SaveJson<T>(T theobject, string filePath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        //sw.WriteLine("#" + Common.GetVersion());

                        using (MemoryStream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer
                            (typeof(T), new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true});
                            serializer.WriteObject(ms, theobject);
                            Encoding enc = Encoding.UTF8;
                            sw.Write(enc.GetString(ms.ToArray()));
                        }
                    }

                    fs.Flush();
                    fs.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static readonly JsonSerializerOptions _options =
        new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public static bool SaveJsonNewton<T>(T theobject, string filePath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            var options = new JsonSerializerOptions(_options)
                            {
                                WriteIndented = true
                            };
                            var jsonString = JsonConvert.SerializeObject(theobject, Formatting.Indented);

                            sw.Write(jsonString);
                        }
                    }

                    fs.Flush();
                    fs.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static T LoadJson<T>(string filePath)
        {
            T result;
            if (!System.IO.File.Exists(filePath))
            {
                return default(T);
            }

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string filetext = sr.ReadToEnd();

                        if (filetext.StartsWith("#"))
                        {
                            int firstlineindex = filetext.IndexOf(System.Environment.NewLine);
                            filetext = filetext.Substring(firstlineindex + System.Environment.NewLine.Length);
                        }

                        using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(filetext)))
                        {
                            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                            settings.UseSimpleDictionaryFormat = true;
                            var deserializer = new DataContractJsonSerializer(typeof(T), settings);
                            result = (T)deserializer.ReadObject(ms);
                        }
                    }

                    fs.Close();
                }
            }
            catch 
            {
                result = default(T);
            }

            return result;
        }

        public static T LoadJsonResource<T>(string resourceName)
        {
            T result;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string filetext = sr.ReadToEnd();

                    if (filetext.StartsWith("#"))
                    {
                        int firstlineindex = filetext.IndexOf(System.Environment.NewLine);
                        filetext = filetext.Substring(firstlineindex + System.Environment.NewLine.Length);
                    }

                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(filetext)))
                    {
                        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                        var deserializer = new DataContractJsonSerializer(typeof(T), settings);
                        result = (T)deserializer.ReadObject(ms);
                    }
                }

                stream.Close();
            }

            return result;
        }

        public static T DeserializeJSONString<T>(string json)
        {
            var instance = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(instance.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
