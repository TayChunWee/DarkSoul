using UnityEngine;

namespace DamageArea
{
    public class DamageBase : MonoBehaviour
    {
        public void SetData(DamageAreaData data) { _data = data; }
        protected DamageAreaData _data;
    }
}