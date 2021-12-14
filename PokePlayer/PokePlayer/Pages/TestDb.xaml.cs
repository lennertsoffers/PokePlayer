using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using PokePlayer.DAL;
using PokePlayer_Library.Models;
using PokePlayer_Library.Models.Pokemon;
using Type = PokePlayer_Library.Models.Pokemon.Type;

namespace PokePlayer.Pages {
	/// <summary>
	/// Interaction logic for TestDb.xaml
	/// </summary>
	public partial class TestDb : Page {
		public TestDb() {
			InitializeComponent();
			// Specie specie = new Specie(483);
			// Specie sp = Specie.GetSpecie(483);
			// Debug.WriteLine(sp.IsLegendary);

			// new Stat(3, 20, 54);
			// new Stat(1, 20, 54);
			// new Stat(5, 20, 54);
			// new Stat(2, 20, 54);
			// new Stat(3, 20, 54);

			// Type t = Type.GetType("normal");
			// Debug.WriteLine("");
			// foreach (var type in t.HalfDamageTo) {
			// 	Debug.WriteLine(type);
			// }

			// Move.GetMove("scratch");
			// Move.GetMove("cut");
			// Move.GetMove("gust");
			// Move.GetMove("fly");
			// Move.GetMove("bind");
			// Move.GetMove("slam");
			// Move.GetMove("stomp");
			// Move.GetMove("tackle");
			// Move.GetMove("tackle");
			// Move.GetMove("thrash");
			// Move.GetMove("wrap");
			// Move.GetMove("leer");
			// Move.GetMove("bite");
			// Move.GetMove("growl");
			// Move.GetMove("roar");
			// Move.GetMove("sing");
			// Move.GetMove("supersonic");
			// Move.GetMove("ember");
			// Move.GetMove("acid");
			// Move.GetMove("mist");
			// Move.GetMove("surf");

			// Pokemon pokemon = new Pokemon(345, 35, "TestPokemon");
			// int id = pokemon.Id;
			//
			// Debug.WriteLine("Pokemon " + Pokemon.GetPokemon(id).NickName + " with id " + Pokemon.GetPokemon(id).Id);
			// foreach (var t in Pokemon.GetPokemon(id).TypeList) {
			// 	Debug.WriteLine(t.TypeName);
			// }
			// Debug.WriteLine(Pokemon.GetPokemon(id).Stats["hp"].StatValue);
			// foreach (var move in Pokemon.GetPokemon(id).Moves.Values) {
			// 	Debug.WriteLine(move.MoveName + "\t" + move.Type.TypeName);
			// }

			// if (!Trainer.TrainerExists("Lennert")) {
			// 	Trainer trainer = new Trainer("Lennert", "1", new Pokemon(234, 23, "starter"));
			// } else {
			// 	Debug.WriteLine("Trainer does already exist");
			// }
			//
			// Trainer lennert = Trainer.GetTrainer("Lennert");
			// foreach (var pokemon in lennert.PokemonList) {
			// 	Debug.WriteLine("-- " + pokemon.Id);
			// }
			//
			// lennert.AddPokemon(new Pokemon(234, 42, "new p"));
			//
			// lennert = Trainer.GetTrainer("Lennert");
			// foreach (var pokemon in lennert.PokemonList) {
			// 	Debug.WriteLine("-- " + pokemon.Id);
			// }

			// Pokemon pokemon = new Pokemon(235, 32, "testP");
			// int id = 1;
			//
			// Debug.WriteLine(Pokemon.GetPokemon(id).GetStat("hp").StatValue);
			// Pokemon.GetPokemon(id).AddExperience(50000);
			// Debug.WriteLine(Pokemon.GetPokemon(id).GetStat("hp").StatValue);
			//
			// Pokemon pokemon = new Pokemon(213, 23, "testP");
			// int id = pokemon.Id;
			//
			// Debug.WriteLine(JsonConvert.SerializeObject(pokemon.MovePpMapping));
			// pokemon = Pokemon.GetPokemon(id);
			// Debug.WriteLine(JsonConvert.SerializeObject(pokemon.MovePpMapping));

		}
	}
}
