using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveGameFree
{
	[Serializable]
	public class PlayerData
	{
		public bool HasSaved = false;
		public bool Reboot = false;
		
		[HideInInspector]
		public int[] Grids;

		public int Score = 0;
		public int BodyNumber = 0;
		public int RabbitKilled = 0;
		public int Lives = 10;
		public int Thoughts = -1;
		public int LooneyFace = 4;

		public float TimeBeforeThoughts = 0.0f;

		public Vector3 Position = Vector3.zero;
		public Quaternion Rotation = Quaternion.identity;
		
		public Quaternion HeartLocalRotation = Quaternion.identity;

		public Vector3 ArmchairPosition = Vector3.zero;
		public Quaternion ArmchairRotation = Quaternion.identity;


		public int myFace = 0;
		public string MainPlanet = "";
		public string OldPlanet = "";
		
		public int NumberOfFaceDestroyed = 0;
		public string[] NameOfFaceDestroyed;
	}
}