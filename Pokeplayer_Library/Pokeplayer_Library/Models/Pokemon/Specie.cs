using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using PokePlayer.DAL;
using PokePlayer_Library.Tools;

namespace PokePlayer_Library.Models.Pokemon {
	public class Specie {
		public int SpecieId { get; }
		public int CaptureRate { get; }
		public string GrowthRate { get; }
		public string SpecieName { get; }
		public string FlavorText { get; }
		public bool IsLegendary { get; }
		public bool IsMythical { get; }

		private static readonly SpecieRepository specieRepository = new SpecieRepository();

		public Specie(int id) {
			JObject specieData = ApiTools.GetSpecieData("https://pokeapi.co/api/v2/pokemon-species/" + id);
			this.SpecieId = id;
			this.CaptureRate = (int) specieData["capture_rate"];
			this.SpecieName = (string) specieData["name"];
			this.GrowthRate = (string) specieData["growth_rate"]["name"];
			this.FlavorText = ((string) specieData["flavor_text_entries"][0]["flavor_text"]).Replace('\n', ' ').Replace('\f', ' ');
			this.IsLegendary = (bool) specieData["is_legendary"];
			this.IsMythical = (bool) specieData["is_mythical"];


			if (!specieRepository.SpecieExists(id)) {
				specieRepository.InsertSpecie(this);
			}
		}

		public Specie() {}

		public static Specie GetSpecie(int id) {
			if (specieRepository.SpecieExists(id)) {
				return specieRepository.GetSpecie(id);
			} else {
				return new Specie(id);
			}
		}
	}
}
