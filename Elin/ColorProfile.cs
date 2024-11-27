using System;
using UnityEngine;

public class ColorProfile : EScriptable
{
	public SkinColorProfile Skin
	{
		get
		{
			return SkinManager.CurrentColors;
		}
	}

	public SkinColorProfile Dark
	{
		get
		{
			return SkinManager.Instance.skinDark.colors._default;
		}
	}

	public SkinColorProfile Light
	{
		get
		{
			return SkinManager.Instance.skinLight.colors._default;
		}
	}

	public void Init()
	{
		this.blockColors.Init();
	}

	public Color GetRarityColor(int r, bool light = false)
	{
		ColorProfile.RarityColors rarityColors = this.rarityColors;
		Color color = rarityColors.colorC;
		switch (r)
		{
		case 2:
			color = rarityColors.colorR;
			break;
		case 3:
			color = rarityColors.colorSR;
			break;
		case 4:
			color = rarityColors.colorSSR;
			break;
		case 5:
			color = rarityColors.colorLE;
			break;
		}
		if (light)
		{
			color *= rarityColors.lightMod;
		}
		return color;
	}

	private void OnValidate()
	{
		if (!Application.isPlaying || !EClass.core.IsGameStarted)
		{
			return;
		}
		EClass._map.RefreshFOVAll();
	}

	public UD_String_MatData matColors;

	public UD_String_LightData lightColors;

	public UD_String_Color elementColors;

	public ColorProfile.RarityColors rarityColors;

	public ColorProfile.BlockColors blockColors;

	public ColorProfile.TextColors textColors;

	public Gradient gradientLVComparison;

	public ColorProfile.PCLights pcLights;

	public Color colorIngCost;

	public Color colorIngPredict;

	public Color colorIngReq;

	public Color colorNew;

	public Color colorAct;

	public Color colorActWarnning;

	public Color colorActCriticalWarning;

	public Color colorHostileAct;

	public Color colorFriend;

	public Color colorHostile;

	public Color colorBuff;

	public Color colorDebuff;

	[Serializable]
	public class RarityColors
	{
		public Color colorC;

		public Color colorR;

		public Color colorSR;

		public Color colorSSR;

		public Color colorLE;

		public float lightMod;
	}

	[Serializable]
	public class BlockColors
	{
		public void Init()
		{
			this.Valid = this.GetColor(ref this._valid);
			this.Warning = this.GetColor(ref this._warning);
			this.Inactive = this.GetColor(ref this._inactive);
			this.Active = this.GetColor(ref this._active);
			this.InactiveOpacity = this.GetColor(ref this._inactiveOpacity);
			this.ActiveOpacity = this.GetColor(ref this._activeOpacity);
			this.Passive = this.GetColor(ref this._passive);
			this.MapHighlight = this.GetColor(ref this._mapHighlight);
		}

		public int GetColor(ref Color c)
		{
			return -((int)(c.a * 10f) * 262144 + (int)(c.r * 64f) * 4096 + (int)(c.g * 64f) * 64 + (int)(c.b * 64f));
		}

		public const int DefColor = 104025;

		public const int DefColorInactive = -1214986;

		public const int DefColorActive = -1173751;

		public const int DefColorValid = -2747875;

		public const int DefColorWarning = -2828814;

		public Color _valid;

		public Color _warning;

		public Color _active;

		public Color _inactive;

		public Color _activeOpacity;

		public Color _inactiveOpacity;

		public Color _passive;

		public Color _mapHighlight;

		public int Valid;

		public int Warning;

		public int Inactive;

		public int Active;

		public int ActiveOpacity;

		public int InactiveOpacity;

		public int Passive;

		public int MapHighlight;
	}

	[Serializable]
	public class TextColors
	{
		public Color damage;

		public Color damagePC;

		public Color damagePCParty;

		public Color damageMana;

		public Color damageStamina;
	}

	[Serializable]
	public class PCLights
	{
		public LightData torch;

		public LightData cateye;
	}
}
