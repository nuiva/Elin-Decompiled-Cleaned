using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Portrait : UIButton
{
	public static List<ModItem<Sprite>> ListPlayerPortraits(int gender, bool nullPortrait = false)
	{
		List<ModItem<Sprite>> list = Portrait.ListPortraits(gender, "c").Concat(Portrait.ListPortraits(gender, "guard")).Concat(Portrait.ListPortraits(gender, "special")).Concat(Portrait.ListPortraits(gender, "foxfolk")).ToList<ModItem<Sprite>>();
		if (nullPortrait)
		{
			list.Insert(0, new ModItem<Sprite>(null, null, null, null));
		}
		return list;
	}

	public static List<ModItem<Sprite>> ListPortraits(string idDict)
	{
		List<ModItem<Sprite>> list = Portrait.dictList.TryGetValue(idDict, null);
		if (list == null || list.Count == 0)
		{
			list = new List<ModItem<Sprite>>();
			foreach (ModItem<Sprite> modItem in Portrait.modPortraits.list)
			{
				if (modItem.id.Contains(idDict))
				{
					list.Add(modItem);
				}
			}
			Portrait.dictList.Add(idDict, list);
		}
		return list;
	}

	public static List<ModItem<Sprite>> ListPortraits(int gender, string cat)
	{
		if (cat.IsEmpty())
		{
			cat = "c";
		}
		string key = cat + gender.ToString();
		List<ModItem<Sprite>> list = Portrait.dictList.TryGetValue(key, null);
		if (list == null || list.Count == 0)
		{
			list = new List<ModItem<Sprite>>();
			foreach (ModItem<Sprite> modItem in Portrait.modPortraits.list)
			{
				string[] array = modItem.id.Split('-', StringSplitOptions.None)[0].Split('_', StringSplitOptions.None);
				if (!(array[0] != cat))
				{
					int num = (array.Length > 1) ? ((array[1] == "m") ? 2 : ((array[1] == "f") ? 1 : 0)) : 0;
					if (num == 0 || gender == 0 || gender == num)
					{
						list.Add(modItem);
					}
				}
			}
			Portrait.dictList.Add(key, list);
		}
		return list;
	}

	public static string GetRandomPortrait(string idDict)
	{
		return Portrait.ListPortraits(idDict).RandomItem<ModItem<Sprite>>().id;
	}

	public static string GetRandomPortrait(int gender, string cat)
	{
		return Portrait.ListPortraits(gender, cat).RandomItem<ModItem<Sprite>>().id;
	}

	public static bool Exists(string id)
	{
		return Portrait.modPortraits.dict.ContainsKey(id);
	}

	public void SetPerson(Person p)
	{
		if (p.hasChara)
		{
			this.SetChara(p.chara, null);
			return;
		}
		if (!p.idPortrait.IsEmpty())
		{
			Rand.SetSeed(p.uidChara);
			this.SetPortrait(p.idPortrait, PCCManager.current.GetBodySet("female").map["hair"].GetRandomColor());
			Rand.SetSeed(-1);
			return;
		}
		this.SetPortrait(p.source.portrait, default(Color));
	}

	public void SetChara(Chara c, PCCData pccData = null)
	{
		this.portrait.enabled = true;
		this.overlay.enabled = true;
		if (this.imageChara)
		{
			this.imageChara.sprite = c.GetSprite(0);
			this.imageChara.SetNativeSize();
			if (this.fixSpritePos)
			{
				this.imageChara.transform.localScale = (c.IsPCC ? new Vector3(1.3f, 1.4f, 1f) : new Vector3(1f, 1f, 1f)) * this.charaScale;
				this.imageChara.rectTransform.pivot = (c.IsPCC ? new Vector2(0.5f, 0.4f) : new Vector2(0.5f, 0.2f));
				this.imageChara.rectTransform.anchoredPosition = (c.IsPCC ? new Vector2(-25f, 20f) : new Vector2(-25f, 20f));
			}
		}
		if (this.imageFaith)
		{
			this.imageFaith.sprite = c.faith.GetSprite();
		}
		if (!c.GetIdPortrait().IsEmpty() && Portrait.modPortraits.GetItem(c.GetIdPortrait(), true) != null)
		{
			if (pccData == null && c.isChara)
			{
				pccData = c.Chara.pccData;
			}
			Color colorOverlay = (pccData != null) ? pccData.GetHairColor(true) : Color.white;
			this.overlay.enabled = true;
			if (pccData == null)
			{
				if (c.GetInt(105, null) != 0)
				{
					colorOverlay = IntColor.FromInt(c.GetInt(105, null));
				}
				else
				{
					Rand.SetSeed(c.uid);
					colorOverlay = PCCManager.current.GetBodySet("female").map["hair"].GetRandomColor();
					Rand.SetSeed(-1);
					if (c.id == "shojo")
					{
						this.overlay.enabled = false;
					}
				}
			}
			this.SetPortrait(c.GetIdPortrait(), colorOverlay);
			return;
		}
		if (this.hideIfNoPortrait)
		{
			this.portrait.enabled = false;
			this.overlay.enabled = false;
			return;
		}
		if (this.spriteNoPortrait)
		{
			this.SetPortrait(false, this.spriteNoPortrait, null, default(Color), null);
			return;
		}
		this.SetPortrait(false, c.GetSprite(0), null, default(Color), null);
		c.SetImage(this.portrait);
		this.portrait.transform.localScale = new Vector3(1f, 1f, 1f);
		this.portrait.preserveAspect = true;
		this.portrait.SetNativeSize();
		this.portrait.Rect().SetAnchor(0.5f, 0f, 0.5f, 0f);
		this.portrait.Rect().SetPivot(0.5f, 0f);
		this.portrait.Rect().anchoredPosition = new Vector2(0f, 0f);
		this.portrait.material = EClass.core.refs.matUIPortraitChara;
	}

	public void SetPortrait(string id, Color colorOverlay = default(Color))
	{
		Sprite @object = Portrait.modPortraits.GetItem(id, false).GetObject(null);
		Sprite object2 = Portrait.modOverlays.GetObject(id + "-overlay", null);
		Sprite spriteFull = (this.enableFull && this.imageFull) ? Portrait.modFull.GetObject(id + "-full", null) : null;
		this.SetPortrait(true, @object, object2, colorOverlay, spriteFull);
	}

	public void SetPortrait(bool isPortrait, Sprite spritePortrait, Sprite spriteOverlay = null, Color colorOverlay = default(Color), Sprite spriteFull = null)
	{
		Portrait.<>c__DisplayClass29_0 CS$<>8__locals1;
		CS$<>8__locals1.isPortrait = isPortrait;
		CS$<>8__locals1.<>4__this = this;
		this.filter = ((CS$<>8__locals1.isPortrait && Core.Instance.config.test.aaPortrait) ? FilterMode.Bilinear : FilterMode.Point);
		this.portrait.rectTransform.anchorMin = this.overlay.rectTransform.anchorMin;
		this.portrait.rectTransform.anchorMax = this.overlay.rectTransform.anchorMax;
		this.portrait.rectTransform.sizeDelta = this.overlay.rectTransform.sizeDelta;
		if (spritePortrait)
		{
			this.portrait.sprite = spritePortrait;
			this.<SetPortrait>g__FixTexture|29_0(spritePortrait, ref CS$<>8__locals1);
		}
		if (this.overlay)
		{
			this.<SetPortrait>g__FixTexture|29_0(spriteOverlay, ref CS$<>8__locals1);
			this.overlay.sprite = spriteOverlay;
			this.overlay.color = new Color(colorOverlay.r * 1.1f, colorOverlay.g * 1.1f, colorOverlay.b * 1.1f, colorOverlay.a);
			this.overlay.SetActive(spriteOverlay);
		}
		if (this.enableFull && this.imageFull)
		{
			this.<SetPortrait>g__FixTexture|29_0(spriteFull, ref CS$<>8__locals1);
			this.imageFull.sprite = spriteFull;
			base.gameObject.SetActive(!spriteFull);
			this.imageFull.SetActive(spriteFull);
		}
		if (!this.showPortrait)
		{
			this.portrait.SetActive(false);
			this.overlay.SetActive(false);
		}
	}

	[CompilerGenerated]
	private void <SetPortrait>g__FixTexture|29_0(Sprite s, ref Portrait.<>c__DisplayClass29_0 A_2)
	{
		if (!s || !A_2.isPortrait)
		{
			return;
		}
		s.texture.wrapMode = TextureWrapMode.Clamp;
		if (s.texture.filterMode != this.filter)
		{
			s.texture.filterMode = this.filter;
		}
	}

	public static ModItemList<Sprite> modPortraitBGFs = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modPortraitBGs = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modPortraits = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modOverlays = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modFull = new ModItemList<Sprite>(3);

	public static Dictionary<string, List<ModItem<Sprite>>> dictList = new Dictionary<string, List<ModItem<Sprite>>>();

	public static HashSet<string> allIds = new HashSet<string>();

	public Image portrait;

	public Image overlay;

	public Image imageChara;

	public Image imageFaith;

	public Image imageFull;

	public Image imageFrame;

	public Sprite spriteNoPortrait;

	public bool enableFull = true;

	public bool hideIfNoPortrait;

	public bool fixSpritePos = true;

	public bool showPortrait = true;

	public float charaScale = 1f;

	private FilterMode filter;
}
