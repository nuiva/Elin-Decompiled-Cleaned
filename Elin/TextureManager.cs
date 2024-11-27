using System;
using System.Collections.Generic;
using System.IO;

public class TextureManager
{
	public string pathTex
	{
		get
		{
			return CorePath.packageCore + "Texture/";
		}
	}

	public void Init()
	{
		BaseTileMap tileMap = EClass.scene.screenElin.tileMap;
		CoreRef.TextureDatas textureData = EClass.core.refs.textureData;
		this.AddBase(textureData.block, "blocks.png", tileMap.passBlock, "_MainTex");
		this.AddBase(textureData.floor, "floors.png", tileMap.passFloor, "_MainTex");
		this.AddBase(textureData.objs_SS, "objs_SS.png", tileMap.passObjSS, "_MainTex");
		this.AddBase(textureData.objs_S, "objs_S.png", tileMap.passObjS, "_MainTex");
		this.AddBase(textureData.objs, "objs.png", tileMap.passObj, "_MainTex");
		this.AddBase(textureData.objs_L, "objs_L.png", tileMap.passObjL, "_MainTex");
		this.AddBase(textureData.roofs, "roofs.png", tileMap.passRoof, "_MainTex");
		this.AddBase(textureData.shadows, "shadows.png", tileMap.passShadow, "_MainTex");
		this.AddBase(textureData.fov, "fov.png", tileMap.passFov, "_MainTex");
		this.AddBase(textureData.objs_C, "objs_C.png", tileMap.passChara, "_MainTex");
		this.AddBase(textureData.objs_CLL, "objs_CLL.png", tileMap.passCharaLL, "_MainTex");
		this.AddBase(textureData.block_snow, "blocks_snow.png", tileMap.passBlock.snowPass, "_MainTex");
		this.AddBase(textureData.floor_snow, "floors_snow.png", tileMap.passFloor.snowPass, "_MainTex");
		this.AddBase(textureData.objs_S_snow, "objs_S_snow.png", tileMap.passObjS.snowPass, "_MainTex");
		this.AddBase(textureData.objs_snow, "objs_snow.png", tileMap.passObj.snowPass, "_MainTex");
		this.AddBase(textureData.objs_L_snow, "objs_L_snow.png", tileMap.passObjL.snowPass, "_MainTex");
		this.Add(tileMap.passRamp, textureData.block, "_MainTex");
		this.Add(tileMap.passWaterBlock, textureData.block, "_MainTex");
		this.Add(tileMap.passFog, textureData.block, "_MainTex");
		this.Add(tileMap.passLiquid, textureData.block, "_MainTex");
		this.Add(tileMap.passBlockEx, textureData.block, "_MainTex");
		this.Add(tileMap.passInner, textureData.block, "_MainTex");
		this.Add(tileMap.passBlockMarker, textureData.block, "_MainTex");
		this.Add(tileMap.passEdge, textureData.floor, "_MainTex");
		this.Add(tileMap.passFloorEx, textureData.floor, "_MainTex");
		this.Add(tileMap.passFloorWater, textureData.floor, "_MainTex");
		this.Add(tileMap.passAutoTile, textureData.floor, "_MainTex");
		this.Add(tileMap.passShore, textureData.floor, "_MainTex");
		this.Add(tileMap.passAutoTileWater, textureData.floor, "_MainTex");
		this.Add(tileMap.passFloorMarker, textureData.floor, "_MainTex");
		this.Add(tileMap.passCharaL, textureData.objs_C, "_MainTex");
		this.Add(tileMap.passIcon, textureData.objs_S, "_MainTex");
		this.AddList(textureData.world, "world.png");
		this.AddList(textureData.bird, "bird1.png");
		foreach (FileInfo fileInfo in new DirectoryInfo(CorePath.user + "/Texture Replace").GetFiles())
		{
			if (fileInfo.Name.EndsWith(".png"))
			{
				this.TryAddReplace(fileInfo, TextureReplace.Source.User, true, false);
			}
		}
		foreach (FileInfo file in EClass.core.mods.replaceFiles)
		{
			this.TryAddReplace(file, TextureReplace.Source.Mod, true, false);
		}
	}

