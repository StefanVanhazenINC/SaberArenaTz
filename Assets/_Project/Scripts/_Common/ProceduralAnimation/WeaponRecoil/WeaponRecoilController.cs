using System.Collections.Generic;
using UnityEngine;

namespace Common.ProcedureAnimation
{
    public class WeaponRecoilController : MonoBehaviour
    {
        [Header("Reference points (local)")] [SerializeField]
        private Transform recoilPosition; // двигаем localPosition

        [SerializeField] private Transform rotationPoint; // двигаем localRotation

        [Header("Profile")] [SerializeField] private WeaponRecoilProfile profile;

        [Header("Multipliers (like your heightPositionRecoil/heightRotationRecoil)")] [Range(0, 1)]
        public float positionAmount = 1f;

        [Range(0, 1)] public float rotationAmount = 1f;

        private Vector3 baseLocalPos;
        private Quaternion baseLocalRot;

        private readonly List<RecoilEvent> events = new();
        private float now;

        // optional follow smoothing
        private Vector3 followPosVel;
        private Vector3 followRotVel; // euler smoothing velocity
        private Vector3 currentPosOffset;
        private Vector3 currentRotEuler;

        private void Awake()
        {
            CacheBase();
        }

        private void OnValidate()
        {
            if (recoilPosition && rotationPoint) CacheBase();
        }

        private void CacheBase()
        {
            baseLocalPos = recoilPosition ? recoilPosition.localPosition : Vector3.zero;
            baseLocalRot = rotationPoint ? rotationPoint.localRotation : Quaternion.identity;

            currentPosOffset = Vector3.zero;
            currentRotEuler = Vector3.zero;
            followPosVel = Vector3.zero;
            followRotVel = Vector3.zero;
        }

        /// <summary>
        /// Вызывай на каждый выстрел.
        /// Можно передать кастомный endOffset/времена, но обычно достаточно профиля.
        /// </summary>
        public void AddRecoil()
        {
            if (!profile) return;

            var posKick = profile.positionKick;
            var rotKick = profile.rotationKickEuler;

            // рандомизация (как у тебя было y/z)
            posKick += new Vector3(
                Random.Range(-profile.positionRandom.x, profile.positionRandom.x),
                Random.Range(-profile.positionRandom.y, profile.positionRandom.y),
                Random.Range(-profile.positionRandom.z, profile.positionRandom.z)
            );

            rotKick += new Vector3(
                Random.Range(-profile.rotationRandom.x, profile.rotationRandom.x),
                Random.Range(-profile.rotationRandom.y, profile.rotationRandom.y),
                Random.Range(-profile.rotationRandom.z, profile.rotationRandom.z)
            );

            // множители высоты
            posKick *= positionAmount;
            rotKick *= rotationAmount;

            events.Add(new RecoilEvent
            {
                startTime = Time.time,
                posKick = posKick,
                rotKickEuler = rotKick,
                kickTime = profile.kickTime,
                returnTime = profile.returnTime
            });
        }

        private void LateUpdate()
        {
            if (!recoilPosition || !rotationPoint || !profile) return;

            now = Time.time;

            // 1) сумма вкладов всех активных recoil events
            Vector3 desiredPos = Vector3.zero;
            Vector3 desiredRotEuler = Vector3.zero;

            for (int i = events.Count - 1; i >= 0; i--)
            {
                var e = events[i];
                float age = now - e.startTime;
                float total = e.kickTime + e.returnTime;

                if (age >= total)
                {
                    events.RemoveAt(i);
                    continue;
                }

                // weight per-axis via curves
                var w = EvaluateWeightPerAxis(age, e.kickTime, e.returnTime, profile.kickCurve, profile.returnCurve);

                desiredPos += Vector3.Scale(e.posKick, w);
                desiredRotEuler += Vector3.Scale(e.rotKickEuler, w);
            }

            // 2) clamp итоговой суммы
            desiredPos = ClampVector(desiredPos, profile.positionClamp);
            desiredRotEuler = ClampVector(desiredRotEuler, profile.rotationClampEuler);

            // 3) применяем (опционально через follow smoothing)
            if (!profile.useFollowSmoothing || profile.followSmoothTime <= 0f)
            {
                recoilPosition.localPosition = baseLocalPos + desiredPos;
                rotationPoint.localRotation = baseLocalRot * Quaternion.Euler(desiredRotEuler);

                currentPosOffset = desiredPos;
                currentRotEuler = desiredRotEuler;
            }
            else
            {
                currentPosOffset = Vector3.SmoothDamp(currentPosOffset, desiredPos, ref followPosVel,
                    profile.followSmoothTime);
                currentRotEuler.x = Mathf.SmoothDampAngle(currentRotEuler.x, desiredRotEuler.x, ref followRotVel.x,
                    profile.followSmoothTime);
                currentRotEuler.y = Mathf.SmoothDampAngle(currentRotEuler.y, desiredRotEuler.y, ref followRotVel.y,
                    profile.followSmoothTime);
                currentRotEuler.z = Mathf.SmoothDampAngle(currentRotEuler.z, desiredRotEuler.z, ref followRotVel.z,
                    profile.followSmoothTime);

                recoilPosition.localPosition = baseLocalPos + currentPosOffset;
                rotationPoint.localRotation = baseLocalRot * Quaternion.Euler(currentRotEuler);
            }
        }

