using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureData : EScriptable
{
	public void TryRefresh()
	{
		DateTime lastWriteTime = File.GetLastWriteTime(this.path);
		bool flag = this.date.DateTime.Year != 1 && this.date.DateTime.Equals(lastWriteTime);
		if (this.dictReplace.Count > 0)
		{
			List<int> list = new List<int>();
			foreach (TextureReplace textureReplace in this.dictReplace.Values)
			{
				if (!File.Exists(textureReplace.file.FullName))
				{
					this.forceRefresh = true;
					list.Add(textureReplace.index);
				}
			}
			foreach (int index in list)
			{
				this.RemoveDeletedReplace(index);
			}
		}
		if (this.forceRefresh)
		{
			this.forceRefresh = false;
			flag = false;
		}
		if (!flag || !this.IsValid())
		{
			this.Load(flag);
		}
		foreach (TextureReplace textureReplace2 in this.dictReplace.Values)
		{
			textureReplace2.TryRefresh(!flag);
		}
		this.date.DateTime = lastWriteTime;
	}

	public void ForceRefresh()
	{
		this.forceRefresh = true;
		this.TryRefresh();
	}

	public static void RefreshAll()
	{
		if (!EClass.core.IsGameStarted)
		{
			return;
		}
		foreach (TextureData textureData in EClass.core.textures.texMap.Values)
		{
			textureData.ForceRefresh();
		}
	}

	public virtual bool IsValid()
	{
		TextureData.Type type = this.type;
		if (type != TextureData.Type.Pass)
		{
			return type != TextureData.Type.World || EClass.scene.tileset.AtlasTexture;
		}
		using (List<MeshPass>.Enumerator enumerator = this.listPass.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.mat.GetTexture(this.texName))
				{
					return false;
				}
			}
		}
		return true;
	}

	public void Load(bool dateMatched)
	{
		if (!dateMatched)
		{
			Texture2D texture2D = IO.LoadPNG(this.path, FilterMode.Point);
			if (!texture2D)
			{
				Debug.Log(this.path);
			}
			if (this.tex.width != texture2D.width || this.tex.height != texture2D.height)
			{
				Debug.Log(string.Concat(new string[]
				{
					this.id,
					"/",
					texture2D.width.ToString(),
					"/",
					texture2D.height.ToString(),
					"/",
					this.path
				}));
			}
			this.tex.SetPixels32(texture2D.GetPixels32());
			this.tex.Apply();
			UnityEngine.Object.Destroy(texture2D);
			Debug.Log("Reloaded Texture:" + this.path);
		}
		TextureData.Type type = this.type;
		if (type != TextureData.Type.Pass)
		{
			if (type != TextureData.Type.World)
			{
				return;
			}
		}
		else
		{
			using (List<MeshPass>.Enumerator enumerator = this.listPass.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MeshPass meshPass = enumerator.Current;
					meshPass.mat.SetTexture(this.texName, this.tex);
					if (meshPass.haveShadowPass)
					{
						meshPass.shadowPass.mat.SetTexture(this.texName, this.tex);
					}
				}
				return;
			}
		}
		EClass.scene.tileset.AtlasTexture = this.tex;
	}

	public void CreateReplace(int index, string path, TextureReplace.Source source, int sizeX, int sizeY)
	{
		string str = this.id + "_" + index.ToString() + ".png";
		Texture2D texture2D = new Texture2D(this.tileW * sizeX, this.tileH * sizeY);
		int srcX = index % 100 * this.tileW;
		int srcY = this.tex.height - index / 100 * this.tileH - texture2D.height;
		Graphics.CopyTexture(this.tex, 0, 0, srcX, srcY, this.tileW * sizeX, this.tileH * sizeY, texture2D, 0, 0, 0, 0);
		byte[] bytes = texture2D.EncodeToPNG();
		try
		{
			File.WriteAllBytes(path + str, bytes);
			EClass.core.textures.TryAddReplace(new FileInfo(path + str), source, true, true);
		}
		catch
		{
		}
		UnityEngine.Object.Destroy(texture2D);
	}

	public void AddReplace(TextureReplace r)
	{
		TextureReplace textureReplace = this.dictReplace.TryGetValue(r.index, null);
		if (textureReplace != null)
		{
			if (r.source == TextureReplace.Source.Local && textureReplace.source != TextureReplace.Source.Local)
			{
				r.original = textureReplace;
			}
			else
			{
				textureReplace.DestoryTex();
			}
		}
		if (r.source == TextureReplace.Source.Local)
		{
			this.listReplaceLocal.Add(r);
		}
		this.dictReplace[r.index] = r;
	}

	public void DeleteReplace(TextureReplace r)
	{
		r.file.Delete();
		this.RemoveDeletedReplace(r.index);
	}

	public void RemoveDeletedReplace(int index)
	{
		TextureReplace textureReplace = this.dictReplace.TryGetValue(index, null);
		if (textureReplace == null)
		{
			return;
		}
		textureReplace.DestoryTex();
		if (textureReplace.original != null && textureReplace.original.file.Exists)
		{
			this.dictReplace[index] = textureReplace.original;
			return;
		}
		this.dictReplace.Remove(index);
	}

	public string id;

	public TextureData.Type type;

	public TextureData.Date date;

	public Texture2D tex;

	public bool forceRefresh;

	public int tileW;

	public int tileH;

	[NonSerialized]
	public string path;

	[NonSerialized]
	public string texName;

	[NonSerialized]
	public List<MeshPass> listPass = new List<MeshPass>();

	[NonSerialized]
	public Dictionary<int, TextureReplace> dictReplace = new Dictionary<int, TextureReplace>();

	[NonSerialized]
	public List<TextureReplace> listReplaceLocal = new List<TextureReplace>();

	[Serializable]
	public class Date
	{
		public DateTime DateTime
		{
			get
			{
				return new DateTime(this.ticks);
			}
			set
			{
				this.ticks = value.Ticks;
			}
		}

		public long ticks;
	}

	public enum Type
	{
		Pass,
		World,
		DestTex
	}
}
