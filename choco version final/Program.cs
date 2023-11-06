using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using choco.models;

public class Program
{
    private static string adminFile = "administrateurs.json";
    private static string acheteurFile = "acheteurs.json";
    private static string articleFile = "articles.json";
    private static string articleAcheteFile = "articlesAchetes.json";

    private static List<Admin> admins;
    private static List<Acheteur> acheteurs;
    private static List<Article> articles;
    private static List<ArticleAchete> panier;

    public static void Main(string[] args)
    {
        Console.WriteLine("Lancement de l'application");

        // Chargement de la base de données (DB)
        LoadData();

        Console.Clear(); 

        Console.WriteLine("Choisissez un rôle :");
        Console.WriteLine("1: Utilisateur");
        Console.WriteLine("2: Administrateur");

        int choix = int.Parse(Console.ReadLine());

        switch (choix)
        {
            case 1:
                RunUtilisateur();
                break;
            case 2:
                if (LoginAdmin())
                {
                    RunAdministrateur();
                }
                break;
            default:
                Console.WriteLine("Choix invalide.");
                break;
        }
    }

    private static void InitializeDB()
    {
        // Création de la DB
        admins = new List<Admin>();
        acheteurs = new List<Acheteur>();
        articles = new List<Article>();
        panier = new List<ArticleAchete>();

        // Initialisation des valeurs par défaut (vous pouvez ajouter les valeurs souhaitées ici)

        // Admins par défaut
        admins.Add(new Admin { Id = Guid.NewGuid(), Login = "admin1", Password = "AZERT1%" });
        admins.Add(new Admin { Id = Guid.NewGuid(), Login = "admin2", Password = "BZERT2@" });

        // Acheteurs par défaut
        acheteurs.Add(new Acheteur { Id = Guid.NewGuid(), Nom = "Dupont", Prenom = "Julie", Adresse = "Adresse1", Telephone = 123456789 });
        acheteurs.Add(new Acheteur { Id = Guid.NewGuid(), Nom = "Dupont", Prenom = "Jean", Adresse = "Adresse2", Telephone = 987654321 });

        // Articles par défaut
        articles.Add(new Article { Id = Guid.NewGuid(), Reference = "Choco1", Prix = 10.99f });
        articles.Add(new Article { Id = Guid.NewGuid(), Reference = "Choco2", Prix = 5.99f });

        // Articles achetés par défaut
        panier.Add(new ArticleAchete
        {
            IdAcheteur = acheteurs[0].Id, 
            IdArticle = articles[0].Id,  
            Quantite = 1,
            DateAchat = new DateTime(2023, 11, 6) 
        });

        panier.Add(new ArticleAchete
        {
            IdAcheteur = acheteurs[0].Id, 
            IdArticle = articles[0].Id,  
            Quantite = 1,
            DateAchat = new DateTime(2023, 11, 5) 
        });

        panier.Add(new ArticleAchete
        {
            IdAcheteur = acheteurs[1].Id, 
            IdArticle = articles[1].Id,  
            Quantite = 1,
            DateAchat = new DateTime(2023, 11, 6) 
        });

        panier.Add(new ArticleAchete
        {
            IdAcheteur = acheteurs[1].Id, 
            IdArticle = articles[1].Id,  
            Quantite = 1,
            DateAchat = new DateTime(2023, 11, 5) 
        });


        // Sauvegarder les valeurs initiales dans les fichiers JSON
        SaveData();

        // Logs des actions de création de la DB
        LogAction("Base de données initialisée");
        LogAction("Admins par défaut créés");
        LogAction("Acheteurs par défaut créés");
        LogAction("Articles par défaut créés");
    }


    private static void LoadData()
    {
        LogAction("Chargement des données depuis les fichiers JSON");

        // Charger les données depuis les fichiers JSON
        if (File.Exists(adminFile))
        {
            string adminsJson = File.ReadAllText(adminFile);
            admins = JsonSerializer.Deserialize<List<Admin>>(adminsJson);
        }
        else
        {
            InitializeDB();
        }

        if (File.Exists(acheteurFile))
        {
            string acheteursJson = File.ReadAllText(acheteurFile);
            acheteurs = JsonSerializer.Deserialize<List<Acheteur>>(acheteursJson);
        }
        else
        {
            InitializeDB();
        }

        if (File.Exists(articleFile))
        {
            string articlesJson = File.ReadAllText(articleFile);
            articles = JsonSerializer.Deserialize<List<Article>>(articlesJson);
        }
        else
        {
            InitializeDB();
        }

        if (File.Exists(articleAcheteFile))
        {
            string panierJson = File.ReadAllText(articleAcheteFile);
            panier = JsonSerializer.Deserialize<List<ArticleAchete>>(panierJson);
        }
        else
        {
            InitializeDB();
        }
    }

