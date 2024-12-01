public class Zone_SisterHouse : Zone_Civilized
{
	public override bool HiddenInRegionMap => true;

	public override void OnActivate()
	{
		EClass.scene.LoadPrefab("SurfaceSister");
	}
}
