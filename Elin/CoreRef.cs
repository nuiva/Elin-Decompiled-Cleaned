using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoreRef : ScriptableObject
{
	public void Init()
	{
		if (this.bgms.Count == 0)
		{
			this.RebuildBGMList();
		}
		this.RefreshBGM();
	}

	public void RefreshBGM()
	{
		this.dictBGM = new Dictionary<int, BGMData>();
		int num = 0;
		foreach (BGMData bgmdata in this.bgms)
		{
			this.dictBGM.Add(bgmdata.id, bgmdata);
			num++;
		}
	}

	public void RebuildBGMList()
	{
		this.bgms.Clear();
		BGMData[] array = Resources.LoadAll<BGMData>("Media/Sound/BGM/");
		List<AudioClip> list = Resources.LoadAll<AudioClip>("Media/Sound/BGM/").ToList<AudioClip>();
		foreach (BGMData bgmdata in array)
		{
			if (bgmdata.id != 0)
			{
				this.bgms.Add(bgmdata);
				foreach (AudioClip audioClip in list)
				{
					if (bgmdata.clip == audioClip)
					{
						list.Remove(audioClip);
						break;
					}
				}
			}
		}
		foreach (AudioClip audioClip2 in list)
		{
			Debug.Log("Unused:" + audioClip2.name);
		}
		this.RefreshBGM();
		Debug.Log("Rebuild BGM Done.");
	}

	public void RebuildSketchList()
	{
		this.dictSketches.Clear();
		for (int i = 0; i < 10; i++)
		{
			foreach (Sprite sprite in Resources.LoadAll<Sprite>("Media/Gallery/" + CoreRef.GetArtDir(i * 100)))
			{
				int key = sprite.name.Split('_', StringSplitOptions.None)[0].ToInt();
				this.dictSketches[key] = sprite.name;
				Debug.Log(key.ToString() + " " + sprite.name);
			}
		}
		Debug.Log("Sketches rebuilt:" + this.dictSketches.Count<KeyValuePair<int, string>>().ToString());
	}

	public static string GetArtDir(int a)
	{
		return CoreRef.ArtDirs[a / 100];
	}

	public void RebuildBiomeList()
	{
		this.biomes.dict.Clear();
		BiomeProfile[] array = Resources.LoadAll<BiomeProfile>("World/Map/Biome/");
		foreach (BiomeProfile biomeProfile in array)
		{
			this.biomes.dict.Add(biomeProfile.name, biomeProfile);
			Debug.Log(biomeProfile.name);
		}
		Debug.Log("Biomes rebuilt:" + array.Length.ToString());
	}

	public void RebuildFireworks()
	{
		this.fireworks.Clear();
		Effect[] array = Resources.LoadAll<Effect>("Media/Effect/General/Firework/");
		foreach (Effect effect in array)
		{
			this.fireworks.Add(effect);
			Debug.Log(effect.name);
		}
		Debug.Log("Fireworks rebuilt:" + array.Length.ToString());
	}

	public CoreRef.Biomes biomes;

	public CoreRef.Crawlers crawlers;

	public CoreRef.StateIcons stateIcons;

	public CoreRef.OrbitIcons orbitIcons;

	public CoreRef.ButtonAssets buttonAssets;

	public CoreRef.PopperSprites popperSprites;

	public CoreRef.TCs tcs;

	public CoreRef.Icons icons;

	public CoreRef.Renderers renderers;

	public CoreRef.TextureDatas textureData;

	public CoreRef.Rects rects;

	public List<Effect> fireworks;

	public List<Sprite> spritesCorner;

	public List<Sprite> icon_HotItem;

	public List<SpriteAsset> bg_msg;

	public List<Sprite> spritesHighlight;

	public List<Sprite> spritesHighlightSpeed;

	public List<Sprite> spritesPotential;

	public List<Sprite> spritesEmo;

	public List<Sprite> spritesContainerIcon;

	public List<BGMData> bgms;

	public List<CoreRef.DefaultRoof> defaultRoofs;

	public Dictionary<int, BGMData> dictBGM;

	public CoreRef.UDInvStyle invStyle;

	public Sprite spriteRecipe;

	public Sprite spriteNull;

	public Sprite spriteArea;

	public Sprite spriteNoIng;

	public Sprite spriteButtonGrid;

	public Sprite spriteButtonGridBad;

	public Sprite spriteThingActor;

	public Sprite spriteDefaultCondition;

	public Material matUIObj;

	public Material matUIPortraitChara;

	public MsgColors msgColors;

	public GameObject debugText;

	public UD_Int_String dictSketches;

	public float testColor;

	public float testColor2;

	public static string[] ArtDirs = new string[]
	{
		"000-099",
		"100-199 Elin",
		"200-299 Elin Chara",
		"300-399 Ylva Illust",
		"400-499 Ylva Other",
		"500-599 Ylva Wall",
		"600-699 Kickstarter",
		"700-799 Goods",
		"800-899 Etc",
		"900-999 MT"
	};

	[Serializable]
	public class UDIconWeather : UDictionary<Weather.Condition, Sprite>
	{
	}

	[Serializable]
	public class UDInvTab : UDictionary<UIInventory.Mode, Sprite>
	{
	}

	[Serializable]
	public class UDInvStyle : UDictionary<string, CoreRef.InventoryStyle>
	{
	}

	[Serializable]
	public class InventoryStyle
	{
		public Sprite bg;

		public Vector2 sizeDelta;

		public Vector2 gridSize;

		public Vector2 sizeContainer;

		public Vector2 posFix;

		public Color gridColor;

		public SoundData sound;
	}

	[Serializable]
	public class Icons
	{
		public Sprite suspend;

		public Sprite resume;

		public Sprite delete;

		public Sprite go;

		public Sprite talk;

		public Sprite caste;

		public Sprite home;

		public Sprite work;

		public Sprite uniformM;

		public Sprite uniformF;

		public Sprite inspect;

		public Sprite up;

		public Sprite down;

		public Sprite trash;

		public Sprite trans;

		public Sprite noHotItem;

		public Sprite defaultHotItem;

		public Sprite defaultAbility;

		public Sprite targetSelf;

		public Sprite targetAny;

		public Sprite mana;

		public Sprite stamina;

		public Sprite shared;

		public Sprite personal;

		public CoreRef.UDInvTab invTab;

		public CoreRef.UDIconWeather weather;

		public List<Sprite> quality;
	}

	[Serializable]
	public class Biomes
	{
		public UD_Biome dict;

		public BiomeProfile Plain;

		public BiomeProfile Sand;

		public BiomeProfile Water;
	}

	[Serializable]
	public class Rects
	{
		public RectData bottomRight;

		public RectData inv;

		public RectData invFloat;

		public RectData invCenter;

		public RectData center;

		public RectData centerFloat;

		public RectData abilityDebug;
	}

	[Serializable]
	public class StateIcons
	{
		public MultiSprite combat;

		public MultiSprite sleep;

		public MultiSprite gather;

		public MultiSprite meditation;

		public MultiSprite selfharm;
	}

	[Serializable]
	public class PopperSprites
	{
		public Sprite[] damage;
	}

	[Serializable]
	public class OrbitIcons
	{
		public Sprite Default;

		public Sprite Search;

		public Sprite Hostile;

		public Sprite Guest;

		public Sprite healthCritical;

		public Sprite healthDanger;

		public Sprite healthLow;
	}

	[Serializable]
	public class Crawlers
	{
		public Crawler start;
	}

	[Serializable]
	public class ButtonAssets
	{
		public Sprite bgDefault;

		public Sprite bgNew;

		public Sprite bgSuperior;

		public Sprite bgLegendary;

		public Sprite bgMythical;

		public Sprite bgArtifact;
	}

	[Serializable]
	public class TCs
	{
		public TCText text;

		public TCCensored censored;

		public TCState state;

		public TCSimpleText simpleText;

		public TCSimpleText simpleTextIcon;

		public Sprite spriteVisited;

		public Sprite spriteConquer;

		public Sprite spriteDeath;
	}

	[Serializable]
	public class TextureDatas
	{
		public TextureData block;

		public TextureData block_snow;

		public TextureData floor;

		public TextureData floor_snow;

		public TextureData objs;

		public TextureData objs_snow;

		public TextureData objs_S;

		public TextureData objs_S_snow;

		public TextureData objs_L;

		public TextureData objs_L_snow;

		public TextureData objs_C;

		public TextureData objs_CLL;

		public TextureData shadows;

		public TextureData fov;

		public TextureData world;

		public TextureData roofs;

		public TextureData objs_SS;

		public TextureData bird;
	}

	[Serializable]
	public class Renderers
	{
		public RenderData pcc;

		public RenderData pcc_L;

		public RenderData obj_wheat;

		public RenderData objS_flat;

		public RenderData objL_harvest;

		public RenderData obj_paint;

		public RenderData objs_shrine;
	}

	[Serializable]
	public class DefaultRoof
	{
		public int idBlock;

		public int idRamp;
	}
}