    private static void SaveData()
    {
        string adminsJson = JsonSerializer.Serialize(admins);
        File.WriteAllText(adminFile, adminsJson);

        string acheteursJson = JsonSerializer.Serialize(acheteurs);
        File.WriteAllText(acheteurFile, acheteursJson);

        string articlesJson = JsonSerializer.Serialize(articles);
        File.WriteAllText(articleFile, articlesJson);

        string panierJson = JsonSerializer.Serialize(panier);
        File.WriteAllText(articleAcheteFile, panierJson);
    }

    private static void LogAction(string action)
    {
        string logFilePath = "logs.txt";

        using (StreamWriter writer = File.AppendText(logFilePath))
        {
            string logText = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {action}";
            writer.WriteLine(logText);
        }

        // Afficher le log dans la console
        Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {action}");
    }

    private static void AjouterArticle()
    {
        Console.Write("Référence de l'article : ");
        string reference = Console.ReadLine();
        Console.Write("Prix de l'article : ");
        float prix = float.Parse(Console.ReadLine());

        articles.Add(new Article { Id = Guid.NewGuid(), Reference = reference, Prix = prix });
        SaveArticlesToFile();
        Console.WriteLine("Article ajouté avec succès.");
        LogAction("Ajout d'un article : " + reference);
    }

    private static void RunUtilisateur()
    {
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8; 

        Console.WriteLine("Bienvenue en tant qu'utilisateur.");

        Acheteur acheteur = SaisirInformationsUtilisateur();
        if (acheteur == null)
        {
            Console.WriteLine("Informations utilisateur invalides. Retour au menu principal.");
            return;
        }

        bool commandeTerminee = false;

        while (!commandeTerminee)
        {
            Console.Clear();
            Console.WriteLine("Menu utilisateur :");
            Console.WriteLine("1: Ajouter un article au panier");
            Console.WriteLine("Appuyez sur 'F' pour terminer la commande");
            Console.WriteLine("Appuyez sur 'P' pour afficher le prix du panier");

            int choix = Console.ReadKey().KeyChar;

            switch (choix)
            {
                case '1':
                    AjouterArticleAuPanier(acheteur);
                    break;
                case '2':
                    AfficherPanier();
                    break;
                case '3':
                    FinaliserCommande(acheteur);
                    commandeTerminee = true;
                    break;
                case '4':
                    return;
                case 'P':
                case 'p':
                    float prixTotal = CalculerPrixTotal(acheteur);
                    Console.OutputEncoding = Encoding.UTF8; 
                    Console.WriteLine($"Prix total du panier : {prixTotal:C}");  
                    Console.WriteLine("Appuyez sur n'importe quelle touche pour continuer.");
                    Console.ReadKey();
                    break;
                case 'F':
                case 'f':
                    FinaliserCommande(acheteur);
                    commandeTerminee = true;
                    break;
                default:
                    Console.WriteLine("Choix invalide.");
                    break;
            }
        }
    }

    private static Acheteur SaisirInformationsUtilisateur()
    {
        Console.Write("Entrez votre nom : ");
        string nom = Console.ReadLine();
        Console.Write("Entrez votre prénom : ");
        string prenom = Console.ReadLine();
        Console.Write("Entrez votre adresse : ");
        string adresse = Console.ReadLine();
        Console.Write("Entrez votre numéro de téléphone : ");
        int telephone;
        if (!int.TryParse(Console.ReadLine(), out telephone))
        {
            Console.WriteLine("Numéro de téléphone invalide.");
            return null;
        }

        // Créez un nouvel acheteur
        Acheteur nouvelAcheteur = new Acheteur { Id = Guid.NewGuid(), Nom = nom, Prenom = prenom, Adresse = adresse, Telephone = telephone };

        // Ajoutez le nouvel acheteur à la liste
        acheteurs.Add(nouvelAcheteur);

        // Sauvegardez les acheteurs mis à jour dans le fichier "acheteurs.json"
        SaveData();

        return nouvelAcheteur;
    }

