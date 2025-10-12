using UnityEngine;

namespace DamageArea
{
    public class Motion : MonoBehaviour
    {
        [Header("Model")]
        [SerializeField] private GameObject _frame_obj;
        [SerializeField] private Material _frame_mt;
        [SerializeField] private GameObject _gauge_obj;
        [SerializeField] private Material _gauge_mt;

        // Time
        public void SetData(DamageAreaData timeData) { _timeData = timeData; }
        [SerializeField] private DamageAreaData _timeData;
        private float _timer = 0;

        enum phase { spawn, gauge, delete }
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
                        _currentPhase = phase.gauge;
                        _timer = 0;
                    }
                    break;
                case phase.gauge:
                    SizeUpPerTime(_gauge_obj, _timeData.GaugeTime);
                    if (_timer > _timeData.GaugeTime)
                    {
                        _currentPhase = phase.delete;
                        _timer = 0;
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
            obj.transform.localScale = new Vector3(scale, scale, scale);
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