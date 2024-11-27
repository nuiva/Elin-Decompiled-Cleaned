using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseStats : EClass
{
	public SourceStat.Row source
	{
		get
		{
			SourceStat.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = EClass.sources.stats.map[this.id]);
			}
			return result;
		}
	}

	public virtual Emo2 EmoIcon
	{
		get
		{
			return Emo2.none;
		}
	}

	public virtual Color GetColor(Gradient gradient)
	{
		return Color.white;
	}

	public virtual Color GetColor(SkinColorProfile c)
	{
		return this.GetColor(c.gradients[this.source.colors.IsEmpty("default")]);
	}

	public Color GetColor()
	{
		return this.GetColor(SkinManager.CurrentColors);
	}

	public virtual ConditionType Type
	{
		get
		{
			return this.source.group.ToEnum(true);
		}
	}

	public virtual string GetText()
	{
		return null;
	}

	public virtual string GetPhaseStr()
	{
		return this.GetText();
	}

	public virtual int GetValue()
	{
		return 0;
	}

	public virtual string idSprite
	{
		get
		{
			return this.source.element.IsEmpty(this.source.alias);
		}
	}

	public virtual Sprite GetSprite()
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", this.idSprite) ?? EClass.core.refs.spriteDefaultCondition;
	}

	public virtual void SetText(UIText t, SkinColorProfile cols = null)
	{
		if (cols == null)
		{
			cols = EClass.ui.skins.currentSkin.colors._default;
		}
		t.SetText(this.GetText(), this.GetColor(cols));
	}

	public virtual bool ShowInWidget
	{
		get
		{
			return true;
		}
	}

	public virtual int GetPhase()
	{
		return 0;
	}

	public virtual Chara Owner
	{
		get
		{
			return BaseStats.CC;
		}
	}

	public void PopText()
	{
		if (!EClass.core.IsGameStarted || !this.Owner.ShouldShowMsg)
		{
			return;
		}
		string phaseStr = this.GetPhaseStr();
		if (phaseStr.IsEmpty() || phaseStr == "#")
		{
			return;
		}
		Popper popper = EClass.scene.popper.Pop(this.Owner.renderer.PositionCenter(), "Condition");
		Color c = this.GetColor() * 1.3f;
		c.r += 0.3f;
		c.g += 0.3f;
		c.b += 0.3f;
		popper.SetText(phaseStr, c);
	}

	public virtual void WriteNote(UINote n, Action<UINote> onWriteNote = null)
	{
		n.Clear();
		n.AddHeader(this.source.GetName(), null);
		n.AddText("NoteText_flavor_element", this.source.GetDetail(), FontColor.DontChange);
		this._WriteNote(n, false);
		n.Build();
	}

	public virtual void _WriteNote(UINote n, bool asChild = false)
	{
		List<string> list = new List<string>();
		foreach (string key in this.source.nullify)
		{
			list.Add("hintNullify".lang(EClass.sources.stats.alias[key].GetName(), null, null, null, null));
		}
		if (list.Count > 0)
		{
			if (!asChild)
			{
				n.Space(8, 1);
			}
			foreach (string str in list)
			{
				n.AddText("_bullet".lang() + str, FontColor.DontChange);
			}
		}
	}

	public static Chara CC;

	[JsonProperty]
	public int id;

	public SourceStat.Row _source;
}
