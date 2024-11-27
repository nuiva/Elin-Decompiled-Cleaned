using System;
using Newtonsoft.Json;
using UnityEngine;

public class HotAction : HotItem
{
	public Color bgColor
	{
		get
		{
			return IntColor.FromInt(this._bgColor);
		}
		set
		{
			this._bgColor = IntColor.ToInt(ref value);
		}
	}

	public override Color SpriteColor
	{
		get
		{
			if (!this.CanChangeIconColor || this._bgColor == 0)
			{
				return base.SpriteColor;
			}
			return this.bgColor;
		}
	}

	public virtual string Id
	{
		get
		{
			return "";
		}
	}

	public virtual bool CanChangeIconColor
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanName
	{
		get
		{
			return true;
		}
	}

	public override string Name
	{
		get
		{
			return this.text.IsEmpty(("hotAction" + this.Id).lang());
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_hot" + this.Id;
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (!EClass.player.CanAcceptInput())
		{
			SE.Beep();
			return;
		}
		this.Perform();
	}

	public virtual void Perform()
	{
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		base.OnShowContextMenu(m);
		if (this.CanName)
		{
			m.AddButton("changeName", delegate()
			{
				Dialog.InputName("dialogChangeName", this.text.IsEmpty(""), delegate(bool cancel, string t)
				{
					if (!cancel)
					{
						this.text = t;
					}
				}, Dialog.InputType.Default);
			}, true);
		}
		if (this.CanChangeIconColor)
		{
			m.AddButton("actChangeColor", delegate()
			{
				if (this._bgColor == 0)
				{
					this.bgColor = Color.white;
				}
				EClass.ui.AddLayer<LayerColorPicker>().SetColor(this.bgColor, Color.white, delegate(PickerState state, Color _c)
				{
					this.bgColor = _c;
					this.button.icon.color = _c;
				});
			}, true);
		}
	}

	[JsonProperty]
	public int _bgColor;

	[JsonProperty]
	public string text;
}
