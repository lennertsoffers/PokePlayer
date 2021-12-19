using Newtonsoft.Json.Linq;
using PokePlayer_Library.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Pokeplayer_Library.DAL;

// Class model for a trainer

namespace PokePlayer_Library.Models.Pokemon {
	public class Pokemon {
		public int Id { get; }
		public int PokemonId { get; }
		public int Level { get; set; }
		public int BaseExperience { get; }
		public int TotalExperience { get; set; }
		public int NextLevelExperience { get; set; }
		public int Hp { get; set; }

		public string NickName { get; }
		public string SpriteFront { get; }
		public string SpriteBack { get; }

		public bool Shiny { get; }

		public Specie Specie { get; set; }
		public List<Dictionary<string, string>> PossibleMoves { get; set; }
		public Dictionary<int, Move> Moves { get; set; }
		public Dictionary<string, Stat> Stats { get; set; }
		public List<Type> TypeList { get; set; }
		public Dictionary<string, int> InBattleStats { get; set; }
		public Dictionary<string, int> NonVolatileStatus { get; set; }
		public Dictionary<string, int> VolatileStatus { get; set; }
		public Dictionary<int, int> MovePpMapping { get; set; }

		// Corresponding model for database interaction
		private static readonly PokemonRepository pokemonRepository = new PokemonRepository();
		
		// No arguments constructor for use with Dapper
		public Pokemon() {}

		// Constructor used for creation of new pokemon
		public Pokemon(int id, int level = 1, string nickName = "") {
			JObject pokemonData = ApiTools.GetApiData("https://pokeapi.co/api/v2/pokemon/" + id);
			this.Id = pokemonRepository.GetAmountOfPokemon() + 1;
			this.PokemonId = id;

			// Create assocation between the pokemon and the specie object
			// Only if the specie is not already stored in the database, a new api call is needed to create the specie
			this.Specie = Specie.GetSpecie(id);
			this.Level = level;
			this.TotalExperience = CalculateTotalXp();
			this.Level = level + 1;
			this.NextLevelExperience = CalculateTotalXp();
			this.Level = level;
			this.BaseExperience = (int) pokemonData["base_experience"];

			// There is a very small change to encounter a shiny pokemon
			this.Shiny = new Random().Next(1, 8192) == 1;
			if (this.Shiny) {
				this.SpriteFront = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/{id}.png";
				this.SpriteBack = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/back/{id}.png";
			} else {
				this.SpriteFront = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{id}.png";
				this.SpriteBack = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/back/{id}.png";
			}

			// If the nickname isn't changed, the name of the specie is chosen as name
			if (nickName != "") {
				this.NickName = nickName;
			} else {
				this.NickName = this.Specie.SpecieName;
			}

			// A list of possible moves is created
			// The name of the move and the level it is learned at is stored here
			// Only moves that are obtainted by leveling up are stored
			// Not the moves but the names are stored in this list, so that we don't waste memory by creating the moves already
			this.PossibleMoves = new List<Dictionary<string, string>>();
			this.Moves = new Dictionary<int, Move>();
			foreach (var move in pokemonData["moves"]) {
				string moveName = (string) move["move"]["name"];
				string learnAt = (string) move["version_group_details"][0]["level_learned_at"];
				string learnMethod = (string) move["version_group_details"][0]["move_learn_method"]["name"];
				if (learnMethod == "level-up") {
					this.PossibleMoves.Add(new Dictionary<string, string> {
						{"name", moveName},
						{"learnAt", learnAt}
					});
				}
			}

			// The moves are sorted on level they are learned ad and than this list is revered
			this.PossibleMoves.Sort((x, y) => int.Parse(x["learnAt"]).CompareTo(int.Parse(y["learnAt"])));
			this.PossibleMoves.Reverse();

			// We add four moves to the moves of the pokemon
			// A pokemon can only have four moves and we only want the moves that are learned the latest
			// But we cannot add moves that are learned at a higher level the pokemon is now
			int j = 1;
			for (var i = 0; i < this.PossibleMoves.Count && j < 5; i++) {
				if (int.Parse(PossibleMoves[i]["learnAt"]) <= level) {
					Move move = Move.GetMove(this.PossibleMoves[i]["name"]);
					if (IsUsefulMove(move)) {
						this.Moves.Add(j, move);
						j++;
					}
				}
			}
			
			// The move-pp mapping is updated with the max pp values of these moves
			UpdateMovePp();

			// The stats are updated for this pokemon
			// These are the maximum stats the pokemon has and not the stats the pokemon has in battle
			this.Stats = new Dictionary<string, Stat> {
				{"hp", new Stat(1, (int) pokemonData["stats"][0]["base_stat"], this.Level)},
				{"attack", new Stat(2, (int) pokemonData["stats"][1]["base_stat"], this.Level)},
				{"defense", new Stat(3, (int) pokemonData["stats"][2]["base_stat"], this.Level)},
				{"special-attack", new Stat(4, (int) pokemonData["stats"][3]["base_stat"], this.Level)},
				{"special-defense", new Stat(5, (int) pokemonData["stats"][4]["base_stat"], this.Level)},
				{"speed", new Stat(6, (int) pokemonData["stats"][5]["base_stat"], this.Level)}
			};

			// A newly created pokemon has an equal amount hp than its max hp
			this.Hp = this.Stats["hp"].StatValue;

			// In this dictionary the stats in a battle are stored
			// These amounts can differ from -6 to 6
			// Note that these values are not the same as the real stats of the pokemon, since the actual stat in battle is a calculation
			// between the real stats and the in battle stats
			this.InBattleStats = new Dictionary<string, int> {
				{"attack", 0},
				{"defense", 0},
				{"special-attack", 0},
				{"special-defense", 0},
				{"speed", 0},
				{"accuracy", 0},
				{"evasion", 0},
				{"critRate", 0}
			};

			// A pokmemon can have more stats associated to it
			this.TypeList = new List<Type>();
			foreach (var t in pokemonData["types"]) {
				this.TypeList.Add(Type.GetType((string) t["type"]["name"]));
			}

			// This is a status that stays even after a battle
			// In this way the pokemon can enter a new battle with the poison effect for example
			this.NonVolatileStatus = new Dictionary<string, int> {
				{"BRN", 0},
				{"FRZ", 0},
				{"PAR", 0},
				{"PSN", 0},
				{"SLP", -1},
				{"FNT", 0}
			};

			// These are status effects that are gone after ending a battle
			this.VolatileStatus = new Dictionary<string, int> {
				{"confusion", -1},
				{"flinch", 0}
			};

			// Insert the pokemon in the database
			pokemonRepository.InsertPokemon(this);
		}
		


