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
using PokePlayer_Library.Models;

namespace PokePlayer.Pages {
	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class Login : Page {
		public Login() {
			InitializeComponent();
		}

		public void ValidateLogin(object sender, RoutedEventArgs e) {
			wrongPassword.Visibility = Visibility.Hidden;
			wrongUser.Visibility = Visibility.Hidden;
			if (Trainer.TrainerExists(username.Text)) {
				Trainer trainer = Trainer.GetTrainer(username.Text);
				if (BCrypt.Net.BCrypt.Verify(password.Password, trainer.Password)) {
					PokePlayerApplication.MainApplication.navBar.Content = new Navbar();
					PokePlayerApplication.MainApplication.mainContent.Content = new View_Party(trainer);
					PokePlayerApplication.MainApplication.fullScreen.Content = new ClearContent();
					PokePlayerApplication.MainApplication.Trainer = trainer;
				} else {
					wrongPassword.Visibility = Visibility.Visible;
					password.Password = "";
				}
			} else {
				wrongUser.Visibility = Visibility.Visible;
				username.Text = "";
				password.Password = "";
			}
		}
	}
}
