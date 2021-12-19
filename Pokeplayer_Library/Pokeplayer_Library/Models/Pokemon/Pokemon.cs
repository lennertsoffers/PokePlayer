using Newtonsoft.Json.Linq;
using PokePlayer_Library.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Pokeplayer_Library.DAL;

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

		private static readonly PokemonRepository pokemonRepository = new PokemonRepository();

		public Pokemon() {}

		public Pokemon(int id, int level = 1, string nickName = "") {
			JObject pokemonData = ApiTools.GetApiData("https://pokeapi.co/api/v2/pokemon/" + id);
			this.Id = pokemonRepository.GetAmountOfPokemon() + 1;
			this.PokemonId = id;
			this.Specie = Specie.GetSpecie(id);
			this.Level = level;
			this.TotalExperience = CalculateTotalXp();
			this.Level = level + 1;
			this.NextLevelExperience = CalculateTotalXp();
			this.Level = level;
			this.BaseExperience = (int) pokemonData["base_experience"];
			this.Shiny = new Random().Next(1, 8192) == 1;
			if (this.Shiny) {
				this.SpriteFront = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/shiny/{id}.png";
				this.SpriteBack = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/back/{id}.png";
			} else {
				this.SpriteFront = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{id}.png";
				this.SpriteBack = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/back/{id}.png";
			}
			if (nickName != "") {
				this.NickName = nickName;
			} else {
				this.NickName = this.Specie.SpecieName;
			}

			this.PossibleMoves = new List<Dictionary<string, string>>();
			this.Moves = new Dictionary<int, Move>();
			foreach (var move in pokemonData["moves"]) {
				string moveName = (string) move["move"]["name"];
				string learnAt = (string) move["version_group_details"][0]["level_learned_at"];
				string learnMethod = (string) move["version_group_details"][0]["move_learn_method"]["name"];
				if (learnMethod == "level-up") {
					this.PossibleMoves.Add(new Dictionary<string, string> {
						{"name", moveName},
						{"learnAt", learnAt},
						{"method", learnMethod}
					});
				}
			}

			this.PossibleMoves.Sort((x, y) => int.Parse(x["learnAt"]).CompareTo(int.Parse(y["learnAt"])));
			this.PossibleMoves.Reverse();

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

			UpdateMovePp();

			this.Stats = new Dictionary<string, Stat> {
				{"hp", new Stat(1, (int) pokemonData["stats"][0]["base_stat"], this.Level)},
				{"attack", new Stat(2, (int) pokemonData["stats"][1]["base_stat"], this.Level)},
				{"defense", new Stat(3, (int) pokemonData["stats"][2]["base_stat"], this.Level)},
				{"special-attack", new Stat(4, (int) pokemonData["stats"][3]["base_stat"], this.Level)},
				{"special-defense", new Stat(5, (int) pokemonData["stats"][4]["base_stat"], this.Level)},
				{"speed", new Stat(6, (int) pokemonData["stats"][5]["base_stat"], this.Level)}
			};

			this.Hp = this.Stats["hp"].StatValue;

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

			this.TypeList = new List<Type>();
			foreach (var t in pokemonData["types"]) {
				this.TypeList.Add(Type.GetType((string) t["type"]["name"]));
			}

			this.NonVolatileStatus = new Dictionary<string, int> {
				{"BRN", 0},
				{"FRZ", 0},
				{"PAR", 0},
				{"PSN", 0},
				{"SLP", -1},
				{"FNT", 0}
			};

			this.VolatileStatus = new Dictionary<string, int> {
				{"confusion", -1},
				{"flinch", 0}
			};

			pokemonRepository.InsertPokemon(this);
		}
		

		// Methods related to experience and leveling up
		public int GetExperienceDifference() {
			return this.NextLevelExperience - this.TotalExperience;
		}

		public HashSet<Move> AddExperience(int experience) {
			this.TotalExperience += experience;

			HashSet<Move> moves = new HashSet<Move>();
			while (GetExperienceDifference() <= 0 && this.Level < 100) {
				moves.UnionWith(this.LevelUp());
			}

			pokemonRepository.UpdatePokemon(this);

			return moves;
		}

		public int CalculateTotalXp() {
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

		public void UpdateMovePp() {
			Dictionary<int, int> movePpMapping = new Dictionary<int, int>();
			foreach (var key in this.Moves.Keys) {
				movePpMapping.Add(key, this.Moves[key].MaxPp);
			}

			this.MovePpMapping = movePpMapping;
		}



		// Methods related to stat and inBattleStat changes
		public Stat GetStat(string statName) {
			return this.Stats[statName];
		}

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

			pokemonRepository.UpdatePokemon(this);
		}

		public void AddHp(int amount) {
			this.Hp += amount;
			if (this.Hp > GetStat("hp").StatValue) {
				this.Hp = GetStat("hp").StatValue;
			}

			pokemonRepository.UpdatePokemon(this);
		}


		public static Pokemon GetPokemon(int id) {
			return pokemonRepository.GetPokemon(id);
		}
	}
}
