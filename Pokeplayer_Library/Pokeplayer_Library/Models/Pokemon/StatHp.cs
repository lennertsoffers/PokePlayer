using Newtonsoft.Json.Linq;
using PokePlayer_Library.Tools;

namespace PokePlayer_Library.Models.Pokemon {
	public class StatHp : Stat {

		public StatHp(int id, int baseStat, int level): base(id, baseStat, level) {
			JObject statData = ApiTools.GetApiData("https://pokeapi.co/api/v2/stat/" + id);
			base.StatValue = CalcStat(level);
			base.PreviousStat = this.StatValue;
		}

		public override int CalcStat(int level) {
			return (int) ((2 * this.BaseStat + this.Iv) * level) / 100 + level + 10;
		}
	}
}
