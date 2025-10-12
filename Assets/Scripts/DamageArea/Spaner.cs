using UnityEngine;

namespace DamageArea
{
    public class Spaner : MonoBehaviour
    {
        [SerializeField] private GameObject _damageArea60;
        [SerializeField] private GameObject _damageArea360;

        public void Spawn(GameObject prefab, Transform transform, DamageAreaData Data)
        {
            Vector3 liftOffset = new Vector3(0, 0.01f, 0);
            GameObject area = Instantiate(prefab, transform.position + liftOffset, transform.rotation);
            area.transform.localScale = Vector3.one * Data.size;

            area.GetComponent<Motion>()?.SetData(Data);
            area.GetComponent<DamageBase>()?.SetData(Data);
        }

        public void Spawn60(Transform transform, DamageAreaData timeData)
            => Spawn(_damageArea60, transform, timeData);

        public void Spawn360(Transform transform, DamageAreaData timeData)
            => Spawn(_damageArea360, transform, timeData);
    }
}