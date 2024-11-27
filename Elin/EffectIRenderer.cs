using System;
using UnityEngine;

public class EffectIRenderer : Effect
{
	public EffectIRenderer Play(Card _card, Card _origin, Vector3 _to)
	{
		if (!EMono.core.IsGameStarted)
		{
			base.Kill();
			return null;
		}
		this.card = _card;
		this.origin = _origin;
		this.destV = _to;
		this.from = _origin.pos.Copy();
		this.snapTimer = -0.01f;
		base.Activate();
		this.OnUpdate();
		if (this.animeData)
		{
			this.anime = new TransAnime
			{
				data = this.animeData,
				renderer = this.card.renderer
			}.Init();
		}
		return this;
	}

	public EffectIRenderer Play(Card _card, Point from, Point to = null, float fixY = 0f)
	{
		this.card = _card;
		this.snapTimer = -0.01f;
		this.from = from.Copy();
		base.Play(from, fixY, to, null);
		if (this.animeData)
		{
			this.anime = new TransAnime
			{
				data = this.animeData,
				renderer = this.card.renderer
			}.Init();
		}
		return this;
	}

	public override void OnUpdate()
	{
		RenderParam renderParam = this.card.GetRenderParam();
		renderParam.color = (float)EMono.scene.screenElin.tileMap.GetApproximateBlocklight(this.from.cell);
		this.timer += Core.delta;
		this.snapTimer -= Core.delta;
		TransAnime transAnime = this.anime;
		if (transAnime != null)
		{
			transAnime.Update();
		}
		if (this.snapTimer < 0f || this.v == Vector3.zero)
		{
			if (this.follow)
			{
				if (this.origin.renderer != null)
				{
					Vector3 position = this.origin.renderer.position;
					Vector3 vector = this.destV * this.timer / this.duration * this.speed;
					this.v.x = position.x + this.posFix.x + vector.x;
					this.v.y = position.y + this.posFix.y + vector.y;
					this.v.z = position.z + this.posFix.z + vector.z;
				}
			}
			else
			{
				this.v.x = base.transform.position.x;
				this.v.y = base.transform.position.y;
				this.v.z = base.transform.position.z;
			}
			if (this.anime != null)
			{
				this.v.x = this.v.x + this.anime.v.x;
				this.v.y = this.v.y + this.anime.v.y;
				this.v.z = this.v.z + this.anime.v.z;
			}
			this.snapTimer += this.snapInterval;
		}
		renderParam.x = this.v.x;
		renderParam.y = this.v.y;
		renderParam.z = this.v.x;
		this.card.renderer.skip = true;
		if (this.card.renderer.usePass)
		{
			this.card.renderer.data.Draw(renderParam);
			return;
		}
		if (this.card.renderer.actor)
		{
			this.card.renderer.actor.OnRender(renderParam);
		}
	}

	public Card card;

	public Card origin;

	public Point from;

	public float snapInterval;

	public bool follow;

	public TransAnimeData animeData;

	public TransAnime anime;

	private float snapTimer;

	private Vector3 v;
}
