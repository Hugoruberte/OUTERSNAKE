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
		public Voidd() : base(ChemicalElement.Voidd, 0f, Voidd.weakness) {}

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
		public Magma(float i = 0f) : base(ChemicalElement.Magma, i) { this.SetRecipe(Magma.combo); }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Earth,
			ChemicalElement.Fire
		};
	}

	public class Steam : ChemicalElementMixEntity {
		public Steam(float i = 0f) : base(ChemicalElement.Steam, i) { this.SetRecipe(Steam.combo); }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Fire,
			ChemicalElement.Water
		};	
	}

	public class Ice : ChemicalElementMixEntity {
		public Ice(float i = 0f) : base(ChemicalElement.Ice, i) { this.SetRecipe(Ice.combo); }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Wind,
			ChemicalElement.Water
		};
	}

	public class Snow : ChemicalElementMixEntity {
		public Snow(float i = 0f) : base(ChemicalElement.Snow, i) { this.SetRecipe(Snow.combo); }

		private static ChemicalElement[] combo = new ChemicalElement[] {
			ChemicalElement.Steam,
			ChemicalElement.Ice
		};
	}

	public class Sand : ChemicalElementMixEntity {
		public Sand(float i = 0f) : base(ChemicalElement.Sand, i) { this.SetRecipe(Sand.combo); }

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

			this.interactiveEngineData = InteractiveEngine.instance.interactiveEngineData;

			if(!this.interactiveEngineData.HasWeaknessesOf(this)) {
				this.interactiveEngineData.SetWeaknessesOf(this, weaknesses);
			}
		}

		private bool IsWeakAgainst(ChemicalElementEntity ent) {
			if(this.interactiveEngineData.HasWinnerBetween(this, ent)) {
				return this.interactiveEngineData.IsWinningAgainst(this, ent);
			}

			int mycount, itscount;
			mycount = itscount = 0;

			foreach(ChemicalElement w in this.interactiveEngineData.GetWeaknessesOf(this)) {
				foreach(ChemicalElement e in this.interactiveEngineData.GetPrimariesOf(ent)) {
					if(w == e) {
						itscount ++;
					}
				}
			}
			foreach(ChemicalElement w in this.interactiveEngineData.GetWeaknessesOf(ent)) {
				foreach(ChemicalElement e in this.interactiveEngineData.GetPrimariesOf(this)) {
					if(w == e) {
						mycount ++;
					}
				}
			}

			bool res = (mycount != itscount && itscount > mycount);
			this.interactiveEngineData.SetWinnerBetween(this, ent, res);

			return res;
		}

		private protected virtual ChemicalElement[] GetCompositionOf() {
			// only does that
			return new ChemicalElement[] {this.type};
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
	}







	public abstract class ChemicalElementMixEntity : ChemicalElementEntity
	{
		private protected void SetRecipe(ChemicalElement[] recipe) {

			// Set weaknesses according to recipe
			if(!this.interactiveEngineData.HasWeaknessesOf(this.type)) {
				this.interactiveEngineData.SetWeaknessesOf(this, this.GetWeaknessesOf(this.type, recipe));
			}

			// Set elements composition by decomposing recipe in its primary element
			if(!this.interactiveEngineData.HasPrimariesOf(this.type)) {
				this.interactiveEngineData.SetPrimariesOf(this, this.GetCompositionOf(this.type, recipe));
			}
		}

		// private protected ChemicalElementMixEntity(ChemicalElement e, float i) : base(e, i) {}

		// does elements in 'a' and 'b' validate this primary element composition
		protected internal bool CouldBeMadeOf(ChemicalElementEntity a, ChemicalElementEntity b) {
			ChemicalElement[] all = this.interactiveEngineData.GetPrimariesOf(a).Concat(this.interactiveEngineData.GetPrimariesOf(b)).ToArray();
			bool found;

			foreach(ChemicalElement e in this.interactiveEngineData.GetPrimariesOf(this)) {
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

		

		









		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* -------------------------------------- STATIC METHODS ---------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		/* ---------------------------------------------------------------------------------------------*/
		protected static object[] STANDARD_PARAMS = new object[] {0f};

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

		// get composition by decomposing 'recipe' in its primary element
		private static ChemicalElement[] GetCompositionOf(ChemicalElement mine, ChemicalElement[] recipe) {
			ChemicalElement[] es0 = GetPrimariesOf(recipe[0]);
			ChemicalElement[] es1 = GetPrimariesOf(recipe[1]);
			ChemicalElement[] es = es0.Concat(es1).ToArray();
			List<ChemicalElement> cache = new List<ChemicalElement>();

			cache.Add(mine);
			foreach(ChemicalElement e in es) {
				if(!cache.Contains(e)) {
					cache.Add(e);
				}
			}

			return cache.ToArray();
		}

		// get primaries element of a particular element
		private static ChemicalElement[] GetPrimariesOf(ChemicalElement e) {
			switch(e) {
				case ChemicalElement.Voidd: 
				case ChemicalElement.Fire:
				case ChemicalElement.Wind:
				case ChemicalElement.Water:
				case ChemicalElement.Earth:
				case ChemicalElement.Lightning:
					return new ChemicalElement[] {e};
			}

			ChemicalElementMixEntity ent;

			foreach(Type type in ChemicalElementMixEntity.types) {
				if(type.Name == e.ToString()) {
					ent = Activator.CreateInstance(type, ChemicalElementEntity.STANDARD_PARAMS) as ChemicalElementMixEntity;
					return this.interactiveEngineData.GetPrimariesOf(ent);
				}
			}

			Debug.LogError($"ERROR: Should have not reached this place but we did with '{e}' !");
			return null;
		}

		// get weaknesses according to recipe element weaknesses
		private static ChemicalElement[] GetWeaknessesOf(ChemicalElement[] recipe) {
			ChemicalElement[] wk0 = GetWeaknessOfElement(recipe[0]);
			ChemicalElement[] wk1 = GetWeaknessOfElement(recipe[1]);
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

		// get weaknesses of a particular element
		private static ChemicalElement[] GetWeaknessOfElement(ChemicalElement e) {
			switch(e) {
				case ChemicalElement.Voidd: return Voidd.weakness;
				case ChemicalElement.Fire: return Fire.weakness;
				case ChemicalElement.Wind: return Wind.weakness;
				case ChemicalElement.Water: return Water.weakness;
				case ChemicalElement.Earth: return Earth.weakness;
				case ChemicalElement.Lightning: return Lightning.weakness;
			}

			ChemicalElementMixEntity ent;

			foreach(Type type in ChemicalElementMixEntity.types) {
				if(type.Name == e.ToString()) {
					ent = Activator.CreateInstance(type, ChemicalElementEntity.STANDARD_PARAMS) as ChemicalElementMixEntity;
					return this.interactiveEngineData.GetWeaknessesOf(ent);
				}
			}

			Debug.LogError($"ERROR: Should have not reached this place but we did with '{e}' !");
			return null;
		}
	}
}
