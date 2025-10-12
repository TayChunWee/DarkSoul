using UnityEngine;

namespace DS
{
    [DisallowMultipleComponent]
    public class CameraHandler : MonoBehaviour
    {
        [Header("Targets")]
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;

        private Transform myTransform;
        private Vector3 cameraTransformPosition;

        [Header("Orbit (deg/sec)")]
        public float lookSpeed  = 180f;
        public float pivotSpeed = 180f;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35f;
        public float maximumPivot = 35f;

        [Header("Follow (Linear)")]
        [SerializeField] private float followMaxSpeed = 40f;
        [SerializeField] private float catchupDistance = 1.2f;
        [SerializeField] private float catchupMultiplier = 2.5f;
        [SerializeField] private float snapDistance = 3.0f;

        [Header("Collision")]
        public float cameraSphereRadius = 0.18f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private float defaultPosition;
        private float targetPositionZ;

        private void Awake()
        {
            myTransform = transform;
            if (cameraTransform) defaultPosition = cameraTransform.localPosition.z;
            targetPositionZ = defaultPosition;
        }

        public void SetTarget(Transform target) => targetTransform = target;

        public void FollowTarget(float delta)
        {
            if (!targetTransform) return;

            Vector3 cur = myTransform.position;
            Vector3 dst = targetTransform.position;
            float dist  = Vector3.Distance(cur, dst);

            if (dist > snapDistance)
            {
                myTransform.position = dst;
            }
            else
            {
                float speed = followMaxSpeed;
                if (dist > catchupDistance)
                {
                    float t = Mathf.InverseLerp(catchupDistance, snapDistance, dist);
                    speed *= Mathf.Lerp(1f, Mathf.Max(1f, catchupMultiplier), t);
                }
                myTransform.position = Vector3.MoveTowards(cur, dst, speed * delta);
            }

            HandleCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle  += mouseXInput * lookSpeed * delta;
            pivotAngle -= mouseYInput * pivotSpeed * delta;
            pivotAngle  = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            myTransform.rotation = Quaternion.Euler(0f, lookAngle, 0f);
            cameraPivotTransform.localRotation = Quaternion.Euler(pivotAngle, 0f, 0f);
        }

        private void HandleCameraCollisions(float delta)
        {
            if (!cameraTransform || !cameraPivotTransform) return;

            targetPositionZ = defaultPosition;

            Vector3 pivotPos = cameraPivotTransform.position;
            Vector3 dir = (cameraTransform.position - pivotPos).normalized;
            float maxDistance = Mathf.Abs(defaultPosition);

            if (Physics.SphereCast(pivotPos, cameraSphereRadius, dir, out RaycastHit hit, maxDistance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                float dist = Vector3.Distance(pivotPos, hit.point);
                targetPositionZ = -(dist - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPositionZ) < minimumCollisionOffset)
                targetPositionZ = -minimumCollisionOffset;

            cameraTransformPosition = cameraTransform.localPosition;
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPositionZ, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}
