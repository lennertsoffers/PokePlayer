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

/// <summary>
/// Class model for a trainer repository
/// Inherits from SqlLiteBase
/// </summary>

namespace Pokeplayer_Library.DAL {
	class TrainerRepository : SqlLiteBase {
		public TrainerRepository() {
			// Database is created when it doensn't exist
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		// Inserts a trainer object in the database
		public void InsertTrainer(Trainer trainer) {
			// Sql strings that must be executed
			string sql = "INSERT INTO Trainer VALUES(@TrainerId, @Name, @Password, @RegenPokemon, @CarryPokemon, @PokemonList)";

			// Creation of lists with id's that can be serialized
			var pokemonList = new List<int>();
			var carryPokemon = new Dictionary<int, int>();
			foreach (var pokemon in trainer.PokemonList) {
				pokemonList.Add(pokemon.Id);
			}
			foreach (var key in trainer.CarryPokemonList.Keys) {
				carryPokemon.Add(key, trainer.CarryPokemonList[key].Id);
			}

			// It isn't possible to just let Dapper handle the conversion of a trainer object to a database field
			// Some attributes must be serialized first
			var dictionary = new Dictionary<string, object> {
				{ "@TrainerId", trainer.TrainerId },
				{ "@Name", trainer.Name },
				{ "@Password", trainer.Password },
				{ "@RegenPokemon", trainer.RegenPokemon },
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

		// Updates the parameters from a trainer in the database
		// Only the parameters that can be updated are updated
		public void UpdateTrainer(Trainer trainer) {
			// Sql string that must be executed
			string sql = "UPDATE Trainer SET RegenPokemon = @RegenPokemon, CarryPokemon = @CarryPokemon, PokemonList = @PokemonList WHERE TrainerId = @TrainerId";

			// Creation of lists with id's that can be serialized
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
				{ "@RegenPokemon", trainer.RegenPokemon },
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

		// Creates a new trainer object from the database
		public Trainer GetTrainer(string name) {
			// Sql strings that must be executed
			string getTrainer = "SELECT TrainerId, Name, Password, RegenPokemon FROM Trainer WHERE Name = @Name";
			string getCarryPokemon = "SELECT CarryPokemon FROM Trainer WHERE Name = @Name";
			string getPokemonList = "SELECT PokemonList FROM Trainer WHERE Name = @Name";
			var dictionary = new Dictionary<string, object> {
				{"@Name", name}
			};
			var parameters = new DynamicParameters(dictionary);

			using (var connection = DbConnectionFactory()) {
				connection.Open();

				// Gets the basic fields from the trainer that don't need to be deserialized
				// Dapper automatically creates a new trainer object from it
				Trainer trainer = connection.QuerySingle<Trainer>(getTrainer, parameters);

				// Gets the pokemon id's of the party pokemons
				Dictionary<int, int> carryPokemonIds = JsonConvert.DeserializeObject<Dictionary<int, int>>((string) connection.QuerySingle<string>(getCarryPokemon, parameters));

				// Gets the pokemon id's from all pokemons
				List<int> pokemonIds = JsonConvert.DeserializeObject<List<int>>((string) connection.QuerySingle<string>(getPokemonList, parameters));
				connection.Close();

				// For each pokemon id in the carrypokemon id's
				// The corresponding pokemon is added
				Dictionary<int, Pokemon> carryPokemon = new Dictionary<int, Pokemon>();
				foreach (var key in carryPokemonIds.Keys) {
					carryPokemon.Add(key, Pokemon.GetPokemon(carryPokemonIds[key]));
				}

				// For each pokemon id in all pokemon id's
				// The corresponding pokemon is added
				List<Pokemon> pokemonList = new List<Pokemon>();
				foreach (var pokemonId in pokemonIds) {
					pokemonList.Add(Pokemon.GetPokemon(pokemonId));
				}

				// If a day has passed since the last regen of the pokemon, it is regenned
				if (trainer.RegenPokemon <= DateTimeOffset.Now.ToUnixTimeMilliseconds()) {
					foreach (var pokemon in pokemonList) {
						pokemon.RegenPokemon();
					}

					foreach (var pokemon in carryPokemon.Values) {
						pokemon.RegenPokemon();
					}

					// The next time to regen the pokemon is set
					trainer.RegenPokemon = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 86400000;
				}

				trainer.CarryPokemonList = carryPokemon;
				trainer.PokemonList = pokemonList;

				// The trainer must be updated becauses there was a change in its pokemons
				UpdateTrainer(trainer);

				return trainer;
			}
		}

		// Gets the count of all the trainers stored in the database
		public int GetAmountOfTrainers() {
			string sql = "SELECT TrainerId FROM Trainer";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query<int>(sql).Count();
			}
		}

		// Checks if a trainer with this name already exists
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
