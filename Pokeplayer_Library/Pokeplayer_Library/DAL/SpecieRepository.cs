using System.Linq;
using Dapper;
using PokePlayer_Library.Models.Pokemon;

/// <summary>
/// Class model for a specie repository
/// Inherits from SqlLiteBase
/// </summary>

namespace PokePlayer.DAL {
	class SpecieRepository : SqlLiteBase {
		public SpecieRepository() {
			// Database is created when it doensn't exist
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		// Inserts a specie object in the database
		public void InsertSpecie(Specie specie) {
			string sql = "INSERT INTO Specie VALUES(@SpecieId, @CaptureRate, @GrowthRate, @SpecieName, @FlavorText, @IsLegendary, @IsMythical);";
			using (var connection = DbConnectionFactory()) {
				connection.Open();

				// Dapper handles the conversion of the attributes of the specie object to the parameters for Sqlite
				connection.Execute(sql, specie);
				connection.Close();
			}
		}

		// Checks if the specie already exists in the database
		public bool SpecieExists(int specieId) {
			string sql = "SELECT * FROM Specie WHERE SpecieId = " + specieId;
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query(sql).Count() > 0;
			}
		}

		// Returns a specie object corresponding to the specieId
		public Specie GetSpecie(int specieId) {
			string sql = "SELECT * FROM Specie WHERE SpecieId = " + specieId;
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				
				// Dapper automatically creates a specie object with the no-arg constructor of the specie class
				Specie specie = connection.QuerySingle<Specie>(sql);
				connection.Close();
				return specie;
			}
		}
	}
}
