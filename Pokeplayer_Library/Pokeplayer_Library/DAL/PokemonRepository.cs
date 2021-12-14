using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using PokePlayer.DAL;
using PokePlayer_Library.Models.Pokemon;
using Type = PokePlayer_Library.Models.Pokemon.Type;

namespace Pokeplayer_Library.DAL {
	class PokemonRepository : SqlLiteBase {
		public PokemonRepository() {
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		public void InsertPokemon(Pokemon pokemon) {
			Debug.WriteLine(JsonConvert.SerializeObject(pokemon.Moves));
			var dictionary = new Dictionary<string, object> {
				{ "@1", pokemon.Id },
				{ "@2", pokemon.PokemonId },
				{ "@3", pokemon.Level },
				{ "@4", pokemon.BaseExperience },
				{ "@5", pokemon.TotalExperience },
				{ "@6", pokemon.NextLevelExperience },
				{ "@7", pokemon.Hp },
				{ "@8", pokemon.NickName },
				{ "@9", pokemon.SpriteFront },
				{ "@10", pokemon.SpriteBack },
				{ "@11", pokemon.Shiny },
				{ "@12", JsonConvert.SerializeObject(pokemon.InBattleStats) },
				{ "@13", JsonConvert.SerializeObject(pokemon.NonVolatileStatus) },
				{ "@14", JsonConvert.SerializeObject(pokemon.VolatileStatus) },
				{ "@15", JsonConvert.SerializeObject(pokemon.PossibleMoves) },
				{ "@16", pokemon.Moves.Count > 0 ? pokemon.Moves[1].Id : "-1" },
				{ "@17", pokemon.Moves.Count > 1 ? pokemon.Moves[2].Id : "-1" },
				{ "@18", pokemon.Moves.Count > 2 ? pokemon.Moves[3].Id : "-1" },
				{ "@19", pokemon.Moves.Count > 3 ? pokemon.Moves[4].Id : "-1" },
				{ "@20", pokemon.Stats["hp"].Id },
				{ "@21", pokemon.Stats["attack"].Id },
				{ "@22", pokemon.Stats["defense"].Id },
				{ "@23", pokemon.Stats["special-attack"].Id },
				{ "@24", pokemon.Stats["special-defense"].Id },
				{ "@25", pokemon.Stats["speed"].Id },
				{ "@26", pokemon.Specie.SpecieId },
			};
			var parameters = new DynamicParameters(dictionary);
			string fillPokemon = "INSERT INTO Pokemon VALUES(" +
			                     "@1, " +
			                     "@2, " +
			                     "@3, " +
			                     "@4, " +
			                     "@5, " +
			                     "@6, " +
			                     "@7, " +
			                     "@8, " +
			                     "@9, " +
			                     "@10, " +
			                     "@11, " +
			                     "@12, " +
			                     "@13, " +
			                     "@14, " +
			                     "@15, " +
			                     "@16, " +
			                     "@17, " +
			                     "@18, " +
			                     "@19, " +
			                     "@20, " +
			                     "@21, " +
			                     "@22, " +
			                     "@23, " +
			                     "@24, " +
			                     "@25, " +
			                     "@26)";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(fillPokemon, parameters);

				foreach (var type in pokemon.TypeList) {
					dictionary = new Dictionary<string, object> {
						{ "@PokemonId", pokemon.Id },
						{ "@TypeId", type.Id }
					};
					parameters = new DynamicParameters(dictionary);
					connection.Execute("INSERT INTO PokemonType VALUES(@PokemonId, @TypeId)", parameters);
				}
				connection.Close();
			}
		}

		public void UpdatePokemon(Pokemon pokemon) {
			var dictionary = new Dictionary<string, object> {
				{"@PId", pokemon.Id},
				{"@Level", pokemon.Level},
				{"@TotalExperience", pokemon.TotalExperience},
				{"@NextLevelExperience", pokemon.NextLevelExperience},
				{"@Hp", pokemon.Hp},
				{"@NonVolatileStatus", JsonConvert.SerializeObject(pokemon.NonVolatileStatus)},
				{"@Move1Id", pokemon.Moves.Count > 0 ? pokemon.Moves[1].Id : "-1"},
				{"@Move2Id", pokemon.Moves.Count > 1 ? pokemon.Moves[2].Id : "-1"},
				{"@Move3Id", pokemon.Moves.Count > 2 ? pokemon.Moves[3].Id : "-1"},
				{"@Move4Id", pokemon.Moves.Count > 3 ? pokemon.Moves[4].Id : "-1"}
			};
			var parameters = new DynamicParameters(dictionary);
			string updatePokemon = "UPDATE Pokemon SET " +
			                       "Level = @Level, " +
			                       "TotalExperience = @TotalExperience, " +
			                       "NextLevelExperience = @NextLevelExperience, " +
			                       "Hp = @Hp, " +
			                       "NonVolatileStatus = @NonVolatileStatus, " +
			                       "Move1Id = @Move1Id, " +
			                       "Move2Id = @Move2Id, " +
			                       "Move3Id = @Move3Id, " +
			                       "Move4Id = @Move4Id " +
			                       "WHERE Id = @PId";

			StatRepository statRepository = new StatRepository();
			foreach (var stat in pokemon.Stats.Values) {
				statRepository.UpdateStat(stat);
			}

			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(updatePokemon, parameters);
				connection.Close();
			}
		}

