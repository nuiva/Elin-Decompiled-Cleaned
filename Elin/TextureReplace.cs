using System;
using System.IO;
using UnityEngine;

public class TextureReplace
{
	public bool user
	{
		get
		{
			return this.source == TextureReplace.Source.User;
		}
	}

	public void TryRefresh(bool force)
	{
		if (!File.Exists(this.file.FullName))
		{
			return;
		}
		DateTime lastWriteTime = File.GetLastWriteTime(this.file.FullName);
		bool flag = this.date.Year != 1 && this.date.Equals(lastWriteTime);
		if (!flag || force || !this.tex)
		{
			this.Load(flag);
		}
		this.date = lastWriteTime;
	}

	public void Load(bool dateMatched)
	{
		if (this.tex)
		{
			UnityEngine.Object.Destroy(this.tex);
		}
		this.tex = IO.LoadPNG(this.file.FullName, FilterMode.Point);
		this.w = this.tex.width;
		this.h = this.tex.height;
		int dstX = this.index % 100 * this.data.tileW;
		int dstY = this.data.tex.height - this.index / 100 * this.data.tileH - this.tex.height;
		Graphics.CopyTexture(this.tex, 0, 0, 0, 0, this.tex.width, this.tex.height, this.data.tex, 0, 0, dstX, dstY);
	}

	public void DestoryTex()
	{
		UnityEngine.Object.Destroy(this.tex);
	}

	public DateTime date;

	public Texture2D tex;

	public FileInfo file;

	public TextureData data;

	public int index;

	public int w;

	public int h;

	public TextureReplace.Source source;

	public TextureReplace original;

	public enum Source
	{
		User,
		Mod,
		Local
	}
}
