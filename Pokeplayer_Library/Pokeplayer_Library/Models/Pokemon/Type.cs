using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Pokeplayer_Library.DAL;
using PokePlayer_Library.Tools;

// Class model for a type

namespace PokePlayer_Library.Models.Pokemon {
	public class Type {
		public int Id { get; }
		public string TypeName { get; }
		public List<string> NoDamageTo { get; set; }
		public List<string> HalfDamageTo { get; set; }
		public List<string> DoubleDamageTo { get; set; }

		// Corresponding model for database interaction
		private static readonly TypeRepository typeRepository = new TypeRepository();

		// Constructor used for creation of new type
		// This constructor is used for Dapper
		public Type(int id, string name) {
			this.Id = id;
			this.TypeName = name;
			NoDamageTo = new List<string>();
			HalfDamageTo = new List<string>();
			DoubleDamageTo = new List<string>();
		}

		// Constructor used for creation of new type
		// The constructor with parameters may only be used within this class
		// If you want to get a specie elsewhere in the program you should use the GetType method
		private Type(string name) {
			JObject typeData = ApiTools.GetApiData("https://pokeapi.co/api/v2/type/" + name);
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

			// Insert the type in the database
			typeRepository.InsertType(this);
		}

		// Public function to get a type
		// If the type is already created and stored in the database, it will return this type
		// Otherwise it will create a new type with an api call
		// This way there will be less data storage and less api calls
		public static Type GetType(string name) {
			if (typeRepository.TypeExists(name)) {
				return typeRepository.GetType(name);
			}
			return new Type(name);
		}

		// Somtimes we need to get a type by id an not by name
		public static Type GetTypeById(int id) {
			string name = typeRepository.GetTypeName(id);
			return GetType(name);
		}
	}
}