		// ---- EXPERIENCE AND LEVELING UP ---- //

		// Calculate the experience needed till the next level
		public int GetExperienceDifference() {
			return this.NextLevelExperience - this.TotalExperience;
		}

		// Adding experience to a pokemon
		// When adding experience, the pokemon can level up
		// When leveling up, the pokemon can learn a new move
		// This function has the main loop for leveling up
		public HashSet<Move> AddExperience(int experience) {
			// Add all this experience to the total
			this.TotalExperience += experience;

			// This list wil hold all moves that can be learned due to levelling up
			HashSet<Move> moves = new HashSet<Move>();
			// Level up while the xp is less than needed for levelling up
			while (GetExperienceDifference() <= 0 && this.Level < 100) {
				moves.UnionWith(this.LevelUp());
			}

			// Update fields in database
			pokemonRepository.UpdatePokemon(this);
			return moves;
		}

		// Calculation of experience
		public int CalculateTotalXp() {
			// The amount of experience needed for a certain level is determined by the growth rate of the specie
			// For each growth rathe, there is a specific formula to calculate the experience
			string gr = this.Specie.GrowthRate;
			int l = this.Level;

			if (gr == "slow") {
				return (int) Math.Round((5d * Math.Pow(l, 3d)) / 4d);
			} else if (gr == "medium") {
				return (int) Math.Pow(l, 3d);
			} else if (gr == "fast") {
				return (int) Math.Round(4d * Math.Pow(l, 3d) / 5d);
			} else if (gr == "medium slow") {
				return (int) Math.Round((6d / 5d * Math.Pow(l, 3d)) - (15d * Math.Pow(l, 2d)) + (100d + l) - 140d);
			} else if (gr == "slow than very fast") {
				if (l < 50) {
					return (int) Math.Round(Math.Pow(l, 3d) * (100d - l) / 50d);
				} else if (l >= 50 && l < 68) {
					return (int) Math.Round(Math.Pow(l, 3d) * (150d - l) / 100d);
				} else if (l >= 68 && l < 98) {
					return (int) Math.Round(Math.Pow(l, 3d) * ((1911d - (10d * l)) / 3d) / 500d);
				} else {
					return (int) Math.Round(Math.Pow(l, 3d) * (160d - l) / 100d);
				}
			} else {
				if (l < 15) {
					return (int) Math.Round(Math.Pow(l, 3d) * ((((l + 1d) / 3d) + 24d) / 50d));
				} else if (l >= 15 && l < 36) {
					return (int) Math.Round(Math.Pow(l, 3d) * ((l + 14d) / 50d));
				} else {
					return (int) Math.Round(Math.Pow(l, 3d) * (((l / 2d) + 32d) / 50d));
				}
			}
		}

