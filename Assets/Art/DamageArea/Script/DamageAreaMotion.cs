using UnityEngine;

namespace DamageArea
{
    public class DamageAreaMotion : MonoBehaviour
    {
        [SerializeField] private GaugeType _gaugeType;
        [Header("Object")]
        [SerializeField] private GameObject _frame_obj;
        [SerializeField] private Material _frame_mt;
        [SerializeField] private GameObject _gauge_obj;
        [SerializeField] private Material _gauge_mt;

        // Time
        public void SetData(DamageAreaData timeData) { _timeData = timeData; }
        private DamageAreaData _timeData;
        private float _timer = 0;

        // DAMotion Phase
        enum phase { spawn, gauge, attack, delete }
        private phase _currentPhase = (int)phase.spawn;

        private void Start()
        {
            FadeIn(_frame_mt);
            FadeIn(_gauge_mt);
        }

        void Update()
        {
            switch (_currentPhase)
            {
                case (int)phase.spawn:
                    SizeUpPerTime(_frame_obj, _timeData.SpawnTime);
                    if (_timer > _timeData.SpawnTime)
                    {
                        ++_currentPhase;
                        _timer = 0;
                    }
                    break;
                case phase.gauge:
                    SizeUpPerTime(_gauge_obj, _timeData.GaugeTime);
                    if (_timer > _timeData.GaugeTime)
                    {
                        ++_currentPhase;
                        _timer = 0;
                    }
                    break;
                case phase.attack:
                    if(_timer > _timeData.AttackTime)
                    {
                        ++_currentPhase;
                    }
                    break;
                case phase.delete:
                    FadeOut(_frame_mt, _timeData.DeleteTime);
                    FadeOut(_gauge_mt, _timeData.DeleteTime);
                    if (_timer > _timeData.DeleteTime)
                    {
                        Destroy(this.gameObject);
                    }
                    break;
            }
            _timer += Time.deltaTime;
        }

        private void SizeUpPerTime(GameObject obj, float time)
        {
            float scale = _timer / time;
            switch (_gaugeType)
            {
                case GaugeType.Normal:
                    obj.transform.localScale = new Vector3(scale, scale, scale);
                    break;
                case GaugeType.Width:
                    obj.transform.localScale = new Vector3(scale, 1, 1);
                    break;
            }
        }

        private void FadeIn(Material mt)
        {
            float alphaValue = 1;
            mt.color = new Color(mt.color.r, mt.color.g, mt.color.b, alphaValue);
        }

        private void FadeOut(Material mt, float time)
        {
            float alphaValue = (time - _timer) / time;
            mt.color = new Color(mt.color.r, mt.color.g, mt.color.b, alphaValue);
        }
    }
}