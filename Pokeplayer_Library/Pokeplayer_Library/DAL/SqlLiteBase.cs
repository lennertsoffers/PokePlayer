using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;
using PokePlayer_Library.Models.Pokemon;

namespace PokePlayer.DAL {
	class SqlLiteBase {
		public SqlLiteBase() {}

		public static SqliteConnection DbConnectionFactory() {
			return new SqliteConnection(@"DataSource=PokePlayerDB.sqlite");
		}

		protected static bool DatabaseExists() {
			return File.Exists("PokePlayerDB.sqlite");
		}

		protected static void CreateDatabase() {
			using (var connection = DbConnectionFactory()) {
				connection.Open();
				connection.Execute(
					@"
					CREATE TABLE Specie(
						SpecieId			INTEGER PRIMARY KEY,
						CaptureRate			INTEGER,
						GrowthRate			VARCHAR(30),
						SpecieName			VARCHAR(30),
						FlavorText			VARCHAR(200),
						IsLegendary			BOOLEAN,
						IsMythical			BOOLEAN
					);

					CREATE TABLE Stat(
						Id					INTEGER PRIMARY KEY,
						StatId				INTEGER,
						StatName			VARCHAR(30),
						BaseStat			INTEGER,
						Iv					INTEGER,
						StatValue			INTEGER,
						PreviousStat		INTEGER
					);

					CREATE TABLE Type(
						TypeId				INTEGER PRIMARY KEY,
						TypeName			VARCHAR(30)
					);

					CREATE TABLE TypeName(
						TypeNameId			INTEGER PRIMARY KEY,
						Name				VARCHAR(30)
					);
					INSERT INTO TypeName VALUES(1, 'normal');
					INSERT INTO TypeName VALUES(2, 'fighting');
					INSERT INTO TypeName VALUES(3, 'flying');
					INSERT INTO TypeName VALUES(4, 'poison');
					INSERT INTO TypeName VALUES(5, 'ground');
					INSERT INTO TypeName VALUES(6, 'bug');
					INSERT INTO TypeName VALUES(7, 'rock');
					INSERT INTO TypeName VALUES(8, 'ghost');
					INSERT INTO TypeName VALUES(9, 'steel');
					INSERT INTO TypeName VALUES(10, 'fire');
					INSERT INTO TypeName VALUES(11, 'water');
					INSERT INTO TypeName VALUES(12, 'grass');
					INSERT INTO TypeName VALUES(13, 'electric');
					INSERT INTO TypeName VALUES(14, 'psychic');
					INSERT INTO TypeName VALUES(15, 'ice');
					INSERT INTO TypeName VALUES(16, 'dragon');
					INSERT INTO TypeName VALUES(17, 'dark');
					INSERT INTO TypeName VALUES(18, 'fairy');

					CREATE TABLE DamageRelations(
						TypeId				INTEGER NOT NULL,
						TypeNameId			INTEGER NOT NULL,
						NoDamageTo			BOOLEAN,
						HalfDamageTo		BOOLEAN,
						DoubleDamageTo		BOOLEAN,
						PRIMARY KEY (TypeId,TypeNameId, NoDamageTo, HalfDamageTo, DoubleDamageTo),
						CONSTRAINT FK_DamageRelations_Type FOREIGN KEY (TypeId) REFERENCES Type(TypeId),
						CONSTRAINT FK_DamageRelations_TypeName FOREIGN KEY (TypeNameId) REFERENCES TypeName(TypeNameId)
					);

					CREATE TABLE Move(
						MoveId				INTEGER PRIMARY KEY,
						Accuracy			INTEGER,
						FlinchChance		INTEGER,
						CritRate			INTEGER,
						MaxPp				INTEGER,
						Power				INTEGER,
						Drain				INTEGER,
						Healing				INTEGER,
						MoveName			VARCHAR(30),
						DamageClass			VARCHAR(30),
						MultiHitType		VARCHAR(30),
						FlavourText			VARCHAR(200),
						Ailment				VARCHAR(200),
						StatChanges			VARCHAR(200),
						TypeId				INTEGER,
						CONSTRAINT FK_Move_Type FOREIGN KEY (TypeId) REFERENCES Type(TypeId)
					);

					CREATE TABLE Pokemon(
						Id					INTEGER PRIMARY KEY,
						PokemonId			INTEGER,
						Level				INTEGER,
						BaseExperience		INTEGER,
						TotalExperience		INTEGER,
						NextLevelExperience	INTEGER,
						Hp					INTEGER,
						NickName			VARCHAR(30),
						SpriteFront			VARCHAR(100),
						SpriteBack			VARCHAR(100),
						Shiny				BOOLEAN,
						InBattleStats		VARCHAR(200),
						NonVolatileStatus	VARCHAR(200),
						VolatileStatus		VARCHAR(200),
						PossibleMoves		VARCHAR(2000),
						MovePpMapping		VARCHAR(200),
						Move1Id				INTEGER,
						Move2Id				INTEGER,
						Move3Id				INTEGER,
						Move4Id				INTEGER,
						StatsHpId			INTEGER,
						StatsAttackId		INTEGER,
						StatsDefenseId		INTEGER,
						StatsSpAttackId		INTEGER,
						StatsSpDefenseId	INTEGER,
						StatsSpeedId		INTEGER,
						SpecieId			INTEGER,
						CONSTRAINT FK_Pokemon_StatsHp FOREIGN KEY (StatsHpId) REFERENCES Stat(Id),
						CONSTRAINT FK_Pokemon_StatsAttack FOREIGN KEY (StatsAttackId) REFERENCES Stat(Id),
						CONSTRAINT FK_Pokemon_StatsDefense FOREIGN KEY (StatsDefenseId) REFERENCES Stat(Id),
						CONSTRAINT FK_Pokemon_StatsSpAttack FOREIGN KEY (StatsSpAttackId) REFERENCES Stat(Id),
						CONSTRAINT FK_Pokemon_StatsSpDefense FOREIGN KEY (StatsSpDefenseId) REFERENCES Stat(Id),
						CONSTRAINT FK_Pokemon_StatsSpeed FOREIGN KEY (StatsSpeedId) REFERENCES Stat(Id),
						CONSTRAINT FK_Pokemon_Specie FOREIGN KEY (SpecieId) REFERENCES Specie(SpecieId)
					);

					CREATE TABLE PokemonType(
						PokemonId			INTEGER,
						TypeId				INTEGER,
						PRIMARY KEY (PokemonId, TypeId),
						CONSTRAINT FK_PokemonType_Pokemon FOREIGN KEY (PokemonId) REFERENCES Pokemon(Id),
						CONSTRAINT FK_PokemonType_Type FOREIGN KEY (TypeId) REFERENCES Type(TypeId)
					);

					CREATE TABLE Trainer(
						TrainerId			INTEGER PRIMARY KEY,
						Name				VARCHAR(30),
						Password			VARCHAR(100),
						RegenPokemon		BIGINT,
						CarryPokemon		VARCHAR(200),
						PokemonList			VARCHAR(2000)
					);
					"
				);

			}
		}
	}
}
