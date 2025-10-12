using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{
    [System.Serializable]
	public class CharacterAnimSpeedModifier
	{
	    public float SpeedScale;
	    public float MaxTime;
	    public float StartTime;
	    public bool SelfDestroy;
	    public CharacterAnimSpeedModifier(float speedScale, float maxTime)
	    {
	        SpeedScale = speedScale;
	        MaxTime = maxTime;
	        StartTime = Time.time;
	        SelfDestroy = true;
	    }
	
	    public CharacterAnimSpeedModifier(float speedScale)
	    {
	        SpeedScale = speedScale;
	        StartTime = Time.time;
	        SelfDestroy = false;
	    }
	
	}
	
	public class AnimSpeedExecutor
	{
	    public AnimSpeedExecutor(CombatController _controller) { _combatController = _controller; }
	    public CombatController _combatController;
	    public void Execute()
	    {
	        _combatController._animator.speed = GetCurrentSpeedModifier();
	    }
	
	    public List<CharacterAnimSpeedModifier> _animSpeedModifiers = new List<CharacterAnimSpeedModifier>();
	    public void AddSpeedModifiers(float SpeedScale, float time)
	    {
	        _animSpeedModifiers.Add(new CharacterAnimSpeedModifier(SpeedScale, time));
	    }
	    public CharacterAnimSpeedModifier AddAnimSpeedModifier(float SpeedScale)
	    {
	        CharacterAnimSpeedModifier modifier = new CharacterAnimSpeedModifier(SpeedScale);
	        _animSpeedModifiers.Add(modifier);
	        return modifier;
	    }
	    public void RemoveAnimSpeedModifier(CharacterAnimSpeedModifier modifier)
	    {
	        _animSpeedModifiers.Remove(modifier);
	    }
	    public float GetCurrentSpeedModifier()
	    {
	        //var LowestSpeed = _animSpeedModifiers.OrderBy(t => t.SpeedScale).Take(1).ToArray();
	        //if(LowestSpeed.Length > 0)
	        //{
	        //    return LowestSpeed[0].SpeedScale;
	        //}
	        float Speed = 1;
	        for (int i = 0; i < _animSpeedModifiers.Count; i++)
	        {
                if(Time.time - _animSpeedModifiers[i].StartTime > _animSpeedModifiers[i].MaxTime && _animSpeedModifiers[i].SelfDestroy)
                {
                    _animSpeedModifiers.RemoveAt(i);
                    i -= 1;
                    continue;
                }
	            Speed *= _animSpeedModifiers[i].SpeedScale;
	        }
	        return Speed;
	    }
	
	}
}
