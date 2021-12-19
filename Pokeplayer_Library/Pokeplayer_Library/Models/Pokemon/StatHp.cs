using Newtonsoft.Json.Linq;
using PokePlayer_Library.Tools;

// Class model for a hp stat
// Inherits from the stat class

namespace PokePlayer_Library.Models.Pokemon {
	public class StatHp : Stat {

		// Constructor used for creation of new hp stat
		public StatHp(int id, int baseStat, int level): base(id, baseStat, level) {
			JObject statData = ApiTools.GetApiData("https://pokeapi.co/api/v2/stat/" + id);
			base.StatValue = CalcStat(level);
			base.PreviousStat = this.StatValue;
		}

		// Formula for caluclating the hp stat
		// This formula differs from the normal stat calculation formula
		public override int CalcStat(int level) {
			return (int) ((2 * this.BaseStat + this.Iv) * level) / 100 + level + 10;
		}
	}
}
