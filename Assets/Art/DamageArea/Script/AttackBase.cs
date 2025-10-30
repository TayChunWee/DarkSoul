using UnityEngine;

namespace DamageArea
{
    public class AttackBase : MonoBehaviour
    {
        [SerializeField] protected Collider _Collider;
        protected DamageAreaData _data;
        public void SetData(DamageAreaData data)
        {
            _data = data;
            if (_data.Damage == 0) this.enabled = false;
        }
    }
}