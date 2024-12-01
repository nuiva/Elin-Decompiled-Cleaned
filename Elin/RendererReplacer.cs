public class RendererReplacer : EClass
{
	public int tile = 1820;

	public RenderData data;

	public SourcePref pref;

	public static RendererReplacer CreateFrom(string id, int shift = 0)
	{
		CardRow cardRow = EClass.sources.cards.map[id];
		return new RendererReplacer
		{
			tile = cardRow._tiles[0] + shift,
			data = cardRow.renderData,
			pref = cardRow.pref
		};
	}
}
