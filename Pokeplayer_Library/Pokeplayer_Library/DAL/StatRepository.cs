using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PokePlayer.DAL;
using PokePlayer_Library.Models.Pokemon;

/// <summary>
/// Class model for a stat repository
/// Inherits from SqlLiteBase
/// </summary>

namespace Pokeplayer_Library.DAL {
	class StatRepository : SqlLiteBase {
		public StatRepository() {
			// Database is created when it doensn't exist
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		// Inserts a stat object in the database
		public void InsertStat(Stat stat) {
			string sql = "INSERT INTO Stat VALUES(@Id, @StatId, @StatName, @BaseStat, @Iv, @StatValue, @PreviousStat);";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				
				// Dapper handles the conversion of the attributes of the specie object to the parameters for Sqlite
				connection.Execute(sql, stat);
				connection.Close();
			}
		}

		// Updates the stat with some new values
		// Only the values that can be changed are updated
		public void UpdateStat(Stat stat) {
			string updateStat = "UPDATE Stat SET StatValue = @StatValue, PreviousStat = @PreviousStat WHERE Id = @Id";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(updateStat, stat);
				connection.Close();
			}
		}

		// Returns the amout of stats there are stored in the database
		public int GetAmountOfStats() {
			string sql = "SELECT Id FROM Stat";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query<int>(sql).Count();
			}
		}

		// Creates a stat object corresponding with the given id
		public Stat GetStat(int id) {
			string sql = "SELECT * FROM Stat WHERE Id = @Id";
			var dictionary = new Dictionary<string, object> {
				{"@Id", id}
			};
			var parameters = new DynamicParameters(dictionary);
			using (var connection = DbConnectionFactory()) {
				connection.Open();

				// Dapper automatically creates a stat object with the no-arg constructor of the stat class
				Stat stat = connection.QuerySingle<Stat>(sql, parameters);
				connection.Close();
				return stat;
			}
		}
	}
}
