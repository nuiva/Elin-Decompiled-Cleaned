using System;

public class DynamicAct : Act
{
	public override string ID
	{
		get
		{
			return this.id;
		}
	}

	public override TargetType TargetType
	{
		get
		{
			return this._targetType;
		}
	}

	public override bool IsHostileAct
	{
		get
		{
			return this.isHostileAct;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return this.cursor;
		}
	}

	public override bool CloseLayers
	{
		get
		{
			return this.closeLayers;
		}
	}

	public override string GetText(string str = "")
	{
		return Lang.Get(this.id);
	}

	public override int PerformDistance
	{
		get
		{
			return this.dist;
		}
	}

	public override bool CanPressRepeat
	{
		get
		{
			return this.canRepeat != null && this.canRepeat();
		}
	}

	public DynamicAct(string _id, Func<bool> _onPerform = null, bool _closeLayers = false)
	{
		this.id = _id;
		this.onPerform = _onPerform;
		this.closeLayers = _closeLayers;
		DynamicAct.lastAct = this;
	}

	public override bool LocalAct
	{
		get
		{
			return !(this.id == "actNewZone") && this.localAct;
		}
	}

	public override bool Perform()
	{
		return this.onPerform != null && this.onPerform();
	}

	public static DynamicAct lastAct;

	public new string id;

	public Func<bool> onPerform;

	public Func<bool> canRepeat;

	public bool closeLayers;

	public bool isHostileAct;

	public bool localAct = true;

	public CursorInfo cursor;

	public int dist = 1;

	public TargetType _targetType = TargetType.Any;
}
