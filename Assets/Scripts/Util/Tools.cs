﻿using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Tools
{
	public class _Transform
	{
		public Vector3 right { get { return this.rotation * Vector3.right; }}
		public Vector3 up { get { return this.rotation * Vector3.up; }}
		public Vector3 forward { get { return this.rotation * Vector3.forward; }}

		public Vector3 position { get; set; }
		public Quaternion rotation { get; set; }

		public _Transform() {
			this.position = Vector3.zero;
			this.rotation = Quaternion.identity;
		}

		public _Transform(Transform t) {
			this.Copy(t);
		}

		public _Transform(Vector3 pos, Quaternion rot) {
			this.position = pos;
			this.rotation = rot;
		}

		public void Copy(Transform tr) {
			this.position = tr.position;
			this.rotation = tr.rotation;
		}

		public void Copy(_Transform tr) {
			this.position = tr.position;
			this.rotation = tr.rotation;
		}
	}



	public static class LayerMaskExtension
	{
		public static bool IsInLayerMask(this int layerMask, int layer) => (layerMask == (layerMask | (1 << layer)));

		public static bool IsInLayerMask(this LayerMask layerMask, int layer) => layerMask.value.IsInLayerMask(layer);

		public static LayerMask Create(params string[] layerNames) => NamesToMask(layerNames);
	 
		public static LayerMask Create(params int[] layerNumbers) => LayerNumbersToMask(layerNumbers);
	 
		public static LayerMask NamesToMask(params string[] layerNames)
		{
			LayerMask ret = (LayerMask)0;
			foreach(var name in layerNames) {
				ret |= (1 << LayerMask.NameToLayer(name));
			}
			return ret;
		}
	 
		public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
		{
			LayerMask ret = (LayerMask)0;
			foreach(var layer in layerNumbers) {
				ret |= (1 << layer);
			}
			return ret;
		}

		public static string[] UserLayerMask()
		{
			string name;
			List<string> output = new List<string>();

			for(int i = 8; i < 32; i++) {
				name = LayerMask.LayerToName(i);
				if(name.Length > 0) {
					output.Add(name);
				}
			}

			return output.ToArray();
		}
	 
		public static LayerMask Inverse(this LayerMask original) => ~original;
	 
		public static LayerMask AddToMask(this LayerMask original, params string[] layerNames) => original | NamesToMask(layerNames);
	 
		public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
		{
			LayerMask invertedOriginal = ~original;
			return ~(invertedOriginal | NamesToMask(layerNames));
		}

		public static string[] MaskToNames(this LayerMask original)
		{
			string layerName;
			int shifted;
			List<string> output = new List<string>();

			for (int i = 0; i < 32; ++i)
			{
				shifted = 1 << i;
				if((original & shifted) == shifted) {
					layerName = LayerMask.LayerToName(i);
					if(!string.IsNullOrEmpty(layerName)) {
						output.Add(layerName);
					}
				}
			}

			return output.ToArray();
		}

		public static string[] MaskToNames(this LayerMask layers, string[] from)
		{
			string layerName;
			int shifted;
			List<string> output = new List<string>();

			for (int i = 0; i < from.Length; ++i)
			{
				shifted = 1 << i;
				if((layers & shifted) == shifted) {
					layerName = from[i];
					if(!string.IsNullOrEmpty(layerName)) {
						output.Add(layerName);
					}
				}
			}

			return output.ToArray();
		}

		public static string MaskToString(this LayerMask original) => MaskToString(original, ", ");
 
		public static string MaskToString(this LayerMask original, string delimiter) => string.Join(delimiter, MaskToNames(original));

		public static string[] AllLayerNames()
		{
			string[] all = new string[32];
			for(int i = 0; i < 32; i++) {
				all[i] = LayerMask.LayerToName(i);
			}
			return all;
		}

		public static LayerMask CastTo(this LayerMask current, string[] from, string[] to)
		{
			string[] masks;
			LayerMask result;

			masks = current.MaskToNames(from);
			result = (LayerMask)0;

			for(int i = 0; i < masks.Length; i++) {
				result |= (1 << System.Array.IndexOf(to, masks[i]));
			}

			return result;
		}
	}



	public static class EditorGUILayoutExtension
	{
		public static LayerMask UserMaskField(string label, LayerMask current)
		{
			string[] users;

			users = LayerMaskExtension.UserLayerMask();

			return EditorGUILayoutExtension.MappedMaskField(label, current, users);
		}

		public static LayerMask MappedMaskField(string label, LayerMask current, string[] map)
		{
			string[] all;
			LayerMask mask;

			if(map.Length == 0) {
				return (LayerMask)0;
			}

			all = LayerMaskExtension.AllLayerNames();

			// "current" comes from all existing layers -> 'casted' to map specified layers
			mask = current.CastTo(all, map);
			// display map specified layers, can select them
			mask = EditorGUILayout.MaskField(label, mask, map);
			// "mask" is on map layers -> 'cast' it on all existing layers
			mask = mask.CastTo(map, all);

			return mask;
		}
		public static LayerMask MappedMaskField(string label, LayerMask current, LayerMask layers)
		{
			string[] map;

			map = layers.MaskToNames(LayerMaskExtension.AllLayerNames());

			return EditorGUILayoutExtension.MappedMaskField(label, current, map);
		}
	}



	public static class CoroutineExtension
	{
		public static void StartAndStopCoroutine(this MonoBehaviour mono, ref IEnumerator handler, IEnumerator coroutine)
		{
			if(handler != null) {
				mono.StopCoroutine(handler);
			}
			handler = coroutine;
			mono.StartCoroutine(handler);
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
		public static Transform DeepFind<T>(this Transform root, string n) where T : Component
		{
			T[] components = root.GetComponentsInChildren<T>();

			foreach(T c in components) {
				if(c.gameObject.name == n) {
					return c.transform;
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
		public static void Split<T>(this T[] array, int index, out T[] first, out T[] second)
		{
			first = array.Take(index).ToArray();
			second = array.Skip(index).ToArray();
		}

		public static void Split<T>(this T[] array, out T[] first, out T[] second)
		{
			ArrayExtension.Split(array, array.Length / 2, out first, out second);
		}

		public static T[] DeepCopy<T>(T[] array)
		{
			T[] a = new T[array.Length];
			for(int i = 0; i < array.Length; i++) {
				a[i] = array[i];
			}
			return a;
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




	public static class RendererExtensions
	{
		public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
		}
	}
}