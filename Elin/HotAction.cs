using Newtonsoft.Json;
using UnityEngine;

public class HotAction : HotItem
{
	[JsonProperty]
	public int _bgColor;

	[JsonProperty]
	public string text;

	public Color bgColor
	{
		get
		{
			return IntColor.FromInt(_bgColor);
		}
		set
		{
			_bgColor = IntColor.ToInt(ref value);
		}
	}

	public override Color SpriteColor
	{
		get
		{
			if (!CanChangeIconColor || _bgColor == 0)
			{
				return base.SpriteColor;
			}
			return bgColor;
		}
	}

	public virtual string Id => "";

	public virtual bool CanChangeIconColor => false;

	public virtual bool CanName => true;

	public override string Name => text.IsEmpty(("hotAction" + Id).lang());

	public override string pathSprite => "icon_hot" + Id;

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (!EClass.player.CanAcceptInput())
		{
			SE.Beep();
		}
		else
		{
			Perform();
		}
	}

	public virtual void Perform()
	{
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		base.OnShowContextMenu(m);
		if (CanName)
		{
			m.AddButton("changeName", delegate
			{
				Dialog.InputName("dialogChangeName", text.IsEmpty(""), delegate(bool cancel, string t)
				{
					if (!cancel)
					{
						text = t;
					}
				});
			});
		}
		if (!CanChangeIconColor)
		{
			return;
		}
		m.AddButton("actChangeColor", delegate
		{
			if (_bgColor == 0)
			{
				bgColor = Color.white;
			}
			EClass.ui.AddLayer<LayerColorPicker>().SetColor(bgColor, Color.white, delegate(PickerState state, Color _c)
			{
				bgColor = _c;
				button.icon.color = _c;
			});
		});
	}
}
