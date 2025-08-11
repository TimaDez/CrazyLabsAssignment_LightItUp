using System.Collections;
using UnityEngine;
using LightItUp.Singletons;

namespace LightItUp
{
    public class ActionRunner : SingletonCreate<ActionRunner>
    {
        public static void WaitAndRun(float delay, System.Action action) {
            Instance.StartCoroutine(Instance.WaitFor(delay, action));
        }

        public static Coroutine Run(IEnumerator routine)
        {
            return Instance.RunCoroutine(routine);
        }
        public static void Run(System.Func<bool> routine)
        {
            Instance.RunCoroutine(Instance.WaitFor(routine));
        }
        public static void StopSpecificCoroutine(Coroutine routineToStop) {
            Instance.StopCoroutine(routineToStop);
        }

        Coroutine RunCoroutine(IEnumerator routine) {
            return StartCoroutine(routine);
        }
        IEnumerator WaitFor(float delay, System.Action action) {
            yield return new WaitForSeconds(delay);
            action();
        }
        IEnumerator WaitFor(System.Func<bool> routine)
        {
            bool done = false;
            while (!done)
            {
                done = routine();
                yield return null;

            }
        }

    }
}
