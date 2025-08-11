using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HyperCasual
{	
	[CreateAssetMenu(fileName = "ConditionsManager", menuName = "[Hyper Casual]/ConditionsManager")]
	public class ConditionsManager : ScriptableObject
	{
		private static ConditionsManager _instance;
        
		public static ConditionsManager Instance
		{
			get
			{				
				if (_instance == null)
				{
					_instance = Resources.Load<ConditionsManager>("Data/ConditionsManager");
				}

				return _instance;
			}            
		}

		[SerializeField]
		public List<ConditionData> conditionsList;
		
		public string conditionsResourcePath = "Data/Conditions";

		public void Init()
		{
			StatisticsService.StatUpdated += StatisticsService_OnStatUpdated;
		}

		private void StatisticsService_OnStatUpdated(string statId)
		{
			//var conditions = conditionsList.FirstOrDefault(x => x.statId == statId);
			
			//conditions
		}

		[ContextMenu("LoadFromResources")]
		public void LoadConditionsFromResources()
		{
			var conditions = Resources.LoadAll<ConditionData>(conditionsResourcePath);

			conditionsList = conditions.ToList();			
		}

		public bool IsComplete(string conditionId)
		{
			var conditionState = conditionsList.FirstOrDefault(x => x.id == conditionId);

			if (conditionState == null)
			{
				throw new Exception("Can't find condition with id: " + conditionId);
			}


			return conditionState.DidComplete;
		}
		
		public List<ConditionData> UpdateConditions()
		{
			var updatedConditions = new List<ConditionData>();

			foreach (var conditionData in conditionsList)
			{
				if (conditionData.DidComplete)
				{
					continue;
				}
					                				
				int statProgress;
					
				bool isComplete = CheckCondition(conditionData, out statProgress);

				if (statProgress > conditionData.MaxProgress)
				{
					updatedConditions.Add(conditionData);
					conditionData.MaxProgress = statProgress;
				}
				
                if (isComplete)
				{
					if (!updatedConditions.Contains(conditionData))
						updatedConditions.Add(conditionData);
					conditionData.MaxProgress = conditionData.value;
					conditionData.DidComplete = true;
				}
			}

			return updatedConditions;
		}
		
		// return the amount for the first condition of the state
		public Vector2 GetConditionProgress(ConditionData condition)
		{			
			Vector2 progress = Vector2.zero;
			
			if (condition.DidComplete)
			{
				progress = new Vector2(condition.value, condition.value);
			}
			else
			{
				//progress = GetConditionProgress(firstCondition);	

				progress = new Vector2(condition.MaxProgress, condition.value);
			}
			
			return progress;
		}

		public Vector2 CalculateConditionProgress(ConditionData condition)
		{
			Vector2 progress = Vector2.zero;

			// there is no progress for true/false conditions
			if (condition.op == ConditionData.Operator.EQ || condition.op == ConditionData.Operator.NE)
			{
				return progress;
			}
			
			progress.y = condition.value;
			
			var stat = StatisticsService.GetStat(condition.statId);

			if (stat == null)
				return progress;

			int valueToCheck = 0;
			
			switch (condition.type)
			{
				case ConditionData.Type.InRun:
					valueToCheck = stat.valueInRun;
					break;
				case ConditionData.Type.InRow:
					valueToCheck = stat.valueInRow;
					break;
				case ConditionData.Type.Total:
					valueToCheck = stat.valueTotal;
					break;						
			}

			progress.x = valueToCheck;

			return progress;
		}
	
		
		bool CheckCondition(ConditionData condition, out int progress)
		{
			var stat = StatisticsService.GetStat(condition.statId);

			progress = 0;
			
			if (stat == null)
				return false;
			
			int valueToCheck = 0;

			switch (condition.type)
			{
				case ConditionData.Type.InRun:
					valueToCheck = stat.valueInRun;
					break;
				case ConditionData.Type.InRow:
					valueToCheck = stat.valueInRow;
					break;
				case ConditionData.Type.Total:
					valueToCheck = stat.valueTotal;
					break;			
			}

			progress = valueToCheck;
			
			return condition.IsComplete (valueToCheck);									
		}
	}
}