    private static void AjouterArticleAuPanier(Acheteur acheteur)
    {
        Console.Write("Référence de l'article : ");
        string reference = Console.ReadLine();
        Console.Write("Quantité : ");
        int quantite;
        if (!int.TryParse(Console.ReadLine(), out quantite))
        {
            Console.WriteLine("Quantité invalide.");
            return;
        }

        Article article = articles.Find(a => a.Reference == reference);
        if (article == null)
        {
            Console.WriteLine("Article introuvable.");
            return;
        }

        panier.Add(new ArticleAchete { IdAcheteur = acheteur.Id, IdArticle = article.Id, Quantite = quantite, DateAchat = DateTime.Now });
        Console.WriteLine("Article ajouté au panier.");

        LogAction($"Ajout de {quantite} {article.Reference} au panier par {acheteur.Nom} {acheteur.Prenom}");
    }
    private static void AfficherPanier()
    {
        Console.Clear();
        Console.WriteLine("Panier actuel :");
        foreach (var articleAchete in panier)
        {
            var article = articles.Find(a => a.Id == articleAchete.IdArticle);
            Console.WriteLine($"Référence : {article.Reference}, Quantité : {articleAchete.Quantite}, Prix unitaire : {article.Prix}€");
        }
    }

    private static void FinaliserCommande(Acheteur acheteur)
    {
        if (panier.Count == 0)
        {
            Console.WriteLine("Le panier est vide. Impossible de finaliser la commande.");
            return;
        }

        float prixTotal = CalculerPrixTotal(acheteur);
        string factureText = ConstruireFacture(acheteur, prixTotal);

        // Créer un répertoire avec le nom de l'utilisateur s'il n'existe pas
        string repertoireUtilisateur = acheteur.Nom;
        Directory.CreateDirectory(repertoireUtilisateur);

        // Générer un nom de fichier unique pour la facture
        string nomFichier = $"{acheteur.Nom}-{acheteur.Prenom}-{DateTime.Now:dd-MM-yyyy--HH-mm}.txt";

        // Chemin complet du fichier de facture 
        string cheminFichier = Path.Combine(repertoireUtilisateur, nomFichier);

        // Enregistrement de la facture dans le fichier
        File.WriteAllText(cheminFichier, factureText);

        Console.WriteLine($"Facture enregistrée dans '{cheminFichier}'.");
        panier.Clear();

        LogAction($"Commande finalisée par {acheteur.Nom} {acheteur.Prenom}");
    }

    private static float CalculerPrixTotal(Acheteur acheteur)
    {
        float prixTotal = 0;
        foreach (var articleAchete in panier)
        {
            var article = articles.Find(a => a.Id == articleAchete.IdArticle);
            prixTotal += article.Prix * articleAchete.Quantite;
        }
        return prixTotal;
    }

    private static string ConstruireFacture(Acheteur acheteur, float prixTotal)
    {
        string factureText = $"Facture pour {acheteur.Nom} {acheteur.Prenom}\n\n";
        foreach (var articleAchete in panier)
        {
            var article = articles.Find(a => a.Id == articleAchete.IdArticle);
            factureText += $"Référence : {article.Reference}, Quantité : {articleAchete.Quantite}, Prix unitaire : {article.Prix}€\n";
        }
        factureText += $"\nPrix total : {prixTotal}€";
        return factureText;
    }

    private static void RunAdministrateur()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Menu administrateur :");
            Console.WriteLine("1: Ajouter un article");
            Console.WriteLine("2: Créer un fichier de facture (somme des articles vendus)");
            Console.WriteLine("3: Créer un fichier de facture (somme des articles vendus par acheteurs)");
            Console.WriteLine("4: Créer un fichier de facture (somme des articles vendus par date d'achat)");
            Console.WriteLine("5: Quitter");

            int choix = int.Parse(Console.ReadLine());

