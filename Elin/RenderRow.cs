using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderRow : SourceData.BaseRow, IRenderSource
{
	public virtual string idString
	{
		get
		{
			return "";
		}
	}

	public virtual string RecipeID
	{
		get
		{
			return "";
		}
	}

	public virtual string pathRenderData
	{
		get
		{
			return "Scene/Render/Data/";
		}
	}

	public virtual string idRenderData
	{
		get
		{
			return this._idRenderData;
		}
	}

	public virtual RenderData defaultRenderData
	{
		get
		{
			return ResourceCache.Load<RenderData>(this.pathRenderData + "_Default");
		}
	}

	public virtual string pathSprite
	{
		get
		{
			return "Scene/Render/Data/";
		}
	}

	public virtual string idSprite
	{
		get
		{
			return "";
		}
	}

	public virtual string prefabName
	{
		get
		{
			return "ThingActor";
		}
	}

	public bool HasTag(CTAG _tag)
	{
		return this.tag.Contains(_tag.ToString());
	}

	public virtual string GetSearchName(bool jp)
	{
		string result;
		if (!jp)
		{
			if ((result = this._nameSearch) == null)
			{
				return this._nameSearch = this.name.ToLower();
			}
		}
		else if ((result = this._nameSearchJP) == null)
		{
			result = (this._nameSearchJP = base.GetText("name", false).ToLower());
		}
		return result;
	}

	public SourceCategory.Row Category
	{
		get
		{
			SourceCategory.Row result;
			if ((result = this._category) == null)
			{
				result = (this._category = this.sources.categories.map[this.category]);
			}
			return result;
		}
	}

	public string RecipeCat
	{
		get
		{
			string result;
			if ((result = this._recipeCat) == null)
			{
				result = (this._recipeCat = this.Category.recipeCat);
			}
			return result;
		}
	}

	public bool ContainsTag(string _tag)
	{
		return this.tag.Contains(_tag);
	}

	public SourceManager sources
	{
		get
		{
			return Core.Instance.sources;
		}
	}

	public override void OnImportData(SourceData data)
	{
		base.OnImportData(data);
		this._tiles = new int[0];
		this.SetTiles();
	}

	public void SetRenderData()
	{
		this.replacer = new SpriteReplacer();
		if (this.idRenderData.IsEmpty())
		{
			this.renderData = this.defaultRenderData;
		}
		else if (this.idRenderData[0] == '@')
		{
			string[] array = this.idRenderData.Replace("@", "").Split('#', StringSplitOptions.None);
			string text = array[0];
			if (array.Length > 1)
			{
				this.aliasPref = array[1];
			}
			this.renderData = RenderRow.DictRenderData.TryGetValue(text, null);
			if (this.renderData == null)
			{
				RenderData renderData = (ResourceCache.Load<RenderData>(this.pathRenderData + text) ?? this.defaultRenderData).Instantiate<RenderData>();
				if (this is SourceChara.Row)
				{
					RenderData renderData2 = renderData;
					renderData2.offset.x = renderData2.offset.x + renderData.pass.pmesh.size.x * 0.5f;
					RenderData renderData3 = renderData;
					renderData3.offset.y = renderData3.offset.y + renderData.pass.pmesh.size.y * 0.5f;
				}
				else
				{
					float num = renderData.pass.pmesh.size.x * 0.5f + renderData.pass.pmesh.pos.x;
					float num2 = renderData.pass.pmesh.size.y * 0.5f + renderData.pass.pmesh.pos.y;
					RenderData renderData4 = renderData;
					renderData4.offset.x = renderData4.offset.x + num;
					RenderData renderData5 = renderData;
					renderData5.offset.y = renderData5.offset.y + num2;
					RenderData renderData6 = renderData;
					renderData6.offsetBack.x = renderData6.offsetBack.x + num;
					RenderData renderData7 = renderData;
					renderData7.offsetBack.y = renderData7.offsetBack.y + num2;
					RenderData renderData8 = renderData;
					renderData8.heldPos.x = renderData8.heldPos.x + num * 0.7f;
					RenderData renderData9 = renderData;
					renderData9.heldPos.y = renderData9.heldPos.y + num2 * 0.7f;
				}
				renderData.pass = null;
				this.renderData = renderData;
				RenderRow.DictRenderData[text] = renderData;
			}
		}
		else
		{
			this.renderData = (ResourceCache.Load<RenderData>(this.pathRenderData + this.idRenderData) ?? this.defaultRenderData);
		}
		if (!Application.isEditor && this.pref == null)
		{
			if (this.aliasPref.IsEmpty())
			{
				this.pref = new SourcePref();
			}
			else
			{
				this.pref = this.sources.cards.map[this.aliasPref].pref;
			}
		}
		if (!this.renderData.initialized)
		{
			this.renderData.Init();
		}
		this.SetTiles();
		string a = this.colorType;
		if (!(a == "alt"))
		{
			if (a == "random")
			{
				this.useRandomColor = true;
			}
		}
		else
		{
			this.useAltColor = true;
		}
		if (this.defMat[0] == '!')
		{
			this.fixedMaterial = true;
			this.defMat = this.defMat.Substring(1, this.defMat.Length - 1);
		}
		else
		{
			this.fixedMaterial = false;
		}
		if (!this.sources.materials.alias.ContainsKey(this.defMat))
		{
			Debug.Log(this.defMat);
		}
		this.DefaultMaterial = this.sources.materials.alias[this.defMat];
	}

	public virtual void SetTiles()
	{
		if (!this.renderData || !this.renderData.pass)
		{
			return;
		}
		if (this._tiles.Length != this.tiles.Length)
		{
			this._tiles = new int[this.tiles.Length];
			for (int i = 0; i < this.tiles.Length; i++)
			{
				this._tiles[i] = this.tiles[i] / 100 * (int)this.renderData.pass.pmesh.tiling.x + this.tiles[i] % 100;
			}
		}
	}

	public int ConvertTile(int tile)
	{
		return this.renderData.ConvertTile(tile);
	}

	public Sprite GetSprite(int dir = 0, int skin = 0, bool snow = false)
	{
		if (this.replacer.HasSprite(this.idSprite))
		{
			return this.replacer.data.GetSprite(snow);
		}
		int[] array = null ?? this._tiles;
		if (this.sprites == null)
		{
			this.sprites = new Sprite[(this.skins == null) ? 1 : (this.skins.Length + 1), (array.Length == 0) ? 1 : array.Length];
		}
		if (skin != 0 && skin >= this.sprites.GetLength(0))
		{
			skin = 0;
		}
		if (dir >= array.Length)
		{
			dir = 0;
		}
		if (!this.sprites[skin, dir])
		{
			MeshPass pass = this.renderData.pass;
			if (pass)
			{
				Texture2D texture2D = pass.mat.GetTexture("_MainTex") as Texture2D;
				ProceduralMesh pmesh = pass.pmesh;
				int num;
				if (skin == 0)
				{
					num = Mathf.Abs(array[dir]);
				}
				else
				{
					num = Mathf.Abs(array[dir] + this.skins[skin - 1] * ((array[dir] > 0) ? 1 : -1));
				}
				int num2 = (int)((float)texture2D.width / pmesh.tiling.x);
				int num3 = (int)((float)texture2D.height / pmesh.tiling.y);
				int num4 = (int)((float)num % pmesh.tiling.x);
				int num5 = (int)((float)num / pmesh.tiling.x);
				this.sprites[skin, dir] = Sprite.Create(texture2D, new Rect((float)(num4 * num2), (float)(texture2D.height - (num5 + 1) * num3), (float)num2, (float)(num3 * (this.renderData.multiSize ? 2 : 1))), Vector2.zero, 100f, 0U, SpriteMeshType.FullRect);
			}
			else
			{
				this.sprites[skin, dir] = SpriteSheet.Get(this.idSprite);
				if (!this.sprites[skin, dir])
				{
					this.sprites[skin, dir] = (Resources.Load<Sprite>(this.pathSprite + this.idSprite) ?? Resources.Load<Sprite>(this.pathSprite + this.idSprite + "_0"));
				}
			}
			if (!this.sprites[skin, dir])
			{
				this.sprites[skin, dir] = Core.Instance.refs.spriteNull;
			}
		}
		return this.sprites[skin, dir];
	}

	public void SetSpriteRenderer(SpriteRenderer sr, Sprite sprite = null, int matCol = 0, bool setTransform = true, int dir = 0)
	{
		sr.sprite = (sprite ?? this.GetSprite(dir, 0, false));
		if (this.renderData is RenderDataThing)
		{
			sr.sprite = EClass.core.refs.spriteThingActor;
		}
		int num = (matCol == 0) ? 104025 : matCol;
		float num2 = (float)(num / 262144) * 0.01f;
		float num3 = 0.02f;
		float num4 = 0.3f;
		if (num2 != 0f)
		{
			num3 *= num2;
		}
		Color color = new Color(num3 * (float)(num % 262144 / 4096) + num4, num3 * (float)(num % 4096 / 64) + num4, num3 * (float)(num % 64) + num4, 1f);
		sr.color = color;
		if (setTransform)
		{
			float num5 = sr.bounds.max.x - sr.bounds.min.x;
			Vector3 max = sr.bounds.max;
			Vector3 min = sr.bounds.min;
			sr.transform.localPosition = new Vector3(-0.5f * num5, 0f, 0f);
		}
	}

	public virtual SourcePref GetPref()
	{
		return this.pref;
	}

	public void SetImage(Image image, Sprite sprite = null, int matCol = 0, bool setNativeSize = true, int dir = 0, int idSkin = 0)
	{
		image.sprite = (sprite ?? this.GetSprite(dir, idSkin, false));
		int num = (matCol == 0) ? 104025 : matCol;
		float num2 = 0.02f;
		image.color = new Color(num2 * (float)(num % 262144 / 4096), num2 * (float)(num % 4096 / 64), num2 * (float)(num % 64), (float)(num / 262144) * 0.01f)
		{
			a = 1f
		};
		RectTransform rectTransform = image.Rect();
		SourcePref sourcePref = this.GetPref();
		rectTransform.pivot = this.renderData.imagePivot - new Vector2(0.01f * (float)sourcePref.pivotX, 0.01f * (float)sourcePref.pivotY);
		float x = Mathf.Abs(image.transform.localScale.x) * (float)((sprite || this._tiles.Length == 0 || this._tiles[dir % this._tiles.Length] >= 0) ? 1 : -1);
		float y = image.transform.localScale.y;
		image.transform.localScale = new Vector3(x, y, image.transform.localScale.z);
		sourcePref.Validate();
		if (setNativeSize)
		{
			image.SetNativeSize();
			if (this.renderData.imageScale.x != 1f || this.renderData.imageScale.y != 1f || sourcePref.scaleIcon != 0)
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * (this.renderData.imageScale.x + (float)sourcePref.scaleIcon * 0.01f), rectTransform.sizeDelta.y * (this.renderData.imageScale.y + (float)sourcePref.scaleIcon * 0.01f));
			}
		}
	}

	public void SetRenderParam(RenderParam p, SourceMaterial.Row mat, int dir)
	{
		p.tile = (float)this.GetTile(mat ?? this.DefaultMaterial, dir);
		p.matColor = (float)this.GetColorInt(mat ?? this.DefaultMaterial);
		p.mat = (mat ?? this.DefaultMaterial);
		p.liquidLv = 0;
		p.dir = dir;
		p.cell = null;
	}

	public unsafe virtual RenderParam GetRenderParam(SourceMaterial.Row mat, int dir, Point point = null, int bridgeHeight = -1)
	{
		RenderParam shared = RenderParam.shared;
		shared.tile = (float)this.GetTile(mat, dir);
		shared.matColor = (float)this.GetColorInt(mat);
		shared.mat = mat;
		shared.liquidLv = 0;
		shared.dir = dir;
		shared.cell = null;
		Vector3 vector;
		if (point == null)
		{
			shared.color = 11010048f;
			vector = Core.Instance.scene.camSupport.renderPos;
			if (this.renderData.multiSize)
			{
				vector.y -= 0.8f;
			}
		}
		else
		{
			shared.color = 10485760f;
			BaseTileMap tileMap = Core.Instance.screen.tileMap;
			vector = *point.Position(bridgeHeight);
		}
		shared.x = vector.x;
		shared.y = vector.y;
		shared.z = vector.z;
		return shared;
	}

	public virtual int GetTile(SourceMaterial.Row mat, int dir = 0)
	{
		return this._tiles[dir % this._tiles.Length];
	}

	public int GetColorInt(SourceMaterial.Row mat)
	{
		if (this.useAltColor)
		{
			if (this.colorMod != 0)
			{
				return BaseTileMap.GetColorInt(ref mat.altColor, this.colorMod);
			}
			return 104025;
		}
		else
		{
			if (this.colorMod != 0)
			{
				return BaseTileMap.GetColorInt(ref mat.matColor, this.colorMod);
			}
			return 104025;
		}
	}

	public static Dictionary<string, RenderData> DictRenderData = new Dictionary<string, RenderData>();

	public int[] tiles;

	public int[] _tiles;

	public int[] skins;

	public int colorMod;

	public int sort;

	public int value;

	public int LV;

	public int chance;

	public int tempChance;

	public int snowTile;

	public string name;

	public string name_JP;

	public string detail;

	public string detail_JP;

	public string _idRenderData;

	public string _tileType;

	public string defMat;

	public string colorType;

	public string category;

	public string idSound;

	public string aliasPref;

	public string[] components;

	public string[] factory;

	public string[] recipeKey;

	public string[] tag;

	public int W = 1;

	public int H = 1;

	public bool multisize;

	public SourcePref pref;

	[NonSerialized]
	public RenderData renderData;

	[NonSerialized]
	public Sprite[,] sprites;

	[NonSerialized]
	public TileType tileType;

	[NonSerialized]
	public bool useAltColor;

	[NonSerialized]
	public bool useRandomColor;

	[NonSerialized]
	public bool fixedMaterial;

	[NonSerialized]
	public SourceMaterial.Row DefaultMaterial;

	[NonSerialized]
	public SpriteReplacer replacer;

	[NonSerialized]
	public string _nameSearch;

	[NonSerialized]
	public string _nameSearchJP;

	[NonSerialized]
	private SourceCategory.Row _category;

	[NonSerialized]
	private string _recipeCat;
}
