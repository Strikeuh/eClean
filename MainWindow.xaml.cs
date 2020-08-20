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
        private string v;
        private string actus;
        private int nbTotal;
        public string compteur;
        public int resultat;
        public int resultat2;
        public string compteur2;
        public long totalsize;
        public string mode;

        public MainWindow()
        {
            // Création de la page. 
            // Lors ce que l'on ouvre la page = 
            InitializeComponent();
            // Attribution du repertoire des dossiers à effacer :
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            try
            {
                CheckActus();
            } catch(Exception ex)
            {
                mode = "hl";
            }
            date.Content = File.ReadAllText("date.txt");
            if (date.Content == "")
                date.Content = "Jamais";
            if (mode == "hl")
            {
                majHL.Visibility = Visibility.Visible;
                majlabelHL.Visibility = Visibility.Visible;
                btnMaj.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Regarde sur le serveur Web la version actuelle et regarde la version du logiciel.
        /// </summary>
        public void CheckVersion()
        {
            string url = "https://ecleantxt.000webhostapp.com/version.txt";
            using (WebClient client = new WebClient())
            {
                string v = client.DownloadString(url).ToString();
                if (v != version)
                {
                    btnMaj.Content = "\n\nMETTRE A JOUR";
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
                    btnMaj.Content = "\n\nMETTRE A JOUR";
                }
            }
        }

        /// <summary>
        /// Regarde sur le serveur web si il y a des actualités à afficher.
        /// </summary>
        public void CheckActus()
        {
            string url = "https://ecleantxt.000webhostapp.com/actualite.txt";
            using (WebClient client = new WebClient())
            {
                string actus = client.DownloadString(url);
                if (actus != string.Empty)
                {
                    actu.Content = actus;
                    actu.Visibility = Visibility.Visible;
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
            date.Content = DateTime.Now;

        }


        private void Button_MAJ_Click_1(object sender, RoutedEventArgs e)
        {
            string version = "V.1.0.0";
            CheckVersion();
        }

        private void Button_histo_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Nombre total de nettoyage : " + File.ReadAllText("nbTotal.txt"), "Statistique", MessageBoxButton.OK, MessageBoxImage.Information);
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
            SaveDate();
            moTotal();
            totalsize = 0;
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
            CheckTotal();
        }
        public void SaveDate()
        {
            string date = DateTime.Today.ToString();
            File.WriteAllText("Date.txt", date);
        }

        private void CheckTotal()
        {
            // Recupère le total en string
            compteur = File.ReadAllText("nbTotal.txt");
            // Convertir string en int :
            int resultat = Int32.Parse(compteur);
            // Ajoute 1 :
            resultat += 1;
            // Convertir int en string :
            compteur = resultat.ToString();
            // Ré écris la nouvelle valeur : 
            File.WriteAllText("nbTotal.txt", compteur);
        }
        private void moTotal()
        {
            // Recupère le total en string
            compteur2 = File.ReadAllText("moTotal.txt");
            // Convertir string en int :
            long resultat2 = Int32.Parse(compteur2);
            // Ajoute 1 :
            resultat2 += totalsize;
            // Convertir int en string :
            compteur2 = resultat2.ToString();
            // Ré écris la nouvelle valeur : 
            File.WriteAllText("moTotal.txt", compteur2);
        }
    }
}
