using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PokePlayer.DAL;
using Type = PokePlayer_Library.Models.Pokemon.Type;

/// <summary>
/// Class model for a type repository
/// Inherits from SqlLiteBase
/// </summary>

namespace Pokeplayer_Library.DAL {
	class TypeRepository : SqlLiteBase {
		public TypeRepository() {
			// Database is created when it doensn't exist
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		// Inserts a move object in the database
		public void InsertType(Type type) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();

				// Inserts the type in the type table
				connection.Execute("INSERT INTO Type VALUES(@Id, @TypeName)", type);

				// Next three loops fill in the damage relations table
				// This is an assosiative table to connect the types with each other
				foreach (var typeName in type.NoDamageTo) {
					
					var dictionary = new Dictionary<string, object>
					{
						{ "@Id", type.Id },
						{ "@TypeName", typeName }
					};
					var parameters = new DynamicParameters(dictionary);
					string sql = "INSERT INTO DamageRelations VALUES(" +
					             "@Id," +
					             "(SELECT TypeNameId FROM TypeName WHERE Name = @TypeName)," +
					             "1," +
					             "0," +
					             "0);";
					connection.Execute(sql, parameters);
				}

				foreach (var typeName in type.HalfDamageTo) {
					var dictionary = new Dictionary<string, object>
					{
						{ "@Id", type.Id },
						{ "@TypeName", typeName }
					};
					var parameters = new DynamicParameters(dictionary);
					string sql = "INSERT INTO DamageRelations VALUES(" +
					             "@Id," +
					             $"(SELECT TypeNameId FROM TypeName WHERE Name = @TypeName)," +
					             "0," +
					             "1," +
					             "0);";
					connection.Execute(sql, parameters);
				}

				foreach (var typeName in type.DoubleDamageTo) {
					var dictionary = new Dictionary<string, object>
					{
						{ "@Id", type.Id },
						{ "@TypeName", typeName }
					};
					var parameters = new DynamicParameters(dictionary);
					string sql = "INSERT INTO DamageRelations VALUES(" +
					             "@Id," +
					             $"(SELECT TypeNameId FROM TypeName WHERE Name = @TypeName)," +
					             "0," +
					             "0," +
					             "1);";
					connection.Execute(sql, parameters);
				}

				connection.Close();
			}
		}

		// Checks if the type with the given name exists
		public bool TypeExists(string typeName) {
			using (var connection = DbConnectionFactory()) {
				var dictionary = new Dictionary<string, object>
				{
					{ "@TypeName", typeName }
				};
				var parameters = new DynamicParameters(dictionary);
				connection.Open();
				bool exists = connection.Query("SELECT TypeName FROM Type WHERE TypeName = @TypeName", parameters).Count() > 0;
				connection.Close();
				return exists;
			}
		}

		// Creates a new type object from the name given
		public Type GetType(string typeName) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				var paramDict1 = new Dictionary<string, object>
				{
					{ "@TypeName", typeName }
				};
				var parameters = new DynamicParameters(paramDict1);
				
				// Gets the id of the type
				int id = connection.QuerySingle<int>("SELECT TypeId FROM Type WHERE TypeName = @TypeName", parameters);

				// Creates a new type object from the id and name
				Type type = new Type(id, typeName);

				var paramDict2 = new Dictionary<string, object>
				{
					{ "@TypeId", type.Id }
				};
				parameters = new DynamicParameters(paramDict2);

				// Sql strings to query the damage relations of the type
				string noDamageTo = "SELECT (SELECT t.Name FROM TypeName t WHERE t.TypeNameId = d.TypeNameId) FROM DamageRelations d WHERE d.TypeId = @TypeId AND d.NoDamageTo = 1";
				string halfDamageTo = "SELECT (SELECT t.Name FROM TypeName t WHERE t.TypeNameId = d.TypeNameId) FROM DamageRelations d WHERE d.TypeId = @TypeId AND d.HalfDamageTo = 1";
				string doubleDamageTo = "SELECT (SELECT t.Name FROM TypeName t WHERE t.TypeNameId = d.TypeNameId) FROM DamageRelations d WHERE d.TypeId = @TypeId AND d.DoubleDamageTo = 1";

				// For each damage relation, the name is added to the corresponding attribute of the type
				foreach (var name in connection.Query<string>(noDamageTo, parameters)) {
					type.NoDamageTo.Add(name);
				};
				
				foreach (var name in connection.Query<string>(halfDamageTo, parameters)) {
					type.HalfDamageTo.Add(name);
				};
				
				foreach (var name in connection.Query<string>(doubleDamageTo, parameters)) {
					type.DoubleDamageTo.Add(name);
				};

				connection.Close();

				return type;
			}
		}

		// Returns the name of a type when the name is given
		public string GetTypeName(int id) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				var dictionary = new Dictionary<string, object> {
					{"@TypeId", id}
				};
				var parameters = new DynamicParameters(dictionary);
				return connection.QuerySingle<string>("SELECT TypeName FROM Type WHERE TypeId = @TypeId", parameters);
			}
		}
	}
}
