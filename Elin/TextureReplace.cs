using System;
using System.IO;
using UnityEngine;

public class TextureReplace
{
	public enum Source
	{
		User,
		Mod,
		Local
	}

	public DateTime date;

	public Texture2D tex;

	public FileInfo file;

	public TextureData data;

	public int index;

	public int w;

	public int h;

	public Source source;

	public TextureReplace original;

	public bool user => source == Source.User;

	public void TryRefresh(bool force)
	{
		if (File.Exists(file.FullName))
		{
			DateTime lastWriteTime = File.GetLastWriteTime(file.FullName);
			bool flag = date.Year != 1 && date.Equals(lastWriteTime);
			if (!flag || force || !tex)
			{
				Load(flag);
			}
			date = lastWriteTime;
		}
	}

	public void Load(bool dateMatched)
	{
		if ((bool)tex)
		{
			UnityEngine.Object.Destroy(tex);
		}
		tex = IO.LoadPNG(file.FullName);
		w = tex.width;
		h = tex.height;
		int dstX = index % 100 * data.tileW;
		int dstY = data.tex.height - index / 100 * data.tileH - tex.height;
		Graphics.CopyTexture(tex, 0, 0, 0, 0, tex.width, tex.height, data.tex, 0, 0, dstX, dstY);
	}

	public void DestoryTex()
	{
		UnityEngine.Object.Destroy(tex);
	}
}
