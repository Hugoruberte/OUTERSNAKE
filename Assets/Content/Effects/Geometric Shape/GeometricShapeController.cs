using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GeometricShapeController : MonoBehaviour
{
	public enum ShapeAlignmentEnum {
		None = 0,
		Up,
		Down
	}

	[Header("Parameters")]
	public ShapeAlignmentEnum alignment = ShapeAlignmentEnum.None;
	[Range(3, 10)] public int edgeCount = 3;
	[Range(0f, 0.4f)] public float width = 0.01f;
	[Range(0f, 10f)] public float size = 1f;
	public bool fill = false;
	public Color color = Color.white;

	private LineRenderer lineRenderer;
	private MeshRenderer fillRenderer;

	private void Awake()
	{
		this.GetRenderers();
	}

	private void GetRenderers()
	{
		this.lineRenderer = GetComponentInChildren<LineRenderer>(true);
		this.fillRenderer = GetComponentInChildren<MeshRenderer>(true);

		if(!this.lineRenderer) {
			Debug.LogWarning("WARNING : There is no LineRenderer !");
		}

		if(!this.fillRenderer) {
			Debug.LogWarning("WARNING : There is no Mesh Renderer !");
		}
	}

	void OnValidate()
	{
		if(!this.lineRenderer || !this.fillRenderer) {
			this.GetRenderers();
		}

		if(!this.lineRenderer || !this.fillRenderer) {
			Debug.Log("We might have a problem johnson.");
		}

		this.UpdateEdge(this.edgeCount);
		this.UpdateFill(this.fill);
		this.UpdateWidth(this.width);
		this.UpdateColor(this.color);
		// this.UpdateAlignment(this.alignment);
	}

	public void UpdateEdge(int count)
	{
		float angle, start;
		Vector3 direction;
		List<Vector3> positions;
		Quaternion rotation;

		angle = 360f / count;
		rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		start = ((count % 2 == 0 && this.alignment != ShapeAlignmentEnum.None) || (count % 2 != 0 && this.alignment == ShapeAlignmentEnum.Up)) ? 0f : angle / 2f;
		direction = (Quaternion.AngleAxis(start, Vector3.forward) * Vector3.down).normalized * (this.size / 2f);

		this.lineRenderer.positionCount = count + 4;
		positions = Shared.vector3List;
		positions.Clear();
		
		for(int i = 0; i < count + 4; ++i) {
			positions.Add(direction);
			direction = rotation * direction;
		}

		this.lineRenderer.SetPositions(positions.ToArray());
		positions.Clear();

		this.edgeCount = count;
	}

	public void UpdateSize(float s)
	{
		this.size = s;

		this.UpdateEdge(this.edgeCount);
	}

	public void UpdateFill(bool f)
	{
		if(f != this.fillRenderer.gameObject.activeInHierarchy) {
			this.fillRenderer.gameObject.SetActive(f);
		}

		Material m = (Application.isPlaying) ? this.fillRenderer.material : this.fillRenderer.sharedMaterial;
		m.SetInt("_EdgeCount", this.edgeCount);

		if(this.alignment == ShapeAlignmentEnum.None) {
			this.fillRenderer.transform.localRotation = Quaternion.Euler(90, 180, 0);
		} else if(this.edgeCount % 2 == 0) {
			this.fillRenderer.transform.localRotation = Quaternion.Euler(90, 360f / this.edgeCount / 2f, 0);
		} else if(this.alignment == ShapeAlignmentEnum.Up) {
			this.fillRenderer.transform.localRotation = Quaternion.Euler(90, 0, 0);
		} else {
			this.fillRenderer.transform.localRotation = Quaternion.Euler(90, 180, 0);
		}

		this.fill = f;
	}

	public void UpdateWidth(float w)
	{
		this.lineRenderer.widthMultiplier = w;

		this.width = w;
	}

	public void UpdateColor(Color c)
	{
		this.lineRenderer.startColor = c;
		this.lineRenderer.endColor = c;

		Material m = (Application.isPlaying) ? this.fillRenderer.material : this.fillRenderer.sharedMaterial;
		m.SetColor("_Color", c);

		this.color = c;
	}

	public void UpdateAlignment(ShapeAlignmentEnum align)
	{
		this.alignment = align;

		this.UpdateEdge(this.edgeCount);
		this.UpdateFill(this.fill);
	}
}
