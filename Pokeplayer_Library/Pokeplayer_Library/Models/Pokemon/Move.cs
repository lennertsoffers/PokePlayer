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
		
		// Corresponding model for database interaction
		private static readonly MoveRepository moveRepository = new MoveRepository();

		// No arguments constructor for use with Dapper
		public Move() {}

		// Constructor used for creation of new move
		// The constructor with parameters may only be used within this class
		// If you want to get a move elsewhere in the program you should use the GetMove method
		private Move(string name) {
			// Get data from the PokeApi using static class ApiTools
			JObject moveData = ApiTools.GetApiData("https://pokeapi.co/api/v2/move/" + name);
			this.Id = (int) moveData["id"];

			// A move can have accuracy null which means it will always hit
			// So accuracy must be set to 100
			this.Accuracy = (int) (moveData["accuracy"].Type != JTokenType.Null ? moveData["accuracy"] : 100);
			this.FlinchChance = (int) moveData["meta"]["flinch_chance"];
			this.CritRate = (int) moveData["meta"]["crit_rate"];
			this.Pp = (int) moveData["pp"];
			this.MaxPp = (int) moveData["pp"];

			// Moves that change stats have no power so power must be set to -1
			this.Power = (int) (moveData["power"].Type != JTokenType.Null ? moveData["power"] : -1);
			this.Drain = (int) moveData["meta"]["drain"];
			this.Healing = (int) moveData["meta"]["healing"];
			this.MoveName = (string) moveData["name"];
			this.DamageClass = (string) moveData["damage_class"]["name"];

			// If the min-hits is not given, the move is a normal move that hits one time
			// Otherwise it is double or random
			if (moveData["meta"]["min_hits"].Type == JTokenType.Null) {
				this.MultiHitType = "single";
			} else {
				if (((int) moveData["meta"]["min_hits"]) == ((int) moveData["meta"]["max_hits"])) {
					this.MultiHitType = "double";
				} else {
					this.MultiHitType = "random";
				}
			}

			// We want the flavour text of the move to be in English
			// If there is no record of the text in Englis the text will be set to '/'
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

			// If the move has no ailments, the name and chance will stay at -1
			// If there is an ailment given, the name wil be change to it's three letter variant like in the game
			// The chance is changed too
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
			// The ailment record is set in the ailment dictionary
			this.Ailment = new Dictionary<string, string> {
				{"name", ailmentName},
				{"chance", ailmentChance}
			};

			// Create assocation between the move and the type object
			// Only if the type is not already stored in the database, a new api call is needed to create the type
			string typename = (string) moveData["type"]["name"];
			this.Type = Type.GetType((string) moveData["type"]["name"]);

			// A move can have more than one stat change, so each on is added to the statChanges list
			this.StatChanges = new List<Dictionary<string, string>>();
			foreach (var statChange in moveData["stat_changes"]) {
				var st = new Dictionary<string, string> {
					{ "name", (string) statChange["stat"]["name"] },
					{ "amount", (string) statChange["change"] }
				};
				this.StatChanges.Add(st);
			}

			// Insert the move in the database
			moveRepository.InsertMove(this);
		}

		// Public function to get a move
		// If the move is already created and stored in the database, it will return this move
		// Otherwise it will create a new move with an api call
		// This way there will be less data storage and less api calls
		public static Move GetMove(string name) {
			// Move already exists in the database
			if (moveRepository.MoveExists(name)) {
				Move m = moveRepository.GetMove(name);
				return m;
			}

			// New move created with an api call
			return new Move(name);
		}

		// Somtimes we need to get a move by id an not by name
		public static Move GetMoveById(int id) {
			string name = moveRepository.GetMoveName(id);
			return GetMove(name);
		}
	}
}
