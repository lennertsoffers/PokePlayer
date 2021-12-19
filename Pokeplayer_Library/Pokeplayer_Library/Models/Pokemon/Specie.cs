using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using PokePlayer.DAL;
using PokePlayer_Library.Tools;

// Class model for a specie

namespace PokePlayer_Library.Models.Pokemon {
	public class Specie {
		public int SpecieId { get; }
		public int CaptureRate { get; }
		public string GrowthRate { get; }
		public string SpecieName { get; }
		public string FlavorText { get; }
		public bool IsLegendary { get; }
		public bool IsMythical { get; }

		// Corresponding model for database interaction
		private static readonly SpecieRepository specieRepository = new SpecieRepository();

		// No arguments constructor for use with Dapper
		public Specie() {}

		// Constructor used for creation of new specie
		// The constructor with parameters may only be used within this class
		// If you want to get a specie elsewhere in the program you should use the GetSpecie method
		private Specie(int id) {
			JObject specieData = ApiTools.GetApiData("https://pokeapi.co/api/v2/pokemon-species/" + id);
			this.SpecieId = id;
			this.CaptureRate = (int) specieData["capture_rate"];
			this.SpecieName = (string) specieData["name"];
			this.GrowthRate = (string) specieData["growth_rate"]["name"];

			// The flavour text of a specie must be in English
			if (specieData["flavor_text_entries"].Type != JTokenType.Null) {
				foreach (var entry in specieData["flavor_text_entries"]) {
					if ((string) entry["language"]["name"] == "en") {
						this.FlavorText = ((string) entry["flavor_text"]).Replace('\n', ' ').Replace('\f', ' ');
						break;
					}
				}
			} else {
				this.FlavorText = "/";
			}

			this.IsLegendary = (bool) specieData["is_legendary"];
			this.IsMythical = (bool) specieData["is_mythical"];

			// Insert the specie in the database
			specieRepository.InsertSpecie(this);
		}

		// Public function to get a specie
		// If the specie is already created and stored in the database, it will return this specie
		// Otherwise it will create a new specie with an api call
		// This way there will be less data storage and less api calls
		public static Specie GetSpecie(int id) {
			if (specieRepository.SpecieExists(id)) {
				return specieRepository.GetSpecie(id);
			} else {
				return new Specie(id);
			}
		}
	}
}
