public class TraitSnow : Trait
{
	public override bool IsThrowMainAction => true;

	public override ThrowType ThrowType => ThrowType.Snow;

	public override EffectDead EffectDead => EffectDead.None;
}
