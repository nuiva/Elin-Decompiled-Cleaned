using Newtonsoft.Json;

public class NumLogCategory : NumLog
{
	[JsonProperty]
	public string id;

	public override string Name => EClass.sources.categories.map[id].GetText();

	public override int Value => EClass._map.Stocked.categoryMap[id].sum;
}