	public void AddBase(TextureData data, string path, MeshPass pass, string texName = "_MainTex")
	{
		this.texMap[data.id] = data;
		data.path = this.pathTex + path;
		data.texName = texName;
		data.dictReplace.Clear();
		data.tileW = data.tex.width / (int)pass.pmesh.tiling.x;
		data.tileH = data.tex.height / (int)pass.pmesh.tiling.y;
		this.texMap[data.id].listPass.Add(pass);
	}

	public void Add(MeshPass to, TextureData from, string texName = "_MainTex")
	{
		this.texMap[from.id].listPass.Add(to);
	}

	public void AddList(TextureData data, string path)
	{
		data.path = this.pathTex + path;
		this.texMap[data.id] = data;
	}

	public void RefreshTextures()
	{
		foreach (TextureData textureData in this.texMap.Values)
		{
			textureData.TryRefresh();
		}
		if (EClass.core.IsGameStarted && EClass._zone != null && EClass._zone.isStarted)
		{
			EClass.scene.ApplyZoneConfig();
		}
	}

	public void OnDropFile(List<string> paths)
	{
		int num = 0;
		foreach (string fileName in paths)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			if (this.TryAddReplace(fileInfo, TextureReplace.Source.User, false, false))
			{
				string text = CorePath.user + "/Texture Replace/" + fileInfo.Name;
				try
				{
					if (File.Exists(text))
					{
						File.Delete(text);
					}
					fileInfo.CopyTo(text, true);
				}
				catch (Exception)
				{
				}
				this.TryAddReplace(new FileInfo(text), TextureReplace.Source.User, true, true);
				num++;
			}
		}
		Msg.Say("Imported " + num.ToString() + "image(s)");
	}

	public bool TryAddReplace(FileInfo file, TextureReplace.Source source = TextureReplace.Source.Mod, bool add = true, bool refresh = false)
	{
		string[] array = Path.GetFileNameWithoutExtension(file.Name).Split('_', StringSplitOptions.None);
		if (array.Length == 1)
		{
			return false;
		}
		string key = array[0];
		int index;
		if (!int.TryParse(array[1], out index))
		{
			return false;
		}
		if (!this.texMap.ContainsKey(key))
		{
			return false;
		}
		TextureData textureData = this.texMap[key];
		if (textureData.listPass.Count == 0)
		{
			return false;
		}
		if (add)
		{
			textureData.AddReplace(new TextureReplace
			{
				file = file,
				index = index,
				data = textureData,
				source = source
			});
			if (refresh)
			{
				textureData.TryRefresh();
			}
		}
		return true;
	}

	public void ApplyLocalReplace(string path)
	{
		foreach (TextureData textureData in this.texMap.Values)
		{
			if (textureData.listReplaceLocal.Count > 0)
			{
				textureData.forceRefresh = true;
				foreach (TextureReplace textureReplace in textureData.listReplaceLocal)
				{
					textureReplace.DestoryTex();
					if (textureReplace.original != null)
					{
						textureData.dictReplace[textureReplace.index] = textureReplace.original;
					}
					else
					{
						textureData.dictReplace.Remove(textureReplace.index);
					}
				}
			}
			textureData.listReplaceLocal.Clear();
		}
		string path2 = path + "Texture Replace";
		if (Directory.Exists(path2))
		{
			foreach (FileInfo file in new DirectoryInfo(path2).GetFiles())
			{
				this.TryAddReplace(file, TextureReplace.Source.Local, true, false);
			}
		}
		this.RefreshTextures();
	}

	public Dictionary<string, TextureData> texMap = new Dictionary<string, TextureData>();
}