		public Pokemon GetPokemon(int id) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				string sql = "SELECT Id, PokemonId, Level, BaseExperience, TotalExperience, NextLevelExperience, Hp, NickName, SpriteFront, SpriteBack, Shiny " +
				             "FROM Pokemon WHERE Id = @Id";
				string getInBattleStats = "SELECT InBattleStats FROM Pokemon WHERE Id = @Id";
				string getNonvolatilestatus = "SELECT NonVolatileStatus FROM Pokemon WHERE Id = @Id";
				string getVolatileStatus = "SELECT VolatileStatus FROM Pokemon WHERE Id = @Id";
				string getPossiblemoves = "SELECT PossibleMoves FROM Pokemon WHERE Id = @Id";
				string getMoveIds = "SELECT Move1Id, Move2Id, Move3Id, Move4Id FROM Pokemon WHERE Id = @Id";
				string getStats = "SELECT StatsHpId, StatsAttackId, StatsDefenseId, StatsSpAttackId, StatsSpDefenseId, StatsSpeedId " +
				                          "FROM Pokemon WHERE Id = @Id";
				string getSpecieId = "SELECT SpecieId FROM Pokemon WHERE Id = @Id";
				var dictionary = new Dictionary<string, object> {
					{ "@Id", id }
				};
				var parameters = new DynamicParameters(dictionary);
				Pokemon pokemon = connection.QuerySingle<Pokemon>(sql, parameters);

				pokemon.InBattleStats = JsonConvert.DeserializeObject<Dictionary<string, int>>(connection.QuerySingle<string>(getInBattleStats, parameters));
				pokemon.NonVolatileStatus = JsonConvert.DeserializeObject<Dictionary<string, int>>(connection.QuerySingle<string>(getNonvolatilestatus, parameters));
				pokemon.VolatileStatus = JsonConvert.DeserializeObject<Dictionary<string, int>>(connection.QuerySingle<string>(getVolatileStatus, parameters));
				pokemon.PossibleMoves = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(connection.QuerySingle<string>(getPossiblemoves, parameters));

				var moveResult = connection.Query(getMoveIds, parameters).Single();
				Dictionary<int, Move> moves = new Dictionary<int, Move>();
				if ((int) moveResult.Move1Id != -1) {
					moves.Add(1, Move.GetMoveById((int) moveResult.Move1Id));
				}
				if ((int) moveResult.Move2Id != -1) {
					moves.Add(2, Move.GetMoveById((int) moveResult.Move2Id));
				}
				if ((int) moveResult.Move3Id != -1) {
					moves.Add(3, Move.GetMoveById((int) moveResult.Move3Id));
				}
				if ((int) moveResult.Move4Id != -1) {
					moves.Add(4, Move.GetMoveById((int) moveResult.Move4Id));
				}

				pokemon.Moves = moves;

				var statsResult = connection.Query(getStats, parameters).Single();
				Dictionary<string, Stat> stats = new Dictionary<string, Stat> {
					{"hp", Stat.GetStat((int) statsResult.StatsHpId)},
					{"attack", Stat.GetStat((int) statsResult.StatsAttackId)},
					{"defense", Stat.GetStat((int) statsResult.StatsDefenseId)},
					{"special-attack", Stat.GetStat((int) statsResult.StatsSpAttackId)},
					{"special-defense", Stat.GetStat((int) statsResult.StatsSpDefenseId)},
					{"speed", Stat.GetStat((int) statsResult.StatsSpeedId)}
				};
				pokemon.Stats = stats;

				pokemon.Specie = Specie.GetSpecie(connection.QuerySingle<int>(getSpecieId, parameters));

				pokemon.TypeList = new List<Type>();
				foreach (var typeId in connection.Query<int>("SELECT TypeId FROM PokemonType WHERE PokemonId = @Id", parameters)) {
					pokemon.TypeList.Add(Type.GetTypeById(typeId));
				}

				return pokemon;
			}
		}

		public int GetAmountOfPokemon() {
			string sql = "SELECT Id FROM Pokemon";
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				return connection.Query<int>(sql).Count();
			}
		}
	}
}