		// Sets the new attributes of the pokemon after leveling up
		// Returns set of moves that can be learned at this new level
		public HashSet<Move> LevelUp() {
			HashSet<Move> moves = new HashSet<Move>();
			this.Level++;
			this.NextLevelExperience = CalculateTotalXp();
			foreach (var s in this.Stats.Values) {
				s.LevelUp(this.Level);
			}
			this.Hp = this.Stats["hp"].StatValue;

			foreach (Dictionary<string, string> m in this.PossibleMoves) {
				if (int.Parse(m["learnAt"]) == this.Level) {
					Move move = Move.GetMove(m["name"]);

					if (IsUsefulMove(move)) {
						moves.Add(move);
					}
				}
			}

			return moves;
		}

		// Returns true is the move has an effect in battle
		// Some moves are very specific and must be programmed completely different
		// I've chosen to exclude these moves
		private bool IsUsefulMove(Move move) {
			return move.StatChanges.Count != 0 ||
			       move.Ailment["name"] != "PAR" ||
			       move.Ailment["name"] != "BRN" ||
			       move.Ailment["name"] != "FRZ" ||
			       move.Ailment["name"] != "PSN" ||
			       move.Ailment["name"] != "SLP" ||
			       move.Ailment["name"] != "confusion" ||
			       move.Power != 0 ||
			       move.Drain != 0 ||
			       move.Healing != 0;
		}

		// For each move in the learned moves of the pokemon, the move-pp mapping is set to the max pp of the move
		public void UpdateMovePp() {
			Dictionary<int, int> movePpMapping = new Dictionary<int, int>();
			foreach (var key in this.Moves.Keys) {
				movePpMapping.Add(key, this.Moves[key].MaxPp);
			}

			this.MovePpMapping = movePpMapping;
		}


		// ---- CHANGES OF IN BATTLE STATS ---- //
		
		// Get a certain stat of the pokemon by name
		public Stat GetStat(string statName) {
			return this.Stats[statName];
		}

		// Lowers the hp of the current pokemon
		// If the hp reaches zero, the pokemon gets the fainted status and all other are reset
		public void LowerHp(int amount) {
			this.Hp -= amount;
			if (this.Hp <= 0) {
				this.Hp = 0;
				this.NonVolatileStatus = new Dictionary<string, int> {
					{"BRN", 0},
					{"FRZ", 0},
					{"PAR", 0},
					{"PSN", 0},
					{"SLP", -1},
					{"FNT", 1}
				};
			}

			// The database must be updated with these changes
			pokemonRepository.UpdatePokemon(this);
		}

		// Adds hp to the pokemon
		// The hp cannot be more than het max hp of the hp stat
		public void AddHp(int amount) {
			this.Hp += amount;
			if (this.Hp > GetStat("hp").StatValue) {
				this.Hp = GetStat("hp").StatValue;
			}

			// Database must be updated with these changes
			pokemonRepository.UpdatePokemon(this);
		}



		// Method to regen the pokemon
		public void RegenPokemon() {
			this.Hp = this.GetStat("hp").StatValue;
			this.NonVolatileStatus = new Dictionary<string, int> {
				{"BRN", 0},
				{"FRZ", 0},
				{"PAR", 0},
				{"PSN", 0},
				{"SLP", -1},
				{"FNT", 0}
			};
			foreach (var key in this.MovePpMapping.Keys) {
				this.MovePpMapping[key] = this.Moves[key].MaxPp;
			}
		}



		// Returns a pokemon object stored in the database with its id
		public static Pokemon GetPokemon(int id) {
			return pokemonRepository.GetPokemon(id);
		}
	}
}
