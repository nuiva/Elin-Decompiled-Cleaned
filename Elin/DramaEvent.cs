public class DramaEvent : EClass
{
	public int progress;

	public string idJump;

	public string idActor;

	public string step;

	public float time;

	public bool temp;

	public DramaSequence sequence;

	public DramaActor actor => sequence.GetActor(idActor);

	public DramaManager manager => sequence.manager;

	public LayerDrama layer => manager.layer;

	public virtual bool Play()
	{
		return true;
	}

	public virtual void Reset()
	{
		progress = 0;
	}
}
