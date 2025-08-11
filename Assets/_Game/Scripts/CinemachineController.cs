using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightItUp.Game;
using LightItUp.Singletons;
using Cinemachine;

namespace LightItUp
{
	public enum GameState { 
		WaitingInput, 
		FollowingPlayer, 
		EndLevel
	}

	public class CinemachineController : SingletonCreate<CinemachineController> 
	{


		private bool ISENABLED = false;
		public CinemachineTargetGroup targetGroup;
		private GameLevel gameLevel;
		private float tempTargetTime = 1f;
		private GameState currentGameState;
		private List<CinemachineTargetGroup.Target> validTargets;
		private bool isSeekingNearestBlock;
		private CinemachineTargetGroup.Target playerTarget;
		private CinemachineTargetGroup.Target currentNearestTarget;
		private float distanceTollerance = 1f;
		private float currentNearestObjectDistance = 0f;
		private bool isInit = false;
		private float unzoomedFocusSize = 0.6f;
		private float zoomedFocusSize = 0.4f;
		private float endGameCameraValidDistance = 40f;

		public void Deactivate()
		{
			isInit = false;
		}

		public void InitController(GameLevel gameLevel)
		{
			if (!ISENABLED) {
				return;
			}
			this.gameLevel = gameLevel;
			if (GetComponent<CinemachineVirtualCamera> () == null) {
				isInit = false;
				return;
			}
			GetComponent<CinemachineVirtualCamera> ().Follow = targetGroup.transform;
			validTargets = new List<CinemachineTargetGroup.Target> ();
			currentNearestObjectDistance = 100f;
			isInit = true;
		}



		public void AnnounceGameState (GameState gameState )
		{

			if (!ISENABLED) {
				return;
			}
			if (!isInit){
				return;
			}
			currentGameState = gameState;
			ChangeGroupAccordingToState ();
		}

		public void AnnounceBlockLit (BlockController block)
		{
			if (!ISENABLED) {
				return;
			}
			if (!isInit){
				return;
			}
			AddTemporaryViewTarget (block.transform, tempTargetTime);
		}

		void AddTemporaryViewTarget(Transform target, float time)
		{
			if (!ISENABLED) {
				return;
			}
			CinemachineTargetGroup.Target newTarget = (new CinemachineTargetGroup.Target ());
			newTarget.target = target;
			newTarget.radius = 1;
			newTarget.weight = 1;
			validTargets.Add (newTarget);
			StartCoroutine(WaitFor(tempTargetTime, () => {
				RemoveTemporaryTarget(target); 
			}));
			RefreshTargetGroup ();
		}

		public float GetOrthoSize()
		{
			if (!ISENABLED) {
				return 0;
			}
			return GetComponent<CinemachineVirtualCamera> ().m_Lens.OrthographicSize;
		}
		void RemoveTemporaryTarget (Transform target)
		{
			
			for (int i = validTargets.Count-1; i >=0; i--) {
				if (validTargets[i].target == target) {
					validTargets.RemoveAt (i);
					RefreshTargetGroup ();
					return;
				}
			}
		}
		IEnumerator WaitFor(float secs, System.Action onComplete)
		{
			
			yield return new WaitForSeconds(secs);
			onComplete();
		}

		void ChangeGroupAccordingToState()
		{
			
			switch (currentGameState) {
			case GameState.WaitingInput:
				SetWaitingInputCamera ();
				break;
			case GameState.FollowingPlayer:
				SetPlayerFollowCamera ();
				break;
			case GameState.EndLevel:
				SetEndLevelCamera ();
				break;
			default:
				break;
			}
		}

		void FixedUpdate()
		{
			if (!ISENABLED) {
				return;
			}
			if (!isInit){
				return;
			}
			if (playerTarget.target == null) {
				return;
			}
			if (isSeekingNearestBlock) {
				FindAndAddNearestBlock ();
			}
		}


		void SetWaitingInputCamera ()
		{
			if (!isInit) {
				return;
			}
			AddAllBlocksToTheGroup ();
			AddPlayerToTheGroup (3f);
			GetComponent<CinemachineVirtualCamera> ().GetCinemachineComponent<CinemachineFramingTransposer> ().m_GroupFramingSize = unzoomedFocusSize;
		}

