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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;

namespace PasswordManagerMx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Account> pwList;
        Account tempAccount; //The account we will get from the AccountWindow
        int line2Edit;
        string pwFileName = "EncryptedPasswordList.txt";
        Dictionary<string, SolidColorBrush> BGcolors = new Dictionary<string, System.Windows.Media.SolidColorBrush>();
        SolidColorBrush GRAY = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));
        SolidColorBrush BLUE = new SolidColorBrush(Color.FromArgb(255, 98, 161, 251));
        SolidColorBrush GREEN = new SolidColorBrush(Color.FromArgb(255, 31, 203, 31));
        SolidColorBrush RED = new SolidColorBrush(Color.FromArgb(255, 251, 98, 98));
        SolidColorBrush YELLOW = new SolidColorBrush(Color.FromArgb(255, 238, 247, 50));
        SolidColorBrush ORANGE = new SolidColorBrush(Color.FromArgb(255, 255, 189, 47));
        SolidColorBrush PURPLE = new SolidColorBrush(Color.FromArgb(255, 224, 25, 197));

        public MainWindow()
        {
            InitializeComponent();
            BGcolors.Add("gray", GRAY);
            BGcolors.Add("blue", BLUE);
            BGcolors.Add("green", GREEN);
            BGcolors.Add("red", RED);
            BGcolors.Add("yellow", YELLOW);
            BGcolors.Add("orange", ORANGE);
            BGcolors.Add("purple", PURPLE);
            loagGUIColor();
            loadPasswordInfo();
            //test();
        }

        public void test() {

            string plainTextpwFile = "TestPasswordFile.txt";
            string encryptedpwFile = "TestPasswordFile_Enctypted.txt";

            //Converting a plain text password file to an encrypted version
            createEncryptedPWFile(plainTextpwFile, encryptedpwFile);

            //Parsing an encrypted password file into a list of Accounts
            List<Account> test = parseEncryptedPWFile(encryptedpwFile);

            //Creating a new Account
            string name = "PornAccoutn";
            string email = "googleemail@gmail.com";
            string username = "Bigdick69";
            string pw = "pornPW";
            List<string> etcInfo = new List<string>();
            etcInfo.Add("porn etcInfo item #1");
            Account pornAccount = new Account(name, email, username, pw, etcInfo);

            //Adding a new account to the encryptedpwFile
            addEncryptedAccount(pornAccount, encryptedpwFile);
            List<Account> test2 = parseEncryptedPWFile(encryptedpwFile);

            //Generating a random password string from using a string as a seed
            string x = "hello";
            string randomPW = generateRandomPW(x, 13);

            //Editing the information of an account in the encrypted pw file
            //In this case editing the information for the account on index 2
            int tempIndex = 2;
            Account tempAcc = test2[2];
            List<string> tempetcInfo = new List<string>() { "Vivienne", "mothers maiden name", "first Car" };
            string tempPW = "MS_PW_#2";
            tempAcc.EtcInfo = tempetcInfo;
            tempAcc.Password = tempPW;
            encryptedPwFileEdit(encryptedpwFile, tempIndex, tempAcc.ToString());
            List<Account> test3 = parseEncryptedPWFile(encryptedpwFile);

            int a = 5;

        }

        //Computes (a^b)mod(m)
        public int modExp(int a, int b, int m)
        {
            if (m == 1)
            {
                return 0;
            }
            else
            {
                int c = 1;
                for (int n = 1; n <= b; n++)
                {
                    c = (a * c) % m;
                }
                return c;
            }
        }

        //Takes a password text file of a given formart
        //AccountName, Email, Username, Password, etcInfo
        //and encrypts it line by line and writes the result to a new file
        public void createEncryptedPWFile(string inputFile, string outputFile) {

            //Encrypt the formatted pw file
            List<string> encryptedLines = new List<string>();
            using (StreamReader sr = new StreamReader(inputFile)) {
                int lineIndex = 0;
                while (!sr.EndOfStream) {
                    string line2Encrypt = sr.ReadLine();
                    encryptedLines.Add(EnigmaLineEncrypt(line2Encrypt, lineIndex));
                    lineIndex++;
                }
                sr.Close();
            }

            using (StreamWriter sw = new StreamWriter(outputFile)) {
                for (int n = 0; n < encryptedLines.Count; n++) {
                    sw.WriteLine(encryptedLines[n]);
                }
                sw.Close();
            }

        }

        //Create an encrypted password file from the internal list of accounts pwList
        public void createEncryptedPWFileInternal(string outputFile) {
            string encryptedLine2Write = "";
            using (StreamWriter sw = new StreamWriter(outputFile))
            {
                for (int n = 0; n < pwList.Count; n++)
                {
                    encryptedLine2Write = EnigmaLineEncrypt(pwList[n].ToString(), n);
                    sw.WriteLine(encryptedLine2Write);
                }
                sw.Close();
            }
        }

        //Reads an encrypted pw file and populates a list of accounts for the application to use
        public List<Account> parseEncryptedPWFile(string inputFile) {
            List<Account> retval = new List<Account>();
            List<string> decryptedLines = new List<string>();

            int lineIndex = 0;
            using (StreamReader sr = new StreamReader(inputFile)) {
                while (!sr.EndOfStream) {
                    string line2Decrypt = sr.ReadLine();
                    decryptedLines.Add(EnigmaLineEncrypt(line2Decrypt, lineIndex));
                    lineIndex++;
                }
                sr.Close();
            }
            for (int n = 0; n < decryptedLines.Count; n++) {
                retval.Add(parsePWFileLine(decryptedLines[n]));
                Debug.WriteLine(n);
            }
            return retval;
        }

        //Parse a line from a decrypted pw file to an instance of the Account class
        public Account parsePWFileLine(string pwFileLine) {
            string[] pwInfo = pwFileLine.Split(',');
            string name = pwInfo[0];
            string email = pwInfo[1];
            string username = pwInfo[2];
            string pw = pwInfo[3];
            //If we have etc info for the account
            if (pwInfo.Length > 4)
            {
                List<string> etcInfo = new List<string>();
                for (int n = 4; n < pwInfo.Length; n++)
                {
                    etcInfo.Add(pwInfo[n]);
                }
                return new Account(name, email, username, pw, etcInfo);
            }
            else {
                return new Account(name, email, username, pw);
            }
        }

        //Add a new account line to the encrypted pw file
        public void addEncryptedAccount(Account newAccount, string encryptedPwFile) {
            //First figure out which what is the line index for encryption/decryption purposes
            int lineIndex = 0;
            using (StreamReader sr = new StreamReader(encryptedPwFile))
            {
                while (!sr.EndOfStream)
                {
                    string line2Decrypt = sr.ReadLine();
                    lineIndex++;
                }
                sr.Close();
            }
            //Create the plain text pw file line
            string line2Encrypt = newAccount.ToString();

            //Create the encrypted pw file line
            string encryptedLine = EnigmaLineEncrypt(line2Encrypt, lineIndex);

            //Write the new line to the encrypted pw file
            using(StreamWriter sw = new StreamWriter(encryptedPwFile, true)) {
                sw.WriteLine(encryptedLine);
                sw.Close();
            }

            
        }

        //Create a random string to be used as a pw
        public string generateRandomPW(string inputData, int passwordLength) {
            DateTime now = DateTime.Now;
            EnigmaMx encrypter = new EnigmaMx(now.Hour, now.Minute, now.Second);
            string hash = Hash(inputData);
            string randomString = encrypter.Encrypt(hash).Trim();
            if (passwordLength < randomString.Length)
            {
                return randomString.Substring(0, passwordLength);
            }
            else {
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

        //Encrypts a string using the Enigma machine with initial conditons based on the lineIndex
        public string EnigmaLineEncrypt(string s, int lineIndex) {
            /*Encrypt each line of the file using the following method
            * 
            * EnigmaMx y = new EnigmaMx(a, b, c);
            * Where a = (13^(lineIndex + 5))mod(94);
            *       b = (31^(lineIndex + 7))mod(94);
            *       c = (101^(lineIndex + 3))mod(94);
            * string EncryptedLine = y.Encrypt(Line2Encrypt);
            * 
            * This equation can be used to decrypt each line as well
           */
            int a = modExp(13, lineIndex + 5, 94);
            int b = modExp(31, lineIndex + 7, 94);
            int c = modExp(101, lineIndex + 3, 94);
            EnigmaMx Encrypter = new EnigmaMx(a, b, c);
            return Encrypter.Encrypt(s);
        }

        //Edit a single line in the encrypted PW file
        public void encryptedPwFileEdit(string pwFile, int lineNumber, string replacementString) {
            //First read the whole encrypted pw file into memory
            List<string> pwFileLineList = new List<string>();
            using (StreamReader sr = new StreamReader(pwFile)) {
                while (!sr.EndOfStream)
                {
                    pwFileLineList.Add(sr.ReadLine());
                }
                sr.Close();
            }

            //Encrypt the replacement string
            string replacementStringEncrypted = EnigmaLineEncrypt(replacementString, lineNumber);

            //Replace the line of interest with the new content
            pwFileLineList[lineNumber] = replacementStringEncrypted;

            //Now write the modified list back to the password file
            using (StreamWriter sw = new StreamWriter(pwFile)) {
                for (int n = 0; n < pwFileLineList.Count; n++) {
                    sw.WriteLine(pwFileLineList[n]);
                }
                sw.Close();
            }


        }

        //Clear the text from the console
        public void clearConsole() {
            console.Text = null;
        }

        //Return true if EncryptedPasswordList.txt exists and is populated with accounts
        public bool loadPasswordInfo() {
            bool retval = false;
            try
            {
                if (File.Exists(pwFileName))
                {
                    using (StreamReader sr = new StreamReader(pwFileName))
                    {
                        string tempLine = sr.ReadLine();
                        if (tempLine != null)
                        {
                            pwList = parseEncryptedPWFile(pwFileName);
                            retval = true;
                            pwDropdown.Items.Clear();
                            for (int n = 0; n < pwList.Count; n++)
                            {
                                pwDropdown.Items.Add(pwList[n].Name);
                            }
                            console.Text = "Successfully loaded password info from " + pwFileName;
                        }
                        else
                        {
                            pwDropdown.Items.Clear();
                            pwList = new List<Account>();
                            console.Text = pwFileName + " has no passwords loaded.";
                        }
                        return retval;
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(pwFileName))
                    {
                        sw.Write("");
                    }
                    pwList = new List<Account>();
                    console.Text = pwFileName + " not found. Creating a new blank " + pwFileName + " file";
                    return retval;
                }
            }
            catch {
                console.Text = "Error loading " + pwFileName;
                return retval;
            }
        }

        //Set the backgroud color of the main GUI and save the result for later
        public void setBackgroundColor(Dictionary<string, SolidColorBrush> c, string color2Set) {
            MainWindow1.Background = c[color2Set];
            using (StreamWriter sw = new StreamWriter("AppColor.txt")) {
                sw.WriteLine(color2Set);
                sw.Close();
            }
        }

        //Load the color of the main GUI on start up
        public void loagGUIColor() {
            try
            {
                using (StreamReader sr = new StreamReader("AppColor.txt"))
                {
                    string tempColor = sr.ReadLine();
                    sr.Close();
                    setBackgroundColor(BGcolors, tempColor);
                }
            }
            catch {
                //If for some reason we cant open AppColor.txt make a new one and set the default color to gray
                using (StreamWriter sw = new StreamWriter("AppColor.txt")) {
                    sw.Write("gray");
                    sw.Close();
                    setBackgroundColor(BGcolors, "gray");
                }
            }
        }

        private void grayColorChange_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundColor(BGcolors, "gray");
        }

        private void blueColorChange_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundColor(BGcolors, "blue");
        }

        private void greenColorChange_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundColor(BGcolors, "green");
        }

        private void redColorChange_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundColor(BGcolors, "red");
        }

        private void yellowColorChange_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundColor(BGcolors, "yellow");
        }

        private void orangeColorChange_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundColor(BGcolors, "orange");
        }

        private void purpleColorChange_Click(object sender, RoutedEventArgs e)
        {
            setBackgroundColor(BGcolors, "purple");
        }

        //Import a list of account information from a password file. Doing this will
        //overwrite EncryptedPasswordList.txt, so promt the user twice to make super
        //sure they want to do this
        private void importPWFile_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Importing new account information will overwrite the currently saved application data. Are you sure you want to continue?", "Account Info Import", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MessageBoxResult messageBoxResult2 = System.Windows.MessageBox.Show("ARE YOU SUPER SURE???", "Account Info Import", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult2 == MessageBoxResult.Yes)
                {
                    console.Text = "Uploading new account information to PasswordManagerMx...";
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == true) {
                        string accoutnImportFile = openFileDialog.FileName;
                        string fileExt = System.IO.Path.GetExtension(accoutnImportFile);
                        if (fileExt.Equals(".txt"))
                        {
                            //Converting a plain text password file to an encrypted version
                            createEncryptedPWFile(accoutnImportFile, "EncryptedPasswordList.txt");
                            loadPasswordInfo();
                        }
                        else {
                            console.AppendText("\nAccount information must be a .txt file");
                        }
                    }

                }
            }
        }

        //Export the encrypted pw file as plain text
        private void exportPWFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog exportFileDialog = new SaveFileDialog();
                exportFileDialog.Title = "Export password file";
                exportFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                exportFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                string file2Export;
                if (exportFileDialog.ShowDialog() == true)
                {
                    file2Export = exportFileDialog.FileName;
                    using (StreamWriter sw = new StreamWriter(file2Export))
                    {
                        for (int n = 0; n < pwList.Count; n++)
                        {
                            string line2Write = pwList[n].ToString();
                            sw.WriteLine(line2Write);
                        }
                        sw.Close();
                    }
                    console.Text = "Successfully exported password information to ";
                    console.AppendText("\n" + file2Export);
                }
            }
            catch {
                console.Text = "Error exporting password file";
            }

        }

        //Display account information in the console
        private void getAccountInfoBut_Click(object sender, RoutedEventArgs e)
        {
            clearConsole();
            if (pwDropdown.SelectedItem == null || pwDropdown.SelectedItem.ToString().Equals(""))
            {
                console.Text = "No valid account selected";
            }
            else {
                string account2Find = pwDropdown.SelectedItem.ToString();
                for (int n = 0; n < pwList.Count; n++) {
                    if (pwList[n].Name.Equals(account2Find)) {
                        console.Text = "Account: " + pwList[n].Name;
                        console.AppendText("\nEmail: " + pwList[n].Email);
                        console.AppendText("\nUsername: " + pwList[n].Username);
                        console.AppendText("\nPassword: " + pwList[n].Password);
                        List<string> tempEtcInfo = pwList[n].EtcInfo;
                        if (tempEtcInfo != null && tempEtcInfo.Count > 0) {
                            for (int m = 0; m < tempEtcInfo.Count; m++) {
                                console.AppendText("\nEtc Item #" + m + ": " + tempEtcInfo[m]);
                            }
                        }
                    }
                }
            }
        }

        //Bring up an Account window for the user to create a new account
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow aw = new AccountWindow(true, false, false);
            aw.RaiseCloseWindowEvent += new EventHandler<closeWindowEventArg>(accountWindow_RaiseEvent);
            aw.Show();
        }

        //Bring up an Account window for the user to update or delete an account
        private void EditAccountButton_Click(object sender, RoutedEventArgs e)
        {
            //First make sure a valid acount is selected
            string tempAccountName = pwDropdown.Text;
            Account tempAccount = null;
            for (int n = 0; n < pwList.Count; n++)
            {
                if (tempAccountName.Equals(pwList[n].Name))
                {
                    tempAccount = pwList[n];
                    line2Edit = n;
                    break;
                }
            }
            if (tempAccount != null)
            {
                AccountWindow aw = new AccountWindow(false, true, true, tempAccount);
                aw.RaiseCloseWindowEvent += new EventHandler<closeWindowEventArg>(accountWindow_RaiseEvent);
                aw.Show();
            }
            else
            {
                console.Text = "A valid account must be selected before editing";
            }
        }

        //When ever the AccountWindow closes the RaiseCloseWindowEvent will trigger this method
        public void accountWindow_RaiseEvent(object sender, closeWindowEventArg e) {
            if (e.State == 0)//New account to add
            {
                tempAccount = e.Acc2Pass;
                addEncryptedAccount(tempAccount, pwFileName);
                pwList.Add(tempAccount);
                pwDropdown.Items.Add(tempAccount.Name);
            }
            else if (e.State == 1)//Account to delete
            {
                pwList.RemoveAt(line2Edit);
                createEncryptedPWFileInternal(pwFileName);
                loadPasswordInfo();

            }
            else {//Account to udpate
                tempAccount = e.Acc2Pass;
                encryptedPwFileEdit(pwFileName, line2Edit, tempAccount.ToString());
                loadPasswordInfo();
            }
        }

    }

}
