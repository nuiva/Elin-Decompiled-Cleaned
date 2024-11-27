using System;
using System.Collections.Generic;
using UnityEngine;

public class AM_Paint : AM_BaseTileSelect
{
	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.None;
		}
	}

	public override bool ShowActionHint
	{
		get
		{
			return false;
		}
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return false;
		}
	}

	public override bool enableMouseInfo
	{
		get
		{
			return false;
		}
	}

	public override bool ShowMaskedThings
	{
		get
		{
			return false;
		}
	}

	public override void OnActivate()
	{
		this.srRect = Util.Instantiate<SpriteRenderer>("Media/Graphics/paintRect", null);
		this.completed = false;
		Msg.Say((this.painter.PaintType == TraitPainter.Type.Camera) ? "askPhoto" : "askPaint");
	}

	public override void OnDeactivate()
	{
		UnityEngine.Object.Destroy(this.srRect.gameObject);
	}

	public void SetPainter(TraitPainter p)
	{
		this.painter = p;
		base.Activate(true, false);
	}

	public override void OnUpdateInput()
	{
		if (this.completed)
		{
			return;
		}
		bool flag = true;
		TraitCanvas canvas = this.painter.GetCanvas();
		if (EClass.ui.isPointerOverUI)
		{
			flag = false;
		}
		this.srRect.color = (flag ? Color.green : Color.red);
		Vector3 mousePosition = Input.mousePosition;
		this.srRect.transform.position = Camera.main.ScreenToWorldPoint(mousePosition).SetZ(-100f);
		float num = 0.02f / EClass.screen.Zoom;
		Sprite sprite = canvas.owner.GetSprite(0);
		this.srRect.transform.localScale = new Vector3((float)sprite.texture.width * num, (float)sprite.texture.height * num, 1f);
		if (EInput.leftMouse.clicked)
		{
			if (!flag)
			{
				SE.Beep();
				return;
			}
			this.srRect.enabled = false;
			this.completed = true;
			Action <>9__1;
			EClass.core.actionsNextFrame.Add(delegate
			{
				List<Action> actionsNextFrame = EClass.core.actionsNextFrame;
				Action item;
				if ((item = <>9__1) == null)
				{
					item = (<>9__1 = delegate()
					{
						ActionMode.DefaultMode.Activate(true, false);
						EClass.pc.SetAI(new AI_Paint
						{
							painter = this.painter,
							canvas = canvas,
							data = canvas.owner.GetPaintData()
						});
					});
				}
				actionsNextFrame.Add(item);
			});
		}
	}

	public SpriteRenderer srRect;

	public TraitPainter painter;

	private bool completed;
}
