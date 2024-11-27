using System;
using UnityEngine;

public class WidgetFeed : Widget
{
	public static bool Intercept
	{
		get
		{
			return WidgetFeed.Instance && WidgetFeed.Instance.extra.intercept;
		}
	}

	public override object CreateExtra()
	{
		return new WidgetFeed.Extra();
	}

	public WidgetFeed.Extra extra
	{
		get
		{
			return base.config.extra as WidgetFeed.Extra;
		}
	}

	public override bool AlwaysTop
	{
		get
		{
			return true;
		}
	}

	public override bool ShowStyleMenu
	{
		get
		{
			return false;
		}
	}

	public bool IgnoreFeed
	{
		get
		{
			return WidgetMainText.Instance && WidgetMainText.Instance.box.isShowingLog;
		}
	}

	public override void OnActivate()
	{
		WidgetFeed.Instance = this;
		this.System("Elin's Inn version " + EMono.core.version.GetText());
		this.System("Welcome, wanderer! Press '?' for help.");
		WidgetMainText.Refresh();
	}

	public override void OnDeactivate()
	{
		WidgetFeed.Instance = null;
		WidgetMainText.Refresh();
	}

	public void Talk(Card c, string id)
	{
		string text = GameLang.Convert(c.GetTalkText(id, true, true));
		this.SayRaw(c, text);
	}

	public PopItem SayRaw(Card c, string text)
	{
		PopItemText popItemText = this.pop.PopText(text, null, this.idPop[this.extra.style], default(Color), default(Vector3), 0f);
		popItemText.GetComponentInChildren<Portrait>().SetChara(c as Chara, null);
		return popItemText;
	}

	public void SayRaw(string idPortrait, string text, string _idPop)
	{
		this.pop.PopText(text, null, _idPop.IsEmpty(this.idPop[this.extra.style]), default(Color), default(Vector3), 0f).GetComponentInChildren<Portrait>().SetPortrait(idPortrait, default(Color));
	}

	public void Nerun(string text, string idPortrait = "UN_nerun")
	{
		this.SayRaw(idPortrait, text, null);
	}

	public void System(string text)
	{
		if (this.extra.intercept)
		{
			this.pop.PopText(text, null, this.idPopSystem, Msg.currentColor, default(Vector3), 0f);
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddSlider("style", (float a) => this.idPop[(int)a], (float)this.extra.style, delegate(float a)
		{
			this.extra.style = (int)a;
			Msg.Nerun("this", "UN_nerun");
		}, 0f, (float)(this.idPop.Length - 1), true, true, false);
		base.SetBaseContextMenu(m);
	}

	public static WidgetFeed Instance;

	public GameObject goDummy;

	public PopManager pop;

	public string[] idPop;

	public string idPopGod;

	public string idPopSystem;

	public class Extra
	{
		public bool showChat;

		public bool intercept;

		public int style;
	}
}
