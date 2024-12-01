public class TraitLightSource : TraitTorch
{
	public int LightRadius => GetParam(1).ToInt();
}
