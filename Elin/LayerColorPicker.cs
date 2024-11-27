using System;
using Empyrean.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

public class LayerColorPicker : ELayer
{
	public void SetColor(Color _startColor, Color _resetColor, Action<PickerState, Color> _onChangeColor)
	{
		UIItem t = this.layoutColors.CreateMold(null);
		for (int i = 0; i < 8; i++)
		{
			UIItem item = Util.Instantiate<UIItem>(t, this.layoutColors);
			int _i = i;
			item.button1.icon.color = IntColor.FromInt(ELayer.core.config.colors[_i]);
			item.button1.SetOnClick(delegate
			{
				this.picker.SelectColor(item.button1.icon.color);
			});
			item.button2.SetOnClick(delegate
			{
				item.button1.icon.color = this.picker.SelectedColor;
				ELayer.core.config.colors[_i] = IntColor.ToInt(this.picker.SelectedColor);
				SE.Tab();
			});
		}
		this.layoutColors.RebuildLayout(false);
		this.picker.ColorUpdated += delegate(Color c)
		{
			_onChangeColor(PickerState.Modify, c);
		};
		this.startColor = _startColor;
		this.resetColor = _resetColor;
		this.picker.Init();
		this.picker.SelectColor(_startColor);
		this.picker.SelectColor(_startColor);
		this.onChangeColor = _onChangeColor;
	}

	public void OnClickConfirm()
	{
		this.onChangeColor(PickerState.Confirm, this.picker.SelectedColor);
		this.Close();
	}

	public void OnClickCancel()
	{
		this.onChangeColor(PickerState.Cancel, this.startColor);
		this.Close();
	}

	public void OnClickReset()
	{
		this.picker.SelectColor(this.resetColor);
		this.onChangeColor(PickerState.Reset, this.resetColor);
	}

	public override bool OnBack()
	{
		if (this.picker.dropper.coroutine != null)
		{
			this.picker.dropper.Stop();
			this.picker.dropper.onDropCanceled();
			return false;
		}
		this.onChangeColor(PickerState.Cancel, this.startColor);
		return base.OnBack();
	}

	public override void OnKill()
	{
		if (this.picker.dropper.coroutine != null)
		{
			this.picker.dropper.Stop();
			this.picker.dropper.onDropCanceled();
		}
		base.OnKill();
		EInput.Consume(false, 1);
	}

	public ColorPicker picker;

	public Action<PickerState, Color> onChangeColor;

	public Color resetColor;

	public Color startColor;

	public GridLayoutGroup layoutColors;
}
