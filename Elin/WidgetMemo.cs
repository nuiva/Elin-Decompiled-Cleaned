using System;
using UnityEngine.UI;

public class WidgetMemo : Widget
{
	public static WidgetMemo Instance;

	public InputField input;

	public Window window;

	public Image bgInput;

	public Text textInput;

	public UIButton buttonClose;

	public UIButton buttonEdit;

	public override bool AlwaysBottom => true;

	public override Type SetSiblingAfter => typeof(WidgetSideScreen);

	public override void OnActivate()
	{
		input.text = EMono.player.memo;
		buttonEdit.SetOnClick(delegate
		{
			ToggleInput(!input.isFocused);
		});
		Instance = this;
	}

	public override void OnDeactivate()
	{
		EMono.player.memo = input.text;
	}

	public void ToggleInput(bool enable)
	{
		input.interactable = enable;
		bgInput.enabled = enable;
		textInput.raycastTarget = enable;
		buttonClose.SetActive(enable);
		if (enable)
		{
			input.Select();
		}
	}

	public override void OnUpdateConfig()
	{
		EMono.player.memo = input.text;
	}

	private void Update()
	{
		if (!input.isFocused)
		{
			if (input.interactable && !InputModuleEX.IsPointerChildOf(this))
			{
				ToggleInput(enable: false);
			}
		}
		else if (!bgInput.enabled)
		{
			ToggleInput(enable: true);
		}
	}
}