        private static Vector3 ClampVector(Vector3 v, Vector3 maxAbs)
        {
            v.x = Mathf.Clamp(v.x, -Mathf.Abs(maxAbs.x), Mathf.Abs(maxAbs.x));
            v.y = Mathf.Clamp(v.y, -Mathf.Abs(maxAbs.y), Mathf.Abs(maxAbs.y));
            v.z = Mathf.Clamp(v.z, -Mathf.Abs(maxAbs.z), Mathf.Abs(maxAbs.z));
            return v;
        }

        private static Vector3 EvaluateWeightPerAxis(
            float age,
            float kickTime,
            float returnTime,
            AxisCurves kickCurve,
            AxisCurves returnCurve)
        {
            // Kick phase: 0..kickTime => 0..1 by kickCurve
            if (age <= kickTime)
            {
                float t = Mathf.Clamp01(age / kickTime);
                return kickCurve.Evaluate(t);
            }

            // Return phase: kickTime..(kick+return) => 1..0
            float tr = Mathf.Clamp01((age - kickTime) / returnTime);

            // returnCurve: 0..1 (how much returned), we convert to remaining weight 1..0
            Vector3 returned = returnCurve.Evaluate(tr);
            return Vector3.one - returned;
        }

        private void OnDrawGizmosSelected()
        {
            if (!profile || !recoilPosition || !rotationPoint) return;

            // рисуем "одиночный выстрел" как траекторию в мире, относительно текущей базы
            // (для настройки кривых это обычно то, что нужно)
            Transform parent = recoilPosition.parent ? recoilPosition.parent : transform;

            Vector3 basePos = Application.isPlaying ? baseLocalPos : recoilPosition.localPosition;
            Quaternion baseRot = Application.isPlaying ? baseLocalRot : rotationPoint.localRotation;

            float total = profile.kickTime + profile.returnTime;
            int n = Mathf.Max(4, profile.gizmoSamples);

            Vector3 prevWorld = Vector3.zero;
            for (int i = 0; i < n; i++)
            {
                float t = (n == 1) ? 0f : (i / (float)(n - 1));
                float age = t * total;

                Vector3 w = EvaluateWeightPerAxis(age, profile.kickTime, profile.returnTime, profile.kickCurve,
                    profile.returnCurve);

                Vector3 localPos = basePos + Vector3.Scale(profile.positionKick * positionAmount, w);
                Vector3 localRotEuler = Vector3.Scale(profile.rotationKickEuler * rotationAmount, w);
                Quaternion localRot = baseRot * Quaternion.Euler(localRotEuler);

                Vector3 worldPos = parent.TransformPoint(localPos);
                Quaternion worldRot = parent.rotation * localRot;

                // точки
                Gizmos.DrawSphere(worldPos, 0.005f);

                // направление
                Vector3 dir = worldRot * Vector3.forward;
                Gizmos.DrawLine(worldPos, worldPos + dir * 0.05f);

                // линия траектории
                if (i > 0) Gizmos.DrawLine(prevWorld, worldPos);
                prevWorld = worldPos;
            }
        }

        private struct RecoilEvent
        {
            public float startTime;
            public Vector3 posKick;
            public Vector3 rotKickEuler;
            public float kickTime;
            public float returnTime;
        }
    }
}