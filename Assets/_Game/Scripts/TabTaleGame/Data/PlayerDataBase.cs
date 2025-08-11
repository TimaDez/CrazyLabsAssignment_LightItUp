using System;
using System.Collections.Generic;
using UnityEngine;

namespace LightItUp.Data
{
    [System.Serializable]
    public abstract class PlayerDataBase<T, U> where T : struct, IConvertible where U : PlayerDataBase<T, U> {
        public event System.Action OnValueChanged;

        public Dictionary<T, int> values;
        bool saveQueued = false;

        public PlayerDataBase() { }

        #region Save and Load
        public static string filePath = Application.persistentDataPath + "/userData.dat";
        public U Load()
        {
            U p = GetDefault();
            p = FileManager.LoadFile<U>(filePath, true);
            if (p == null)
                p = GetDefault();
            return p;
        }
        protected abstract U GetDefault();

        protected void RunOnValueChanged()
        {
            if (OnValueChanged != null)
                OnValueChanged();
        }

        public void SaveData()
        {
            //Debug.LogError("save data");
            FileManager.SaveFile(filePath, this, true);
        }

        public void SaveQueued()
        {
            if (saveQueued)
            {
                SaveData();
                saveQueued = false;
            }
        }
        protected void QueueSaveData()
        {
            saveQueued = true;
        }
        #endregion


        #region ModifyValues
        public void ValueWasUpdated() {
            if (OnValueChanged != null)
            {
                OnValueChanged();
            } 
        }
        protected int GetValue(T valueType, int defaultValue = 0)
        {
            if (values == null)
                values = new Dictionary<T, int>();
            if (!values.ContainsKey(valueType))
            {
                values.Add(valueType, defaultValue);
            }
            return values[valueType];
        }

        protected void SetValue(T valueType, int value)
        {
            if (values == null)
            {
                values = new Dictionary<T, int>();
            }
        
            if (!values.ContainsKey(valueType))
            {
                values.Add(valueType, value);
            }
            else
                values[valueType] = value;
            QueueSaveData();
            if (OnValueChanged != null)
                OnValueChanged();
        }
        #endregion
    }
}
