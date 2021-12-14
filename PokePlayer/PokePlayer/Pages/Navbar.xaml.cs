using System;
using System.Collections.Generic;
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

namespace PokePlayer.Pages {
	/// <summary>
	/// Interaction logic for Navbar.xaml
	/// </summary>
	public partial class Navbar : Page {
		public Navbar() {
			InitializeComponent();
		}

		public void viewParty(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.mainContent.Content =
				new View_Party(PokePlayerApplication.MainApplication.Trainer);
		}

		public void switchPokemon(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.mainContent.Content =
				new Switch_Pokemon(PokePlayerApplication.MainApplication.Trainer);
		}

		public void wildBattle(object sender, RoutedEventArgs e) {
			Battle battle = new Battle(PokePlayerApplication.MainApplication.Trainer);
			PokePlayerApplication.MainApplication.mainContent.Content = battle;
			battle.StartBattle();
		}

		public void Logout(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.fullScreen.Content = new PreLogin();
		}
	}
}
