using System;
using UnityEngine;
using UnityEngine.UI;
using LightItUp.Data;

namespace LightItUp.Data
{
    [RequireComponent(typeof(Image))]
    public abstract class ImageDataDisplayerBase<T> : MonoBehaviour where T : struct, IConvertible
    {
        public T data;
        protected Image _image;
        protected Image image
        {
            get
            {
                if (_image == null)
                    _image = GetComponent<Image>();
                return _image;
            }
        }
        public virtual void OnEnable()
        {
            GameData.PlayerData.OnValueChanged += OnDataChanged;
            OnDataChanged();
        }
        public virtual void OnDisable()
        {
            GameData.PlayerData.OnValueChanged -= OnDataChanged;
        }
        protected virtual void OnDataChanged()
        {
            image.sprite = GetSprite();
            SetupImageSettings();
        }
        protected abstract Sprite GetSprite();
        protected abstract void SetupImageSettings();

    }
}
