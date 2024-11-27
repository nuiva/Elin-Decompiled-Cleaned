using System;

public class DramaEvent : EClass
{
	public DramaActor actor
	{
		get
		{
			return this.sequence.GetActor(this.idActor);
		}
	}

	public DramaManager manager
	{
		get
		{
			return this.sequence.manager;
		}
	}

	public LayerDrama layer
	{
		get
		{
			return this.manager.layer;
		}
	}

	public virtual bool Play()
	{
		return true;
	}

	public virtual void Reset()
	{
		this.progress = 0;
	}

	public int progress;

	public string idJump;

	public string idActor;

	public string step;

	public float time;

	public bool temp;

	public DramaSequence sequence;
}
