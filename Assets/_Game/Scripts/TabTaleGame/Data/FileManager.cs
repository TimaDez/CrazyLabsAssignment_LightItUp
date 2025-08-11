using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightItUp.Data
{
    public static class FileManager
    {
        public static T LoadFile<T>(string path, bool decrypt = false)
        {
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                var s = reader.ReadToEnd();
                reader.Close();
                if (decrypt)
                    s = DecryptString(s);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s);
            }
            return default(T);
        }
        public static void SaveFile(string path, object obj, bool encrypt = false)
        {
            var s = obj.GetType() != typeof(string) ? Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented) : (string)obj;
            if (encrypt)
                s = EncryptString(s);
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(s);
            writer.Close();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

        }
#if UNITY_EDITOR
        public static void DeleteFile(string path) {
            if (File.Exists(path))
            {
                File.Delete(path);
                AssetDatabase.Refresh();
            }
        }
#endif

        public static void SaveAsEnumList(string enumName, string filePath, List<string> data, string msg)
        {
            List<string> added = new List<string>();
            string s = "// "+ msg + "\n";
            s += "public enum "+ enumName + " {\n";
            foreach (var v in data)
            {
                if (!ValidateName(v, added.Contains(v)))
                {
                    continue;
                }
                added.Add(v);
                s += "  " + v + ",\n";
            }
            s += "}\n";
            SaveFile(filePath, s);
        }
        public static void SaveAsStringConstsInClass(string className, string filePath, List<string> data, string msg, bool addAllIdsList = true)
        {
            List<string> added = new List<string>();
            string s = "// "+ msg + "\n";
            if (addAllIdsList)
            {
                s += "using System.Collections.Generic;\n\n";
            }        
            s += "public static class "+ className + " {\n";
            foreach (var v in data)
            {
                if (!ValidateName(v, added.Contains(v)))
                {
                    continue;
                }
                added.Add(v);
                s += string.Format("    public const string {0} = \"{0}\";\n", v);
            }
            s += "\n";
            if (addAllIdsList)
            {
                added.Clear();
                s += "      public static List<string> allIds = new List<string>() {\n";
                s += "            \"\",\n";
                foreach (var v in data)
                {
                    if (!ValidateName(v, added.Contains(v)))
                    {
                        continue;
                    }
                    added.Add(v);
                    s += string.Format("            \"{0}\",\n", v);
                }
                s += "  };\n";
            }
            s += "}\n";

            SaveFile(filePath, s);
        }

        public static bool ValidateName(string n, bool inAdded)
        {
            if (string.IsNullOrEmpty(n))
            {
                Debug.LogError("Empty clipName");
                return false;
            }
            if (inAdded)
            {
                Debug.LogError("clipName exists more than once? " + n);
                return false;
            }
            if (!IsAsciiLetter(n[0]))
            {
                Debug.LogError("clipName start character is invalid? " + n);
                return false;
            }
            return true;
        }

        public static bool IsAsciiLetter(char c)
        {

            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

        }

        public static string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                Debug.LogError("Could not decrypt: "+fe.Message);
                decrypted = "";
            }
            return decrypted;
        }

        public static string EncryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
    }
}
