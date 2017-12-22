using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Devart.Data.MySql;

namespace ImpotDomaine
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 
    public struct Reduction
    {
        public double min;
        public double max;
        public double pourcentage;
    }

    public struct NbPart
    {
        public int min;
        public int max;
        public double pourcentage;
        public double nbPart;
    }
    public partial class MainWindow : Window
    {

        String lnom;
        String lprenom;
        double lsalaireBrut;
        int lconjoint;
        int lnombreJour;
        int lenfants;
        int nombre = 0;
        int indexCategory = 6;
        List<String> employes = new List<string>();
        MySqlConnection cn;
        public MainWindow()
        {
           
            InitializeComponent();
            dateJour.Content = "Date d'aujourd'hui :" + DateTime.Now.ToString("dddd dd MMMM yyyy");
            
            

            try
            {
                cn = new MySqlConnection("SERVER=localhost;PORT=3306;DATABASE=impot;UID=root;");
                if (cn.State == System.Data.ConnectionState.Closed)
                {
                    cn.Open();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erreur de connexion");
            }
            employes = listeEmployes();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            List<String> listes = new List<string>() { nom.Text,
            prenom.Text, salaireBrut.Text, joint.Text, nbrJour.Text, enfant.Text};
            if (verifiListe(listes))
            {
                MessageBox.Show("Tous les champs doivent etre remplis");

            }
            else
            {
                indexCategory = 6;
                this.lnom = nom.Text;
                this.lprenom = prenom.Text;
                bool res;
                res = double.TryParse(salaireBrut.Text, out this.lsalaireBrut);
                res &= int.TryParse(joint.Text, out this.lconjoint);
                res &= int.TryParse(nbrJour.Text, out this.lnombreJour);
                res &= int.TryParse(enfant.Text, out this.lenfants);
                int salaireBase = lnombreJour * 50000 / 30;
                if (res == false)
                {
                    MessageBox.Show("verifier les types des donnees saisies");

                }
                else
                {
                    if (this.lnombreJour < 1 || this.lnombreJour > 360)
                    {
                        MessageBox.Show("LE NOMBRE DE JOURS DOIT ETRE COMPRIS ENTRE 1-360 JOURS");
                    }
                    else
                    {

                        if (salaireBase > lsalaireBrut)
                        {
                            MessageBox.Show("le salaire saisi est trop petit");
                        }
                        else
                        {
                            if (this.lconjoint != 0 && this.lconjoint != 1)
                            {
                                MessageBox.Show("les valeurs possibles pour conjoint : 0 ou 1");
                            }
                            else
                            {
                                // nombre ++;
                                 
                                /* if (!employes.Contains(liste))
                                 {
                                     employes.Add(liste);
                                 }*/

                                double fiscal =  this.lsalaireBrut*360 / this.lnombreJour;
                                double abat = Convert.ToInt32((((fiscal * 0.3)) < 900000) ? (fiscal * 0.3) : 900000);
                                fiscalAnnuel.Content = fiscal;
                                abattement.Content = abat;
                                double bfap = fiscal - abat;
                                fiscalAbattement.Content = fiscal - abat;
                                double nbPart = Math.Min((this.lenfants + lconjoint) * 0.5 + 1, 5);
                                nombrePart.Content = Math.Min((this.lenfants + lconjoint) * 0.5 + 1, 5);
                                String redString = String.Empty;
                                String pourString = String.Empty;
                                try
                                {
                                    redString = File.ReadAllText(@"C:\Users\Cheikhouna\source\repos\ImpotDomaine\reduction.txt");
                                    pourString = File.ReadAllText(@"C:\Users\Cheikhouna\source\repos\ImpotDomaine\pourcentageReduction.txt");
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show("le fichier de reduction est introuvable", ex.Message);
                                }
                                // a apporter des modifications
                                double[] redTab = Array.ConvertAll(redString.Split(','), Double.Parse );
                                double[] pourcentage = Array.ConvertAll(pourString.Split(';'), Double.Parse); 
                                double irppSomme = 0.0;

                                Reduction[] tab = new Reduction[6];
                                for (int i = 0; i < 6; i++)
                                {
                                    tab[i].min = redTab[i];
                                    tab[i].max = redTab[i + 1];
                                    tab[i].pourcentage = pourcentage[i];
                                }

                                for (int i = 0; i < 6; i++)
                                {
                                    if (bfap > tab[i].min && bfap <= tab[i].max)
                                    {
                                        indexCategory = i;
                                        break;
                                    }

                                }

                                bool mill = false;
                                if(indexCategory == 6)
                                {
                                    indexCategory = 5;
                                    mill = true;
                                }
                                for (int i = indexCategory; i > 0; i--)
                                {
                                    

                                    if (i == indexCategory && mill == false)
                                    {
                                        Console.WriteLine(irppSomme);
                                        irppSomme += (bfap - tab[i - 1].max) * tab[i].pourcentage;
                                    }
                                    else
                                        irppSomme += (tab[i].max - tab[i - 1].max) * tab[i].pourcentage;
                                    Console.WriteLine(irppSomme);
                                }

                                NbPart[] tabNbPart = new NbPart[9];
                                String redStringMin = String.Empty;
                                String redStringMax = String.Empty;
                                String pourString1 = String.Empty;
                                String nbPartString = String.Empty;
                                int[] redTabmin  = null;
                                int[] redTabMax = null;
                                double[] pourcentage1 = null;
                                double[] nbpart = null;
                                try
                                {
                                    redStringMin = File.ReadAllText(@"C:\Users\Cheikhouna\source\repos\ImpotDomaine\reducMin.txt");
                                    redStringMax = File.ReadAllText(@"C:\Users\Cheikhouna\source\repos\ImpotDomaine\reducMax.txt");
                                    pourString1 = File.ReadAllText(@"C:\Users\Cheikhouna\source\repos\ImpotDomaine\pourcentage.txt");
                                    nbPartString = File.ReadAllText(@"C:\Users\Cheikhouna\source\repos\ImpotDomaine\part.txt");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("le fichier de reduction est introuvable");
                                }
                                try
                                {
                                    redTabmin = Array.ConvertAll(redStringMin.Split(','), int.Parse);
                                    redTabMax = Array.ConvertAll(redStringMax.Split(','), int.Parse);

                                    pourcentage1 = Array.ConvertAll(pourString1.Split(';'), Double.Parse);
                                    nbpart = Array.ConvertAll(nbPartString.Split(';'), Double.Parse);
                                }catch(Exception ex)
                                {
                                    MessageBox.Show("Erreur du format du fichier");
                                }
                                for (int i = 0; i < 9; i++)
                                {
                                    tabNbPart[i].min = redTabmin[i];
                                    tabNbPart[i].max = redTabMax[i];
                                    tabNbPart[i].pourcentage = pourcentage1[i];
                                    tabNbPart[i].nbPart = nbpart[i];
                                }

                                for (int i = 0; i < 9; i++)
                                {
                                    if (nbPart == tabNbPart[i].nbPart)
                                    {
                                        indexCategory = i;
                                        break;
                                    }

                                }

                                double reduction = irppSomme * tabNbPart[indexCategory].pourcentage;
                                if (reduction < tabNbPart[indexCategory].min)
                                    reduction = tabNbPart[indexCategory].min;
                                else if (reduction > tabNbPart[indexCategory].max)
                                    reduction = tabNbPart[indexCategory].max;

                                double immpot = irppSomme - reduction;
                                irpp.Content = irppSomme;
                                reduc.Content = reduction;
                                impot.Content = immpot < 0 ? 0 : immpot;
                                salaireNet.Content = "Salaire Net  : " + (fiscal - immpot);
                                String liste = this.lnom + "\t" + this.lprenom + "\t" + this.lconjoint + "\t" +
                                     this.lnombreJour + "\t" + this.lenfants + "\t" + this.lsalaireBrut;
                                if (!ExisteInBD(this.lnom, this.lprenom, this.lnombreJour, this.lconjoint, this.lenfants,
                                    this.lsalaireBrut))
                                {
                                    try
                                    {
                                        cn.Open();
                                        MySqlCommand cmd = new MySqlCommand("insert into employes values(@nom, @prenom,@joint, @nbrJour, @enfant, @salaireBrut)", cn);
                                        cmd.Parameters.AddWithValue("@nom", nom.Text);
                                        cmd.Parameters.AddWithValue("@prenom", prenom.Text);
                                        cmd.Parameters.AddWithValue("@joint", int.Parse(joint.Text));
                                        cmd.Parameters.AddWithValue("@nbrJour", int.Parse(nbrJour.Text));
                                        cmd.Parameters.AddWithValue("@enfant", int.Parse(enfant.Text));
                                        cmd.Parameters.AddWithValue("@salaireBrut", this.lsalaireBrut);
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                        cn.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Erreur de connexion à la base de données");
                                    }
                                    
                                    this.employes = listeEmployes();
                                }
                                
                            }
                        }
                    }
                }
            }
        }

        public bool verifiListe(List<String> listes)
        {
            foreach (String liste in listes)
            {
                if (String.IsNullOrEmpty(liste))
                {
                    return true;
                }
            }
            return false;
        }

        public List<String> listeEmployes()
        {
            List<String> emps = new List<string>();
            emps.Add("Nom \t Prenom \t conjoint \t Jours \t enfants \t Salaire");
            try
            {
                cn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Employes", cn);
                //cmd.ExecuteNonQuery();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    String chaine = Convert.ToString(reader["nom"] + "\t" + reader["prenom"] + "\t" + reader["conjoint"] + "\t" +
                        reader["nombreJours"] + "\t" + reader["enfant"] + "\t" + reader["salaire"]);
                    emps.Add(chaine);
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion à la base de données");
            }
            return emps;
        }
        private void quitter_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }

        private void lister_Click(object sender, RoutedEventArgs e)
        {
            if (employes == null)
            {
                MessageBox.Show("la liste d'employes est vide");
            }
            else
            {
                String l = "";
                foreach (String em in employes)
                {
                    l += "\n"+em;
                }
                MessageBox.Show(l);
            }
        }

        public bool ExisteInBD(String n, String p, int nb, int c,int e, double s)
        {
            bool resultat = false;
            try
            {
                cn.Open();
                MySqlCommand cmd = new MySqlCommand("select nom from Employes where nom='"+n+"'" +
                    " and prenom='" + p + "'"+
                    " and conjoint="+c +
                    " and nombreJours="+nb+ 
                    " and enfant="+e+
                    " and salaire="+s, cn);
                
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read() )
                {
                    resultat = true;
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion à la base de données");
            }
            
            return resultat;
        }

        private void annuler_Click(object sender, RoutedEventArgs e)
        {
            nom.Text = "";
            prenom.Text = "";
            joint.Text = "";
            salaireBrut.Text = "";
            enfant.Text = "";
            nbrJour.Text = "";
        }
    }


    
}
