using UnityEngine;

public class WidgetPopText : Widget
{
	public static WidgetPopText Instance;

	public PopManager pop;

	public Sprite[] icons;

	public override void OnActivate()
	{
		Instance = this;
	}

	public static void SayPick(Thing t, int num)
	{
		if ((bool)Instance)
		{
			Instance._SayPick(t, num);
		}
	}

	public static void SayValue(string name, int a, bool negative = false, Sprite sprite = null)
	{
		if ((bool)Instance)
		{
			Instance._SayValue(name, a, negative, sprite);
		}
	}

	public static void Say(string text, FontColor fontColor = FontColor.Default, Sprite sprite = null)
	{
		if ((bool)Instance)
		{
			Instance._Say(text, fontColor, sprite);
		}
	}

	public void _SayPick(Thing t, int num)
	{
		PopItemText popItemText = pop.PopText("notifyAddThing".lang(t.GetName(NameStyle.Full, num), t.Num.ToString() ?? ""), null, "PopNotification");
		t.SetImage(popItemText.image);
		popItemText.image.SetActive(enable: true);
	}

	public void _SayValue(string name, int a, bool negative = false, Sprite sprite = null)
	{
		Say(name + " " + ((a > 0) ? "+" : "") + a, (!negative) ? FontColor.Default : FontColor.Bad, sprite);
	}

	public void _Say(string text, FontColor fontColor = FontColor.Default, Sprite sprite = null)
	{
		PopItemText popItemText = pop.PopText(text, sprite ?? icons[0], "PopNotification");
		if (fontColor != FontColor.Default)
		{
			popItemText.text.color = SkinManager.Instance.skinDark.Colors.GetTextColor(fontColor);
		}
	}
}
