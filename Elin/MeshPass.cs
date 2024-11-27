using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshPass : ScriptableObject
{
	public void Init()
	{
		this.batchSize = ((this.batches.Count > 0) ? this.batches[0].size : 1023);
		if (!this.initialized || this.mesh == null)
		{
			if (this.subPass)
			{
				this.subPass.Init();
			}
			if (this.snowPass)
			{
				this.snowPass.Init();
			}
			if (this.shadowPass)
			{
				this.shadowPass.Init();
			}
			this.batches.Add(new MeshBatch(this));
			this._Refresh();
			this.initialized = true;
		}
		this.idx = (this.batchIdx = 0);
	}

	public unsafe void Add(Point point, float tile = 0f, float color = 0f)
	{
		Vector3 vector = *point.Position();
		this._batch = this.batches[this.batchIdx];
		this._batch.matrices[this.idx].m03 = vector.x;
		this._batch.matrices[this.idx].m13 = vector.y;
		this._batch.matrices[this.idx].m23 = vector.z;
		this._batch.tiles[this.idx] = tile;
		if (this.setColor)
		{
			this._batch.colors[this.idx] = color;
		}
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void Add(ref Vector3 v, float tile = 0f, float color = 0f)
	{
		this._batch = this.batches[this.batchIdx];
		this._batch.matrices[this.idx].m03 = v.x;
		this._batch.matrices[this.idx].m13 = v.y;
		this._batch.matrices[this.idx].m23 = v.z;
		this._batch.tiles[this.idx] = tile;
		if (this.setColor)
		{
			this._batch.colors[this.idx] = color;
		}
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void Add(float x, float y, float z, float tile = 0f, float color = 0f)
	{
		this._batch = this.batches[this.batchIdx];
		this._batch.matrices[this.idx].m03 = x;
		this._batch.matrices[this.idx].m13 = y;
		this._batch.matrices[this.idx].m23 = z;
		this._batch.tiles[this.idx] = tile;
		if (this.setColor)
		{
			this._batch.colors[this.idx] = color;
		}
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void AddWithScale(float x, float y, float z, int tile, float scale)
	{
		this._batch = this.batches[this.batchIdx];
		Matrix4x4[] matrices = this._batch.matrices;
		float num = -scale / 2f + 0.5f;
		matrices[this.idx].m03 = x + num;
		matrices[this.idx].m13 = y + num;
		matrices[this.idx].m23 = z;
		matrices[this.idx].m00 = scale;
		matrices[this.idx].m11 = scale;
		this._batch.tiles[this.idx] = (float)tile;
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void AddShadow(MeshPassParam p, ref Vector3 fix)
	{
		this._batch = this.batches[this.batchIdx];
		this._batch.matrices[this.idx].SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
		this._batch.matrices[this.idx].m03 = p.x + fix.x;
		this._batch.matrices[this.idx].m13 = p.y + fix.y;
		this._batch.matrices[this.idx].m23 = p.z + fix.z;
		if (this.setTile)
		{
			this._batch.tiles[this.idx] = p.tile;
		}
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void AddShadow(float x, float y, float z, ShadowData.Item s, SourcePref pref, int dir = 0, bool snow = false)
	{
		int angle = s.angle;
		float num = (float)s.scaleX * 0.01f;
		float num2 = (float)s.scaleY * 0.01f;
		bool flag = dir % 2 == 1;
		int num3 = flag ? 1 : -1;
		this._batch = this.batches[this.batchIdx];
		Matrix4x4[] matrices = this._batch.matrices;
		matrices[this.idx].m03 = x - 0.01f * (float)((dir >= 2) ? (flag ? pref.shadowBRX : pref.shadowBX) : (flag ? pref.shadowRX : pref.shadowX)) + ((0.7f * -num + 0.7f) * (float)num3 + (float)s.x * 0.01f) * (float)num3;
		matrices[this.idx].m13 = y - 0.01f * (float)((dir >= 2) ? (flag ? pref.shadowBRY : pref.shadowBY) : (flag ? pref.shadowRY : pref.shadowY)) + 0.6f * -num2 + 0.6f + (float)s.y * 0.01f;
		matrices[this.idx].m23 = z;
		matrices[this.idx].m00 = num;
		matrices[this.idx].m11 = num2;
		if (angle == 0)
		{
			matrices[this.idx].m00 = num;
			matrices[this.idx].m01 = 0f;
			matrices[this.idx].m10 = 0f;
			matrices[this.idx].m11 = num2;
		}
		else
		{
			this.tempV.z = (float)(-(float)angle);
			Quaternion quaternion = Quaternion.Euler(0f, 0f, (float)(-(float)angle));
			this.tempE.x = 1f - quaternion.z * quaternion.z * 2f;
			this.tempE.y = -quaternion.w * quaternion.z * 2f;
			matrices[this.idx].m00 = this.tempE.x * num;
			matrices[this.idx].m01 = this.tempE.y * num2;
			matrices[this.idx].m10 = -this.tempE.y * num;
			matrices[this.idx].m11 = this.tempE.x * num2;
		}
		this._batch.tiles[this.idx] = (float)((s.tile + (snow ? 1000 : 0)) * num3);
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void AddWithRotation(float x, float y, float z, float sx, float sy, int tile, float angle, bool flip)
	{
		float num = 1f;
		float num2 = 1f;
		int num3 = flip ? -1 : 1;
		this._batch = this.batches[this.batchIdx];
		Matrix4x4[] matrices = this._batch.matrices;
		matrices[this.idx].m03 = x + ((0.7f * -num + 0.7f) * (float)num3 + sx * 0.01f) * (float)num3;
		matrices[this.idx].m13 = y + 0.6f * -num2 + 0.6f + sy * 0.01f;
		matrices[this.idx].m23 = z;
		matrices[this.idx].m00 = num;
		matrices[this.idx].m11 = num2;
		this.tempV.z = -angle;
		Quaternion quaternion = Quaternion.Euler(0f, 0f, -angle);
		this.tempE.x = 1f - quaternion.z * quaternion.z * 2f;
		this.tempE.y = -quaternion.w * quaternion.z * 2f;
		matrices[this.idx].m00 = this.tempE.x * num;
		matrices[this.idx].m01 = this.tempE.y * num2;
		matrices[this.idx].m10 = -this.tempE.y * num;
		matrices[this.idx].m11 = this.tempE.x * num2;
		this._batch.tiles[this.idx] = (float)(tile * num3);
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void Add(MeshPassParam p)
	{
		this._batch = this.batches[this.batchIdx];
		this._batch.matrices[this.idx].m03 = p.x;
		this._batch.matrices[this.idx].m13 = p.y;
		this._batch.matrices[this.idx].m23 = p.z;
		if (this.setTile)
		{
			this._batch.tiles[this.idx] = p.tile;
		}
		if (this.setColor)
		{
			this._batch.colors[this.idx] = p.color;
		}
		if (this.setMatColor)
		{
			this._batch.matColors[this.idx] = p.matColor;
		}
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void Add(float x, float y, float z, float tile, float color, float matColor = 0f)
	{
		this._batch = this.batches[this.batchIdx];
		this._batch.matrices[this.idx].m03 = x;
		this._batch.matrices[this.idx].m13 = y;
		this._batch.matrices[this.idx].m23 = z;
		this._batch.tiles[this.idx] = tile;
		if (this.setColor)
		{
			this._batch.colors[this.idx] = color;
		}
		if (this.setMatColor)
		{
			this._batch.matColors[this.idx] = matColor;
		}
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void Add(MeshPassParam p, float tile, float color, float matColor = 0f)
	{
		this._batch = this.batches[this.batchIdx];
		this._batch.matrices[this.idx].m03 = p.x;
		this._batch.matrices[this.idx].m13 = p.y;
		this._batch.matrices[this.idx].m23 = p.z;
		this._batch.tiles[this.idx] = tile;
		if (this.setColor)
		{
			this._batch.colors[this.idx] = color;
		}
		if (this.setMatColor)
		{
			this._batch.matColors[this.idx] = matColor;
		}
		this.idx++;
		if (this.idx == this.batchSize)
		{
			this.NextBatch();
		}
	}

	public void Draw()
	{
		if (this.haveSnowPass)
		{
			this.snowPass.Draw();
		}
		if (this.haveSubPass)
		{
			this.subPass.Draw();
		}
		if (this.haveShadowPass)
		{
			this.shadowPass.Draw();
		}
		if (this.idx == 0 && this.batchIdx == 0)
		{
			return;
		}
		for (int i = 0; i < this.batchIdx + 1; i++)
		{
			int num = (i == this.batchIdx) ? this.idx : this.batches[i].size;
			if (num == 0)
			{
				break;
			}
			if (this.setTile)
			{
				this.batches[i].mpb.SetFloatArray("_Tiles", this.batches[i].tiles);
			}
			if (this.setColor)
			{
				this.batches[i].mpb.SetFloatArray("_Color", this.batches[i].colors);
			}
			if (this.setMatColor)
			{
				this.batches[i].mpb.SetFloatArray("_MatColor", this.batches[i].matColors);
			}
			if (this.renderQueue != 0)
			{
				if (this.batches[i].mat == null)
				{
					this.batches[i].mat = new Material(this.mat);
				}
				this.batches[i].mat.renderQueue = this.renderQueue + i;
				Graphics.DrawMeshInstanced(this.mesh, 0, this.batches[i].mat, this.batches[i].matrices, num, this.batches[i].mpb, ShadowCastingMode.Off, false, 0, Camera.main);
			}
			else
			{
				Graphics.DrawMeshInstanced(this.mesh, 0, this.mat, this.batches[i].matrices, num, this.batches[i].mpb, ShadowCastingMode.Off, false, 0, Camera.main);
			}
		}
		this.lastBatchCount = this.batchIdx;
		this.lastCount = this.batchIdx * this.batchSize + this.idx;
		this.idx = (this.batchIdx = 0);
		if (this.resize)
		{
			Debug.Log("#pass Resize Pass:" + base.name);
			this.resize = false;
			this.batches[0] = new MeshBatch(this);
		}
	}

	public void DrawEmpty()
	{
		this.idx = (this.batchIdx = 0);
	}

	public void NextBatch()
	{
		this.idx = 0;
		this.batchIdx++;
		if (this.batchIdx >= this.batches.Count)
		{
			if (this.batchSize != 1023)
			{
				this.batchSize = 1023;
				this.resize = true;
			}
			this.batches.Add(new MeshBatch(this));
		}
	}

	private Mesh SpriteToMesh(Sprite sprite)
	{
		Mesh mesh = new Mesh();
		mesh.SetVertices(Array.ConvertAll<Vector2, Vector3>(sprite.vertices, (Vector2 c) => c).ToList<Vector3>());
		mesh.uv = sprite.uv;
		mesh.SetTriangles(Array.ConvertAll<ushort, int>(sprite.triangles, (ushort c) => (int)c), 0);
		return mesh;
	}

	public override string ToString()
	{
		return base.name;
	}

	public void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(this.mesh);
		foreach (MeshPass meshPass in Core.Instance.scene.passes)
		{
			meshPass.initialized = false;
			meshPass.batches.Clear();
			meshPass.Init();
		}
	}

	public float meshX
	{
		get
		{
			return this.pmesh.size.x;
		}
		set
		{
			this.pmesh.size.x = value;
		}
	}

	public void OnValidate()
	{
		this.Refresh();
	}

	public void _Refresh()
	{
		this.haveSubPass = this.subPass;
		this.haveShadowPass = this.shadowPass;
		this.haveSnowPass = this.snowPass;
		if (!this.mesh)
		{
			if (this.pmesh)
			{
				this.mesh = this.pmesh.GetMesh();
			}
			else if (this.sprite)
			{
				this.mesh = this.SpriteToMesh(this.sprite);
			}
		}
		if (this.haveSnowPass)
		{
			this.snowPass.mesh = this.mesh;
		}
		if (this.haveSubPass)
		{
			this.subPass.mesh = this.mesh;
		}
		if (this.haveShadowPass)
		{
			this.shadowPass.mesh = this.mesh;
		}
	}

	public const int TokenLiquid = 10000;

	public const int TokenLowWall = 1000000;

	public const int TokenLowWallDefault = 3000000;

	public MeshPass subPass;

	public MeshPass snowPass;

	public MeshPass shadowPass;

	public Material mat;

	public Mesh mesh;

	public ProceduralMesh pmesh;

	public Sprite sprite;

	public bool setTile;

	public bool setColor;

	public bool setMatColor;

	public bool setExtra;

	public int renderQueue;

	public MeshPassParam _p = new MeshPassParam();

	public FilterMode filter;

	public int lastCount;

	public int lastBatchCount;

	[NonSerialized]
	public bool haveSubPass;

	[NonSerialized]
	public bool haveShadowPass;

	[NonSerialized]
	public bool haveSnowPass;

	[NonSerialized]
	public int idx;

	[NonSerialized]
	public int batchIdx;

	[NonSerialized]
	public int batchSize = 1023;

	[NonSerialized]
	public List<MeshBatch> batches = new List<MeshBatch>();

	[NonSerialized]
	private bool initialized;

	[NonSerialized]
	private bool resize;

	[NonSerialized]
	public Texture preserveTex;

	private MeshBatch _batch;

	private Vector3 tempV = Vector3.zero;

	private Vector2 tempE = Vector2.zero;

	[NonSerialized]
	private bool first = true;
}
