using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

 namespace CombatEditor
{	
	
# if UNITY_EDITOR
	public class AbilityEventPreview_Particles : AbilityEventPreview_CreateObjWithHandle
	{
	    public AbilityEventObj_Particles Obj => (AbilityEventObj_Particles)_EventObj;
	
	    public AbilityEventPreview_Particles(AbilityEventObj Obj) : base(Obj)
	    {
	        _EventObj = Obj;
	    }
	
	
	    public bool PreviewActive()
	    {
	        return eve.Previewable;
	    }
	
	    float ParticleInitSimulateSpeed = 1;
	
	    public override void InitPreview()
	    {
	        base.InitPreview();
	        SetParticleData();
	    }
        ParticleSystem particle;

	    public void SetParticleData()
	    {
	        if (InstantiatedObj != null)
	        {
                particle = InstantiatedObj.GetComponent<ParticleSystem>();
	            if (particle != null)
	            {
	                particle.Stop();
	                particle.useAutoRandomSeed = false;
	                var main = particle.main;
	                ParticleInitSimulateSpeed = main.simulationSpeed;
	                //main.simulationSpeed = 0;
	            }
	        }
	    }
	
	    /// <summary>
	    /// The particle's real time is influenced by timescale event.
	    /// </summary>
	    /// <param name="ScaledPercentage"></param>
	    public override void PreviewRunningInScale(float ScaledPercentage)
	    {
	        base.PreviewRunningInScale(ScaledPercentage);
	        if (InstantiatedObj == null) return;
	        //Debug.Log("Simulate?");
	        SimulateParticles(ScaledPercentage);
	    }
	    public void SimulateParticles(float ScaledPercentage)
	    {
	        //Set Preview Percentage
	        if (Obj.IsActive)
	        {
	            var main = InstantiatedObj.GetComponent<ParticleSystem>().main;
	            main.simulationSpeed = ParticleInitSimulateSpeed;
	
	            bool IsInRange = false;
	            if (EventObj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventRange && CurrentInScaledRange)
	            {
	                IsInRange = true;
	            }
	            if(EventObj.GetEventTimeType() == AbilityEventObj.EventTimeType.EventTime && ScaledPercentage >= StartTimeScaledPercentage)
	            {
	                IsInRange = true;
	            }
	            //ParticleSystem need 1/60f to start simulate
	            if (IsInRange)
	            {
                    particle.Simulate(1 / 60f + (ScaledPercentage - StartTimeScaledPercentage) * AnimLength, true, true);
                    //InstantiatedObj.GetComponent<ParticleSystem>().Simulate(0.1f, true, false);
                    SceneView.RepaintAll();
	            }
	            else
	            {
                    particle.Simulate(0, true, true);
                    SceneView.RepaintAll();
	            }
	        }
	    }
	}
	
#endif
}
