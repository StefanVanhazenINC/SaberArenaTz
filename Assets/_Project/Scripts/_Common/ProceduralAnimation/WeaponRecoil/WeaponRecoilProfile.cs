using UnityEngine;

namespace Common.ProcedureAnimation
{
    [CreateAssetMenu(menuName = "Weapons/Recoil Profile", fileName = "RecoilProfile")]
    public class WeaponRecoilProfile : ScriptableObject
    {
        [Header("Kick amplitudes (local space)")]
        public Vector3 positionKick = new Vector3(0.02f, 0.02f, -0.08f);

        public Vector3 rotationKickEuler = new Vector3(-3f, 1.2f, 1.2f);

        [Header("Timing (seconds)")] [Min(0.001f)]
        public float kickTime = 0.06f;

        [Min(0.001f)] public float returnTime = 0.10f;

        [Header("Shape (0..1 -> 0..1, but you CAN overshoot / go negative)")]
        public AxisCurves kickCurve = AxisCurves.DefaultKick();

        public AxisCurves returnCurve = AxisCurves.DefaultReturn();

        [Header("Randomization (per shot)")]
        public Vector3 positionRandom = new Vector3(0.01f, 0.01f, 0.00f); // +/- range on each axis

        public Vector3 rotationRandom = new Vector3(0.00f, 0.8f, 0.8f); // +/- range on each axis

        [Header("Clamp (final summed offset)")]
        public Vector3 positionClamp = new Vector3(0.08f, 0.08f, 0.20f);

        public Vector3 rotationClampEuler = new Vector3(20f, 20f, 20f);

        [Header("Optional smoothing (spring-follow)")]
        public bool useFollowSmoothing = false;

        [Min(0.0f)] public float followSmoothTime = 0.03f; // 0 = no smoothing

        [Header("Gizmos")] [Range(4, 64)] public int gizmoSamples = 24;
    }

    [System.Serializable]
    public struct AxisCurves
    {
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;

        public Vector3 Evaluate(float t01)
        {
            t01 = Mathf.Clamp01(t01);
            return new Vector3(
                x != null ? x.Evaluate(t01) : t01,
                y != null ? y.Evaluate(t01) : t01,
                z != null ? z.Evaluate(t01) : t01
            );
        }

        public static AxisCurves DefaultKick()
        {
            // плавный рост 0->1
            var c = AnimationCurve.EaseInOut(0, 0, 1, 1);
            return new AxisCurves { x = c, y = c, z = c };
        }

        public static AxisCurves DefaultReturn()
        {
            // плавный возврат 0->1 (мы потом превращаем это в 1->0)
            var c = AnimationCurve.EaseInOut(0, 0, 1, 1);
            return new AxisCurves { x = c, y = c, z = c };
        }
    }
}