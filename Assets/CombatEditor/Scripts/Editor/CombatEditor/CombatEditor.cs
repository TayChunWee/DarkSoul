
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace CombatEditor
{	

	public class Track
	{
	    public Object trackContent;
	    public int TrackTriggerFrame;
	    public int TrackEndFrame;
	    public bool TrackIsSelected;
	    public TimeLineHelper helper;
	}
	
	public class AnimEventTrack
	{
	    public AbilityEvent eve;
	    public TimeLineHelper helper;
	    public int StartFrame;
	    public int EndFrame;
	    public AnimEventTrack(AbilityEvent e , EditorWindow window)
	    {
	        eve = e;
	        StartFrame = (int)e.GetEventStartTime();
	        EndFrame = (int)e.GetEventEndTime();
	        helper = new TimeLineHelper( window );
	    }
	}
	
	public partial class CombatEditor : EditorWindow
	{
	    public static string SandBoxPath = "Assets/ScriptableObjects/Abilities/Sandbox/";
	    public static string TemplatesPath = "Assets/ScriptableObjects/Abilities/Templates/";
	
	    public static float Height_Top = 40;
	    public static float LineHeight = 25;
	
	
	    Rect boxRect;
	    public float valueChangeValue = 5; 
	    public float Width_TrackLabel = 250;
	    int HeaderFontSize = 15;
	    public Vector2 TimeLineWindowSize = new Vector2(2000, 300);
	
	    public int FrameIntervalCount = 6;
	    public int FrameIntervalDistance => Mathf.RoundToInt( 10 * TimeLineScaler);
	
	    public Vector2 TestTrackPosition;
	    bool IsDraggingTracks;
	    int CurrentDraggedFrame;
	    Object TrackObj;
	    public List<Track> tracks = new List<Track>();
	
	    public float Width_Inspector = 350;
	    TimeLineHelper TopFrameThumbHelper;
	
	    int CurrentFrame = 0;
	
	    Rect L3TrackAvailableRect;
	    Rect L2Rect;
	    int AnimFrameCount;
	
	    public bool IsInited = false;
	    GUIStyle MyBoxGUIStyle;
	    static GUIStyle MyDeleteButtonStyle;
	    public List<AnimEventTrack> AnimEventTracks;
	
	    public AbilityScriptableObject SelectedAbilityObj;
	    public TimeLineHelper AnimClipHelper;
	
	    public float Width_Ability = 200;
	
	    public CombatPreviewController _previewer;
	
	    public bool PreviewNeedReload;
	
	    //Preview will reload after frame update.
	    public void RequirePreviewReload()
	    {
	        PreviewNeedReload = true;
	    }

	    private void OnEnable()
	    {
	        EditorSceneManager.activeSceneChangedInEditMode += ChangeScene;
	        AssemblyReloadEvents.afterAssemblyReload += AnimationBackToStart;

            AssemblyReloadEvents.beforeAssemblyReload += OnEndPreview;
	        UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChange;
	    }
	  
	    private void OnDisable()
	    {
            EditorSceneManager.activeSceneChangedInEditMode -= ChangeScene;

            AssemblyReloadEvents.beforeAssemblyReload -= OnEndPreview;

            AssemblyReloadEvents.afterAssemblyReload -= AnimationBackToStart;
	        UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChange;
	    }
	


	    string LastSceneName = "";
	    public void ChangeScene(Scene s1, Scene s2)
	    {
	        if(LastSceneName!= s2.name)
	        {
	            ClearCombatController();
	        }
	        LastSceneName = s2.name;
	    }
	
	
	    public void PlayModeStateChange(UnityEditor.PlayModeStateChange state)
	    {
	        if(state == UnityEditor.PlayModeStateChange.ExitingEditMode)
	        {
	            var previewer = this._previewer;
	            if (previewer != null)
	            {
	                previewer.OnPlayModeStart();
	            }
	        }
	    }
	
	    [MenuItem("Tools/CombatEditor")]
	    public static void Init()
	    {
	        // Get existing open window or if none, make a new one:
	        CombatEditor window = (CombatEditor)EditorWindow.GetWindow(typeof(CombatEditor));
	        window.Show();
	        window.InitWindow();
	    }
	    public void InitWindow()
	    {
	        _previewer = new CombatPreviewController();
	           IsInited = true;
	        TopFrameThumbHelper = new TimeLineHelper(this);
	        InitRect();
	        InitSplitLine();
	    }

        static int SplitterIntervalDistance = 10;
	    public void InitRect()
	    {
	        L2Rect = new Rect(Width_Ability, 0, Width_TrackLabel - SplitterIntervalDistance, position.height);
	        L3TrackAvailableRect = new Rect(Width_Ability + Width_TrackLabel, Height_Top, AnimFrameCount * FrameIntervalDistance, position.height - Height_Top);
	    }
	    bool ReloadAfterStart;
	
	    private void OnGUI()
	    {
            //ConfigController
            if (!IsInited)
	        {
	            InitWindow();
	        }
	        InitGUIStyle();
	
	        PaintL1();
	        PaintL2();
	        PaintL3();
	
	        PaintSplitLine();
	        PaintRenameField();


            Resetter();


	    }
	

        public void Resetter()
        {
            Event e = Event.current;
            if(e.type == EventType.KeyDown)
            {
                if(e.keyCode == KeyCode.B)
                {
                    Debug.Log("ClearPreviews?");
                    OnEndPreview();
                }
            }
        }

	    float StartPlayTime;
	
	
	    Vector2 InspectorScrollPos;
	    Color SelectedTrackColor => new Color(Color.green.r, Color.green.g, Color.green.b, 0.2f);
	
	    public Color HorizontalLineColor => Color.grey;
	   
	    Vector2 Scroll_Track;
	    public int SelectedTrackIndex;
	    Vector2 Scroll_Fields;
	   
	    bool IsInspectingAnimationConfig;
	    Object CurrentInspectedObj;
	    public GUIStyle HeaderStyle;
	
	    public Color SelectedColor => Color.yellow;
	    public Color OnInspectedColor => Color.green;
	
	    public enum InspectedType{ Null,AnimationConfig,Track,CombatConfig, PreviewConfig}
	    public InspectedType CurrentInspectedType;
	    //public CurrentInspectedAbility;
	
	    Editor CurrentAbilityEditor;
	    GUIStyle AbilityConfigStyle;
	
	    GUIStyle AbilityButtonStyle;
	    GUIStyle PopUpStyle;
	
	    public int CurrentSelectedAbilityIndex;
	
	    Vector2 AbilityScroll;
	    float Width_Scroll = 12;
	    
	}
	
}
