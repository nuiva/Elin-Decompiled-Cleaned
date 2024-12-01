using System;
using Empyrean.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

public class LayerColorPicker : ELayer
{
	public ColorPicker picker;

	public Action<PickerState, Color> onChangeColor;

	public Color resetColor;

	public Color startColor;

	public GridLayoutGroup layoutColors;

	public void SetColor(Color _startColor, Color _resetColor, Action<PickerState, Color> _onChangeColor)
	{
		UIItem t = layoutColors.CreateMold<UIItem>();
		for (int i = 0; i < 8; i++)
		{
			UIItem item = Util.Instantiate(t, layoutColors);
			int _i = i;
			item.button1.icon.color = IntColor.FromInt(ELayer.core.config.colors[_i]);
			item.button1.SetOnClick(delegate
			{
				picker.SelectColor(item.button1.icon.color);
			});
			item.button2.SetOnClick(delegate
			{
				item.button1.icon.color = picker.SelectedColor;
				ELayer.core.config.colors[_i] = IntColor.ToInt(picker.SelectedColor);
				SE.Tab();
			});
		}
		layoutColors.RebuildLayout();
		picker.ColorUpdated += delegate(Color c)
		{
			_onChangeColor(PickerState.Modify, c);
		};
		startColor = _startColor;
		resetColor = _resetColor;
		picker.Init();
		picker.SelectColor(_startColor);
		picker.SelectColor(_startColor);
		onChangeColor = _onChangeColor;
	}

	public void OnClickConfirm()
	{
		onChangeColor(PickerState.Confirm, picker.SelectedColor);
		Close();
	}

	public void OnClickCancel()
	{
		onChangeColor(PickerState.Cancel, startColor);
		Close();
	}

	public void OnClickReset()
	{
		picker.SelectColor(resetColor);
		onChangeColor(PickerState.Reset, resetColor);
	}

	public override bool OnBack()
	{
		if (picker.dropper.coroutine != null)
		{
			picker.dropper.Stop();
			picker.dropper.onDropCanceled();
			return false;
		}
		onChangeColor(PickerState.Cancel, startColor);
		return base.OnBack();
	}

	public override void OnKill()
	{
		if (picker.dropper.coroutine != null)
		{
			picker.dropper.Stop();
			picker.dropper.onDropCanceled();
		}
		base.OnKill();
		EInput.Consume();
	}
}
