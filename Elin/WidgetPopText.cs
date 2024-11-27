using System;
using UnityEngine;

public class WidgetPopText : Widget
{
	public override void OnActivate()
	{
		WidgetPopText.Instance = this;
	}

	public static void SayPick(Thing t, int num)
	{
		if (!WidgetPopText.Instance)
		{
			return;
		}
		WidgetPopText.Instance._SayPick(t, num);
	}

	public static void SayValue(string name, int a, bool negative = false, Sprite sprite = null)
	{
		if (!WidgetPopText.Instance)
		{
			return;
		}
		WidgetPopText.Instance._SayValue(name, a, negative, sprite);
	}

	public static void Say(string text, FontColor fontColor = FontColor.Default, Sprite sprite = null)
	{
		if (!WidgetPopText.Instance)
		{
			return;
		}
		WidgetPopText.Instance._Say(text, fontColor, sprite);
	}

	public void _SayPick(Thing t, int num)
	{
		PopItemText popItemText = this.pop.PopText("notifyAddThing".lang(t.GetName(NameStyle.Full, num), t.Num.ToString() ?? "", null, null, null), null, "PopNotification", default(Color), default(Vector3), 0f);
		t.SetImage(popItemText.image);
		popItemText.image.SetActive(true);
	}

	public void _SayValue(string name, int a, bool negative = false, Sprite sprite = null)
	{
		WidgetPopText.Say(name + " " + ((a > 0) ? "+" : "") + a.ToString(), negative ? FontColor.Bad : FontColor.Default, sprite);
	}

	public void _Say(string text, FontColor fontColor = FontColor.Default, Sprite sprite = null)
	{
		PopItemText popItemText = this.pop.PopText(text, sprite ?? this.icons[0], "PopNotification", default(Color), default(Vector3), 0f);
		if (fontColor != FontColor.Default)
		{
			popItemText.text.color = SkinManager.Instance.skinDark.Colors.GetTextColor(fontColor);
		}
	}

	public static WidgetPopText Instance;

	public PopManager pop;

	public Sprite[] icons;
}
