using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using My.Tools;

namespace Utility.AI
{
	[CustomEditor(typeof(UtilityAIBehaviour), true)]
	public class UtilityAIBehaviourEditor : Editor
	{
		private SerializedProperty actions;
		private SerializedProperty actionRef;
		private SerializedProperty scorers;
		private SerializedProperty scoreRef;

		private GUIStyle buttonStyle;
		private Texture trashTexture;

		private string scorer;

		private string[] actionMethodNames;
		private string[] scorerMethodNames;
		private string[] scorerConditionMethodNames;
		private string[] scorerCurveMethodNames;

		private Rect baserect;
		private Rect actionrect;
		private Rect scorerect;

		private bool isCond;

		private readonly Color actionColor = new Color32(170, 170, 170, 255);
		private readonly Color scorerColor = new Color32(140, 140, 140, 255);

		// private float val = 0f;


		void OnEnable()
		{
			actions = serializedObject.FindProperty("actions");

			SerializedProperty actionCandidateNamesProperty = serializedObject.FindProperty("actionCandidates");
			SerializedProperty scorerConditionCandidateNamesProperty = serializedObject.FindProperty("scorerConditionCandidates");
			SerializedProperty scorerCurveCandidateNamesProperty = serializedObject.FindProperty("scorerCurveCandidates");

			this.actionMethodNames = InitializeActionCandidateList(serializedObject.targetObject.GetType(), actionCandidateNamesProperty);

			this.scorerCurveMethodNames = InitializeScorerCandidateList<float>(serializedObject.targetObject.GetType(), scorerCurveCandidateNamesProperty);
			this.scorerConditionMethodNames = InitializeScorerCandidateList<bool>(serializedObject.targetObject.GetType(), scorerConditionCandidateNamesProperty);

			List<string> temp = new List<string>();
			temp.AddRange(this.scorerConditionMethodNames);
			foreach(string s in this.actionMethodNames) {
				temp.Add(s + " is running");
			}
			
			this.scorerConditionMethodNames = temp.ToArray();
			temp.Clear();
			temp.AddRange(this.scorerCurveMethodNames);
			temp.AddRange(this.scorerConditionMethodNames);
			
			this.scorerMethodNames = temp.ToArray();

			this.trashTexture = EditorGUIUtility.FindTexture("d_TreeEditor.Trash");
		}

