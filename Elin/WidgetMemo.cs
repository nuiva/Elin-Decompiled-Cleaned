using System;
using UnityEngine.UI;

public class WidgetMemo : Widget
{
	public override bool AlwaysBottom
	{
		get
		{
			return true;
		}
	}

	public override Type SetSiblingAfter
	{
		get
		{
			return typeof(WidgetSideScreen);
		}
	}

	public override void OnActivate()
	{
		this.input.text = EMono.player.memo;
		this.buttonEdit.SetOnClick(delegate
		{
			this.ToggleInput(!this.input.isFocused);
		});
		WidgetMemo.Instance = this;
	}

	public override void OnDeactivate()
	{
		EMono.player.memo = this.input.text;
	}

	public void ToggleInput(bool enable)
	{
		this.input.interactable = enable;
		this.bgInput.enabled = enable;
		this.textInput.raycastTarget = enable;
		this.buttonClose.SetActive(enable);
		if (enable)
		{
			this.input.Select();
		}
	}

	public override void OnUpdateConfig()
	{
		EMono.player.memo = this.input.text;
	}

	private void Update()
	{
		if (!this.input.isFocused)
		{
			if (this.input.interactable && !InputModuleEX.IsPointerChildOf(this))
			{
				this.ToggleInput(false);
				return;
			}
		}
		else if (!this.bgInput.enabled)
		{
			this.ToggleInput(true);
		}
	}

	public static WidgetMemo Instance;

	public InputField input;

	public Window window;

	public Image bgInput;

	public Text textInput;

	public UIButton buttonClose;

	public UIButton buttonEdit;
}
