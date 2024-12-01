public class RenderDataChara : RenderDataCard
{
	public override string prefabName => "CharaActor";

	private void OnValidate()
	{
		_offset = offset;
	}
}
