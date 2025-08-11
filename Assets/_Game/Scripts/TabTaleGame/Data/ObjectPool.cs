using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Game;
using LightItUp.Singletons;

namespace LightItUp.Data
{
    public class ObjectPool : SingletonLoad<ObjectPool>
    {
        public LitArea litAreaPrefab;
        public ObjectPool<LitArea> litAreas;

        public LightUpFX lightUpFXPrefab;
        public ObjectPool<LightUpFX> lightUpFXs;

        public CelebrationFX celebrationFXPrefab;
        public ObjectPool<CelebrationFX> celebrationFXs;

        public TutorialText tutorialTextPrefab;
        public ObjectPool<TutorialText> tutorialTexts;

        public ParticleFXBasic starPickupFXPrefab;
        public ObjectPool<ParticleFXBasic> starPickupFXs;

        public void Init() { }
        public override void Awake()
        {
            base.Awake();
            litAreas = GetPool(litAreaPrefab, 10, transform);
            lightUpFXs = GetPool(lightUpFXPrefab, 10, transform);
            celebrationFXs = GetPool(celebrationFXPrefab, 2, transform);
            tutorialTexts = GetPool(tutorialTextPrefab, 4, transform);
            starPickupFXs = GetPool(starPickupFXPrefab, 3, transform);
        }
        ObjectPool<T> GetPool<T>(T prefab, int startCount, Transform t) where T : PooledObject
        {
            return new ObjectPool<T>(prefab, startCount, t);
        }


        public static LitArea GetLitArea()
        {
            return Instance.litAreas.GetObject();
        }
        public static void ReturnLitArea(LitArea b)
        {
            Instance.litAreas.ReturnObject(b);
        }

        public static LightUpFX GetLightUpFX()
        {
            return Instance.lightUpFXs.GetObject();
        }
        public static void ReturnLightUpFX(LightUpFX lightUpFX)
        {
            Instance.lightUpFXs.ReturnObject(lightUpFX);
        }

        public static CelebrationFX GetCelebrationFX()
        {
            return Instance.celebrationFXs.GetObject();
        }
        public static void ReturnCelebrationFX(CelebrationFX celebFX)
        {
            Instance.celebrationFXs.ReturnObject(celebFX);
        }

        public static TutorialText GetTutorialText()
        {
            return Instance.tutorialTexts.GetObject();
        }
        public static void ReturnTutorialText(TutorialText tutorialText)
        {
            Instance.tutorialTexts.ReturnObject(tutorialText);
        }

        public static ParticleFXBasic GetStarFX()
        {
            var a = Instance.starPickupFXs.GetObject();
            a.Init(ReturnStarFX);
            return a;
        }
        public static void ReturnStarFX(ParticleFXBasic starFX)
        {
            Instance.starPickupFXs.ReturnObject(starFX);
        }
    }

    public class ObjectPool<T> where T : PooledObject {
        T prefab;
        Transform pivot;
        List<T> unusedObjects;
        List<T> usedObjects;

        public ObjectPool(T prefab, int startCount, Transform pivot) {
            unusedObjects = new List<T>();
            usedObjects = new List<T>();
            this.prefab = prefab;
            this.pivot = pivot;
            for (int i = 0; i < startCount; i++)
            {
                unusedObjects.Add(CreateObj());
            }
        }
        T CreateObj() {
            var p = GameObject.Instantiate(prefab);
            p.transform.SetParent(pivot);
            p.OnReturnedPoolObj();
            return p;
        }
        public T GetObject()
        {
            T o = null;
            if (unusedObjects.Count <= 0)
            {
                o = CreateObj();
            }
            else
            {
                o = unusedObjects[0];
                unusedObjects.RemoveAt(0);
            } 
            usedObjects.Add(o);
            o.transform.SetParent(null);
            o.OnInitPoolObj();
            return o;
        }
        public void ReturnObject(T obj)
        {
            if (!usedObjects.Contains(obj)) {
                if (unusedObjects.Contains(obj))
                {
                    Debug.LogError("Returned object already in unused?", obj);
                }
                else
                {
                    Debug.LogError("Returned object does not belong to this pool?", obj);
                    usedObjects.Add(obj);
                }            
                Debug.Break();
                //return;
            
            }
            obj.transform.SetParent(pivot);
            obj.OnReturnedPoolObj();
            usedObjects.Remove(obj);
            unusedObjects.Add(obj);
        }

    }
}