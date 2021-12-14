using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PokePlayer.DAL;
using PokePlayer_Library.Models.Pokemon;

namespace Pokeplayer_Library.DAL {
	class StatRepository : SqlLiteBase {
		public StatRepository() {
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		public int InsertStat(Stat stat) {
			string sql = "INSERT INTO Stat VALUES(@Id, @StatId, @StatName, @BaseStat, @Iv, @StatValue, @PreviousStat);";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Execute(sql, stat);
			}
		}

		public void UpdateStat(Stat stat) {
			string updateStat = "UPDATE Stat SET StatValue = @StatValue, PreviousStat = @PreviousStat WHERE Id = @Id";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(updateStat, stat);
				connection.Close();
			}
		}

		public int GetAmountOfStats() {
			string sql = "SELECT Id FROM Stat";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query<int>(sql).Count();
			}
		}

		public Stat GetStat(int id) {
			string sql = "SELECT * FROM Stat WHERE Id = @Id";
			var dictionary = new Dictionary<string, object> {
				{"@Id", id}
			};
			var parameters = new DynamicParameters(dictionary);
			using (var connection = DbConnectionFactory()) {
				return connection.QuerySingle<Stat>(sql, parameters);
			}
		}
	}
}
