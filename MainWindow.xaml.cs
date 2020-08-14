using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace eClean
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string version = "1.0.0";
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;

        public MainWindow()
        {
            InitializeComponent();
            // Attribution du repertoire des dossiers à effacer :
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            CheckActus();
            date.Content = File.ReadAllText("date.txt");
        }

        public void CheckVersion()
        {
            string url = "http://localhost/eclean-siteweb/version.txt";
            using (WebClient client = new WebClient())
            {
                string v = client.DownloadString(url);
                if(v != version)
                {
                    MessageBox.Show("Une mise à jour est disponible. Vous allez être rediriger.", "Version du logiciel", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    try
                    {
                        Process.Start(new ProcessStartInfo("https://github.com/Strikeuh/eClean/archive/master.zip")
                        {
                            UseShellExecute = true
                        });
                        // On nomme l'erreur
                    }
                    catch (Exception ex)
                    {
                        // On affiche l'erreur afin de comprendre le problème.
                        Console.WriteLine("Erreur lors du chargement de la page d'aide.");
                        Console.WriteLine("Erreur : " + ex.Message);
                    }

                }
                else
                {
                    MessageBox.Show("Votre logiciel est à jour.", "Version du logiciel.", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
        }

        public void CheckActus()
        {
            string url = "http://localhost/eclean-siteweb/actualite.txt";
            using (WebClient client = new WebClient())
            {
                string actus = client.DownloadString(url);
                if (actus != string.Empty)
                {
                    actu.Content = actus;
                    actu.Visibility = Visibility.Visible;
                    actu2.Visibility = Visibility.Visible;
                    acturectangle.Visibility = Visibility.Visible;
                }
            }

        }

        // Calcul de la taille d'un dossier : 
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }

        // Vider fichiers / dossiers :
        public void ClearTempdData(DirectoryInfo di)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName + " supprimé.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                    continue;
                }
            }
        foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(dir.FullName + " dossier supprimé.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                    continue;
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fenetre.Close();
        }

        private void Button_MAJ_Click_1(object sender, RoutedEventArgs e)
        {
            string version = "V.1.0.0";
            CheckVersion();
        }

        private void Button_histo_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dernier nettoyage le : " + datage.Content, "Historique des nettoyages");
        }

        private void Button_Aide_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ce n'est pas une documentation officiel. Mais une simple aide pour un logiciel se comportant pareil.", "Documentation");
            // Mettre tout les bouts de code à risque dans des "try"
            try
            {
                Process.Start(new ProcessStartInfo("http://www.anthony-cardinale.fr/pc-cleaner/pc-cleaner-manuel.pdf")
                {
                    UseShellExecute = true
                });
                // On nomme l'erreur
            } catch (Exception ex)
            {
                // On affiche l'erreur afin de comprendre le problème.
                Console.WriteLine("Erreur lors du chargement de la page d'aide.");
                Console.WriteLine("Erreur : " + ex.Message);
            }
            }

        private void Button_Analyser_1(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }

        public void AnalyseFolders()
        {
            Console.WriteLine("Début de l'analyse.");
            long totalsize = 0;

            try
            {
                totalsize += DirSize(winTemp) / 1000000;
                totalsize += DirSize(appTemp) / 1000000;
            } catch (Exception ex)
            {
                Console.WriteLine("Impossible d'analyser : " + ex.Message);
            }

            espace.Content = totalsize + "Mo";
            Titre.Content = "Analyse effectué !";
            date.Content = DateTime.Today;
            SaveDate();
        }

        private void Button_Nettoyer_1(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours...");
            btnClean.Content = "NETTOYAGE EN COURS...";

            Clipboard.Clear();

            try
            {
                ClearTempdData(winTemp);
            } catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            try
            {
                ClearTempdData(appTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            btnClean.Content = "\n\n NETTOYAGE TERMINER";
            Titre.Content = "Nettoyage effectué !";
            espace.Content = "0 Mo";
        }
        public void SaveDate()
        {
            string date = DateTime.Today.ToString();
            File.WriteAllText("Date.txt", date);
        }
    }
}
