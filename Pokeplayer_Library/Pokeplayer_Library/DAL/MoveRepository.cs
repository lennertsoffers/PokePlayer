using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokePlayer.DAL;
using PokePlayer_Library.Models.Pokemon;
using Newtonsoft.Json;
using Dapper;
using Type = PokePlayer_Library.Models.Pokemon.Type;

namespace Pokeplayer_Library.DAL {
	class MoveRepository : SqlLiteBase {
		public MoveRepository() {
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		public void InsertMove(Move move) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				var dictionary = new Dictionary<string, object> {
					{ "@MoveId", move.Id },
					{ "@Accuracy", move.Accuracy },
					{ "@FlinchChance", move.FlinchChance },
					{ "@CritRate", move.CritRate },
					{ "@MaxPp", move.MaxPp },
					{ "@Power", move.Power },
					{ "@Drain", move.Drain },
					{ "@Healing", move.Healing },
					{ "@MoveName", move.MoveName },
					{ "@DamageClass", move.DamageClass },
					{ "@MultiHitType", move.MultiHitType },
					{ "@FlavourText", move.FlavourText },
					{ "@Ailment", JsonConvert.SerializeObject(move.Ailment) },
					{ "@StatChanges", JsonConvert.SerializeObject(move.StatChanges) },
					{ "@T", move.Type.Id }
				};
				var parameters = new DynamicParameters(dictionary);
				string sql = "INSERT INTO Move VALUES(@MoveId, @Accuracy, @FlinchChance, @CritRate, @MaxPp, @Power, @Drain, @Healing, @MoveName, @DamageClass, @MultiHitType, @FlavourText, @Ailment, @StatChanges, @T)";
				connection.Execute(sql, parameters);
			}
		}

		public bool MoveExists(string name) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				var dictionary = new Dictionary<string, object> {
					{"@MoveName", name}
				};
				var parameters = new DynamicParameters(dictionary);
				return connection.Query("SELECT MoveName FROM Move WHERE MoveName = @MoveName", parameters).Count() > 0;
			}
		}

		public Move GetMove(string name) {
			string sql =
				"SELECT Accuracy, FlinchChance, CritRate, MaxPp, Power, Drain, Healing, MoveName, DamageClass, MultiHitType, FlavourText " +
				"FROM Move WHERE MoveName = @MoveName";
			string getMoveId = "SELECT MoveId FROM Move WHERE MoveName = @MoveName";
			string getAilment = "SELECT Ailment FROM Move WHERE MoveName = @MoveName";
			string getStatChanges = "SELECT StatChanges FROM Move WHERE MoveName = @MoveName";
			string getTypeId = "SELECT TypeId FROM Move WHERE MoveName = @MoveName";

			using (var connection = DbConnectionFactory()) {
				connection.Open();
				var dictionary = new Dictionary<string, object> {
					{"@MoveName", name}
				};
				var parameters = new DynamicParameters(dictionary);
				Move move = connection.QuerySingle<Move>(sql, parameters);
				move.Id = connection.QuerySingle<int>(getMoveId, parameters);
				var ailment = JsonConvert.DeserializeObject<Dictionary<string, string>>(connection.QuerySingle<string>(getAilment, parameters)) ;
				var statChanges = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(connection.QuerySingle<string>(getStatChanges, parameters));
				Type type = Type.GetTypeById(connection.QuerySingle<int>(getTypeId, parameters));
				move.Ailment = ailment;
				move.StatChanges = statChanges;
				move.Type = type;
				return move;
			}
		}

		public string GetMoveName(int id) {
			var dictionary = new Dictionary<string, object> {
				{"@MoveId", id}
			};
			var parameters = new DynamicParameters(dictionary);
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.QuerySingle<string>("SELECT MoveName FROM Move WHERE MoveId = @MoveId", parameters);
			}
		}
	}
}