            switch (choix)
            {
                case 1:
                    AjouterArticle();
                    break;
                case 2:
                    CreerFactureSommeArticlesVendus();
                    break;
                case 3:
                    CreerFactureSommeArticlesVendusParAcheteurs();
                    break;
                case 4:
                    CreerFactureSommeArticlesVendusParDateAchat();
                    break;
                case 5:
                    return;
                default:
                    Console.WriteLine("Choix invalide.");
                    break;
            }
        }
    }

    private static bool LoginAdmin()
    {
        Console.Write("Login administrateur : ");
        string login = Console.ReadLine();
        Console.Write("Mot de passe administrateur : ");
        string password = Console.ReadLine();

        // Vérifier l'authentification de l'administrateur
        List<Admin> admins = ReadAdminsFromFile();
        Admin admin = admins.Find(a => a.Login == login && a.Password == password);
        if (admin != null)
        {
            Console.WriteLine("Authentification réussie en tant qu'administrateur.");
            return true;
        }
        else
        {
            Console.WriteLine("Authentification échouée. Identifiants incorrects.");
            return false;
        }
    }

    private static void EnregistrerFacture(string factureText, string nomFichier)
    {
        string repertoireFactures = "Factures";

        // Créer le répertoire s'il n'existe pas
        Directory.CreateDirectory(repertoireFactures);

        // Chemin complet du fichier de facture
        string cheminFichier = Path.Combine(repertoireFactures, nomFichier);

        // Enregistrement de la facture dans le fichier
        File.WriteAllText(cheminFichier, factureText);

        Console.WriteLine($"Facture enregistrée dans '{cheminFichier}'.");
    }

    private static void CreerFactureSommeArticlesVendus()
    {
        float sommeArticlesVendus = CalculerSommeArticlesVendus();
        string factureText = $"Somme des chocolats vendus : {sommeArticlesVendus}€";
        EnregistrerFacture(factureText, "FactureSommeArticlesVendus.txt");
        LogAction("Création d'une facture : somme des articles vendus");
    }

    private static float CalculerSommeArticlesVendus()
    {
        float somme = 0;

        foreach (var articleAchete in panier)
        {
            var article = articles.Find(a => a.Id == articleAchete.IdArticle);
            somme += article.Prix * articleAchete.Quantite;
        }

        return somme;
    }

    private static void CreerFactureSommeArticlesVendusParAcheteurs()
    {
        Dictionary<Guid, float> sommeParAcheteur = CalculerSommeArticlesVendusParAcheteurs();
        string factureText = "Somme des chocolats vendus par acheteur :\n";

        foreach (var acheteur in sommeParAcheteur)
        {
            Acheteur acheteurInfo = acheteurs.Find(a => a.Id == acheteur.Key);
            factureText += $"{acheteurInfo.Nom} {acheteurInfo.Prenom} : {acheteur.Value}€\n";
        }

        EnregistrerFacture(factureText, "FactureSommeArticlesVendusParAcheteur.txt");
        LogAction("Création de factures : somme des articles vendus par acheteurs");
    }


    private static Dictionary<Guid, float> CalculerSommeArticlesVendusParAcheteurs()
    {
        Dictionary<Guid, float> sommeParAcheteur = new Dictionary<Guid, float>();

        foreach (var articleAchete in panier)
        {
            var article = articles.Find(a => a.Id == articleAchete.IdArticle);

            if (!sommeParAcheteur.ContainsKey(articleAchete.IdAcheteur))
            {
                sommeParAcheteur[articleAchete.IdAcheteur] = 0;
            }

            sommeParAcheteur[articleAchete.IdAcheteur] += article.Prix * articleAchete.Quantite;
        }

        return sommeParAcheteur;
    }
    private static void CreerFactureSommeArticlesVendusParDateAchat()
    {
        Dictionary<DateTime, float> sommeParDate = CalculerSommeArticlesVendusParDateAchat();
        string factureText = "Somme des chocolats vendus par date d'achat :\n";

        foreach (var date in sommeParDate)
        {
            factureText += $"{date.Key:dd-MM-yyyy} : {date.Value}€\n";
        }

        EnregistrerFacture(factureText, "FactureSommeArticlesVendusParDateAchat.txt");
        LogAction("Création de factures : somme des articles vendus par date d'achat");
    }


    private static Dictionary<DateTime, float> CalculerSommeArticlesVendusParDateAchat()
    {
        Dictionary<DateTime, float> sommeParDate = new Dictionary<DateTime, float>();

        foreach (var articleAchete in panier)
        {
            var article = articles.Find(a => a.Id == articleAchete.IdArticle);

            if (!sommeParDate.ContainsKey(articleAchete.DateAchat.Date))
            {
                sommeParDate[articleAchete.DateAchat.Date] = 0;
            }

            sommeParDate[articleAchete.DateAchat.Date] += article.Prix * articleAchete.Quantite;
        }

        return sommeParDate;
    }

    private static void SaveArticlesToFile()
    {
        string articlesJson = JsonSerializer.Serialize(articles);
        File.WriteAllText("articles.json", articlesJson);
    }

    private static List<Admin> ReadAdminsFromFile()
    {
        string adminsJson = File.ReadAllText("administrateurs.json");
        return JsonSerializer.Deserialize<List<Admin>>(adminsJson);
    }

}

