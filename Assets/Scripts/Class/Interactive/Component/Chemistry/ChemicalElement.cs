using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;


namespace Interactive.Engine
{
	public enum ChemicalElement
	{
		Voidd = 0,
		Fire,
		Water,
		Wind,
		Earth,
		Ice,
		Lightning,
		Magma,
		Steam,
		Snow,
		Sand
	}


	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ------------------------------------- STANDARD ELEMENT --------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	/* ---------------------------------------------------------------------------------------------*/
	public class Voidd : ChemicalElementEntity {
		public Voidd() : base(ChemicalElement.Voidd, 0f) {this.weaknesses = Voidd.weakness;}

		protected internal static ChemicalElement[] weakness = Enum.GetValues(typeof(ChemicalElement)) as ChemicalElement[];
	}

	public class Fire : ChemicalElementEntity {
		public Fire(float i = 0f) : base(ChemicalElement.Fire, i) {this.weaknesses = Fire.weakness;}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Water,
			ChemicalElement.Lightning
		};
	}

	public class Wind : ChemicalElementEntity {
		public Wind(float i = 0f) : base(ChemicalElement.Wind, i) {this.weaknesses = Wind.weakness;}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Fire,
			ChemicalElement.Water
		};
	}

	public class Earth : ChemicalElementEntity {
		public Earth(float i = 0f) : base(ChemicalElement.Earth, i) {this.weaknesses = Earth.weakness;}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Wind,
			ChemicalElement.Fire
		};
	}

	public class Lightning : ChemicalElementEntity {
		public Lightning(float i = 0f) : base(ChemicalElement.Lightning, i) {this.weaknesses = Lightning.weakness;}

		protected internal static ChemicalElement[] weakness = new ChemicalElement[] {
			ChemicalElement.Earth,
			ChemicalElement.Wind
		};
	}

	public class Water : ChemicalElementEntity {
		public Water(float i = 0f) : base(ChemicalElement.Water, i) {this.weaknesses = Water.weakness;}

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
		public Magma(float i = 0f) : base(ChemicalElement.Magma, i) { this.recipe = Magma.combo; }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Earth,
			ChemicalElement.Fire
		};
	}

	public class Steam : ChemicalElementMixEntity {
		public Steam(float i = 0f) : base(ChemicalElement.Steam, i) { this.recipe = Steam.combo; }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Fire,
			ChemicalElement.Water
		};	
	}

	public class Ice : ChemicalElementMixEntity {
		public Ice(float i = 0f) : base(ChemicalElement.Ice, i) { this.recipe = Ice.combo; }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Wind,
			ChemicalElement.Water
		};
	}

	public class Snow : ChemicalElementMixEntity {
		public Snow(float i = 0f) : base(ChemicalElement.Snow, i) { this.recipe = Snow.combo; }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Steam,
			ChemicalElement.Ice
		};
	}

	public class Sand : ChemicalElementMixEntity {
		public Sand(float i = 0f) : base(ChemicalElement.Sand, i) { this.recipe = Sand.combo; }

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
		protected ChemicalElement[] weaknesses;

		private protected ChemicalElement[] _elements = null;
		protected internal virtual ChemicalElement[] elements {
			get {
				if(this._elements == null) {
					this._elements = new ChemicalElement[] {this.type};
				}
				return this._elements;
			}
		}

		private const float MIN_INTENSITY = 0f;
		private const float MAX_INTENSITY = 100f;
		private float _intensity = MIN_INTENSITY;
		public float intensity {
			get { return this._intensity; }
			set { this._intensity = (value > MAX_INTENSITY) ? MAX_INTENSITY : ((value < MIN_INTENSITY) ? MIN_INTENSITY : value); }
		}

		public ChemicalElementEntity(ChemicalElement e, float i)
		{
			this.type = e;
			this.intensity = i;
		}

		private bool IsWeakAgainst(ChemicalElementEntity ent) {
			bool isIn = knownWeakAgainst.ContainsKey(this.type);
			if(isIn) {
				Tuple<ChemicalElement, bool> w = knownWeakAgainst[this.type].Find(x => x.Item1 == ent.type);
				if(w != null) {
					return w.Item2;
				}
			}

			int mycount, itscount;
			mycount = itscount = 0;

			foreach(ChemicalElement w in this.weaknesses) {
				foreach(ChemicalElement e in ent.elements) {
					if(w == e) {
						itscount ++;
					}
				}
			}
			foreach(ChemicalElement w in ent.weaknesses) {
				foreach(ChemicalElement e in this.elements) {
					if(w == e) {
						mycount ++;
					}
				}
			}
			bool res = (mycount != itscount && itscount > mycount);
			if(!isIn) {
				knownWeakAgainst.Add(this.type, new List<Tuple<ChemicalElement, bool>>());
			}
			knownWeakAgainst[this.type].Add(new Tuple<ChemicalElement, bool>(ent.type, res));
			return res;
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
			return ChemicalElementEntity.voidd;
		}




		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* -------------------------------------- STATIC METHODS ---------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		public static Voidd voidd = new Voidd();
		protected static object[] STANDARD_PARAMS = new object[] {0f};
		private static Dictionary<ChemicalElement, List<Tuple<ChemicalElement, bool>>> knownWeakAgainst = new Dictionary<ChemicalElement, List<Tuple<ChemicalElement, bool>>>();

		private protected static ChemicalElementEntity GetWinnerBetween(ChemicalElementEntity a, ChemicalElementEntity b) {
			if(a.IsWeakAgainst(b)) {
				return b;
			} else if(b.IsWeakAgainst(a)) {
				return a;
			} else if(a.intensity != b.intensity) {
				return (a.intensity > b.intensity) ? a : b;
			} else {
				return ChemicalElementEntity.voidd;
			}
		}

		protected static ChemicalElement[] GetWeaknessOf(ChemicalElement e) {
			switch(e) {
				case ChemicalElement.Voidd: return Voidd.weakness;
				case ChemicalElement.Fire: return Fire.weakness;
				case ChemicalElement.Wind: return Wind.weakness;
				case ChemicalElement.Water: return Water.weakness;
				case ChemicalElement.Earth: return Earth.weakness;
				case ChemicalElement.Lightning: return Lightning.weakness;
			}

			foreach(Type type in ChemicalElementMixEntity.types) {
				if(type.Name == e.ToString()) {
					return ((ChemicalElementMixEntity)Activator.CreateInstance(type, ChemicalElementEntity.STANDARD_PARAMS)).weaknesses;
				}
			}

			Debug.LogError($"ERROR: Should have not reached this place but we did with '{e}' !");
			return null;
		}

		protected static ChemicalElement[] GetElementOf(ChemicalElement e) {
			switch(e) {
				case ChemicalElement.Voidd: 
				case ChemicalElement.Fire:
				case ChemicalElement.Wind:
				case ChemicalElement.Water:
				case ChemicalElement.Earth:
				case ChemicalElement.Lightning:
					return new ChemicalElement[] {e};
			}

			foreach(Type type in ChemicalElementMixEntity.types) {
				if(type.Name == e.ToString()) {
					return ((ChemicalElementMixEntity)Activator.CreateInstance(type, ChemicalElementEntity.STANDARD_PARAMS)).elements;
				}
			}

			Debug.LogError($"ERROR: Should have not reached this place but we did with '{e}' !");
			return null;
		}
	}







	public abstract class ChemicalElementMixEntity : ChemicalElementEntity
	{
		private ChemicalElement[] _recipe;
		protected ChemicalElement[] recipe {
			get { return this._recipe; }
			set {
				this._recipe = value;

				if(!mixWeaknesses.ContainsKey(this.type)) {
					this.weaknesses = this.SetWeakness();
					mixWeaknesses.Add(this.type, this.weaknesses);
				} else {
					this.weaknesses = mixWeaknesses[this.type];
				}
			}
		}
		protected internal override ChemicalElement[] elements {
			get {
				if(this._elements == null) {
					this._elements = this.GetAllElements();
				}
				return this._elements;
			}
		}

		private protected ChemicalElementMixEntity(ChemicalElement e, float i) : base(e, i) {}

		protected internal bool CouldBeMadeOf(ChemicalElementEntity a, ChemicalElementEntity b) {
			ChemicalElement[] all = a.elements.Concat(b.elements).ToArray();
			bool found;

			foreach(ChemicalElement e in this.recipe) {
				found = false;
				foreach(ChemicalElement k in all) {
					if(e == k) {
						found = true;
						break;
					}
				}
				if(!found) {
					return false;
				}
			}
			return true;
		}

		private ChemicalElement[] SetWeakness() {
			if(_recipe == null) {
				return null;
			}

			ChemicalElement[] wk0 = GetWeaknessOf(recipe[0]);
			ChemicalElement[] wk1 = GetWeaknessOf(recipe[1]);
			ChemicalElement[] wk = wk0.Concat(wk1).ToArray();
			List<ChemicalElement> cache = new List<ChemicalElement>();
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

			return cache.ToArray();
		}

		private ChemicalElement[] GetAllElements() {
			ChemicalElement[] es0 = GetElementOf(recipe[0]);
			ChemicalElement[] es1 = GetElementOf(recipe[1]);
			ChemicalElement[] es = es0.Concat(es1).ToArray();
			List<ChemicalElement> cache = new List<ChemicalElement>();

			cache.Add(this.type);
			foreach(ChemicalElement e in es) {
				if(!cache.Contains(e)) {
					cache.Add(e);
				}
			}

			return cache.ToArray();
		}





		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* -------------------------------------- STATIC METHODS ---------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		private static Dictionary<ChemicalElement, ChemicalElement[]> mixWeaknesses = new Dictionary<ChemicalElement, ChemicalElement[]>();

		private static ChemicalElementMixEntity[] _mixes = null;
		protected internal static ChemicalElementMixEntity[] mixes {
			get {
				if(_mixes == null) { _mixes = GetAllMixes(); }
				return _mixes;
			}
		}

		private static Type[] _types = null;
		protected internal static Type[] types {
			get {
				if(_types == null) { _types = GetAllTypes(); }
				return _types;
			}
		}

		private static ChemicalElementMixEntity[] GetAllMixes() {
			Type[] ts = types;
			ChemicalElementMixEntity[] res = new ChemicalElementMixEntity[ts.Length];
			int i = 0;

			foreach(Type type in ts) {
				res[i++] = (ChemicalElementMixEntity)Activator.CreateInstance(type, ChemicalElementEntity.STANDARD_PARAMS);
			}

			return res;
		}

		private static Type[] GetAllTypes() {
			Type t = typeof(ChemicalElementMixEntity);
			return Assembly.GetAssembly(t).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t)).ToArray();
		}

		private static Dictionary<Tuple<ChemicalElement, ChemicalElement>, ChemicalElementEntity> knownMixes = new Dictionary<Tuple<ChemicalElement, ChemicalElement>, ChemicalElementEntity>();

		protected internal static ChemicalElementEntity MixTwoElement(ChemicalElementEntity a, ChemicalElementEntity b) {
			
			if(a.type == ChemicalElement.Voidd) {
				return b;
			} else if(b.type == ChemicalElement.Voidd) {
				return a;
			}

			List<ChemicalElementMixEntity> candidates = null;
			ChemicalElementEntity winner = null;
			Tuple<ChemicalElement, ChemicalElement> couple;

			couple = Tuple.Create(a.type, b.type);
			if(knownMixes.ContainsKey(couple)) {
				return knownMixes[couple];
			}
			couple = Tuple.Create(b.type, a.type);
			if(knownMixes.ContainsKey(couple)) {
				return knownMixes[couple];
			}

			candidates = new List<ChemicalElementMixEntity>();

			foreach(ChemicalElementMixEntity mix in mixes) {
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

			knownMixes.Add(couple, winner);

			return winner;
		}
	}
}