		public sealed override void OnInspectorGUI()
		{
			UtilityAIBehaviour script = target as UtilityAIBehaviour;

			// Rect n = new Rect();
			// n.x = baserect.x + 50;
			// n.y += 500;
			// n.width = 200;
			// n.height = 16;
			// val = EditorGUI.FloatField(n, "Val", val);

			DrawDefaultInspector();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Utility AI", EditorStyles.boldLabel);
			baserect = EditorGUILayout.GetControlRect(true, 0);

			actionrect = baserect;
			actionrect.height += 17;
			EditorGUI.LabelField(actionrect, "Update Interval", EditorStyles.miniLabel);

			actionrect.x += 90;
			actionrect.width += -95;
			script.updateRate = EditorGUI.Slider(actionrect, script.updateRate, 0.02f, 1f); 

			buttonStyle = new GUIStyle("Button");

			// First action position
			baserect.y += 39;
			baserect.width += -5;
			baserect.height += 49;

			// Actions field
			for(int act = 0; act < actions.arraySize; ++act)
			{
				actionRef = actions.GetArrayElementAtIndex(act);
				scorers = actionRef.FindPropertyRelative("scorers");

				// Background
				actionrect = baserect;
				actionrect.y += -18;
				actionrect.height += -1;
				EditorGUI.DrawRect(actionrect, actionColor);
				
				// Action field
				actionrect = baserect;
				actionrect.x += 8;
				actionrect.y += -12;
				actionrect.width += -35;
				actionrect.height = 14;
				this.ActionMethodField(actionrect, actionRef);

				// Trash field
				actionrect = baserect;
				actionrect.width = 16;
				actionrect.height = 16;
				actionrect.x += baserect.width + -23;
				actionrect.y += -13;
				if(EditorApplication.isPlaying){GUI.enabled = false;}
				if(GUI.Button(actionrect, this.trashTexture, GUIStyle.none)) {
					script.displayScorers.RemoveAt(act);
					script.displayParameters.RemoveAt(act);
					script.RemoveActionAt(act);
					serializedObject.Update();
					return;
				}
				if(EditorApplication.isPlaying){GUI.enabled = true;}

				// Arrow scorers field
				actionrect = baserect;
				actionrect.x += 19;
				actionrect.y += 8;
				actionrect.width = 0;
				script.displayScorers[act] = EditorGUI.Foldout(actionrect, script.displayScorers[act], "Scorers");
				if(script.displayScorers[act]) {
					script.displayParameters[act] = false;
				}

				// Arrow parameters field
				actionrect.x += baserect.width + -142;
				actionrect.width = 97;
				actionrect.height = 16;
				this.buttonStyle.fontSize = 9;
				script.displayParameters[act] = GUI.Toggle(actionrect, script.displayParameters[act], "show parameters", this.buttonStyle);
				if(script.displayParameters[act]) {
					script.displayScorers[act] = false;
				}

				// Parameters fields
				if(script.displayParameters[act])
				{
					actionrect = baserect;

					// Background
					actionrect.y += 30;
					actionrect.height = 30;
					EditorGUI.DrawRect(actionrect, actionColor);

					scorerect = actionrect;
					scorerect.x += 8;
					scorerect.y += -2;
					scorerect.width += -16;
					scorerect.height += -4;
					EditorGUI.DrawRect(scorerect, scorerColor);

					buttonStyle.fontSize = 9;
					actionrect.height = 15;
					actionrect.y += 3;

					if(EditorApplication.isPlaying){GUI.enabled = false;}
					actionrect.width = 64;
					actionrect.x += baserect.width + -239;
					script.actions[act].isStoppable = GUI.Toggle(actionrect, script.actions[act].isStoppable, "stoppable", buttonStyle);

					actionrect.x += actionrect.width + 1;
					actionrect.width = 80;
					script.actions[act].isParallelizable = GUI.Toggle(actionrect, script.actions[act].isParallelizable, "parallelizable", buttonStyle);

					actionrect.x += actionrect.width + 1;
					actionrect.width = 79;
					script.actions[act].isForceAlone = GUI.Toggle(actionrect, script.actions[act].isForceAlone, "force alone", buttonStyle);
					if(EditorApplication.isPlaying){GUI.enabled = true;}

					// Set position for next action with scorers displayed
					baserect.y += 30;
				}
				// Scorers fields
				else if(script.displayScorers[act])
				{
					actionrect = baserect;

					// Background
					actionrect.y += 30;
					actionrect.height = (scorers.arraySize > 0) ? 49 * scorers.arraySize + 25 : 22;
					EditorGUI.DrawRect(actionrect, actionColor);

					// Display scorers fields
					for(int sco = 0; sco < scorers.arraySize; ++sco)
					{
						scorerect = actionrect;
						scoreRef = scorers.GetArrayElementAtIndex(sco);

						// Background
						scorerect.x += 8;
						scorerect.y += -2;
						scorerect.width += -16;
						scorerect.height = 46;
						EditorGUI.DrawRect(scorerect, scorerColor);

						// Select scorer field
						scorerect.x += 4;
						scorerect.y += 5;
						scorerect.width += -27;
						scorerect.height = 15;
						if(EditorApplication.isPlaying){GUI.enabled = false;}
						scorer = this.ScorerMethodField(scorerect, scoreRef);
						if(EditorApplication.isPlaying){GUI.enabled = true;}
						isCond = this.IsScorerCondition(scorer);
						script.actions[act].scorers[sco].isCondition = isCond;

						scorerect.y += 21;

						// Display condition parameters field
						if(isCond)
						{
							EditorGUI.LabelField(scorerect, "Score");
							scorerect.x += 66;
							scorerect.width += -48;
							script.actions[act].scorers[sco].score = EditorGUI.IntField(scorerect, script.actions[act].scorers[sco].score);

							// 'Not' option field
							buttonStyle.fontSize = 10;
							scorerect.y += -21;
							scorerect.width = 31;
							scorerect.height = 15;
							if(EditorApplication.isPlaying){GUI.enabled = false;}
							script.actions[act].scorers[sco].not = GUI.Toggle(scorerect, script.actions[act].scorers[sco].not, "Not", buttonStyle);
							if(EditorApplication.isPlaying){GUI.enabled = true;}
						}
						// Display curve parameters field
						else
						{
							EditorGUI.LabelField(scorerect, "Curve");
							scorerect.x += 66;
							scorerect.width += -48;

							if(script.actions[act].scorers[sco].curve == null) {
								script.actions[act].scorers[sco].curve = new AnimationCurve();
							}

							script.actions[act].scorers[sco].curve = EditorGUI.CurveField(scorerect, script.actions[act].scorers[sco].curve);
							scorerect.y += -21;
						}

						// Trash button field
						scorerect.x = actionrect.width + -13;
						scorerect.width = 15;
						scorerect.height = 15;
						if(EditorApplication.isPlaying){GUI.enabled = false;}
						if(GUI.Button(scorerect, this.trashTexture, GUIStyle.none)) {
							script.actions[act].RemoveScorerAt(sco);
							serializedObject.Update();
							return;
						}
						if(EditorApplication.isPlaying){GUI.enabled = true;}

						// Set position for next scorer
						actionrect.y += 49;
					}

					// Add scorer button field
					actionrect.x += 8;
					actionrect.y += (scorers.arraySize > 0) ? 0 : -2;
					actionrect.width = baserect.width + -15;
					actionrect.height = 18;
					if(EditorApplication.isPlaying){GUI.enabled = false;}
					buttonStyle.fontSize = 10;
					if(GUI.Button(actionrect, "ADD NEW SCORER", buttonStyle))
					{
						int nb = script.actions[act].scorers.Count;

						if(this.scorerMethodNames.Length == nb) {
							Debug.Log($"No need to add another scorer, there is enough ! (Number of scorer found: {nb})");
						} else if(this.scorerMethodNames.Length == 0) {
							Debug.Log($"There is no scorer yet !");
						} else {
							int scorerIndex = this.GetNextAvailableScorer(script.actions[act].scorers);
							script.actions[act].AddScorer(this.scorerMethodNames[scorerIndex], this.IsScorerCondition(scorer), scorerIndex);
							
							serializedObject.Update();
						}
					}
					if(EditorApplication.isPlaying){GUI.enabled = true;}

					// Set position for next action with scorers displayed
					baserect.y += (scorers.arraySize > 0) ? scorers.arraySize * 49 + 25 : 22;
				}

				// Set position for next action
				baserect.y += 52;
			}

			baserect.y += -16;
			baserect.width += 1;
			actionrect = baserect;
			actionrect.height = 23;
			if(EditorApplication.isPlaying){GUI.enabled = false;}
			if(GUI.Button(actionrect, "ADD NEW ACTION")) {
				if(actionMethodNames.Length == script.actions.Count) {
					Debug.Log($"There is no more action to add ! (Number of action found: {actionMethodNames.Length})");
				} else {
					script.displayScorers.Add(false);
					script.displayParameters.Add(false);
					script.AddAction(this.actionMethodNames[script.actions.Count], script.actions.Count);
					serializedObject.Update();
				}
			}
			if(EditorApplication.isPlaying){GUI.enabled = true;}


			if (EditorGUI.EndChangeCheck()){
				script.UpdateAllMaxCache();
			}
			EditorUtilityExtension.SetDirtyOnGUIChange(script);
			serializedObject.ApplyModifiedProperties();
		}














