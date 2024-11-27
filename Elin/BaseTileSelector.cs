using System;
using UnityEngine;

public class BaseTileSelector : EMono
{
	public ActionMode mode
	{
		get
		{
			return EMono.scene.actionMode;
		}
	}

	public BaseTileSelector.SelectType selectType
	{
		get
		{
			return this.mode.selectType;
		}
	}

	public BaseTileSelector.HitType hitType
	{
		get
		{
			return this.mode.hitType;
		}
	}

	public BaseTileSelector.BoxType boxType
	{
		get
		{
			return this.mode.boxType;
		}
	}

	public int hitW
	{
		get
		{
			return this.mode.hitW;
		}
	}

	public int hitH
	{
		get
		{
			return this.mode.hitH;
		}
	}

	public bool multisize
	{
		get
		{
			return this.hitW != 1 || this.hitH != 1;
		}
	}

	public int Width
	{
		get
		{
			if (!Scene.HitPoint.IsValid)
			{
				return 0;
			}
			if (this.start != null)
			{
				return Mathf.Abs(Scene.HitPoint.x - this.start.x) + 1;
			}
			return 1;
		}
	}

	public int Height
	{
		get
		{
			if (!Scene.HitPoint.IsValid)
			{
				return 0;
			}
			if (this.start != null)
			{
				return Mathf.Abs(Scene.HitPoint.z - this.start.z) + 1;
			}
			return 1;
		}
	}

	public void OnChangeActionMode()
	{
		this.start = null;
		this.RefreshMouseInfo(true);
	}

	public virtual void OnRenderTile(Point point, HitResult result, bool shouldHide)
	{
	}

	public void OnUpdate()
	{
		Point hitPoint = Scene.HitPoint;
		if (!ActionMode.Adv.IsActive)
		{
			this.RefreshMouseInfo(true);
		}
		if (EInput.leftMouse.pressedLong && this.mode.AllowAutoClick)
		{
			this.autoClick = true;
		}
		if (this.autoClick && !EInput.leftMouse.pressing)
		{
			this.autoClick = false;
			if (this.start != null)
			{
				this.mode.OnSelectEnd(true);
				this.start = null;
				this.RefreshSummary();
				this.RefreshMouseInfo(true);
			}
		}
		if (this.autoClick && hitPoint.Distance(this.lastClickPoint) > 1 && this.selectType == BaseTileSelector.SelectType.Multiple && this.start != null)
		{
			this.start = hitPoint.Copy();
			this.mode.OnSelectStart(this.start);
			this.RefreshMouseInfo(true);
			this.lastClickPoint.Set(hitPoint);
		}
		if (((EInput.rightScroll && EInput.rightMouse.clicked) || (!EInput.rightScroll && EInput.rightMouse.down) || Input.GetKeyDown(KeyCode.Escape)) && !EMono.ui.wasActive)
		{
			if (this.selectType == BaseTileSelector.SelectType.Multiple && this.start != null)
			{
				this.mode.OnSelectEnd(true);
				this.start = null;
				this.RefreshSummary();
				this.RefreshMouseInfo(true);
			}
			else
			{
				this.mode.OnCancel();
			}
			EMono.ui.hud.hint.UpdateText();
		}
		if (!hitPoint.IsValid || EMono.ui.isPointerOverUI || EMono.ui.wasActive || this.mode.selectType == BaseTileSelector.SelectType.None || !EMono.pc.ai.ShowCursor)
		{
			return;
		}
		if (this.mode.hitType != BaseTileSelector.HitType.None)
		{
			if (this.mode.IsFillMode())
			{
				this.start = null;
				this.ProcessFillTiles(hitPoint, BaseTileSelector.ProcessMode.Render);
			}
			else
			{
				this.ProcessTiles(this.start ?? hitPoint, hitPoint, BaseTileSelector.ProcessMode.Render);
			}
		}
		if (this.start == null && (this.mode.AreaHihlight == AreaHighlightMode.Edit || this.mode.AreaHihlight == AreaHighlightMode.Sim) && hitPoint.area != null)
		{
			hitPoint.area.OnHoverArea(EMono.screen.guide.passArea);
		}
		if ((this.autoClick || EInput.leftMouse.down || (this.mode.ContinuousClick && EInput.leftMouse.pressing)) && this.mode.CanSelectTile)
		{
			if (this.start != null || hitPoint.IsSeen)
			{
				if (this.mode.IsFillMode())
				{
					this.TryProcessTiles(hitPoint);
				}
				else if (this.selectType == BaseTileSelector.SelectType.Single)
				{
					HitResult hitResult = this.mode._HitTest(this.temp, this.start);
					if ((hitResult == HitResult.Valid || hitResult == HitResult.Warning) && this.mode.CanProcessTiles())
					{
						this.TryProcessTiles(hitPoint);
					}
					else if (!this.autoClick)
					{
						SE.Beep();
					}
				}
				else
				{
					if (this.start == null)
					{
						this.start = hitPoint.Copy();
						this.mode.OnSelectStart(this.start);
						this.RefreshMouseInfo(true);
						if (this.autoClick)
						{
							this.TryProcessTiles(hitPoint);
						}
					}
					else
					{
						this.TryProcessTiles(hitPoint);
					}
					if (this.autoClick)
					{
						this.start = hitPoint.Copy();
						this.mode.OnSelectStart(this.start);
						this.RefreshMouseInfo(true);
					}
				}
			}
			this.lastClickPoint.Set(hitPoint);
			EMono.ui.hud.hint.UpdateText();
		}
	}

