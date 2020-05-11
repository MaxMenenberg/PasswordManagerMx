using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.IO;

namespace PasswordManagerMx
{
    /// <summary>
    /// Interaction logic for AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private Account Acc;
        private int defaulPWLength = 13;
        //This window has an event called RaiseCloseWindowEvent that takes in a closeWindowEventArg object.
        //When the window is closed, RaiseCloseWindowEvent will trigger and store Acc into the closeWindowEventArg.
        //The MainWindow can then attached its own method to the triggering of RaiseCloseWindowEvent. When
        //the MainWindow method tirggers it will have access to the closeWindowEventArg that was modified
        //by the AccountWindow's RaiseCloseWindowEvent
        public event EventHandler<closeWindowEventArg> RaiseCloseWindowEvent;

        public AccountWindow(bool addAccountEn, bool updateAccountEn, bool deleteAccountEn)
        {
            InitializeComponent();
            //Fill the text boxes with empty strings to avoid null reference exceptions
            AccountNameEntry.Text = "";
            emailEntry.Text = "";
            usernameEntry.Text = "";
            passwordEntry.Text = "";
            etc1Entry.Text = "";
            etc2Entry.Text = "";
            etc3Entry.Text = "";
            etc4Entry.Text = "";
            etc5Entry.Text = "";
            AddAccountButton.IsEnabled = addAccountEn;
            updateAccountButton.IsEnabled = updateAccountEn;
            deleteAccountButton.IsEnabled = deleteAccountEn;
        }

        public AccountWindow(bool addAccountEn, bool updateAccountEn, bool deleteAccountEn, Account Accountx) {
            InitializeComponent();
            //Fill the text boxes with empty strings to avoid null reference exceptions
            AccountNameEntry.Text = Accountx.Name;
            emailEntry.Text = Accountx.Email;
            usernameEntry.Text = Accountx.Username;
            passwordEntry.Text = Accountx.Password;
            if (Accountx.EtcInfo == null)
            {
                etc1Entry.Text = "";
                etc2Entry.Text = "";
                etc3Entry.Text = "";
                etc4Entry.Text = "";
                etc5Entry.Text = "";
            }
            else {
                //Pad the etc info list in case there are less than 5 elements
                int etcInfoCount = Accountx.EtcInfo.Count;
                List<string> tempEtcInfo = Accountx.EtcInfo;
                for (int n = 0; n < 5 - etcInfoCount; n++) {
                    tempEtcInfo.Add("");
                }

                etc1Entry.Text = tempEtcInfo[0];
                etc2Entry.Text = tempEtcInfo[1];
                etc3Entry.Text = tempEtcInfo[2];
                etc4Entry.Text = tempEtcInfo[3];
                etc5Entry.Text = tempEtcInfo[4];
            }
            AddAccountButton.IsEnabled = addAccountEn;
            updateAccountButton.IsEnabled = updateAccountEn;
            deleteAccountButton.IsEnabled = deleteAccountEn;
            Acc = Accountx;
        }

        //Populates the password entry field with a randomly generated password
        private void randomPWButton_Click(object sender, RoutedEventArgs e)
        {
            if (pwLengthEntry.Text == null || pwLengthEntry.Text.Equals(""))
            {
                passwordEntry.Text = generateRandomPW(seedEntry.Text, defaulPWLength);
            }
            else {
                try
                {
                    int tempPWLenth = Math.Abs(Convert.ToInt32(pwLengthEntry.Text));
                    passwordEntry.Text = generateRandomPW(seedEntry.Text, tempPWLenth);
                }
                catch {
                    accountEntryConsole.Text = "Password Length must be a positive integer.";
                }
            }
        }

        //Create a random string to be used as a pw
        public string generateRandomPW(string inputData, int passwordLength)
        {
            DateTime now = DateTime.Now;
            EnigmaMx encrypter = new EnigmaMx(now.Hour, now.Minute, now.Second);
            string hash = Hash(inputData);
            string randomString = encrypter.Encrypt(hash).Trim();
            //We dont want the random pw to have commas as it is stord in a .csv file
            randomString = randomString.Replace(',', 'x');
            if (passwordLength < randomString.Length)
            {
                return randomString.Substring(0, passwordLength);
            }
            else
            {
                return randomString;
            }
        }

