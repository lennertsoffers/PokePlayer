using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Pokeplayer_Library.DAL;
using PokePlayer_Library.Tools;

namespace PokePlayer_Library.Models.Pokemon {
	public class Stat {
		public int Id { get; set; }
		public int StatId { get; set; }
		public string StatName { get; set; }
		public int BaseStat { get; set; }
		public int Iv { get; set; }
		public int StatValue { get; set; }
		public int PreviousStat { get; set; }

		private static readonly StatRepository statRepository = new StatRepository();

		public Stat(int id, int baseStat, int level) {
			JObject statData = ApiTools.GetSpecieData("https://pokeapi.co/api/v2/stat/" + id);
			this.Id = statRepository.GetAmountOfStats() + 1;
			this.StatId = id;
			this.StatName = (string) statData["name"];
			this.BaseStat = baseStat;
			this.Iv = new Random().Next(0, 15);
			this.StatValue = CalcStat(level);
			this.PreviousStat = this.StatValue;

			statRepository.InsertStat(this);
		}

		public Stat() {}

		public void LevelUp(int level) {
			this.PreviousStat = this.StatValue;
			this.StatValue = CalcStat(level);
		}

		public virtual int CalcStat(int level) {
			return (int) ((2 * this.BaseStat + this.Iv) * level) / 100 + 5;
		}

		public static Stat GetStat(int id) {
			return statRepository.GetStat(id);
		}
	} 
}