using GtaLauncher.confuguration;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace GtaLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game game;

        public MainWindow()
        {
            InitializeComponent();
            game = new Game();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(Configuration.Instanse.GamePath))
                {
                    if (!SelectPath()) return;
                }
                game.Start();

            }
            catch (Exception ex)
            {
                game.Dispose();
            }
        }

        private bool SelectPath()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Configuration.Instanse.GamePath = dialog.FileName + "/";
                return true;
            }
            return false;
        }
    }
}
