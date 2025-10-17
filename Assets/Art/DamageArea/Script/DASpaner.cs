using UnityEngine;

namespace DamageArea
{
    public class DASpaner : MonoBehaviour
    {
        [Header("Circle Type")]
        [SerializeField] private GameObject _C_60;
        [SerializeField] private GameObject _C_360;
        [Header("Rect Type")]
        [SerializeField] private GameObject _R_Center;
        [SerializeField] private GameObject _R_End;

        #region SpawnBase

        public void Spawn(GameObject prefab, DamageAreaData Data)
        {
            Vector3 liftOffset = new Vector3(0, 0.01f, 0);
            GameObject area = Instantiate(prefab, transform.position + liftOffset, transform.rotation);
            area.transform.localScale = Vector3.one * Data.Size;

            area.GetComponent<DAMotion>()?.SetData(Data);
            area.GetComponent<DamageBase>()?.SetData(Data);
        }

        public void Spawn(GameObject prefab, Vector3 addPos, DamageAreaData Data)
        {
            Vector3 liftOffset = new Vector3(0, 0.01f, 0);
            GameObject area = Instantiate(prefab, transform.position + addPos + liftOffset, transform.rotation);
            area.transform.localScale = Vector3.one * Data.Size;

            area.GetComponent<DAMotion>()?.SetData(Data);
            area.GetComponent<DamageBase>()?.SetData(Data);
        }

        #endregion

        #region Circle

        public void Spawn60(DamageAreaData Data)
            => Spawn(_C_60, Data);

        public void Spawn360(Vector3 addPos, DamageAreaData Data)
            => Spawn(_C_360, addPos, Data);

        #endregion

        #region Rect

        public void SpawnRect(int longth, DamageAreaData Data)
        {
            float spawnDist = 0;
            Vector3 rectPos = Vector3.zero;
            for (int i = 1; i < longth; i++)
            {
                SpawnRectCenter(rectPos, Data);
                spawnDist = i * Data.Size;
                float angle = transform.localEulerAngles.y * Mathf.Deg2Rad;
                rectPos.x = spawnDist * Mathf.Sin(angle);
                rectPos.z = spawnDist * Mathf.Cos(angle);
            }
            SpawnRectEnd(rectPos, Data);
        }

        private void SpawnRectCenter(Vector3 addPos, DamageAreaData Data)
            => Spawn(_R_Center, addPos, Data);

        private void SpawnRectEnd(Vector3 addPos, DamageAreaData Data)
            => Spawn(_R_End, addPos, Data);

        #endregion
    }
}