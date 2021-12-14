using System.Linq;
using Dapper;
using PokePlayer_Library.Models.Pokemon;

namespace PokePlayer.DAL {
	class SpecieRepository : SqlLiteBase {
		public SpecieRepository() {
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		public int InsertSpecie(Specie specie) {
			string sql = "INSERT INTO Specie VALUES(@SpecieId, @CaptureRate, @GrowthRate, @SpecieName, @FlavorText, @IsLegendary, @IsMythical);";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Execute(sql, specie);
			}
		}

		public bool SpecieExists(int specieId) {
			string sql = "SELECT * FROM Specie WHERE SpecieId = " + specieId;
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query(sql).Count() > 0;
			}
		}

		public Specie GetSpecie(int specieId) {
			string sql = "SELECT * FROM Specie WHERE SpecieId = " + specieId;
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.QuerySingle<Specie>(sql);
			}
		}
	}
}
