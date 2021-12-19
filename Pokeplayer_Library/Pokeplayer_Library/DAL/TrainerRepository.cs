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
			string sql = "INSERT INTO Trainer VALUES(@TrainerId, @Name, @Password, @RegenPokemon, @CarryPokemon, @PokemonList)";

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
				{ "@RegenPokemon", trainer.RegenPokemon },
				{ "@CarryPokemon", JsonConvert.SerializeObject(carryPokemon) },
				{ "@PokemonList", JsonConvert.SerializeObject(pokemonList) },
			};

			Debug.WriteLine(dictionary["@RegenPokemon"]);

			var parameters = new DynamicParameters(dictionary);

			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(sql, parameters);
				connection.Close();
			}
		}

		public void UpdateTrainer(Trainer trainer) {
			string sql = "UPDATE Trainer SET RegenPokemon = @RegenPokemon, CarryPokemon = @CarryPokemon, PokemonList = @PokemonList WHERE TrainerId = @TrainerId";

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

		public Trainer GetTrainer(string name) {
			string getTrainer = "SELECT TrainerId, Name, Password, RegenPokemon FROM Trainer WHERE Name = @Name";
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

				if (trainer.RegenPokemon <= DateTimeOffset.Now.ToUnixTimeMilliseconds()) {
					foreach (var pokemon in pokemonList) {
						RegenPokemon(pokemon);
					}

					foreach (var pokemon in carryPokemon.Values) {
						RegenPokemon(pokemon);
					}

					trainer.RegenPokemon = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 86400000;
				}

				trainer.CarryPokemonList = carryPokemon;
				trainer.PokemonList = pokemonList;
				UpdateTrainer(trainer);

				return trainer;
			}
		}

		private void RegenPokemon(Pokemon pokemon) {
			pokemon.Hp = pokemon.GetStat("hp").StatValue;
			pokemon.NonVolatileStatus = new Dictionary<string, int> {
				{"BRN", 0},
				{"FRZ", 0},
				{"PAR", 0},
				{"PSN", 0},
				{"SLP", -1},
				{"FNT", 0}
			};
			foreach (var key in pokemon.MovePpMapping.Keys) {
				pokemon.MovePpMapping[key] = pokemon.Moves[key].MaxPp;
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
