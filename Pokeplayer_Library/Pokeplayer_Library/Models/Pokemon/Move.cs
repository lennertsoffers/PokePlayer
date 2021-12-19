using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Pokeplayer_Library.DAL;
using PokePlayer_Library.Tools;

// Class model for a move

namespace PokePlayer_Library.Models.Pokemon {
	public class Move {
		public int Id { get; set; }
		public int Accuracy { get; }
		public int FlinchChance { get; }
		public int CritRate { get; }
		public int Pp { get; set; }
		public int MaxPp { get; }
		public int Power { get; }
		public int Drain { get; }
		public int Healing { get; }

		public string MoveName { get; }
		public string DamageClass { get; }
		public string MultiHitType { get; }
		public string FlavourText { get; }
		
		public Dictionary<string, string> Ailment { get; set; }
		public Type Type { get; set; }
		public List<Dictionary<string, string>> StatChanges { get; set; }

		private static readonly MoveRepository moveRepository = new MoveRepository();


		public Move() {}

		public Move(string name) {
			JObject moveData = ApiTools.GetApiData("https://pokeapi.co/api/v2/move/" + name);

			this.Id = (int) moveData["id"];
			this.Accuracy = (int) (moveData["accuracy"].Type != JTokenType.Null ? moveData["accuracy"] : 100);
			this.FlinchChance = (int) moveData["meta"]["flinch_chance"];
			this.CritRate = (int) moveData["meta"]["crit_rate"];
			this.Pp = (int) moveData["pp"];
			this.MaxPp = (int) moveData["pp"];
			this.Power = (int) (moveData["power"].Type != JTokenType.Null ? moveData["power"] : -1);
			this.Drain = (int) moveData["meta"]["drain"];
			this.Healing = (int) moveData["meta"]["healing"];

			this.MoveName = (string) moveData["name"];
			this.DamageClass = (string) moveData["damage_class"]["name"];
			if (moveData["meta"]["min_hits"].Type == JTokenType.Null) {
				this.MultiHitType = "single";
			} else {
				if (((int) moveData["meta"]["min_hits"]) == ((int) moveData["meta"]["max_hits"])) {
					this.MultiHitType = "double";
				} else {
					this.MultiHitType = "random";
				}
			}

			if (moveData["flavor_text_entries"].Type != JTokenType.Null) {
				foreach (var entry in moveData["flavor_text_entries"]) {
					if ((string) entry["language"]["name"] == "en") {
						this.FlavourText = ((string) entry["flavor_text"]).Replace('\n', ' ').Replace('\f', ' ');
						break;
					}
				}
			} else {
				this.FlavourText = "/";
			}


			string ailmentName = "-1";
			string ailmentChance = "-1";
			if (moveData["meta"]["ailment"]["name"].Type != JTokenType.Null) {
				string n = (string) moveData["meta"]["ailment"]["name"];
				ailmentChance = (string) moveData["meta"]["ailment_chance"];
				ailmentName = n switch {
					"paralysis" => "PAR",
					"burn" => "BRN",
					"freeze" => "FRZ",
					"poison" => "PSN",
					"sleep" => "SLP",
					"confusion" => "confusion",
					_ => n
				};
			}

			this.Ailment = new Dictionary<string, string> {
				{"name", ailmentName},
				{"chance", ailmentChance}
			};
			string typename = (string) moveData["type"]["name"];
			this.Type = Type.GetType((string) moveData["type"]["name"]);

			this.StatChanges = new List<Dictionary<string, string>>();
			foreach (var statChange in moveData["stat_changes"]) {
				var st = new Dictionary<string, string> {
					{ "name", (string) statChange["stat"]["name"] },
					{ "amount", (string) statChange["change"] }
				};
				this.StatChanges.Add(st);
			}

			moveRepository.InsertMove(this);
		}

		public static Move GetMove(string name) {
			if (moveRepository.MoveExists(name)) {
				Move m = moveRepository.GetMove(name);
				return m;
			}

			return new Move(name);
		}

		public static Move GetMoveById(int id) {
			string name = moveRepository.GetMoveName(id);
			return GetMove(name);
		}
	}
}