		private void ActionMethodField(Rect pos, SerializedProperty properties)
		{
			SerializedProperty methodNameProperty = properties.FindPropertyRelative("method");
			SerializedProperty indexProperty = properties.FindPropertyRelative("index");
			SerializedProperty activeProperty = properties.FindPropertyRelative("active");

			float w = pos.width;
			pos.width = 15;
			activeProperty.boolValue = EditorGUI.Toggle(pos, activeProperty.boolValue);

			pos.x += 15;
			pos.width += w - 29;
			pos.height = 15;

			EditorGUI.LabelField(pos, "Action");

			// place holder when no candidates are available
			if(actionMethodNames.Length == 0) {
				EditorGUI.LabelField(pos, "No action found !");
				return;
			}

			pos.x += 55;
			pos.width += -55;

			if(EditorApplication.isPlaying){GUI.enabled = false;}
			// select method from candidates
			indexProperty.intValue = EditorGUI.Popup(pos, indexProperty.intValue, actionMethodNames);
			if(indexProperty.intValue < actionMethodNames.Length) {
				methodNameProperty.stringValue = actionMethodNames[indexProperty.intValue];
			} else {
				indexProperty.intValue = Array.IndexOf(actionMethodNames, methodNameProperty.stringValue);
			}
			if(EditorApplication.isPlaying){GUI.enabled = true;}
		}