        //Returns a Sha1 hash of a string
        public string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        //Clear the text in the account entry console
        public void clearAccountConsole() {
            accountEntryConsole.Text = null;
        }

        //Return the new account to the main GUI
        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountNameEntry.Text == null || AccountNameEntry.Text.Equals(""))
            {
                accountEntryConsole.Text = "Must have a non empty account name";
            }
            else {
                string name = AccountNameEntry.Text;
                string email = emailEntry.Text;
                string userName = usernameEntry.Text;
                string password = passwordEntry.Text;

                //Construct the etc Info List
                List<string> tempEtcInfo = new List<string>();
                tempEtcInfo.Add(etc1Entry.Text);
                tempEtcInfo.Add(etc2Entry.Text);
                tempEtcInfo.Add(etc3Entry.Text);
                tempEtcInfo.Add(etc4Entry.Text);
                tempEtcInfo.Add(etc5Entry.Text);

                List<string> tempEtcInfo2 = new List<string>();
                for (int n = 0; n < 5; n++) {
                    if (!tempEtcInfo[n].Equals("")) {
                        tempEtcInfo2.Add(tempEtcInfo[n]);
                    }
                }

                if (tempEtcInfo2.Count == 0) {
                    Acc = new Account(name, email, userName, password);
                }
                else {
                    Acc = new Account(name, email, userName, password, tempEtcInfo2);
                }
                RaiseCloseWindowEvent(this, new closeWindowEventArg(Acc, 0));
                this.Close();
            }
        }

        //Return the updated account to the GUI
        private void updateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountNameEntry.Text == null || AccountNameEntry.Text.Equals(""))
            {
                accountEntryConsole.Text = "Must have a non empty account name";
            }
            else
            {
                string name = AccountNameEntry.Text;
                string email = emailEntry.Text;
                string userName = usernameEntry.Text;
                string password = passwordEntry.Text;

                //Construct the etc Info List
                List<string> tempEtcInfo = new List<string>();
                tempEtcInfo.Add(etc1Entry.Text);
                tempEtcInfo.Add(etc2Entry.Text);
                tempEtcInfo.Add(etc3Entry.Text);
                tempEtcInfo.Add(etc4Entry.Text);
                tempEtcInfo.Add(etc5Entry.Text);

                List<string> tempEtcInfo2 = new List<string>();
                for (int n = 0; n < 5; n++)
                {
                    if (!tempEtcInfo[n].Equals(""))
                    {
                        tempEtcInfo2.Add(tempEtcInfo[n]);
                    }
                }

                if (tempEtcInfo2.Count == 0)
                {
                    Acc = new Account(name, email, userName, password);
                }
                else
                {
                    Acc = new Account(name, email, userName, password, tempEtcInfo2);
                }
                RaiseCloseWindowEvent(this, new closeWindowEventArg(Acc, 2));
                this.Close();
            }
        }

        //Delete the updated account from the system
        private void deleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to delete this account from Password Manager Mx?", "Delete Account?", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                RaiseCloseWindowEvent(this, new closeWindowEventArg(Acc, 1));
                this.Close();
            }
            else if (dialogResult == MessageBoxResult.No)
            {
                //do nothing
            }
            
        }
    }

    //Right before the window closes this class gets created and stored 
    //with the account we want to pass as an event
    public class closeWindowEventArg : EventArgs {
        private Account acc2pass;
        private int state;
        public closeWindowEventArg(Account x, int y) {
            acc2pass = x;
            state = y;
        }
        public Account Acc2Pass {
            get { return acc2pass; }
        }
        //state = 0 --> The account being passed is newly created
        //state = 1 --> The account being passed should be deleted from the system
        //state != 0 || != 1 --> The account being passed is an update version of an existing account
        public int State{
            get { return state; }
        }

    }
}
