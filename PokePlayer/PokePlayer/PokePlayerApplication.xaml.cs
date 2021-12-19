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
using System.Windows.Shapes;
using Caliburn.Micro;
using PokePlayer.Pages;
using PokePlayer_Library.Models;
using PokePlayer_Library.Models.Pokemon;

namespace PokePlayer {
	/// <summary>
	/// Interaction logic for PokePlayerApplication.xaml
	/// This is the window shown at the startup of the application
	/// </summary>
	public partial class PokePlayerApplication : Window {
		public Trainer Trainer { get; set; }

		public static PokePlayerApplication MainApplication { get; set; }
		public PokePlayerApplication() {
			InitializeComponent();

			// The PokePlayerApplication has 3 frames
			// - fullscreen: Takes the whole screen, for a battle for example
			// - navbar: Navbar is shown here
			// - maincontent: Conent is put here if the navbar must be shown too
			fullScreen.Content = new PreLogin();
			MainApplication = this;
		}
	}
}
