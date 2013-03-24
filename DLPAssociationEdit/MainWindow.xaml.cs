using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DRDLPRegistry;
using System.Windows.Controls;

namespace DRDLPAssociationEdit
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string TEST_ASSOCIATION_PROGRAM = @"C:\Users\Admin\DRDLPSystem\DRDLPFileTransformation\bin\Debug\DRDLPFileTransformation.exe";
		private const string STATUS_LABEL_OPEN_WITH_MENU_TEXT = "Open with menu is ";
		private const string STATUS_LABEL_OPEN_WITH_MENU_ENAMBLED_TEXT = "enabled.";
		private const string STATUS_LABEL_OPEN_WITH_MENU_DISABLED_TEXT = "disabled.";

		protected struct ViewModel
		{
			public string Extingtion { get; set; }
			public bool Selected { get; set; }

			public ViewModel(string extingtion, bool selected)
				: this()
			{
				Extingtion = extingtion;
				Selected = selected;
			}

			public ViewModel(string extingtion)
				: this()
			{
				Extingtion = extingtion;
				Selected = false;
			}
		}

		private List<ViewModel> _extRegistryKeys;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			var searchResult = new ObservableCollection<ViewModel>(string.IsNullOrEmpty(SearchTextTextBox.Text)
								   ? _extRegistryKeys
								   : _extRegistryKeys.Where(el => el.Extingtion.ToLower().Contains(SearchTextTextBox.Text.Trim().ToLower())));

			SearchResultListView.ItemsSource = searchResult;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LoadAllExtingtions();

			var openWithMenuStatuts = RegistryWork.OpenWithMenuEnabled;

			HideOpenWithmenuCheckBox.IsChecked = openWithMenuStatuts;
			StatusLabel.Content = STATUS_LABEL_OPEN_WITH_MENU_TEXT + (openWithMenuStatuts ? STATUS_LABEL_OPEN_WITH_MENU_DISABLED_TEXT : STATUS_LABEL_OPEN_WITH_MENU_ENAMBLED_TEXT);
		}

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("This configuration give the ability to select needed file extinctions to protect.", "About document related DLP system file association configuration", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void HideOpenWithmenuCheckBox_Click(object sender, RoutedEventArgs e)
		{
			var openWithMenuStatuts = !(HideOpenWithmenuCheckBox.IsChecked.HasValue && HideOpenWithmenuCheckBox.IsChecked.Value);

			RegistryWork.OpenWithMenuEnabled = openWithMenuStatuts;
			StatusLabel.Content = STATUS_LABEL_OPEN_WITH_MENU_TEXT + (!openWithMenuStatuts ? STATUS_LABEL_OPEN_WITH_MENU_DISABLED_TEXT : STATUS_LABEL_OPEN_WITH_MENU_ENAMBLED_TEXT);
		}

		private void RestoreDefault_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var assoxiationsToRestore = _extRegistryKeys.Where(el => el.Selected).ToList();

			if (!assoxiationsToRestore.Any()) 
				return;

			RegistryWork.RestoreOriginalFileAssociation(assoxiationsToRestore.Select(el => el.Extingtion));

			LoadAllExtingtions();
			SearchResultListView.ItemsSource = null;
		}

		private void SelectedCheckBox_Click(object sender, RoutedEventArgs e)
		{
			var selectedItem = sender as CheckBox;

			if ((selectedItem == null) || !(selectedItem.DataContext is ViewModel))
				return;

			var selectedItemViewModel = (ViewModel)selectedItem.DataContext;

			if (selectedItemViewModel.Selected)
				RegistryWork.ChageFileAssociation(selectedItemViewModel.Extingtion, TEST_ASSOCIATION_PROGRAM);
			else
				RegistryWork.RestoreOriginalFileAssociation(selectedItemViewModel.Extingtion);

			_extRegistryKeys[_extRegistryKeys.IndexOf(_extRegistryKeys.First(el => el.Extingtion == selectedItemViewModel.Extingtion))] = new ViewModel(selectedItemViewModel.Extingtion, selectedItemViewModel.Selected);
		}

		private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				SearchButton_Click(sender, e);
		}

		private void LoadAllExtingtions()
		{
			var selectedKeys = RegistryWork.GetAllFileExtingtion();
			if (selectedKeys != null)
				_extRegistryKeys = selectedKeys.Select(el => new ViewModel(el.Key, el.Value)).OrderByDescending(el => el.Selected).ThenBy(el => el.Extingtion).ToList();
			else
			{
				MessageBox.Show("Not found registry keys for file association, program terminated.", "DLPAssociationEdit",
								MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Hide();
		} 

	}
}
