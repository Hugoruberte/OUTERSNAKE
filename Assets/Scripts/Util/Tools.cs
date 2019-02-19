using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Tools
{
	public class _Transform
	{
		public Vector3 right = Vector3.right;
		public Vector3 up = Vector3.up;
		public Vector3 forward = Vector3.forward;

		public Vector3 position = Vector3.zero;
		public Quaternion rotation = Quaternion.identity;

		public _Transform(){
			// empty
		}

		public _Transform(Transform t)
		{
			this.Copy(t);
		}

		public _Transform(Vector3 pos, Quaternion rot)
		{
			this.position = pos;
			this.rotation = rot;
		}

		public void Copy(Transform tr)
		{
			this.right = tr.right;
			this.up = tr.up;
			this.forward = tr.forward;
			this.position = tr.position;
			this.rotation = tr.rotation;
		}

		public void Copy(_Transform tr)
		{
			this.right = tr.right;
			this.up = tr.up;
			this.forward = tr.forward;
			this.position = tr.position;
			this.rotation = tr.rotation;
		}
	}

	public static class TransformExtension
	{
		//Breadth-first search
		public static Transform DeepFind(this Transform root, string n)
		{
			Transform result = root.Find(n);
			if(result != null) {
				return result;
			}

			foreach(Transform child in root) {
				result = child.DeepFind(n);
				if(result != null) {
					return result;
				}
			}

			return null;
		}

		public static Vector3 TransformPointUnscaled(this Transform pT, Vector3 position)
		{
			Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(pT.position, pT.rotation, Vector3.one);
			return localToWorldMatrix.MultiplyPoint3x4(position);
		}
		public static Vector3 InverseTransformPointUnscaled(this Transform pT, Vector3 pos)
		{
			Matrix4x4 worldToLocalMatrix = Matrix4x4.TRS(pT.position, pT.rotation, Vector3.one).inverse;
			return worldToLocalMatrix.MultiplyPoint3x4(pos);
		}

		public static Transform SetPositionX(this Transform pTransform, float pValue)
		{
			Vector3 lPosition = pTransform.position;
			lPosition.x = pValue;
			pTransform.position = lPosition;
			return pTransform;
		}
		public static Transform SetPositionY(this Transform pTransform, float pValue)
		{
			Vector3 lPosition = pTransform.position;
			lPosition.y = pValue;
			pTransform.position = lPosition;
			return pTransform;
		}
		public static Transform SetPositionZ(this Transform pTransform, float pValue)
		{
			Vector3 lPosition = pTransform.position;
			lPosition.z = pValue;
			pTransform.position = lPosition;
			return pTransform;
		}


		public static Transform SetLocalPositionX(this Transform pTransform, float pValue)
		{
			Vector3 lPosition = pTransform.localPosition;
			lPosition.x = pValue;
			pTransform.localPosition = lPosition;
			return pTransform;
		}
		public static Transform SetLocalPositionY(this Transform pTransform, float pValue)
		{
			Vector3 lPosition = pTransform.localPosition;
			lPosition.y = pValue;
			pTransform.localPosition = lPosition;
			return pTransform;
		}
		public static Transform SetLocalPositionZ(this Transform pTransform, float pValue)
		{
			Vector3 lPosition = pTransform.localPosition;
			lPosition.z = pValue;
			pTransform.localPosition = lPosition;
			return pTransform;
		}


		public static Transform SetLocalScaleX(this Transform pTransform, float pValue)
		{
			Vector3 lScale = pTransform.localScale;
			lScale.x = pValue;
			pTransform.localScale = lScale;
			return pTransform;
		}
		public static Transform SetLocalScaleY(this Transform pTransform, float pValue)
		{
			Vector3 lScale = pTransform.localScale;
			lScale.y = pValue;
			pTransform.localScale = lScale;
			return pTransform;
		}
		public static Transform SetLocalScaleZ(this Transform pTransform, float pValue)
		{
			Vector3 lScale = pTransform.localScale;
			lScale.z = pValue;
			pTransform.localScale = lScale;
			return pTransform;
		}


		public static Transform SetLocalRotationX(this Transform pTransform, float pValue)
		{
			Vector3 lScale = pTransform.localEulerAngles;
			lScale.x = pValue;
			pTransform.localEulerAngles = lScale;
			return pTransform;
		}
		public static Transform SetLocalRotationY(this Transform pTransform, float pValue)
		{
			Vector3 lScale = pTransform.localEulerAngles;
			lScale.y = pValue;
			pTransform.localEulerAngles = lScale;
			return pTransform;
		}
		public static Transform SetLocalRotationZ(this Transform pTransform, float pValue)
		{
			Vector3 lScale = pTransform.localEulerAngles;
			lScale.z = pValue;
			pTransform.localEulerAngles = lScale;
			return pTransform;
		}


		public static Quaternion AbsoluteRotation(this Quaternion qt, int degreeAcc = 90)
		{
			int signx = (int)Mathf.Sign(qt.eulerAngles.x);
			int signy = (int)Mathf.Sign(qt.eulerAngles.y);
			int signz = (int)Mathf.Sign(qt.eulerAngles.z);

			int abx = Mathf.Abs(Mathf.RoundToInt(qt.eulerAngles.x));
			int aby = Mathf.Abs(Mathf.RoundToInt(qt.eulerAngles.y));
			int abz = Mathf.Abs(Mathf.RoundToInt(qt.eulerAngles.z));

			int qtx = (abx / degreeAcc) * degreeAcc;
			int qty = (aby / degreeAcc) * degreeAcc;
			int qtz = (abz / degreeAcc) * degreeAcc;

			int mid = Mathf.RoundToInt(degreeAcc / 2f);

			qtx += (abx % degreeAcc < mid) ? 0 : degreeAcc;
			qty += (aby % degreeAcc < mid) ? 0 : degreeAcc;
			qtz += (abz % degreeAcc < mid) ? 0 : degreeAcc;

			qtx *= signx;
			qty *= signy;
			qtz *= signz;

			return Quaternion.Euler(qtx, qty, qtz);
		}
		public static Quaternion AbsoluteRotation(this Transform pTransform, int degreeAcc = 90)
		{
			Quaternion qt = pTransform.rotation;
			
			return (AbsoluteRotation(qt, degreeAcc));
		}
		public static void SetAbsoluteRotation(this Transform pTransform, int degreeAcc = 90)
		{
			Quaternion qt = pTransform.rotation;
			Quaternion newQt = AbsoluteRotation(pTransform, degreeAcc);
			pTransform.rotation = newQt;
		}
		public static Quaternion SetAbsoluteRotation(this Quaternion qt, int degreeAcc = 90)
		{
			qt = AbsoluteRotation(qt, degreeAcc);
			return qt;
		}


		public static Vector3 AbsolutePosition(this Transform pTransform)
		{
			Vector3 pos = pTransform.position;
			pos.Set(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
			return pos;
		}
		public static void SetAbsolutePosition(this Transform pTransform)
		{
			pTransform.position = AbsolutePosition(pTransform);
		}
	}

	public static class Vector3Extension
	{
		public static Vector3 RoundToInt(Vector3 vect)
		{
			return new Vector3(Mathf.RoundToInt(vect.x), Mathf.RoundToInt(vect.y), Mathf.RoundToInt(vect.z));
		}

		public static Vector3 SetRoundToInt(this Vector3 vect)
		{
			vect.Set(Mathf.RoundToInt(vect.x), Mathf.RoundToInt(vect.y), Mathf.RoundToInt(vect.z));
			return vect;
		}

		public static bool IsWorldlyOrthogonal(this Vector3 v)
		{
			Vector3 n = v.normalized;

			if(Mathf.Abs(Vector3.Dot(n, Vector3.right)) == 1)
				return true;
			else if(Mathf.Abs(Vector3.Dot(n, Vector3.up)) == 1)
				return true;
			else if(Mathf.Abs(Vector3.Dot(n, Vector3.forward)) == 1)
				return true;

			return false;
		}

		public static Vector3 SetWorldlyOrthogonal(this Vector3 v)
		{
			Vector3[] test;
			Vector3 n, res;
			float max, value;

			res = Vector3.zero;
			max = 0f;
			n = v.normalized;
			test = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};

			foreach(Vector3 t in test) {
				value = Vector3.Dot(n, t);
				if(Mathf.Abs(value) > max) {
					max = Mathf.Abs(value);
					res = Mathf.Sign(value) * t;
				}
			}

			v = res;
			return res;
		}
	}

	public static class Vector2Extension
	{
		public static Vector2 RoundToInt(Vector2 vect)
		{
			return new Vector2(Mathf.RoundToInt(vect.x), Mathf.RoundToInt(vect.y));
		}

		public static void SetRoundToInt(this Vector2 vect)
		{
			vect.Set(Mathf.RoundToInt(vect.x), Mathf.RoundToInt(vect.y));
		}
	}

	public static class ListExtension
	{
		public static void Shuffle<T>(this List<T> ts)
		{
			int count = ts.Count;

			for(int i = 0; i < count; i++) {
				int r = UnityEngine.Random.Range(i, count);
				T tmp = ts[i];
				ts[i] = ts[r];
				ts[r] = tmp;
			}
		}
	}

	public static class ArrayExtension
	{
		public static CellEnum[] DeepCopy(CellEnum[] arr1)
		{
			CellEnum[] arr2 = new CellEnum[arr1.Length];
			for(int i = 0; i < arr1.Length; i++)
			arr2[i] = arr1[i];
			return arr2;
		}

		public static string[] DeepCopy(string[] arr1)
		{
			string[] arr2 = new string[arr1.Length];
			for(int i = 0; i < arr1.Length; i++)
			arr2[i] = arr1[i];
			return arr2;
		}

		public static string[] DeepCopy(List<string> lis1)
		{
			List<string> lis2 = new List<string>(lis1);
			return lis2.ToArray();
		}

		public static Vector3[] DeepCopy(Vector3[] arr1)
		{
			Vector3[] arr2 = new Vector3[arr1.Length];
			for(int i = 0; i < arr1.Length; i++)
			arr2[i] = arr1[i];
			return arr2;
		}

		public static Quaternion[] DeepCopy(Quaternion[] arr1)
		{
			Quaternion[] arr2 = new Quaternion[arr1.Length];
			for(int i = 0; i < arr1.Length; i++)
			arr2[i] = arr1[i];
			return arr2;
		}
	}

	public static class ColorExtension
	{
		public static Color SetColorR(this Color pColor, float pValue)
		{
			Color temp = pColor;
			temp.r = pValue;
			pColor = temp;
			return pColor;
		}
		public static Color SetColorG(this Color pColor, float pValue)
		{
			Color temp = pColor;
			temp.g = pValue;
			pColor = temp;
			return pColor;
		}
		public static Color SetColorB(this Color pColor, float pValue)
		{
			Color temp = pColor;
			temp.b = pValue;
			pColor = temp;
			return pColor;
		}
		public static Color SetColorA(this Color pColor, float pValue)
		{
			Color temp = pColor;
			temp.a = pValue;
			pColor = temp;
			return pColor;
		}
		
		public static Text SetColorR(this Text pText, float pValue)
		{
			Color temp = pText.color;
			temp.r = pValue;
			pText.color = temp;
			return pText;
		}
		public static Text SetColorG(this Text pText, float pValue)
		{
			Color temp = pText.color;
			temp.g = pValue;
			pText.color = temp;
			return pText;
		}
		public static Text SetColorB(this Text pText, float pValue)
		{
			Color temp = pText.color;
			temp.b = pValue;
			pText.color = temp;
			return pText;
		}
		public static Text SetColorA(this Text pText, float pValue)
		{
			Color temp = pText.color;
			temp.a = pValue;
			pText.color = temp;
			return pText;
		}


		public static TextMesh SetColorR(this TextMesh pTextMesh, float pValue)
		{
			Color temp = pTextMesh.color;
			temp.r = pValue;
			pTextMesh.color = temp;
			return pTextMesh;
		}
		public static TextMesh SetColorG(this TextMesh pTextMesh, float pValue)
		{
			Color temp = pTextMesh.color;
			temp.g = pValue;
			pTextMesh.color = temp;
			return pTextMesh;
		}
		public static TextMesh SetColorB(this TextMesh pTextMesh, float pValue)
		{
			Color temp = pTextMesh.color;
			temp.b = pValue;
			pTextMesh.color = temp;
			return pTextMesh;
		}
		public static TextMesh SetColorA(this TextMesh pTextMesh, float pValue)
		{
			Color temp = pTextMesh.color;
			temp.a = pValue;
			pTextMesh.color = temp;
			return pTextMesh;
		}

		public static Renderer SetColorR(this Renderer pRenderer, float pValue)
		{
			Color temp = pRenderer.material.color;
			temp.r = pValue;
			pRenderer.material.color = temp;
			return pRenderer;
		}
		public static Renderer SetColorG(this Renderer pRenderer, float pValue)
		{
			Color temp = pRenderer.material.color;
			temp.g = pValue;
			pRenderer.material.color = temp;
			return pRenderer;
		}
		public static Renderer SetColorB(this Renderer pRenderer, float pValue)
		{
			Color temp = pRenderer.material.color;
			temp.b = pValue;
			pRenderer.material.color = temp;
			return pRenderer;
		}
		public static Renderer SetColorA(this Renderer pRenderer, float pValue)
		{
			Color temp = pRenderer.material.color;
			temp.a = pValue;
			pRenderer.material.color = temp;
			return pRenderer;
		}

		public static Image SetColorR(this Image pImage, float pValue)
		{
			Color temp = pImage.color;
			temp.r = pValue;
			pImage.color = temp;
			return pImage;
		}
		public static Image SetColorG(this Image pImage, float pValue)
		{
			Color temp = pImage.color;
			temp.g = pValue;
			pImage.color = temp;
			return pImage;
		}
		public static Image SetColorB(this Image pImage, float pValue)
		{
			Color temp = pImage.color;
			temp.b = pValue;
			pImage.color = temp;
			return pImage;
		}
		public static Image SetColorA(this Image pImage, float pValue)
		{
			Color temp = pImage.color;
			temp.a = pValue;
			pImage.color = temp;
			return pImage;
		}

		public static Renderer SetFlatColor(this Renderer pRenderer, Color pColor)
		{
			pRenderer.material.SetColor("_LightPositiveX", pColor);
			pRenderer.material.SetColor("_LightPositiveY", pColor);
			pRenderer.material.SetColor("_LightPositiveZ", pColor);
			return pRenderer;
		}
		public static Color GetFlatColor(this Renderer pRenderer)
		{
			return pRenderer.material.GetColor("_LightPositiveX");
		}
	}

	public static class RectTransformExtension
	{
		public static RectTransform SetAnchoredPositionX(this RectTransform pAnchor, float pValue)
		{
			Vector2 lScale = pAnchor.anchoredPosition;
			lScale.x = pValue;
			pAnchor.anchoredPosition = lScale;
			return pAnchor;
		}
		public static RectTransform SetAnchoredPositionY(this RectTransform pAnchor, float pValue)
		{
			Vector2 lScale = pAnchor.anchoredPosition;
			lScale.y = pValue;
			pAnchor.anchoredPosition = lScale;
			return pAnchor;
		}


		public static RectTransform SetSizeDeltaX(this RectTransform pAnchor, float pValue)
		{
			Vector2 lScale = pAnchor.sizeDelta;
			lScale.x = pValue;
			pAnchor.sizeDelta = lScale;
			return pAnchor;
		}
		public static RectTransform SetSizeDeltaY(this RectTransform pAnchor, float pValue)
		{
			Vector2 lScale = pAnchor.sizeDelta;
			lScale.y = pValue;
			pAnchor.sizeDelta = lScale;
			return pAnchor;
		}

		public static RectTransform SetOffsetMaxX(this RectTransform pAnchor, float pValue)
		{
			Vector2 lScale = pAnchor.offsetMax;
			lScale.x = pValue;
			pAnchor.offsetMax = lScale;
			return pAnchor;
		}
		public static RectTransform SetOffsetMaxY(this RectTransform pAnchor, float pValue)
		{
			Vector2 lScale = pAnchor.offsetMax;
			lScale.y = pValue;
			pAnchor.offsetMax = lScale;
			return pAnchor;
		}
	}

	public static class CellExtension
	{
		public static Vector3 CellToPosition(int nb, Transform planet)
		{
			int face = nb /(22*22);
			Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			Vector3[] pointeurs = new Vector3[6] {Vector3.right, Vector3.up, Vector3.forward, -Vector3.right, -Vector3.up, -Vector3.forward};

			Vector3 lign;
			Vector3 column;
			Vector3 pointeur;

			if(face > 2)	//Reverse
			{
				lign = ligns[(face + 2) % 3];
				column = columns[(face + 1) % 3];
			}
			else
			{
				lign = ligns[(face + 1) % 3];
				column = columns[(face + 2) % 3];
			}

			pointeur = pointeurs[face];

			return planet.position + pointeur*15.5f + lign*10.5f - column*10.5f + column*(nb%22) - lign*((nb-22*22*face)/22);
		}

		public static Quaternion CellToRotation(int nb)
		{
			int face = nb/(22*22);
			Quaternion rot = Quaternion.identity;

			switch(face)
			{
				//X
				case 0:
				case 3:
				rot = (face == 0) ? Quaternion.Euler(0,0,270) : Quaternion.Euler(0,0,90);
				break;

				//Y
				case 1:
				case 4:
				rot = (face == 1) ? Quaternion.identity : Quaternion.Euler(180,0,0);
				break;

				//Z
				case 2:
				case 5:
				rot = (face == 2) ? Quaternion.Euler(90,0,0) : Quaternion.Euler(270,0,0);
				break;

				default:
				Debug.LogError("Grid too big : cell " + nb);
				break;
			}

			return rot;
		}

		public static void SetOccupedCell(this Transform planet, int cell, int range)
		{
			PlanetScript script = planet.GetComponent<PlanetScript>();
			int len = script.Grid.Length;
			int cell_index;

			for(int i = -range; i <= range; i++)
			{
				for(int j = -range; j <= range; j++)
				{
					cell_index = cell + i*22 + j;
					if((i != 0 || j != 0) && cell_index > 0 && cell_index < len)
					script.Grid[cell_index] = CellEnum.Occuped;
				}
			}
		}

		public static void SetOccupedCell(this PlanetScript script, int cell, int range)
		{
			int len = script.Grid.Length;
			int cell_index;

			for(int i = -range; i <= range; i++)
			{
				for(int j = -range; j <= range; j++)
				{
					cell_index = cell + i*22 + j;
					if((i != 0 || j != 0) && cell_index > 0 && cell_index < len)
					script.Grid[cell_index] = CellEnum.Occuped;
				}
			}
		}

		public static int PositionToCell(this Transform pTransform, Transform planet)
		{
			Vector3 position = pTransform.position;

			int cell = -1;
			int a;
			int b;
			int myface = (int)FindFace(pTransform.position, planet.position);

			Vector3 vector;
			Vector3 zero;

			Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			
			Vector3 lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
			Vector3 column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

			zero = planet.position + 10.5f * lign - 10.5f * column;

			vector = Vector3.Scale((zero - position), lign);
			if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
			else
			a = Mathf.RoundToInt(vector.z);

			vector = Vector3.Scale((position - zero), column);
			if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
			else
			b = Mathf.RoundToInt(vector.z);

			if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
			cell = myface*22*22 + a*22 + b;

			return cell;
		}

		public static int PositionToCell(this Transform pTransform, Transform planet, int myface)
		{
			Vector3 position = pTransform.position;

			int cell = -1;
			int a;
			int b;

			Vector3 vector;
			Vector3 zero;

			Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			
			Vector3 lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
			Vector3 column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

			zero = planet.position + 10.5f * lign - 10.5f * column;

			vector = Vector3.Scale((zero - position), lign);
			if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
			else
			a = Mathf.RoundToInt(vector.z);

			vector = Vector3.Scale((position - zero), column);
			if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
			else
			b = Mathf.RoundToInt(vector.z);

			if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
			cell = myface*22*22 + a*22 + b;

			return cell;
		}

		public static int PositionToCell(this Transform pTransform, Transform planet, Faces myFace)
		{
			Vector3 position = pTransform.position;

			int cell = -1;
			int a;
			int b;
			int myface = (int)myFace;

			Vector3 vector;
			Vector3 zero;

			Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			
			Vector3 lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
			Vector3 column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

			zero = planet.position + 10.5f * lign - 10.5f * column;

			vector = Vector3.Scale((zero - position), lign);
			if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
			else
			a = Mathf.RoundToInt(vector.z);

			vector = Vector3.Scale((position - zero), column);
			if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
			else
			b = Mathf.RoundToInt(vector.z);

			if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
			cell = myface*22*22 + a*22 + b;

			return cell;
		}

		public static int PositionToCell(Vector3 position, Transform planet)
		{
			int cell = -1;
			int a;
			int b;
			int myface = (int)FindFace(position, planet.position);

			Vector3 vector;
			Vector3 zero;

			Vector3[] ligns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			Vector3[] columns = new Vector3[3] {Vector3.right, Vector3.up, Vector3.forward};
			
			Vector3 lign = (myface > 2) ? ligns[(myface + 2) % 3] : ligns[(myface + 1) % 3];
			Vector3 column = (myface > 2) ? columns[(myface + 1) % 3] : columns[(myface + 2) % 3];

			zero = planet.position + 10.5f * lign - 10.5f * column;

			vector = Vector3.Scale((zero - position), lign);
			if(Mathf.RoundToInt(vector.x) != 0)
			a = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			a = Mathf.RoundToInt(vector.y);
			else
			a = Mathf.RoundToInt(vector.z);

			vector = Vector3.Scale((position - zero), column);
			if(Mathf.RoundToInt(vector.x) != 0)
			b = Mathf.RoundToInt(vector.x);
			else if(Mathf.RoundToInt(vector.y) != 0)
			b = Mathf.RoundToInt(vector.y);
			else
			b = Mathf.RoundToInt(vector.z);

			if(a >= 0 && b >= 0 && a <= 21 && b <= 21)
			cell = myface*22*22 + a*22 + b;

			return cell;
		}


		private static Faces FindFace(Vector3 position, Vector3 planetPosition)
		{
			Vector3 dir = (position - planetPosition).normalized;

			int up = Mathf.RoundToInt(Vector3.Dot(dir, Vector3.up));
			int right = Mathf.RoundToInt(Vector3.Dot(dir, Vector3.right));
			int forward = Mathf.RoundToInt(Vector3.Dot(dir, Vector3.forward));

			if(up != 0)
			{
				return (up > 0) ? Faces.FaceY1 : Faces.FaceY2;
			}
			else if(right != 0)
			{
				return (right > 0) ? Faces.FaceX1 : Faces.FaceX2;
			}
			else
			{
				return (forward > 0) ? Faces.FaceZ1 : Faces.FaceZ2;
			}
		}


		public static Faces ZoneToFace(Transform zone)
		{
			Vector3 f = zone.forward;

			int up = Mathf.RoundToInt(Vector3.Dot(f, Vector3.up));
			int right = Mathf.RoundToInt(Vector3.Dot(f, Vector3.right));
			int forward = Mathf.RoundToInt(Vector3.Dot(f, Vector3.forward));

			if(up != 0)
			{
				return (up > 0) ? Faces.FaceY1 : Faces.FaceY2;
			}
			else if(right != 0)
			{
				return (right > 0) ? Faces.FaceX1 : Faces.FaceX2;
			}
			else
			{
				return (forward > 0) ? Faces.FaceZ1 : Faces.FaceZ2;
			}
		}
	}
}