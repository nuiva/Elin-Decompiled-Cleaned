using System;

public class TaskDesignation : TaskPoint, ISyncScreen
{
	public override bool HasProgress
	{
		get
		{
			return true;
		}
	}

	public virtual int idMarker
	{
		get
		{
			return 0;
		}
	}

	public virtual int W
	{
		get
		{
			return 1;
		}
	}

	public virtual int H
	{
		get
		{
			return 1;
		}
	}

	public virtual bool ShowOrbit
	{
		get
		{
			return false;
		}
	}

	public virtual string GetTextOrbit()
	{
		return this.Name;
	}

	public virtual bool Working
	{
		get
		{
			return this.owner != null;
		}
	}

	public override bool ShowAuto
	{
		get
		{
			return true;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Arrow;
		}
	}

	public TaskManager.Designations Designations
	{
		get
		{
			return EClass._map.tasks.designations;
		}
	}

	public bool PointHasOtherDesignation()
	{
		TaskDesignation taskDesignation = this.Designations.mapAll.TryGetValue(this.pos.index, null);
		return taskDesignation != null && taskDesignation != this;
	}

	public virtual void DrawMarker(int x, int z, RenderParam p)
	{
		int num = this.Working ? 20 : 19;
		if (this.isBlock)
		{
			EClass.screen.tileMap.passGuideBlock.Add(p, (float)num, 0f, 0f);
			return;
		}
		EClass.screen.tileMap.passGuideFloor.Add(p, (float)num, 0f, 0f);
	}

	public long Sync
	{
		get
		{
			return this.sync;
		}
	}

	public virtual void Draw(int x, int z, RenderParam p)
	{
		this.sync = EClass.scene.syncFrame;
		if (!this.isSynced)
		{
			this.OnEnterScreen();
			EClass.scene.syncList.Add(this);
		}
		this.DrawMarker(x, z, p);
	}

	public virtual void OnEnterScreen()
	{
		this.isSynced = true;
	}

	public virtual void OnLeaveScreen()
	{
		this.isSynced = false;
		if (this.orbit)
		{
			this.DespawnOrbit();
		}
	}

	public virtual void TrySpawnOrbit()
	{
		this.orbit = PoolManager.Spawn<TCOrbitTask>("tcOrbitTask", "Scene/Render/Actor/Component/TCOrbitTask", null);
		this.orbit.SetOwner(this);
	}

	public void DespawnOrbit()
	{
		PoolManager.Despawn(this.orbit);
		this.orbit = null;
	}

	public long sync;

	public bool isSynced;

	public TCOrbitTask orbit;
}