	public void TryProcessTiles(Point _end)
	{
		if (EInput.skipFrame > 0)
		{
			return;
		}
		if (!this.mode.CanProcessTiles())
		{
			if (!EMono.screen.tileSelector.autoClick)
			{
				SE.Beep();
			}
			return;
		}
		Point point = _end.Copy();
		ActionMode mode = this.mode;
		this.mode.OnBeforeProcessTiles();
		if (this.mode.IsFillMode())
		{
			this.ProcessFillTiles(point, BaseTileSelector.ProcessMode.Prpcess);
		}
		else if (this.start == null)
		{
			this.mode.OnProcessTiles(point, -1);
		}
		else
		{
			this.ProcessTiles(this.start, point, BaseTileSelector.ProcessMode.Prpcess);
			this.mode.OnSelectEnd(false);
		}
		if (this.mode != mode)
		{
			return;
		}
		this.mode.OnAfterProcessTiles(this.start, point);
		this.start = null;
		this.ExecuteSummary();
		this.RefreshMouseInfo(true);
		this.mode.OnFinishProcessTiles();
	}

	public void ProcessTiles(Point start, Point end, BaseTileSelector.ProcessMode processMode)
	{
		new Point();
		object obj = (start.x > end.x) ? start.x : end.x;
		int num = (start.z < end.z) ? start.z : end.z;
		int num2 = (start.x > end.x) ? end.x : start.x;
		int num3 = (start.z < end.z) ? end.z : start.z;
		object obj2 = obj;
		int num4 = obj2 - num2 + 1;
		int num5 = num3 - num + 1;
		BaseTileSelector.BoxType boxType = this.boxType;
		int num6 = -1;
		int num7 = 0;
		this.firstInMulti = false;
		if (this.multisize && this.selectType == BaseTileSelector.SelectType.Single && processMode != BaseTileSelector.ProcessMode.Prpcess)
		{
			num2 -= this.hitW - 1;
			num3 += this.hitH - 1;
			num4 += this.hitW - 1;
			num5 += this.hitH - 1;
		}
		if (processMode == BaseTileSelector.ProcessMode.Prpcess)
		{
			this.processing = true;
		}
		for (int i = obj2; i > num2 - 1; i--)
		{
			int j = num;
			while (j < num3 + 1)
			{
				if (boxType != BaseTileSelector.BoxType.Fence || (num4 <= 1 && num5 <= 1))
				{
					goto IL_12A;
				}
				num6 = ((num4 > num5) ? 0 : 1);
				if ((num6 != 0 || j == start.z) && (num6 != 1 || i == start.x))
				{
					goto IL_12A;
				}
				IL_15E:
				j++;
				continue;
				IL_12A:
				this.temp.Set(i, j);
				if (start.x != i || start.z != j)
				{
					this._ProcessTile(start, this.temp, processMode, num6);
					goto IL_15E;
				}
				goto IL_15E;
			}
			num7++;
			if (num7 > 30000)
			{
				Debug.Log(num7);
				break;
			}
		}
		this.firstInMulti = true;
		this._ProcessTile(start, start, processMode, num6);
		this.processing = false;
	}

	public void ProcessFillTiles(Point start, BaseTileSelector.ProcessMode processMode)
	{
		EMono._map.ForeachPoint(delegate(Point dest)
		{
			if (!start.Equals(dest))
			{
				this._ProcessTile(start, dest, processMode, -1);
			}
		});
		this._ProcessTile(start, start, processMode, -1);
	}

	private void _ProcessTile(Point start, Point dest, BaseTileSelector.ProcessMode processMode, int dir = -1)
	{
		if (!dest.IsValid)
		{
			return;
		}
		HitResult hitResult = this.mode._HitTest(dest, start);
		switch (processMode)
		{
		case BaseTileSelector.ProcessMode.Render:
			this.mode.OnRenderTile(dest, hitResult, dir);
			this.firstInMulti = false;
			return;
		case BaseTileSelector.ProcessMode.Prpcess:
			if (hitResult == HitResult.Valid || hitResult == HitResult.Warning)
			{
				this.mode.OnProcessTiles(dest, dir);
				return;
			}
			break;
		case BaseTileSelector.ProcessMode.Summary:
			this.mode.OnRefreshSummary(dest, hitResult, this.summary);
			break;
		default:
			return;
		}
	}