		void SetEndLevelCamera ()
		{
			if (!isInit) {
				return;
			}

			AddCloseBlocksToTheGroup ();
			AddPlayerToTheGroup (2.5f);
			GetComponent<CinemachineVirtualCamera> ().GetCinemachineComponent<CinemachineFramingTransposer> ().m_GroupFramingSize = unzoomedFocusSize;
		}
		void AddPlayerToTheGroup(float playerWeight)
		{
			playerTarget = (new CinemachineTargetGroup.Target ());
			playerTarget.target = gameLevel.player.transform;
			playerTarget.radius = 1;
			playerTarget.weight = playerWeight;
			validTargets.Add(playerTarget);
			RefreshTargetGroup ();

		}


		void SetPlayerFollowCamera ()
		{
			isSeekingNearestBlock = true;
			GetComponent<CinemachineVirtualCamera> ().GetCinemachineComponent<CinemachineFramingTransposer> ().m_GroupFramingSize = zoomedFocusSize;
			ClearAllBlocksFromTargets ();
			currentNearestTarget = new CinemachineTargetGroup.Target ();
			validTargets.Remove (playerTarget);
			AddPlayerToTheGroup (2.5f);
			FindAndAddNearestBlock ();
		}

	
		void ClearAllBlocksFromTargets()
		{
			for (int i = validTargets.Count-1; i >=0; i--) {
				if (validTargets[i].target.gameObject.GetComponent<BlockController>() != null) {
					validTargets.RemoveAt (i);
				}
			}
			RefreshTargetGroup ();
		}

		void FindAndAddNearestBlock()
		{
			float shortestDistance = 100;

			float currentDistance = 0;

			if (currentNearestTarget.target == null || currentNearestTarget.target.gameObject.GetComponent<BlockController> ().IsLit) {
				validTargets.Remove (currentNearestTarget);
				currentNearestObjectDistance = 100f;
			} else {
				currentNearestObjectDistance = Vector3.Distance (playerTarget.target.transform.position, currentNearestTarget.target.position);
			}
			int nearestObjectId = -1;

			for (int i = 0; i < gameLevel.blocks.Count; i++) {
				if (!gameLevel.blocks[i].IsLit) {
					currentDistance = Vector3.Distance (playerTarget.target.transform.position, gameLevel.blocks [i].transform.position);

					if (currentDistance <= shortestDistance) {
						shortestDistance = currentDistance;
						nearestObjectId = i;
					}


				}
			}

			if (nearestObjectId == -1)
				return;
			
			if (shortestDistance >= currentNearestObjectDistance - distanceTollerance) {
				return;
			}

			if (currentNearestTarget.target == null || currentNearestTarget.target != gameLevel.blocks[nearestObjectId].transform)
			 {
				validTargets.Remove (currentNearestTarget);
				CinemachineTargetGroup.Target newTarget = new CinemachineTargetGroup.Target ();
				newTarget.target = gameLevel.blocks[nearestObjectId].transform;
				newTarget.radius = 1;
				newTarget.weight = 1;
				currentNearestTarget = newTarget;
				validTargets.Add (currentNearestTarget);
			}
			RefreshTargetGroup ();

		}


		void RefreshTargetGroup()
		{
			targetGroup.m_Targets = validTargets.ToArray ();
		}

		void AddCloseBlocksToTheGroup()
		{
			for (int i = 0; i < gameLevel.blocks.Count; i++) {
				float playerToBlockDistance = Vector3.Distance (playerTarget.target.position, gameLevel.blocks [i].transform.position);
				if (playerToBlockDistance < endGameCameraValidDistance) {

					CinemachineTargetGroup.Target newTarget = (new CinemachineTargetGroup.Target ());
					newTarget.target = gameLevel.blocks[i].transform;
					newTarget.radius = 1;
					newTarget.weight = 1;
					validTargets.Add (newTarget);
				}
			}
			RefreshTargetGroup ();
		}
		void AddAllBlocksToTheGroup ()
		{
			for (int i = 0; i < gameLevel.blocks.Count; i++) {
				CinemachineTargetGroup.Target newTarget = (new CinemachineTargetGroup.Target ());
				newTarget.target = gameLevel.blocks[i].transform;
				newTarget.radius = 1;
				newTarget.weight = 1;
				validTargets.Add (newTarget);
			}
			RefreshTargetGroup ();
		}
	}


}