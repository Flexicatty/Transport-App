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
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace NEA_A_Lvl_Project
{
    public class LoginWindowModel
    {
        private static Aes aes = Aes.Create(); //creates keys
        public static bool containsincorrectchar { get; set; }
        private static string currenttoken;
        public LoginWindowModel()
        {
            aes.GenerateIV();
            aes.GenerateKey();
        }
        public static void Loginenter(string logintext, string password) // If button or enter is clicked run login check subroutine
        {
            if (containsincorrectchar == true) //Cannot enter comma or slash due to how user login data is stored
            {
                MessageBox.Show("Cannot use , or /");
            }
            else
            {
                string logindata = logintext + ',' + password; //CSV login data of username and password
                Logincheck(logindata, logintext, aes); //take the combined string and gets ready to send to server
            }
        }
        public static void Logincheck(string logindata, string logintext, Aes aes) //checks the login info with the server
        {
            string encrypteddata = encryptdata(logindata); //encrypt the username and password data using symmetrical key
            byte[][] publickey = Server.Getpublickey(); //gets public key from server *placeholder*
            byte[][] encryptedsymkey = encryptkey(publickey); //encrypt the symmetrical encryption key using the asymmetrical public key
            Server.Checklogin(encryptedsymkey, encrypteddata); //sending encrypted symmetrical key and encrypted pass/username
            string data = Server.Returntoclient();
            currenttoken = Server.GetCurrentToken();
            data = decrypt(data);
            MessageBox.Show(data);
            if (data != "fail")
            {
                Settings.loggedin = true;
                stats(data);
            }
            else
            {
                Settings.loggedin = false;
            }
        }

        public static string CreateLogin(string logindata)
        {
            string encrypteddata = encryptdata(logindata); //encrypt the username and password data using symmetrical key
            byte[][] publickey = Server.Getpublickey(); //gets public key from server *placeholder*
            byte[][] encryptedsymkey = encryptkey(publickey); //encrypt the symmetrical encryption key using the asymmetrical public key
            Server.Createlogin(encryptedsymkey, encrypteddata);
            string data = Server.Returntoclient();
            return decrypt(data);
        }

        public static string encryptdata(string filedata) //encrypts the login data using Symmetrical Encryption
        {
            byte[] encrypteddataarr; //creates a local array to store the encrpyt clientside data

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV); //uses symmetrical key and Iv to create the encryptor

            using (MemoryStream memoryStream = new MemoryStream()) //opens a memory stream
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) //uses the memory stream to be able to write the encrypted data to
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) //streamwriter uses the cryptostream stream to get ready to encrypt data onto the memory stream
                    {
                        streamWriter.Write(filedata); //data is written to cryptostream which encrypts and writes to memory stream
                    }

                    encrypteddataarr = memoryStream.ToArray(); //data array is updated with the new encrypted data
                }
            }
            return Convert.ToBase64String(encrypteddataarr); //returns array contaning encrypted clients login data
        }
        public static byte[][] encryptkey(byte[][] publickey) //encryptes the Symmetrical Encryption Key and IV
        {
            byte[] modulus = publickey[0];
            byte[] exponent = publickey[1]; //initialises the byte arrays to the public key information (modulus and exponent) which we got from the server

            byte[] encryptedSymmetricKey;
            byte[] encryptedSymmetricIV; //creates local byte variable to store the encrypted symmetrical encryption key and IV

            RSA rsa = RSA.Create(); //creates a new instance of the RSA class. 

            RSAParameters rsaKeyInfo = new RSAParameters(); //Create a new instance of the RSAParameters structure.

            rsaKeyInfo.Modulus = modulus;
            rsaKeyInfo.Exponent = exponent; //Set rsaKeyInfo values to the public key values

            rsa.ImportParameters(rsaKeyInfo); //Imports key parameters into rsa. 

            encryptedSymmetricKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
            encryptedSymmetricIV = rsa.Encrypt(aes.IV, RSAEncryptionPadding.Pkcs1); //Encrypt the clients symmetric encryption key and IV.  
            byte[][] encryptedkey = { encryptedSymmetricIV, encryptedSymmetricKey }; //Combines the encrypted symmetric key and IV into one staggerd byte array
            return encryptedkey; //returns the combined encrypted symmetric key and IV
        }
        public static string decrypt(string filedata) //decrypts clients encrypted data using the clients decrypted Symmetrical encryption Key and IV
        {
            byte[] buffer = Convert.FromBase64String(filedata); //creates the buffer byte array for the memory stream ;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV); //creates a decryptor with the Symmetrical Key and IV

            using (MemoryStream memoryStream = new MemoryStream(buffer)) //opens a new memory stream with a buffer of the filedata
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) //uses the memory stream to be able to read the encrypted data from
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream)) //reads the encrypted client data from cryptostream which has decrypted the client data from the memory stream
                    {
                        filedata = streamReader.ReadToEnd(); //replace all the encrypted client data with the decrypted client data
                    }
                }
            }
            return filedata; //return the decrypted client data
        }
        public static void connectServer()
        {
            MessageBox.Show("Connected to server");
        }
        public static void disconnectServer(TcpClient client)
        {
            client.Close();
        }

        public static void stats(string data) //apply values to the stats *placeholder* 
        {
            Stats.SetData(JsonConvert.DeserializeObject<Statsdata>(data)); //deserializes data and stores it in class Statsdata                   
        }
    }
}
