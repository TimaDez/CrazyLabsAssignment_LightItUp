using System;
using System.Collections;
using System.Collections.Generic;
using LightItUp.Data;
using UnityEngine;
using UnityEngine.UI;

namespace LightItUp
{
    [CreateAssetMenu(fileName = "StarGateInfo", menuName = "[Tabtale]/StarGateInfo", order = 1)]
    public class StarGateInfo : ScriptableObject
    {
        public int starsRequired;
        public int levelLock;


    }
}
