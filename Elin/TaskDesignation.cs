public class TaskDesignation : TaskPoint, ISyncScreen
{
	public long sync;

	public bool isSynced;

	public TCOrbitTask orbit;

	public override bool HasProgress => true;

	public virtual int idMarker => 0;

	public virtual int W => 1;

	public virtual int H => 1;

	public virtual bool ShowOrbit => false;

	public virtual bool Working => owner != null;

	public override bool ShowAuto => true;

	public override CursorInfo CursorIcon => CursorSystem.Arrow;

	public TaskManager.Designations Designations => EClass._map.tasks.designations;

	public long Sync => sync;

	public virtual string GetTextOrbit()
	{
		return Name;
	}

	public bool PointHasOtherDesignation()
	{
		TaskDesignation taskDesignation = Designations.mapAll.TryGetValue(pos.index);
		if (taskDesignation != null)
		{
			return taskDesignation != this;
		}
		return false;
	}

	public virtual void DrawMarker(int x, int z, RenderParam p)
	{
		int num = (Working ? 20 : 19);
		if (isBlock)
		{
			EClass.screen.tileMap.passGuideBlock.Add(p, num, 0f);
		}
		else
		{
			EClass.screen.tileMap.passGuideFloor.Add(p, num, 0f);
		}
	}

	public virtual void Draw(int x, int z, RenderParam p)
	{
		sync = EClass.scene.syncFrame;
		if (!isSynced)
		{
			OnEnterScreen();
			EClass.scene.syncList.Add(this);
		}
		DrawMarker(x, z, p);
	}

	public virtual void OnEnterScreen()
	{
		isSynced = true;
	}

	public virtual void OnLeaveScreen()
	{
		isSynced = false;
		if ((bool)orbit)
		{
			DespawnOrbit();
		}
	}

	public virtual void TrySpawnOrbit()
	{
		orbit = PoolManager.Spawn<TCOrbitTask>("tcOrbitTask", "Scene/Render/Actor/Component/TCOrbitTask");
		orbit.SetOwner(this);
	}

	public void DespawnOrbit()
	{
		PoolManager.Despawn(orbit);
		orbit = null;
	}
}
