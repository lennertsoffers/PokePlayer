﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Caliburn.Micro;
using PokePlayer.Pages;
using PokePlayer_Library.Models;
using PokePlayer_Library.Models.Pokemon;

namespace PokePlayer {
	/// <summary>
	/// Interaction logic for PokePlayerApplication.xaml
	/// </summary>
	public partial class PokePlayerApplication : Window {
		public Trainer Trainer { get; set; }

		public static PokePlayerApplication MainApplication { get; set; }
		public PokePlayerApplication() {
			InitializeComponent();
			// this.Trainer = new Trainer("lennert", "1", new Pokemon(new Random().Next(1, 700), 30));
			// this.Trainer.AddPokemon(new Pokemon(new Random().Next(1, 700), 30));
			// this.Trainer.AddPokemon(new Pokemon(new Random().Next(1, 700), 30));
			// this.Trainer.AddPokemon(new Pokemon(new Random().Next(1, 700), 30));
			// this.Trainer.AddPokemon(new Pokemon(new Random().Next(1, 700), 30));
			// this.Trainer.AddPokemon(new Pokemon(new Random().Next(1, 700), 30));

			navBar.Content = new Navbar();
			mainContent.Content = new TestDb();
			MainApplication = this;
		}
	}
}