	public bool IsInRange(int x, int z, Point end)
	{
		if (this.start.x > end.x)
		{
			if (x > this.start.x || x < end.x)
			{
				return false;
			}
		}
		else if (x < this.start.x || x > end.x)
		{
			return false;
		}
		if (this.start.z > end.z)
		{
			if (z > this.start.z || z < end.z)
			{
				return false;
			}
		}
		else if (z < this.start.z || z > end.z)
		{
			return false;
		}
		return true;
	}

	public unsafe void RefreshMouseInfo(bool force = false)
	{
		bool enable = true;
		Point hitPoint = Scene.HitPoint;
		if (EMono.ui.IsActive || EMono.ui.isPointerOverUI || !this.mode.enableMouseInfo || !hitPoint.IsValid)
		{
			enable = false;
		}
		else if (!hitPoint.Equals(this.lastPoint) || force)
		{
			if (this.start != null && !hitPoint.Equals(this.lastPoint))
			{
				SE.Play("highlight3");
			}
			this.RefreshSummary();
			string text = "";
			int num = 0;
			if (this.start != null)
			{
				int num2 = Mathf.Abs(hitPoint.x - this.start.x) + 1;
				int num3 = Mathf.Abs(hitPoint.z - this.start.z) + 1;
				if (!hitPoint.IsValid)
				{
					num2 = 0;
					num3 = 0;
				}
				text = string.Concat(new string[]
				{
					text,
					"(",
					num2.ToString(),
					" x ",
					num3.ToString(),
					")"
				});
			}
			else if (this.selectType == BaseTileSelector.SelectType.Multiple)
			{
				text += "selectRange".lang();
			}
			if (this.selectType == BaseTileSelector.SelectType.Single)
			{
				num = this.mode.CostMoney;
			}
			else if (this.start != null)
			{
				num = this.summary.money;
			}
			EMono.ui.mouseInfo.textCost.SetActive(num != 0);
			if (num != 0)
			{
				EMono.ui.mouseInfo.textCost.text = (num.ToString() ?? "");
			}
			if (this.mode.TopHeight(hitPoint) != -1)
			{
				text += "hintAltitude".lang(this.mode.TopHeight(hitPoint).ToString() ?? "", null, null, null, null);
			}
			if (EMono.scene.actionMode == ActionMode.Picker)
			{
				AM_Picker.Result result = ActionMode.Picker.Test(hitPoint, false);
				if (result.IsValid)
				{
					text += result.GetText();
				}
			}
			if (this.mode.IsRoofEditMode(null))
			{
				text = text + Environment.NewLine + "roofEdit".lang();
			}
			else if (this.mode.IsFillMode())
			{
				text = text + Environment.NewLine + "fillMode".lang();
			}
			text += "\n";
			EMono.ui.mouseInfo.text.SetText(*this.mode.SetMouseInfo(ref text));
		}
		EMono.ui.mouseInfo.SetActive(enable);
		this.lastPoint.Set(hitPoint);
	}

	public void ExecuteSummary()
	{
		if (!BuildMenu.Instance)
		{
			return;
		}
		this.summary.Execute();
		BuildMenu.Instance.info1.Refresh();
		this.RefreshSummary();
		this.mode.SEExecuteSummary();
	}

	public void RefreshSummary()
	{
		Point hitPoint = Scene.HitPoint;
		this.summary.Clear();
		this.ProcessTiles(this.start ?? hitPoint, hitPoint, BaseTileSelector.ProcessMode.Summary);
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.RefreshBalance();
		}
	}

	public BaseTileSelector.HitType inspectHitType;

	public bool inspectorHighlight;

	public HitSummary summary = new HitSummary();

	[NonSerialized]
	public Point start;

	[NonSerialized]
	public Point temp = new Point();

	[NonSerialized]
	public Point lastPoint = new Point();

	[NonSerialized]
	public Point lastClickPoint = new Point();

	[NonSerialized]
	public bool processing;

	[NonSerialized]
	public bool autoClick;

	public bool firstInMulti;

	public enum SelectType
	{
		Single,
		Multiple,
		None
	}

	public enum HitType
	{
		None,
		Default,
		Floor,
		Block,
		Inspect
	}

	public enum BoxType
	{
		Box,
		Fence
	}

	public enum ProcessMode
	{
		Render,
		Prpcess,
		Summary
	}
}
