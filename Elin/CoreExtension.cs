using System;
using UnityEngine;

public static class CoreExtension
{
	public static Color GetFixedColor(Color c, bool dark)
	{
		float num = 0.5f;
		SkinColorProfile @default = (dark ? SkinManager.Instance.skinDark.colors : SkinManager.CurrentSkin.colors)._default;
		float num2 = 1f + @default.contrast;
		num = ((!(c.r + c.g + c.b > 1.5f)) ? (0.5f - @default.strength) : (0.5f + @default.strength));
		c.r = Mathf.Clamp((c.r - 0.5f) * num2 + num, 0f, 1f);
		c.g = Mathf.Clamp((c.g - 0.5f) * num2 + num, 0f, 1f);
		c.b = Mathf.Clamp((c.b - 0.5f) * num2 + num, 0f, 1f);
		return c;
	}

	public static string TagColorGoodBad(this string s, Func<bool> funcGood, bool dark = false)
	{
		SkinColorProfile @default = (dark ? SkinManager.Instance.skinDark.colors : SkinManager.CurrentSkin.colors)._default;
		return "<color=" + GetFixedColor(funcGood() ? @default.textGood : @default.textBad, dark).ToHex() + ">" + s + "</color>";
	}

	public static string TagColorGoodBad(this string s, Func<bool> funcGood, Func<bool> funcBad, bool dark = false)
	{
		SkinColorProfile @default = (dark ? SkinManager.Instance.skinDark.colors : SkinManager.CurrentSkin.colors)._default;
		bool flag = funcGood();
		bool flag2 = funcBad();
		if (!flag && !flag2)
		{
			return s;
		}
		return "<color=" + GetFixedColor(flag ? @default.textGood : @default.textBad, dark).ToHex() + ">" + s + "</color>";
	}

	public static UICurrency AttachCurrency(this Window window)
	{
		return Util.Instantiate<UICurrency>("UI/Util/UICurrency", window.transform);
	}

	public static string TagColor(this string s, FontColor c, SkinColorProfile colors = null)
	{
		return s.TagColor((colors ?? SkinManager.CurrentColors).GetTextColor(c));
	}

	public static string TagColor(this string text, Func<bool> funcGood, SkinColorProfile colors = null)
	{
		SkinColorProfile skinColorProfile = colors ?? SkinManager.CurrentColors;
		return text.TagColor(funcGood() ? skinColorProfile.textGood : skinColorProfile.textBad);
	}

	public static string TagColor(this string text, Func<bool> funcGood, Func<bool> funcBad, SkinColorProfile colors = null)
	{
		SkinColorProfile skinColorProfile = colors ?? SkinManager.CurrentColors;
		return text.TagColor(funcGood() ? skinColorProfile.textGood : ((funcBad != null && funcBad()) ? skinColorProfile.textBad : skinColorProfile.textDefault));
	}
}
