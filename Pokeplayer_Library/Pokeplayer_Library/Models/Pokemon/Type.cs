using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Pokeplayer_Library.DAL;
using PokePlayer_Library.Tools;

namespace PokePlayer_Library.Models.Pokemon {
	public class Type {
		public int Id { get; }
		public string TypeName { get; }
		public List<string> NoDamageTo { get; set; }
		public List<string> HalfDamageTo { get; set; }
		public List<string> DoubleDamageTo { get; set; }

		private static readonly TypeRepository typeRepository = new TypeRepository();

		public Type(int id, string name) {
			this.Id = id;
			this.TypeName = name;
			NoDamageTo = new List<string>();
			HalfDamageTo = new List<string>();
			DoubleDamageTo = new List<string>();
		}

		private Type(string name) {
			JObject typeData = ApiTools.GetSpecieData("https://pokeapi.co/api/v2/type/" + name);
			this.Id = (int) typeData["id"];
			this.TypeName = name;
			this.NoDamageTo = new List<string>();
			this.HalfDamageTo = new List<string>();
			this.DoubleDamageTo = new List<string>();

			foreach (var entry in typeData["damage_relations"]["no_damage_to"]) {
				this.NoDamageTo.Add((string) entry["name"]);
			}

			foreach (var entry in typeData["damage_relations"]["half_damage_to"]) {
				this.HalfDamageTo.Add((string) entry["name"]);
			}

			foreach (var entry in typeData["damage_relations"]["double_damage_to"]) {
				this.DoubleDamageTo.Add((string) entry["name"]);
			}

			typeRepository.InsertType(this);
		}

		public static Type GetType(string name) {
			if (typeRepository.TypeExists(name)) {
				return typeRepository.GetType(name);
			}
			return new Type(name);
		}

		public static Type GetTypeById(int id) {
			string name = typeRepository.GetTypeName(id);
			return GetType(name);
		}
	}
}
