using UnityEngine;

namespace Lighthouse.Match3.Configs
{
    [CreateAssetMenu(fileName = "TransitionConfig", menuName = "Lighthouse/TransitionConfig", order = 10)]
    public class TransitionConfig : ScriptableObject
    {
        [Header("Movement")] public float SwapDuration = 0.2f; // seconds
        public float FallDurationPerCell = 0.08f; // seconds per cell moved

        [Header("Fade")] public float FadeOutDuration = 0.15f; // seconds

        [Header("Shake")] public float ShakeDuration = 0.15f; // seconds
        public float ShakeMagnitude = 0.08f; // local units
    }
}

 

