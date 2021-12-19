using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Pokeplayer_Library.DAL;
using PokePlayer_Library.Tools;

// Class model for a specie

namespace PokePlayer_Library.Models.Pokemon {
	public class Stat {
		public int Id { get; set; }
		public int StatId { get; set; }
		public string StatName { get; set; }
		public int BaseStat { get; set; }
		public int Iv { get; set; }
		public int StatValue { get; set; }
		public int PreviousStat { get; set; }

		// Corresponding model for database interaction
		private static readonly StatRepository statRepository = new StatRepository();

		// No arguments constructor for use with Dapper
		public Stat() {}

		// Constructor used for creation of new stat
		public Stat(int id, int baseStat, int level) {
			JObject statData = ApiTools.GetApiData("https://pokeapi.co/api/v2/stat/" + id);
			this.Id = statRepository.GetAmountOfStats() + 1;
			this.StatId = id;
			this.StatName = (string) statData["name"];
			this.BaseStat = baseStat;
			this.Iv = new Random().Next(0, 15);
			this.StatValue = CalcStat(level);
			this.PreviousStat = this.StatValue;

			// Insert the stat in the database
			statRepository.InsertStat(this);
		}

		// If the pokemon goes a level up, that stat must be updated too
		public void LevelUp(int level) {
			this.PreviousStat = this.StatValue;
			this.StatValue = CalcStat(level);
		}

		// Formula for caluclating a normal stat
		public virtual int CalcStat(int level) {
			return (int) ((2 * this.BaseStat + this.Iv) * level) / 100 + 5;
		}

		// Returns a stat object from the database
		public static Stat GetStat(int id) {
			return statRepository.GetStat(id);
		}
	} 
}