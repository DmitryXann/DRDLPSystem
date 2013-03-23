using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DRDLPRegistry;

namespace DRDLPAssociationEdit
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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

		private readonly IEnumerable<ViewModel> _extRegistryKeys;

		public MainWindow()
		{
			InitializeComponent();

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

		private void Search_Button_Click(object sender, RoutedEventArgs e)
		{
			var searchResult = string.IsNullOrEmpty(SearchTextTextBox.Text)
				                   ? _extRegistryKeys
				                   : _extRegistryKeys.Where(el => el.Extingtion.ToLower().Contains(SearchTextTextBox.Text.Trim().ToLower()));

			SearchResultListView.ItemsSource = searchResult;
			SaveChangesButton.IsEnabled = searchResult.Any();
		}

		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			//RegistryWork.RestoreOriginalFileAssociation(".docx");
			RegistryWork.ChageFileAssociation(".docx", @"C:\Windows\SysWow64\cmd.exe");
		}

		private void SaveChanges_Button_Click(object sender, RoutedEventArgs e)
		{
		}

	}
}
