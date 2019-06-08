using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell
{
	public struct Ledge
	{
		public Vector3 direction;
		public bool isLedge;

		public Ledge(Vector3 d, bool l)
		{
			this.direction = d;
			this.isLedge = l;
		}
	}

	private List<Cellable> elements = new List<Cellable>();

	public readonly Vector3 position;
	public readonly Vector3 normal;
	public readonly Vector3 local;
	public readonly List<Ledge> ledges;
	public readonly bool isInner = false;
	public bool isBound { get => !this.isInner; }
	public bool isWalkable {
		get {
			foreach(Cellable e in this.elements) {
				if(!e.isWalkable) {
					return false;
				}
			}
			return true;
		}
	}
	public readonly Grill grill;
	public Surface surface { get => this.grill.surface; }


	public Cell(Grill g, Vector3 pos, Vector3 n, Vector3 l, Transform t, bool bound)
	{
		this.grill = g;
		this.position = pos;
		this.normal = n;
		this.local = l;

		this.ledges = new List<Ledge>();

		// If grill said cell is bound, then check if it's really
		// true (it can be false if two grill are next to each other).
		// Else we can trust the constructor : this cell is inner.
		this.isInner = (bound) ? this.IsReallyInnerCell(t) : true;
	}

	public void AddElement(Cellable e)
	{
		if(!elements.Contains(e)) {
			elements.Add(e);
		}
	}

	public void RemoveElement(Cellable e)
	{
		if(elements.Contains(e)) {
			elements.Remove(e);
		}
	}







	private bool IsReallyInnerCell(Transform t)
	{
		Vector3 pos, p;
		Vector3[] directions;
		int targetMask;
		bool l;

		pos = position + normal * 0.5f;
		directions = new Vector3[4] {t.right, -t.right, t.forward, -t.forward};
		targetMask = (1 << LayerMask.NameToLayer("Ground"));

		foreach(Vector3 d in directions) {
			p = pos + d;
			if(!Physics.Raycast(p, -normal, 1f, targetMask)) {
				l = !Physics.Raycast(pos, p - pos, 1f, targetMask);
				this.ledges.Add(new Ledge(d, l));
			}
		}

		return (this.ledges.Count == 0);
	}


	public override string ToString() => $"Cell: position = {this.position}, normal = {this.normal}, isInner = {this.isInner}";
}
