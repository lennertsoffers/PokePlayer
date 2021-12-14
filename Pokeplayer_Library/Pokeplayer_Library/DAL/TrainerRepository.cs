using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using PokePlayer.DAL;
using PokePlayer_Library.Models;
using PokePlayer_Library.Models.Pokemon;

namespace Pokeplayer_Library.DAL {
	class TrainerRepository : SqlLiteBase {
		public TrainerRepository() {
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		public void InsertTrainer(Trainer trainer) {
			string sql = "INSERT INTO Trainer VALUES(@TrainerId, @Name, @Password, @CarryPokemon, @PokemonList)";

			var pokemonList = new List<int>();
			var carryPokemon = new Dictionary<int, int>();
			foreach (var pokemon in trainer.PokemonList) {
				pokemonList.Add(pokemon.Id);
			}
			foreach (var key in trainer.CarryPokemonList.Keys) {
				carryPokemon.Add(key, trainer.CarryPokemonList[key].Id);
			}

			var dictionary = new Dictionary<string, object> {
				{ "@TrainerId", trainer.TrainerId },
				{ "@Name", trainer.Name },
				{ "@Password", trainer.Password },
				{ "@CarryPokemon", JsonConvert.SerializeObject(carryPokemon) },
				{ "@PokemonList", JsonConvert.SerializeObject(pokemonList) },
			};

			var parameters = new DynamicParameters(dictionary);

			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(sql, parameters);
				connection.Close();
			}
		}

		public void UpdateTrainer(Trainer trainer) {
			string sql = "UPDATE Trainer SET CarryPokemon = @CarryPokemon, PokemonList = @PokemonList WHERE TrainerId = @TrainerId";

			var pokemonList = new List<int>();
			var carryPokemon = new Dictionary<int, int>();
			foreach (var pokemon in trainer.PokemonList) {
				pokemonList.Add(pokemon.Id);
			}
			foreach (var key in trainer.CarryPokemonList.Keys) {
				carryPokemon.Add(key, trainer.CarryPokemonList[key].Id);
			}

			var dictionary = new Dictionary<string, object> {
				{ "@TrainerId", trainer.TrainerId },
				{ "@CarryPokemon", JsonConvert.SerializeObject(carryPokemon) },
				{ "@PokemonList", JsonConvert.SerializeObject(pokemonList) },
			};

			var parameters = new DynamicParameters(dictionary);

			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(sql, parameters);
				connection.Close();
			}
		}

		public Trainer GetTrainer(string name) {
			string getTrainer = "SELECT TrainerId, Name, Password FROM Trainer WHERE Name = @Name";
			string getCarryPokemon = "SELECT CarryPokemon FROM Trainer WHERE Name = @Name";
			string getPokemonList = "SELECT PokemonList FROM Trainer WHERE Name = @Name";
			var dictionary = new Dictionary<string, object> {
				{"@Name", name}
			};
			var parameters = new DynamicParameters(dictionary);

			using (var connection = DbConnectionFactory()) {
				connection.Open();
				Trainer trainer = connection.QuerySingle<Trainer>(getTrainer, parameters);
				Dictionary<int, int> carryPokemonIds = JsonConvert.DeserializeObject<Dictionary<int, int>>((string) connection.QuerySingle<string>(getCarryPokemon, parameters));
				List<int> pokemonIds = JsonConvert.DeserializeObject<List<int>>((string) connection.QuerySingle<string>(getPokemonList, parameters));
				connection.Close();

				Dictionary<int, Pokemon> carryPokemon = new Dictionary<int, Pokemon>();
				foreach (var key in carryPokemonIds.Keys) {
					carryPokemon.Add(key, Pokemon.GetPokemon(carryPokemonIds[key]));
				}
				
				List<Pokemon> pokemonList = new List<Pokemon>();
				foreach (var pokemonId in pokemonIds) {
					pokemonList.Add(Pokemon.GetPokemon(pokemonId));
				}
				
				trainer.CarryPokemonList = carryPokemon;
				trainer.PokemonList = pokemonList;

				return trainer;
			}
		}

		public int GetAmountOfTrainers() {
			string sql = "SELECT TrainerId FROM Trainer";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query<int>(sql).Count();
			}
		}

		public bool TrainerExists(string name) {
			string sql = "SELECT TrainerId FROM Trainer WHERE Name = @Name";
			var dictionary = new Dictionary<string, object> {
				{ "@Name", name }
			};
			var parameters = new DynamicParameters(dictionary);
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query(sql, parameters).Count() > 0;
			}
		}
	}
}
