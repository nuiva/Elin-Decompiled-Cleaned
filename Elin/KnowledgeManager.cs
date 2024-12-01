using Newtonsoft.Json;

public class KnowledgeManager : EClass
{
	[JsonProperty]
	public KnowledgeList<KnowledgeRecipe> recipes = new KnowledgeList<KnowledgeRecipe>();

	[JsonProperty]
	public KnowledgeList<KnowledgeFaction> factions = new KnowledgeList<KnowledgeFaction>();

	[JsonProperty]
	public KnowledgeList<KnowledgeResearch> resarches = new KnowledgeList<KnowledgeResearch>();
}