		private string ScorerMethodField(Rect pos, SerializedProperty properties)
		{
			SerializedProperty methodNameProperty = properties.FindPropertyRelative("method");
			SerializedProperty indexProperty = properties.FindPropertyRelative("index");

			if(this.IsScorerCondition(methodNameProperty.stringValue)) {
				EditorGUI.LabelField(pos, "Condition");
				pos.x += 98;
				pos.width += -98;
				indexProperty.intValue = EditorGUI.Popup(pos, indexProperty.intValue, this.scorerMethodNames);
			} else {
				EditorGUI.LabelField(pos, "Mapper");
				pos.x += 65;
				pos.width += -65;
				indexProperty.intValue = EditorGUI.Popup(pos, indexProperty.intValue, this.scorerMethodNames);
			}

			if(indexProperty.intValue < this.scorerMethodNames.Length) {
				methodNameProperty.stringValue = this.scorerMethodNames[indexProperty.intValue];
			} else {
				indexProperty.intValue = Array.IndexOf(this.scorerMethodNames, methodNameProperty.stringValue);
			}

			return methodNameProperty.stringValue;
		}





		private string[] InitializeActionCandidateList(System.Type type, SerializedProperty candidateNamesProperty)
		{
			System.Type[] paramTypes_1 = new System.Type[] {typeof(MovementController)};
			System.Type[] paramTypes_2 = new System.Type[] {typeof(MovementController), typeof(UtilityAction)};
			IList<MemberInfo> candidateList = new List<MemberInfo>();
			string[] candidateNames;
			string[] blacklist = new string[] {"AddAction", "CancelInvoke", "StopAllCoroutines"};
			int i = 0;

			type.FindMembers(
				MemberTypes.Method,
				BindingFlags.Instance | BindingFlags.Public,
				(member, criteria) => {
					MethodInfo method;
					if((((method = type.GetMethod(member.Name, paramTypes_1)) != null) || ((method = type.GetMethod(member.Name, paramTypes_2)) != null))
					&& (method.ReturnType == typeof(void) || method.ReturnType == typeof(IEnumerator))
					&& Array.IndexOf(blacklist, method.Name) == -1)
					{
						candidateList.Add(method);
						return true;
					}
					return false;
				},
				null
			);

			// clear/resize/initialize storage containers
			candidateNamesProperty.ClearArray();
			candidateNamesProperty.arraySize = candidateList.Count;
			candidateNames = new string[candidateList.Count];

			// assign storage containers
			i = 0;
			foreach(SerializedProperty element in candidateNamesProperty) {
				element.stringValue = candidateNames[i] = candidateList[i++].Name;
			}
			
			return candidateNames;
		}

		private string[] InitializeScorerCandidateList<T>(System.Type type, SerializedProperty candidateNamesProperty)
		{
			System.Type[] paramTypes = new System.Type[] {typeof(MovementController)};
			IList<MemberInfo> candidateList = new List<MemberInfo>();
			string[] candidateNames;
			string[] blacklist = new string[] {"IsInvoking", "Equals", "get_useGUILayout", "get_runInEditMode", "get_enabled", "get_isActiveAndEnabled"};
			int i = 0;

			type.FindMembers(
				MemberTypes.Method,
				BindingFlags.Instance | BindingFlags.Public,
				(member, criteria) => {
					MethodInfo method;
					if((method = type.GetMethod(member.Name, paramTypes)) != null
						&& method.ReturnType == typeof(T)
						&& Array.IndexOf(blacklist, method.Name) == -1) {
						candidateList.Add(method);
						return true;
					}
					return false;
				},
				null
			);

			// clear/resize/initialize storage containers
			candidateNamesProperty.ClearArray();
			candidateNamesProperty.arraySize = candidateList.Count;
			candidateNames = new string[candidateList.Count];

			// assign storage containers
			i = 0;
			foreach(SerializedProperty element in candidateNamesProperty) {
				element.stringValue = candidateNames[i] = candidateList[i++].Name;
			}
			
			return candidateNames;
		}



		private int GetNextAvailableScorer(List<UtilityScorer> ls)
		{
			for(int i = 0; i < this.scorerMethodNames.Length; ++i) {
				if(!ls.Exists(x => x.method.Equals(this.scorerMethodNames[i]))) {
					return i;
				}
			}

			return 0;
		}

		private bool IsScorerCondition(string name)
		{
			for(int i = 0; i < this.scorerConditionMethodNames.Length; ++i) {
				if(this.scorerConditionMethodNames[i].Equals(name)) {
					return true;
				}
			}

			return false;
		}
	}
}



