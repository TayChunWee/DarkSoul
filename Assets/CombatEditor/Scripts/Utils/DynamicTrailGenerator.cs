using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace CombatEditor
{	
	public class DynamicTrailGenerator
	{
	    Vector3[] _vertices;
	    Queue<int> trianglesQueue;
	
	
	    Queue<Vector3> BaseQueue;
	    Queue<Vector3> TipQueue;
	
	    int[] _triangles;
	    int _frameCount;
	    Vector2[] _uvs;
	    Mesh _mesh;
	    public GameObject _trailMeshObj;
	    int FrameCount;
	    Transform _base;
	    Transform _tip;
	    int FrameIndex;
	
	
	    int _maxFrame;
	    int _fadeMul;
	    static int NUM_VERTICES = 12;
	    public Material _trailMat;
	    public int QuadCount = 0;
	    int _trailSubs;
	
	    static int StaticUVScale = 3;
	    AbilityEventObj_DynamicTrail.TrailBehavior _uvMethod;
	    // 0: normal, uv is 0 -1 from back to front,  1: uv is static like sekiro.
	    //public int UVUpdateMethods;
	
	    //QuadIndex After Inited, used for static UV to make sekiro-like trail vfx;
	    int EmmitedQuadIndex;
	
	    public DynamicTrailGenerator( Transform BaseTransform, Transform TipTransform , int MaxFrame,int trailSubs, int FadeMul ,Material TrailMat, AbilityEventObj_DynamicTrail.TrailBehavior uvMethod )
	    {
	        _base = BaseTransform;
	        _tip = TipTransform;
	        _maxFrame = MaxFrame;
	        _fadeMul = FadeMul;
	        _trailMat = TrailMat;
	        _trailSubs = trailSubs;
	
	        EmmitedQuadIndex = 0;
	        _uvMethod = uvMethod;
	    }
	
	    public GameObject InitTrailMesh()
	    {
	        _trailMeshObj = new GameObject("MeshTrail");
	        var _filter = _trailMeshObj.AddComponent<MeshFilter>();
	        var _renderer = _trailMeshObj.AddComponent<MeshRenderer>();
	
	        _renderer.receiveShadows = false;
	        _renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	        _mesh = new Mesh();
	        _trailMeshObj.GetComponent<MeshFilter>().mesh = _mesh;
	        _trailMeshObj.GetComponent<Renderer>().material = _trailMat;
	        trianglesQueue = new Queue<int>();
	        BaseQueue = new Queue<Vector3>();
	        TipQueue = new Queue<Vector3>();
	        return _trailMeshObj;
	    }
	    public void SetTrail()
	    {
	       
	    }
	    int QueuedCount;
	    Vector3[] SpineVertices;
	
	    public void StopTrailHard()
	    {
	        BaseQueue.Clear();
	        TipQueue.Clear();
	        QueuedCount = 0;
	
	        UpdateTrail();
	
	        //BaseQueue.Clear();
	        //TipQueue.Clear();
	        //QueuedCount = 0;
	
	        //if (BaseQueue.Count > 0)
	        //{
	        //    while (BaseQueue.Count > 0)
	        //    {
	        //        BaseQueue.Dequeue();
	        //        TipQueue.Dequeue();
	        //        QueuedCount -= 1;
	        //    }
	        //}
	        //UpdateTrail();
	
	
	    }
	
	    public void StopTrailSmoothly()
	    {
	
	        if (_base == null || _tip == null || _trailMeshObj == null)
	        {
	            return;
	        }
	
	        if (_uvMethod == AbilityEventObj_DynamicTrail.TrailBehavior.FlowUV)
	        {
	            if (BaseQueue.Count > 0)
	            {
	                for (int i = 0; i < _fadeMul; i++)
	                {
	                    if (BaseQueue.Count > 0)
	                    {
	                        BaseQueue.Dequeue();
	                        TipQueue.Dequeue();
	                        QueuedCount -= 1;
	                    }
	                }
	            }
	            //Even if the trail is stopping , it still need to generate new meshes at head, or the trail would stop immediately. 
	            if (BaseQueue.Count >= 0)
	            {
	                UpdateTrailOnCurrentFrame();
	            }
	        }
	    }
	    public void UpdateTrailOnCurrentFrame()
	    {
	
	        if (_base == null || _tip == null || _trailMeshObj == null)
	        {
	            return;
	        }
	        UpdateBaseandTipQueue();
	        UpdateTrail();
	    }
	
	    public void UpdateTrail()
	    {
	        if (_base == null || _tip == null || _trailMeshObj == null)
	        {
	            return;
	        }
	        UpdateAllVertices();
	
	        if (_uvMethod == AbilityEventObj_DynamicTrail.TrailBehavior.FlowUV)
	        {
	            UpdateAllUVTo_0_1();
	        }
	        if (_uvMethod == AbilityEventObj_DynamicTrail.TrailBehavior.StaticUV)
	        {
	            UpdateHeadUVByEmmitedCount();
	        }
	        UpdateAllTriangles();
	
	        if (QuadCount == 0)
	        {
	            _uvs = new Vector2[0];
	        }
	
	        _mesh.Clear();
	        _mesh.vertices = SpineVertices;
	        _mesh.triangles = _triangles;
	        _mesh.uv = _uvs;
	    }
	
	
	    public void UpdateHeadUVByEmmitedCount()
	    {
	        var LastFrameUV = _uvs;
	        _uvs = new Vector2[QuadCount * NUM_VERTICES];
	
	        if (QuadCount < 1) return;
	        for (int i = 0; i < LastFrameUV.Length; i++)
	        {
	            _uvs[i] = LastFrameUV[i];
	        }
	
	        var NewGeneratedMeshCount = (_uvs.Length - LastFrameUV.Length) / NUM_VERTICES;
	
	        for (int i = 0; i < NewGeneratedMeshCount; i++)
	        {
	
	            var StartIndex = ( EmmitedQuadIndex - (i+1));
	            //Update head.
	            var LeftDown = new Vector2(StaticUVScale * ((float)StartIndex / _maxFrame), 0);
	            var LeftUp = new Vector2(StaticUVScale * ((float)StartIndex / _maxFrame), 1);
	            var RightDown = new Vector2(StaticUVScale * ((float)(StartIndex+1) / _maxFrame), 0);
	            var RightUp = new Vector2(StaticUVScale *  ((float)(StartIndex+1) / _maxFrame), 1);
	
	
	
	            var UVStartIndex = (QuadCount - (i+1)) * NUM_VERTICES;
	            _uvs[UVStartIndex] = RightDown;
	            _uvs[UVStartIndex + 1] = RightUp;
	            _uvs[UVStartIndex + 2] = LeftUp;
	
	            _uvs[UVStartIndex + 3] = RightDown;
	            _uvs[UVStartIndex + 4] = LeftUp;
	            _uvs[UVStartIndex + 5] = RightUp;
	
	            _uvs[UVStartIndex + 6] = LeftUp;
	            _uvs[UVStartIndex + 7] = RightDown;
	            _uvs[UVStartIndex + 8] = LeftDown;
	
	            _uvs[UVStartIndex + 9] = LeftUp;
	            _uvs[UVStartIndex + 10] = LeftDown;
	            _uvs[UVStartIndex + 11] = RightDown;
	        }
	    }
	
	    public void UpdateAllUVTo_0_1()
	    {
	
	        _uvs = new Vector2[QuadCount * NUM_VERTICES];
	        for (int i = 0; i < QuadCount; i++)
	        {
	            //int UVIndex = QueuedCount;
	            var LeftDown = new Vector2((float)(i - 1) / QuadCount, 0);
	            var LeftUp = new Vector2((float)(i - 1) / QuadCount, 1);
	            var RightDown = new Vector2((float)i / QuadCount, 0);
	            var RightUp = new Vector2((float)i / QuadCount, 1);
	
	
	            var UVStartIndex = i * NUM_VERTICES;
	
	            _uvs[UVStartIndex] = RightDown;
	            _uvs[UVStartIndex + 1] = RightUp;
	            _uvs[UVStartIndex + 2] = LeftUp;
	
	            _uvs[UVStartIndex + 3] = RightDown;
	            _uvs[UVStartIndex + 4] = LeftUp;
	            _uvs[UVStartIndex + 5] = RightUp;
	
	            _uvs[UVStartIndex + 6] = LeftUp;
	            _uvs[UVStartIndex + 7] = RightDown;
	            _uvs[UVStartIndex + 8] = LeftDown;
	
	            _uvs[UVStartIndex + 9] = LeftUp;
	            _uvs[UVStartIndex + 10] = LeftDown;
	            _uvs[UVStartIndex + 11] = RightDown;
	        }
	    }
	
	
	    public void UpdateAllTriangles()
	    {
	        _triangles = new int[QuadCount * NUM_VERTICES];
	
	        for (int i = 0; i < QuadCount * NUM_VERTICES; i++)
	        {
	            _triangles[i] = i;
	        }
	    }
	
	    public void UpdateAllVertices()
	    {
	        if (BaseQueue.Count >= 3)
	        {
	            CatmullRom BaseSpine = new CatmullRom(BaseQueue.ToArray(), _trailSubs, false);
	            BaseSpine.DrawSpline(Color.white);
	            CatmullRom TipSpine = new CatmullRom(TipQueue.ToArray(), _trailSubs, false);
	            TipSpine.DrawSpline(Color.white);
	
	
	
	            QuadCount = BaseSpine.GetPoints().Length - 1;
	
	
	            SpineVertices = new Vector3[NUM_VERTICES * QuadCount];
	            for (int i = 0; i < QuadCount; i++)
	            {
	                var StartIndex = i * NUM_VERTICES;
	
	                var basePos = BaseSpine.GetPoints()[i + 1].position;
	                var tipPos = TipSpine.GetPoints()[i + 1].position;
	                var lastBasePos = BaseSpine.GetPoints()[i].position;
	                var lastTipPos = TipSpine.GetPoints()[i].position;
	
	                SpineVertices[StartIndex] = basePos;
	                SpineVertices[StartIndex + 1] = tipPos;
	                SpineVertices[StartIndex + 2] = lastTipPos;
	
	                SpineVertices[StartIndex + 3] = basePos;
	                SpineVertices[StartIndex + 4] = lastTipPos;
	                SpineVertices[StartIndex + 5] = tipPos;
	
	                SpineVertices[StartIndex + 6] = lastTipPos;
	                SpineVertices[StartIndex + 7] = basePos;
	                SpineVertices[StartIndex + 8] = lastBasePos;
	
	                SpineVertices[StartIndex + 9] = lastTipPos;
	                SpineVertices[StartIndex + 10] = lastBasePos;
	                SpineVertices[StartIndex + 11] = basePos;
	            }
	        }
	        else
	        {
	            QuadCount = 0;
	        }
	    }
	    public void UpdateBaseandTipQueue()
	    {
	
	        if (_uvMethod == AbilityEventObj_DynamicTrail.TrailBehavior.FlowUV)
	        {
	            if (QueuedCount > _maxFrame)
	            {
	                if (BaseQueue.Count > 0)
	                {
	                    BaseQueue.Dequeue();
	                }
	                if (TipQueue.Count > 0)
	                {
	                    TipQueue.Dequeue();
	                }
	
	                if (QueuedCount > 0)
	                {
	                    QueuedCount -= 1;
	                }
	            }
	        }
	
	        QueuedCount += 1;
	        BaseQueue.Enqueue(_base.transform.position);
	        TipQueue.Enqueue(_tip.transform.position);
	        EmmitedQuadIndex += _trailSubs;
	    }
	
	
	}
	
}
