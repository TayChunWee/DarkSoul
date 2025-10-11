using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatEditor
{
    [AbilityEvent]
    [CreateAssetMenu(menuName = "AbilityEvents / SFX")]
    public class AbilityEventObj_SFX : AbilityEventObj
    {
        public List<AudioClip> clips;
        public float Volume = 1;
        public override EventTimeType GetEventTimeType()
        {
            return EventTimeType.EventTime;
        }
        public override AbilityEventEffect Initialize()
        {
            return new AbilityEventEffect_SFX(this);
        }
        public override AbilityEventPreview InitializePreview()
        {
            return new AbilityEventPreview_SFX(this);
        }
    }
    public class AbilityEventEffect_SFX : AbilityEventEffect
    {
        public AbilityEventEffect_SFX(AbilityEventObj Obj) : base(Obj)
        {
            _EventObj = Obj;
        }
        public override void StartEffect()
        {
            base.StartEffect();
            var clip = ((AbilityEventObj_SFX)_EventObj).clips[Random.Range(0, ((AbilityEventObj_SFX)_EventObj).clips.Count)];
            clip.PlayClip(((AbilityEventObj_SFX)_EventObj).Volume);
        }
    }

    public static class AudioHelper
    {
        public static void PlayClip(this AudioClip clip, float Volume = 1)
        {
            AudioSource source = new GameObject("AudioSource").AddComponent<AudioSource>();
            source.volume = Volume;
            source.clip = clip;
            source.Play();
            GameObject.Destroy(source.gameObject, clip.length);
        }
    }

}
