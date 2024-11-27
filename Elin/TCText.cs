using System;
using UnityEngine;
using UnityEngine.UI;

public class TCText : TCUI
{
	public override Vector3 FixPos
	{
		get
		{
			return TC._setting.textPos;
		}
	}

	public void Say(string s, float duration = 0f)
	{
		char c = s[0];
		PopItem p;
		if (c <= '*')
		{
			if (c == '(')
			{
				p = this.pop.PopText(s, null, EMono.core.config.ui.balloonBG ? "PopText_alt" : "PopTextThinking", default(Color), default(Vector3), duration);
				goto IL_17F;
			}
			if (c == '*')
			{
				p = this.pop.PopText(s, null, "PopTextOno", default(Color), default(Vector3), duration);
				goto IL_17F;
			}
		}
		else
		{
			if (c == '@')
			{
				int num = int.Parse(s[1].ToString());
				p = this.pop.PopText(s.Substring(2), null, TCText.popIDs[num], default(Color), default(Vector3), duration);
				goto IL_17F;
			}
			if (c == '^')
			{
				p = this.pop.PopText(s.Substring(1), null, "PopTextBroadcast", default(Color), default(Vector3), duration);
				goto IL_17F;
			}
		}
		p = this.pop.PopText(s, null, EMono.core.config.ui.balloonBG ? "PopText_alt" : "PopText", default(Color), default(Vector3), duration);
		IL_17F:
		if (p)
		{
			EMono.core.actionsNextFrame.Add(delegate
			{
				if (p != null && p.gameObject != null)
				{
					p.RebuildLayout(true);
					ContentSizeFitter[] componentsInChildren = p.GetComponentsInChildren<ContentSizeFitter>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].enabled = false;
					}
					LayoutGroup[] componentsInChildren2 = p.GetComponentsInChildren<LayoutGroup>();
					for (int i = 0; i < componentsInChildren2.Length; i++)
					{
						componentsInChildren2[i].enabled = false;
					}
				}
			});
		}
	}

	public void ShowEmo(Emo emo, float duration)
	{
		Sprite sprite = SpriteSheet.Get("Media/Graphics/Icon/icons_32", "emo_" + emo.ToString());
		if (this.lastEmo != null)
		{
			this.pop.Kill(this.lastEmo, false);
		}
		this.lastEmo = this.pop.PopText("", sprite, "PopTextEmo", default(Color), default(Vector3), duration);
	}

	public override void OnDraw(ref Vector3 pos)
	{
		if (!this.pop.enabled)
		{
			this.render.RemoveTC(this);
			return;
		}
		Vector3 vector = pos;
		this.lastPos = pos;
		base.OnDraw(ref vector);
	}

	public override void OnKill()
	{
		base.DrawImmediate(ref this.lastPos);
		this.pop.CopyAll(EMono.ui.rectDynamic);
		this.pop.KillAll(true);
	}

	public static string[] popIDs = new string[]
	{
		"PopTextSys",
		"PopTextGod"
	};

	public PopManager pop;

	[NonSerialized]
	public PopItemText lastEmo;
}
