using static UnityEngine.Debug;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;


namespace Interactive.Engine
{
	// Max 32 elements
	public enum ChemicalElement
	{
		// STANDARD
		Void = 1,
		Fire = 2,
		Water = 4,
		Wind = 8,
		Earth = 16,
		Lightning = 32,

		// MIX
		Ice = 64,
		Magma = 128,
		Steam = 256,
		Snow = 512,
		Sand = 1024
	}


	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------- STANDARD ELEMENT --------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public class Void : ChemicalElementEntity {
		public Void(float i = 0f) : base(ChemicalElement.Void, i, Void.weakness) {}

		protected internal static ChemicalElement[] weakness = Enum.GetValues(typeof(ChemicalElement)) as ChemicalElement[];
	}

	public class Fire : ChemicalElementEntity {
		public Fire(float i = 0f) : base(ChemicalElement.Fire, i, Fire.weakness) {}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Water,
			ChemicalElement.Lightning
		};
	}

	public class Wind : ChemicalElementEntity {
		public Wind(float i = 0f) : base(ChemicalElement.Wind, i, Wind.weakness) {}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Fire,
			ChemicalElement.Water
		};
	}

	public class Earth : ChemicalElementEntity {
		public Earth(float i = 0f) : base(ChemicalElement.Earth, i, Earth.weakness) {}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Wind,
			ChemicalElement.Fire
		};
	}

	public class Lightning : ChemicalElementEntity {
		public Lightning(float i = 0f) : base(ChemicalElement.Lightning, i, Lightning.weakness) {}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Earth,
			ChemicalElement.Wind
		};
	}

	public class Water : ChemicalElementEntity {
		public Water(float i = 0f) : base(ChemicalElement.Water, i, Water.weakness) {}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Lightning,
			ChemicalElement.Earth
		};
	}






	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* --------------------------------------- MIX ELEMENTS ----------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public class Magma : ChemicalElementMixEntity {
		public Magma(float i = 0f) : base(ChemicalElement.Magma, i, Magma.combo) {}

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Earth,
			ChemicalElement.Fire
		};
	}

	public class Steam : ChemicalElementMixEntity {
		public Steam(float i = 0f) : base(ChemicalElement.Steam, i, Steam.combo) {}

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Fire,
			ChemicalElement.Water
		};	
	}

	public class Ice : ChemicalElementMixEntity {
		public Ice(float i = 0f) : base(ChemicalElement.Ice, i, Ice.combo) {}

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Wind,
			ChemicalElement.Water
		};
	}

	public class Snow : ChemicalElementMixEntity {
		public Snow(float i = 0f) : base(ChemicalElement.Snow, i, Snow.combo) {}

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Steam,
			ChemicalElement.Ice
		};
	}

	public class Sand : ChemicalElementMixEntity {
		public Sand(float i = 0f) : base(ChemicalElement.Sand, i, Sand.combo) {}

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Earth,
			ChemicalElement.Ice
		};
	}

















	/* ----------------------------------------------------------------------------------------------*/
	/* ----------------------------------------------------------------------------------------------*/
	/* ----------------------------------------------------------------------------------------------*/
	/* ----------------------------- DO NOT LOOK AT THESE, IT IS SCARY ------------------------------*/
	/* ----------------------------------------------------------------------------------------------*/
	/* ----------------------------------------------------------------------------------------------*/
	/* ----------------------------------------------------------------------------------------------*/
	public abstract class ChemicalElementEntity
	{
		public readonly ChemicalElement type;

		private protected InteractiveEngine interactiveEngine;

		private protected static object[] STANDARD_PARAMS = new object[] {0f};

		private const float MIN_INTENSITY = 0f;
		private const float MAX_INTENSITY = 100f;
		private float _intensity = MIN_INTENSITY;
		public float intensity {
			get { return this._intensity; }
			set { this._intensity = (value > MAX_INTENSITY) ? MAX_INTENSITY : ((value < MIN_INTENSITY) ? MIN_INTENSITY : value); }
		}

		public ChemicalElementEntity(ChemicalElement e, float i, ChemicalElement[] weaknesses) {
			this.type = e;
			this.intensity = i;

			this.interactiveEngine = InteractiveEngine.instance;

			this.SetPrimariesAndWeaknesses(weaknesses);
		}

		private protected virtual void SetPrimariesAndWeaknesses(ChemicalElement[] weaknesses) {
			// Set weaknesses
			if(!this.interactiveEngine.HasWeaknessesOf(this)) {
				this.interactiveEngine.SetWeaknessesOf(this, weaknesses);
			}

			// Set elements composition
			if(!this.interactiveEngine.HasPrimariesOf(this)) {
				this.interactiveEngine.SetPrimariesOf(this, new ChemicalElement[] {this.type});
			}
		}

		private bool IsStrongAgainst(ChemicalElementEntity other) {
			if(this.interactiveEngine.HasWinnerBetween(this, other)) {
				return this.interactiveEngine.IsWinningAgainst(this, other);
			}

			int mywin, hiswin;
			mywin = hiswin = 0;

			foreach(ChemicalElement w in this.interactiveEngine.GetPrimariesOf(this)) {
				foreach(ChemicalElement e in this.interactiveEngine.GetPrimariesOf(other)) {
					if(w == e) {
						hiswin ++;
					}
				}
			}
			foreach(ChemicalElement w in this.interactiveEngine.GetPrimariesOf(other)) {
				foreach(ChemicalElement e in this.interactiveEngine.GetPrimariesOf(this)) {
					if(w == e) {
						mywin ++;
					}
				}
			}

			bool isMainWinning = (mywin != hiswin && mywin > hiswin);
			this.interactiveEngine.SetWinnerBetween(this, other, isMainWinning);

			return isMainWinning;
		}

		public override string ToString() => $"{this.type}";



		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------- * OPERATOR -----------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		// ELEMENT * ELEMENT
		public static ChemicalElementEntity operator *(ChemicalElementEntity a, ChemicalElementEntity b) {
			if(a.type == b.type) {
				a.intensity = b.intensity = a.intensity + b.intensity;
				return a;
			}
			ChemicalElementEntity m = ChemicalElementMixEntity.MixTwoElement(a, b);
			if(m != null) {
				return m;
			}
			return ChemicalElementEntity.GetWinnerBetween(a, b);
		}

		// ELEMENT * MATERIAL
		public static ChemicalElementEntity operator *(ChemicalMaterialEntity a, ChemicalElementEntity b) => b * a;
		public static ChemicalElementEntity operator *(ChemicalElementEntity a, ChemicalMaterialEntity b) {
			foreach(ChemicalElement e in b.vulnerabilities) {
				if(a.type == e) {
					return a;
				}
			}
			return new Void();
		}



		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* -------------------------------------- STATIC METHODS ---------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/		
		private protected static ChemicalElementEntity GetWinnerBetween(ChemicalElementEntity a, ChemicalElementEntity b) {
			if(a.IsStrongAgainst(b)) {
				return a;
			} else if(b.IsStrongAgainst(a)) {
				return b;
			} else if(a.intensity != b.intensity) {
				return (a.intensity > b.intensity) ? a : b;
			} else {
				return new Void();
			}
		}
	}













	public abstract class ChemicalElementMixEntity : ChemicalElementEntity
	{
		private protected ChemicalElementMixEntity(ChemicalElement e, float i, ChemicalElement[] recipe) : base(e, i, recipe) {}

		private protected override void SetPrimariesAndWeaknesses(ChemicalElement[] recipe) {
			// Set weaknesses according to recipe
			if(!this.interactiveEngine.HasWeaknessesOf(this)) {
				this.interactiveEngine.SetWeaknessesOf(this, GetWeaknessesByDecomposition(recipe));
			}

			// Set elements composition by decomposing recipe in its primary element
			if(!this.interactiveEngine.HasPrimariesOf(this)) {
				this.interactiveEngine.SetPrimariesOf(this, GetPrimariesByDecomposition(recipe));
			}
		}

		// does elements in 'a' and 'b' validate this primary element composition
		protected internal bool CouldBeMadeOf(ChemicalElementEntity a, ChemicalElementEntity b) {
			ChemicalElement[] a1 = this.interactiveEngine.GetPrimariesOf(a);
			ChemicalElement[] a2 = this.interactiveEngine.GetPrimariesOf(b);
			bool found;

			foreach(ChemicalElement e in this.interactiveEngine.GetPrimariesOf(this)) {
				found = false;

				// search in first array
				foreach(ChemicalElement k in a1) {
					if(e == k) {
						found = true;
						break;
					}
				}
				// if not found in first array then
				if(!found) {
					// search in second array
					foreach(ChemicalElement k in a2) {
						if(e == k) {
							found = true;
							break;
						}
					}
				}
				// if not found in either one of them
				if(!found) {
					return false;
				}
			}
			return true;
		}

		

		









		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* -------------------------------------- STATIC METHODS ---------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		private static ChemicalElementMixEntity[] _mixes = null;
		private static ChemicalElementMixEntity[] mixes {
			get {
				if(_mixes == null) {
					_mixes = GetAllMixes();
				}
				return _mixes;
			}
		}

		private static ChemicalElementEntity Spawn(ChemicalElementEntity e) {
			// only does that
			return Activator.CreateInstance(e.GetType(), ChemicalElementMixEntity.STANDARD_PARAMS) as ChemicalElementEntity;
		}

		protected internal static ChemicalElementEntity MixTwoElement(ChemicalElementEntity a, ChemicalElementEntity b) {
			if(a.type == ChemicalElement.Void) {
				return b;
			} else if(b.type == ChemicalElement.Void) {
				return a;
			}

			if(InteractiveEngine.instance.HasMixOf(a, b)) {
				string name = InteractiveEngine.instance.GetMixOf(a, b);
				if(name == null) {
					return null;
				}
				Type t = Type.GetType(name);
				return Activator.CreateInstance(t, ChemicalElementMixEntity.STANDARD_PARAMS) as ChemicalElementEntity;
			}

			List<ChemicalElementMixEntity> candidates = null;
			ChemicalElementEntity winner = null;

			candidates = InteractiveEngine.instance.chemicalElementMixEntityPoolList;

			foreach(ChemicalElementMixEntity mix in ChemicalElementMixEntity.mixes) {
				if(mix.type != a.type && mix.type != b.type && mix.CouldBeMadeOf(a, b)) {
					candidates.Add(mix);
				}
			}

			if(candidates.Count > 0) {
				winner = candidates[0];
				for(int i = 1; i < candidates.Count; i++) {
					winner = GetWinnerBetween(winner, candidates[i]);
				}
			}

			candidates.Clear();

			InteractiveEngine.instance.SetMixOf(a, b, winner);

			return (winner != null) ? Spawn(winner) : null;
		}

		// get composition by decomposing 'recipe' in its primary element
		private static ChemicalElement[] GetPrimariesByDecomposition(ChemicalElement[] recipe) {
			ChemicalElement[] es0 = InternalGetPrimariesOf(recipe[0]);
			ChemicalElement[] es1 = InternalGetPrimariesOf(recipe[1]);
			List<ChemicalElement> cache = InteractiveEngine.instance.chemicalElementPoolList;
			ChemicalElement[] result;

			foreach(ChemicalElement e in es0) {
				if(!cache.Contains(e)) {
					cache.Add(e);
				}
			}
			foreach(ChemicalElement e in es1) {
				if(!cache.Contains(e)) {
					cache.Add(e);
				}
			}

			result = cache.ToArray();
			cache.Clear();

			return result;
		}

		// get weaknesses according to recipe element weaknesses
		private static ChemicalElement[] GetWeaknessesByDecomposition(ChemicalElement[] recipe) {
			ChemicalElement[] wk0 = InternalGetWeaknessOf(recipe[0]);
			ChemicalElement[] wk1 = InternalGetWeaknessOf(recipe[1]);
			ChemicalElement[] wk = wk0.Concat(wk1).ToArray();
			List<ChemicalElement> cache = InteractiveEngine.instance.chemicalElementPoolList;
			ChemicalElement[] result;
			bool found;

			foreach(ChemicalElement e in wk) {
				// do not add weakness which are part of our recipe !
				found = false;
				foreach(ChemicalElement a in recipe) {
					if(e == a) {
						found = true;
						break;
					}
				}

				if(!found && !cache.Contains(e)) {
					cache.Add(e);
				}
			}

			result = cache.ToArray();
			cache.Clear();

			return result;
		}

		// get primaries element of a particular element
		private static ChemicalElement[] InternalGetPrimariesOf(ChemicalElement e) {
			Type t = Type.GetType(typeof(ChemicalElementEntity).Namespace + "." + e.ToString());

			if(!t.IsSubclassOf(typeof(ChemicalElementMixEntity))) {
				return new ChemicalElement[] {e};
			}

			ChemicalElementMixEntity ent = Activator.CreateInstance(t, ChemicalElementMixEntity.STANDARD_PARAMS) as ChemicalElementMixEntity;

			return InteractiveEngine.instance.GetPrimariesOf(ent);
		}

		// get weaknesses of a particular element
		private static ChemicalElement[] InternalGetWeaknessOf(ChemicalElement e) {
			Type t = Type.GetType(typeof(ChemicalElementEntity).Namespace + "." + e.ToString());

			if(!t.IsSubclassOf(typeof(ChemicalElementMixEntity))) {
				return t.GetField("weakness", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as ChemicalElement[];
			}

			ChemicalElementMixEntity ent = Activator.CreateInstance(t, ChemicalElementMixEntity.STANDARD_PARAMS) as ChemicalElementMixEntity;

			return InteractiveEngine.instance.GetWeaknessesOf(ent);
		}

		private static ChemicalElementMixEntity[] GetAllMixes() {
			Type t;
			Type[] ts;
			ChemicalElementMixEntity[] res;

			t = typeof(ChemicalElementMixEntity);
			ts = Assembly.GetAssembly(t).GetTypes().Where(myType => myType.IsSubclassOf(t) && myType.IsClass && !myType.IsAbstract).ToArray();
			res = new ChemicalElementMixEntity[ts.Length];

			for(int i = 0; i < ts.Length; i++) {
				res[i] = Activator.CreateInstance(ts[i], ChemicalElementMixEntity.STANDARD_PARAMS) as ChemicalElementMixEntity;
			}

			return res;
		}
	}
}
