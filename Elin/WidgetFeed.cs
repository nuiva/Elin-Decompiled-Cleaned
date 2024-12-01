using UnityEngine;

public class WidgetFeed : Widget
{
	public class Extra
	{
		public bool showChat;

		public bool intercept;

		public int style;
	}

	public static WidgetFeed Instance;

	public GameObject goDummy;

	public PopManager pop;

	public string[] idPop;

	public string idPopGod;

	public string idPopSystem;

	public static bool Intercept
	{
		get
		{
			if ((bool)Instance)
			{
				return Instance.extra.intercept;
			}
			return false;
		}
	}

	public Extra extra => base.config.extra as Extra;

	public override bool AlwaysTop => true;

	public override bool ShowStyleMenu => false;

	public bool IgnoreFeed
	{
		get
		{
			if ((bool)WidgetMainText.Instance)
			{
				return WidgetMainText.Instance.box.isShowingLog;
			}
			return false;
		}
	}

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Instance = this;
		System("Elin's Inn version " + EMono.core.version.GetText());
		System("Welcome, wanderer! Press '?' for help.");
		WidgetMainText.Refresh();
	}

	public override void OnDeactivate()
	{
		Instance = null;
		WidgetMainText.Refresh();
	}

	public void Talk(Card c, string id)
	{
		string text = GameLang.Convert(c.GetTalkText(id, stripPun: true));
		SayRaw(c, text);
	}

	public PopItem SayRaw(Card c, string text)
	{
		PopItemText popItemText = pop.PopText(text, null, idPop[extra.style]);
		popItemText.GetComponentInChildren<Portrait>().SetChara(c as Chara);
		return popItemText;
	}

	public void SayRaw(string idPortrait, string text, string _idPop)
	{
		pop.PopText(text, null, _idPop.IsEmpty(idPop[extra.style])).GetComponentInChildren<Portrait>().SetPortrait(idPortrait);
	}

	public void Nerun(string text, string idPortrait = "UN_nerun")
	{
		SayRaw(idPortrait, text, null);
	}

	public void System(string text)
	{
		if (extra.intercept)
		{
			pop.PopText(text, null, idPopSystem, Msg.currentColor);
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddSlider("style", (float a) => idPop[(int)a], extra.style, delegate(float a)
		{
			extra.style = (int)a;
			Msg.Nerun("this");
		}, 0f, idPop.Length - 1, isInt: true);
		SetBaseContextMenu(m);
	}
}
