using System;

public class BackerContentInspector : EMono
{
	private void Awake()
	{
		BackerContentInspector.Instance = this;
	}

	public void Apply()
	{
		this.content.Apply();
	}

	public void Remove()
	{
		this.content.Remove();
	}

	public static BackerContentInspector Instance;

	public BackerContentInspector.Content content;

	[Serializable]
	public class Content
	{
		public virtual string name
		{
			get
			{
				return this.id.ToString() ?? "";
			}
		}

		public virtual void OnValidate()
		{
		}

		public virtual void Apply()
		{
		}

		public virtual void Remove()
		{
		}

		public int id;
	}

	public class ContentObj : BackerContentInspector.Content
	{
		public override string name
		{
			get
			{
				return this.p.cell.GetObjName();
			}
		}

		public override void Apply()
		{
			EMono._map.ApplyBackerObj(this.p, this.id);
		}

		public override void Remove()
		{
			EMono._map.backerObjs.Remove(this.p.index);
		}

		public Point p;
	}

	public class ContentCard : BackerContentInspector.Content
	{
		public override string name
		{
			get
			{
				return this.c.id + "/" + this.c.Name;
			}
		}

		public override void Apply()
		{
			this.c.ApplyBacker(this.id);
		}

		public override void Remove()
		{
			this.c.RemoveBacker();
		}

		public Card c;
	}
}
