using System;
using RSG;
using UnityEngine;

namespace HyperCasual.Revive
{
    public static class ReviveService
    {
        public static string revivePanelLocation = "Game/Revive-Root";
        
        public static int revivesPerRun { get; private set; }

        private static Promise<bool> revivePromise;

        public static void StartRun()
        {
            revivesPerRun = 0;
        }
        
        public static IPromise<bool> Perform(Func<bool> ShouldOfferRevive, int countdownTime, int timeToShowSkip)
        {   
            revivePromise = new Promise<bool>();
            
            
            if (!ShouldOfferRevive())
            {                
                revivePromise.Resolve(false);
                return revivePromise;
            }

            var prefab = Resources.Load<GameObject>(revivePanelLocation);

            if (prefab == null)
            {
                Debug.LogError("Can't load reviveHud prefab at: " + revivePanelLocation);                
                revivePromise.Resolve(false);
                return revivePromise;
            }

            var go = GameObject.Instantiate(prefab);
            
            ReviveHud.ReviveDoneEvent += OnReviveDoneEvent;
            
            go.GetComponent<ReviveHud>().StartCount(countdownTime, timeToShowSkip);

            return revivePromise;
        }

        

        private static void OnReviveDoneEvent(bool shouldRevive)
        {
            Debug.Log("ReviveService: On Revive Done " + shouldRevive);
            ReviveHud.ReviveDoneEvent -= OnReviveDoneEvent;

            if (revivePromise == null)
            {
                return;
            }
            
            if (shouldRevive)
            {
                revivesPerRun++; 
                revivePromise.Resolve(true);
            }
            else
            {
                revivePromise.Resolve(false);
            }
        }
    }
}