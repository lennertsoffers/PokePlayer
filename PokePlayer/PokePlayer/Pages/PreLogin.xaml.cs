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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PokePlayer.Pages {
	/// <summary>
	/// Interaction logic for PreLogin.xaml
	/// </summary>
	public partial class PreLogin : Page {
		public PreLogin() {
			InitializeComponent();
		}

		public void Login(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.fullScreen.Content = new Login();
		}

		public void NewPlayer(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.fullScreen.Content = new NewPlayer();
		}

		public void Quit(object sender, RoutedEventArgs e) {
			System.Windows.Application.Current.Shutdown();
		}
	}
}
