using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace NEA_A_Lvl_Project
{
    static class Server
    {
        //
        public static byte[] Key1 { get; set; } = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 }; //will be done outside class
        private static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); //creates public/private key pair
        private static string outputdata;
        private static string currenttoken;
        public static void Init()
        {

        }
        public static byte[][] Getpublickey()
        {
            RSAParameters RSAinfo = rsa.ExportParameters(false); //gets the rsainfo from ONLY the public key
            byte[][] publickey = { RSAinfo.Modulus, RSAinfo.Exponent }; //exports public key as a staggered byte array
            return publickey; //returns public key to client
        }
        public static string GetCurrentToken()
        {
            return currenttoken;
        }
        public static void Checklogin(byte[][] encryptedsymkey, string encrypteddata)
        {
            byte[][] symkey = Decryptkey(encryptedsymkey); //decrypts encrypted client Symmetrical Key and IV
            string filedata = Decrypt(symkey,encrypteddata); //decrypts encrypted client data using the decrypted clients symmetrical key
            using (FileStream filestream = new FileStream("logins.txt", FileMode.Open)) //opens file that stores login data *will encypt in future to make more secure*
            {
                using (StreamReader stream = new StreamReader(filestream)) //open streamreader to read from login data
                {
                    string s = stream.ReadToEnd(); //reads stream to end
                    string[] logins = s.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); //splits logins at the end of each line into a local array
                    bool logfail = true;
                    foreach (string login in logins) //goes through each login username and password
                    {
                        if(login.Substring(0, login.LastIndexOf(',')) == filedata)
                        {
                            string token = login.Split(',')[2].Trim(); //gets user token
                            currenttoken = token; //saves the token on login
                            Encrypt(symkey, GetUserData(token)); //*placeholder* will sync account data in future
                            logfail = false;
                        }
                    }
                    if (logfail == true)
                    {
                        Encrypt(symkey, "fail");
                    }

                }
            }       
        }
        public static void Createlogin(byte[][] encryptedsymkey, string encrypteddata)
        {
            byte[][] symkey = Decryptkey(encryptedsymkey); //decrypts encrypted client Symmetrical Key and IV
            string filedata = Decrypt(symkey, encrypteddata);
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            token = token.Replace('/', '£').Replace('\\', '£'); //if there are any '/' in the token, replace them with unique char because of file reading later
            currenttoken = token;
            using (StreamReader streamread = new StreamReader("logins.txt"))
            {
                string s = streamread.ReadToEnd(); //reads stream to end
                streamread.Close();
                string[] logins = s.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); //splits logins at the end of each line into a local array
                bool logfail = false;
                string username = filedata.Split(new char[] {','})[0];
                string msg = "error";
                foreach (string login in logins) //goes through each login username and password
                {
                    if (login.Split(new char[] { ','})[0] == username)
                    {
                        logfail = true;
                        msg = "exists";
                        break;
                        //throw new NotImplementedException(); //if data client sent to server matches a username and password in login file, send fail message back 
                    }
                }
                if (!logfail)
                {
                    using (StreamWriter _writer = new StreamWriter("logins.txt", append: true))
                    {
                        _writer.Write(filedata + "," + token + Environment.NewLine);
                    }
                    string path = Environment.CurrentDirectory + "\\stats\\" + token;
                    Statsdata statsdata = new Statsdata() { Username = username, name = "", numofjourneys = 0, totalkmtravelled = 0, totalmoneyspent = 0, timespenttravelling = new TimeSpan(0,0,0)}; //default values for Statsdata
                    string json = JsonConvert.SerializeObject(statsdata, Formatting.Indented);
                    Save(path, json);
                }
                msg = "success";            
                Encrypt(symkey, msg);
            }
        }
        private static string Decrypt(byte[][] symkey, string filedata) //decrypts clients encrypted data using the clients decrypted Symmetrical encryption Key and IV
        {
            byte[] buffer = Convert.FromBase64String(filedata); //creates the buffer byte array for the memory stream 

            using (Aes aes = Aes.Create()) //initalises a new Aes Class
            {
                aes.Key = symkey[1]; //sets the Aes Key to the clients Symmetrical key
                aes.IV = symkey[0]; //sets the Aes IV to the clients Symmetrical key
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
            }
            return filedata; //return the decrypted client data
        }
        private static byte[][] Decryptkey(byte[][] key) //decrypts encypted symmetrical key usin Asymmetrical decryption
        {
            key[0] = rsa.Decrypt(key[0], RSAEncryptionPadding.Pkcs1);
            key[1] = rsa.Decrypt(key[1], RSAEncryptionPadding.Pkcs1); //Decrypt the symmetric key and IV using the Private Key from server
            return key; //returns decrypted symmetrical encryption key and IV
        }
        public static void Encrypt(byte[][]symkey, string data) //*placeholder*
        {
            byte[] encrypteddataarr; //creates a local array to store the encrpyt clientside data
            using (Aes aes = Aes.Create()) //initalises a new Aes Class
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(symkey[1], symkey[0]); //uses symmetrical key and Iv to create the encryptor

                using (MemoryStream memoryStream = new MemoryStream()) //opens a memory stream
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) //uses the memory stream to be able to write the encrypted data to
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) //streamwriter uses the cryptostream stream to get ready to encrypt data onto the memory stream
                        {
                            streamWriter.Write(data); //data is written to cryptostream which encrypts and writes to memory stream
                        }

                        encrypteddataarr = memoryStream.ToArray(); //data array is updated with the new encrypted data
                    }
                }
            }
            outputdata = Convert.ToBase64String(encrypteddataarr); //returns array to client contaning encrypted clients login data
        }
        private static string GetUserData(string token)
        {
            try
            {
                using (StreamReader _reader = new StreamReader(Environment.CurrentDirectory + "\\stats\\" + token))
                {
                    string data = _reader.ReadToEnd();
                    return data;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.Source + e.StackTrace);
                throw;
            }
        }
        private static void Save(string path, string data) //Save data to a file
        {
            using (StreamWriter _writer = new StreamWriter(path))
            {
                _writer.Write(data);
            }
        }
        public static void SaveStats(byte[][] encryptedsymkey, string encrypteddata)
        {
            byte[][] symkey = Decryptkey(encryptedsymkey); //decrypts encrypted client Symmetrical Key and IV
            string filedata = Decrypt(symkey, encrypteddata);
            string path = Environment.CurrentDirectory + "\\stats\\" + currenttoken;
            Save(path, filedata);
        }
        public static string Returntoclient() //https to send data through to client
        {
            return outputdata;
        }
        public static void Endcommunication()
        {

        }
    }
}
