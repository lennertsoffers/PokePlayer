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

namespace Pokeplayer_Library.DAL {
	class TypeRepository : SqlLiteBase {
		public TypeRepository() {
			if (!DatabaseExists()) {
				CreateDatabase();
			}
		}

		public void InsertType(Type type) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute("INSERT INTO Type VALUES(@Id, @TypeName)", type);
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
			}
		}

		public bool TypeExists(string typeName) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				var dictionary = new Dictionary<string, object>
				{
					{ "@TypeName", typeName }
				};
				var parameters = new DynamicParameters(dictionary);
				bool exists = connection.Query("SELECT TypeName FROM Type WHERE TypeName = @TypeName", parameters).Count() > 0;
				return exists;
			}
		}

		public Type GetType(string typeName) {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				var paramDict1 = new Dictionary<string, object>
				{
					{ "@TypeName", typeName }
				};
				var parameters = new DynamicParameters(paramDict1);
				int id = connection.QuerySingle<int>("SELECT TypeId, TypeName FROM Type WHERE TypeName = @TypeName", parameters);
				string tName = connection.QuerySingle<string>("SELECT TypeName FROM Type WHERE TypeName = @TypeName", parameters);
				Type type = new Type(id, tName);
				var paramDict2 = new Dictionary<string, object>
				{
					{ "@TypeId", type.Id }
				};
				parameters = new DynamicParameters(paramDict2);
				string noDamageTo = "SELECT (SELECT t.Name FROM TypeName t WHERE t.TypeNameId = d.TypeNameId) FROM DamageRelations d WHERE d.TypeId = @TypeId AND d.NoDamageTo = 1";
				string halfDamageTo = "SELECT (SELECT t.Name FROM TypeName t WHERE t.TypeNameId = d.TypeNameId) FROM DamageRelations d WHERE d.TypeId = @TypeId AND d.HalfDamageTo = 1";
				string doubleDamageTo = "SELECT (SELECT t.Name FROM TypeName t WHERE t.TypeNameId = d.TypeNameId) FROM DamageRelations d WHERE d.TypeId = @TypeId AND d.DoubleDamageTo = 1";

				foreach (var name in connection.Query<string>(noDamageTo, parameters)) {
					type.NoDamageTo.Add(name);
				};
				
				foreach (var name in connection.Query<string>(halfDamageTo, parameters)) {
					type.HalfDamageTo.Add(name);
				};
				
				foreach (var name in connection.Query<string>(doubleDamageTo, parameters)) {
					type.DoubleDamageTo.Add(name);
				};

				return type;
			}
		}

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
