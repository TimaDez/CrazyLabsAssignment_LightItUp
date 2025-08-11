using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
using LightItUp.Singletons;

namespace LightItUp.Data
{
    public abstract class GameSettingsBase<T> : SingletonAsset<T> where T : GameSettingsBase<T>
    {
        public string filePath = "Assets/_Game/Resources/GameSettings.txt";

        protected abstract string SaveJson();
        protected abstract void LoadJson(string json);
#if UNITY_EDITOR
        public override void Init()
        {
            Load();
        }
#endif
        public void Save()
        {

            var s = SaveJson();
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Write(s);
            writer.Close();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public void Load()
        {
            StreamReader reader = new StreamReader(filePath);
            var s = reader.ReadToEnd();
            reader.Close();
            LoadJson(s);
        }
    }
}
