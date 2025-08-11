using System;
using System.Collections.Generic;
using LightItUp;
using UnityEngine;

namespace HyperCasual
{
    [CreateAssetMenu(fileName = "Condition", menuName = "[Hyper Casual]/Condition Data")]
    public class ConditionData : ScriptableObject
    {
        private static string completeKeyPrefix = "completed_condition_";
        private static string progressKeyprefix = "progress_condition_";
        
        public bool DidComplete
        {
            get
            {
                return PlayerPrefs.GetInt(completeKeyPrefix + id, 0) > 0;
            }
            set
            {
                PlayerPrefs.SetInt(completeKeyPrefix + id, value ? 1 : 0);
            }
        }	
		
        public int MaxProgress
        {
            get
            {
                return PlayerPrefs.GetInt(progressKeyprefix + id, 0);
            }
            set
            {
                PlayerPrefs.SetInt(progressKeyprefix + id, value);
            }
        }
        
        public enum Operator { EQ, NE, LT, LTE, GT, GTE }
        public enum Type { InRun, InRow, Total }

        public string id;

        void OnValidate()
        {
            if (autoGenrateId)
                id = statId + "_" + value;
        }
                
        [Header("Properties")]        
        [StringInList(typeof(GameStats), "GetTypes")]        
        public string statId;
        public Operator op;
        public Type type;
        public int value;

        [Header("Metadata")]
        [TextArea]
        public string description;

        public bool autoGenrateId;
        
        public bool IsComplete(int checkValue)
        {
            //Debug.Log("CheckCond id="+id+" is " + checkValue + " " + condition + " " + value);
            switch (op)
            {
                case Operator.EQ:
                    return checkValue == value;
                case Operator.NE:
                    return checkValue != value;
                case Operator.GT:
                    return checkValue > value;
                case Operator.GTE:
                    return checkValue >= value;
                case Operator.LT:
                    return checkValue < value;
                case Operator.LTE:
                    return checkValue <= value;
            }
            return false;
        }
    }    
}