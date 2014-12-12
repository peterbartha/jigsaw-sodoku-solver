using DesktopApp.Databases;
using DesktopApp.Structure;
using Facebook;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private const string AppId = "977023372311996";
        private const string ExtendedPermissions = "email";
        private string _accessToken;
        private MySqlDB db;

        public Login()
        {
            InitializeComponent();

            db = new MySqlDB();

            exitIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Window_Exit);
            topPanel.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
        }

        private void CheckCredentials()
        {
            bool hasError = false;
            String strUser = usernameTxt.Text;
            String strPass = passwordTxt.Password;

            if (db.OpenConnection())
            {
                using (var com = new MySqlCommand("SELECT * FROM Users WHERE Email=@email", db.Connection))
                {
                    try
                    {
                        com.Parameters.Add(new MySqlParameter("@email", strUser));
                        var reader = com.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string hash = Hash.PasswordHash.CreateHash(strPass, (byte[])reader["Salt"]);
                                string srvHash = Convert.ToString(reader["Hash"]);
                                if (hash == srvHash)
                                {
                                    //MessageBox.Show("You have been successfully logged in!");
                                    LoggedInUser user = new LoggedInUser(reader.GetInt32("Id"), reader["Username"].ToString(), usernameTxt.Text);
                                    var mainWindow = new MainWindow(user);
                                    mainWindow.Show();
                                    this.Hide();
                                }
                                else hasError = true;
                            }
                        }
                        else hasError = true;

                        if (hasError)
                        {
                            MessageBox.Show("Error occured. Please try again later!");
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Error happend: Could not login. (" + exception.Message + ")");
                    }
                    finally
                    {
                        db.CloseConnection();
                    }
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            CheckCredentials();
        }

        private void facebookLogin_Click(object sender, RoutedEventArgs e)
        {
            var fbLoginDialog = new FacebookLoginDialog(AppId, ExtendedPermissions);
            fbLoginDialog.ShowDialog();

            DisplayAppropriateMessage(fbLoginDialog.FacebookOAuthResult);
        }

        private void DisplayAppropriateMessage(FacebookOAuthResult facebookOAuthResult)
        {
            if (facebookOAuthResult != null)
            {
                if (facebookOAuthResult.IsSuccess)
                {
                    _accessToken = facebookOAuthResult.AccessToken;
                    var fb = new FacebookClient(facebookOAuthResult.AccessToken);

                    dynamic result = fb.Get("/me");
                    string email = Convert.ToString(result.email);

                    if (db.OpenConnection())
                    {
                        using (var com = new MySqlCommand("SELECT * FROM Users WHERE Email=@email", db.Connection))
                        {
                            try
                            {
                                com.Parameters.Add(new MySqlParameter("@email", email));
                                var reader = com.ExecuteReader();

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        var user = new LoggedInUser(reader.GetInt32("Id"), reader["Username"].ToString(), email);
                                        var mainWindow = new MainWindow(user); // user
                                        mainWindow.Show();
                                        this.Hide();
                                    }
                                }

                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show("Error happend: Could not login. (" + exception.Message + ")");
                            }
                            finally
                            {
                                db.CloseConnection();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(facebookOAuthResult.ErrorDescription);
                }
            }
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            var regForm = new Register(this);
            regForm.Show();
            this.Hide();
        }

        public void setUserField(string username)
        {
            usernameTxt.Text = username;
        }

        private void Window_Exit(object sender, EventArgs e)
        {
            db.CloseConnection();
            Application.Current.Shutdown();
        }

        ~Login()
        {
            db.CloseConnection();
        }
    }
}
