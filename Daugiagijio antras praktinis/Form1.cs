using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Threading;


namespace Daugiagijio_antras_praktinis
{
    public partial class Form1 : Form
    {
        ManualResetEvent manualResetEvent = new ManualResetEvent(true);
        CancellationTokenSource cancellationTokenSource;
        //private string[] filePaths;
      
        List<string> filePaths;
        List<string> catalogPaths;
        private string password = "dainorasdainorasdainorasdainoras";
        private string IVkey = "1234567890ABCDEF";
        List<string> md5HashValuesCalculatedBefore;
        List<string> md5HashValuesCalculatedAfter;
        private string mainFolderName;
        private string mainFolderPath;
        private string mainMD5ValuesPath;
        private int progressBarCount = 0;
        CancellationToken cancellationToken;
        private List<string> revokeZip;
        List<string> revoke;

        public Form1()
        {
            InitializeComponent();
        }

        private void validationForEncryption()
        {
            string path = @"D:\KOLEGIJOS MEDZIAGA\DAUGIAGIJIS PROGRAMAVIMAS\Daugiagijio antras praktinis\md5 values";
            string newFile = path + "\\MD5 hash values for folder_" + mainFolderName;
            if (File.Exists(newFile))
                throw new Exception("Šie failai jau užšifruoti");
        }

        private void validationForDecryption()
        {
                string path = @"D:\KOLEGIJOS MEDZIAGA\DAUGIAGIJIS PROGRAMAVIMAS\Daugiagijio antras praktinis\md5 values";
                string newFile = path + "\\MD5 hash values for folder_" + mainFolderName;
                if (!File.Exists(newFile))
                    throw new Exception("Šie failai nebuvo užšifruoti");
        }

        private void encrypt_Click(object sender, EventArgs e)
        {
            try
            {
                mainFolderName = null;
                filePaths = new List<string>();
                catalogPaths = new List<string>();
                progressBarCount = 0;
                cancellationTokenSource = new CancellationTokenSource();
                getFilesFromFolderEncrypt();
                if (mainFolderName != null)
                {
                    validationForEncryption();
                    md5HashValuesCalculatedBefore = new List<string>();
                    md5HashValuesCalculatedAfter = new List<string>();
                    ZipDirectories();
                    cancellationToken = cancellationTokenSource.Token;
                    Thread encryptionThread = new Thread(encryptMethods);
                    Thread progressBarThread = new Thread(() => progressBarValue(encryptionThread));
                    progressBarThread.Start();
                    encryptionThread.Start();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void decrypt_Click(object sender, EventArgs e)
        {
            try
            {
                mainFolderName = null ;
                md5HashValuesCalculatedBefore = new List<string>();
                md5HashValuesCalculatedAfter = new List<string>();
                filePaths = new List<string>();
                progressBarCount = 0;
                getFilesFromFolderDecrypt();
                if (mainFolderName != null)
                {
                    validationForDecryption();
                    md5HashValuesCalculatedBefore = new List<string>();
                    md5HashValuesCalculatedAfter = new List<string>();
                    cancellationTokenSource = new CancellationTokenSource();
                    cancellationToken = cancellationTokenSource.Token;
                    Thread decryptionThread = new Thread(decryptMethods);
                    Thread progressBarThread = new Thread(() => progressBarValue(decryptionThread));
                    progressBarThread.Start();
                    decryptionThread.Start();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void decryptMethods()
        {
            SearchTextFile();
            ReadMD5ValuesFromFile();
            DecryptEveryFile();
        }
        
        private void encryptMethods()
        {
            mainMD5ValuesPath = CreateTextFileForSavingMD5Values();
            EncryptEveryFile();
        }

        private void progressBarValue(Thread thread)
        {
            progressBarSettings();
            while (true)
            {
                this.Invoke((Action)delegate
                {
                    progressBar.Value = progressBarCount;
                });
               
                if (!thread.IsAlive)
                {
                    progressBarCount = 0;
                    break;
                }
            }
        }

        private void progressBarSettings()
        {
            this.Invoke((Action)delegate
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = filePaths.Count;
                progressBar.Value = 0;
            });
        }

        private void getFilesFromFolderEncrypt()
        {

            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            if (browserDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string catalog in Directory.GetDirectories(browserDialog.SelectedPath, "*", SearchOption.TopDirectoryOnly))
                    catalogPaths.Add(catalog);
                foreach (string file in Directory.GetFiles(browserDialog.SelectedPath))
                    filePaths.Add(file);
                mainFolderName = Path.GetFileName(browserDialog.SelectedPath);
                mainFolderPath = Path.GetFullPath(browserDialog.SelectedPath);
            }
            else
                return;
        }

        private void getFilesFromFolderDecrypt()
        {

            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            if (browserDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in Directory.GetFiles(browserDialog.SelectedPath))
                    filePaths.Add(file);
                mainFolderName = Path.GetFileName(browserDialog.SelectedPath);
                mainFolderPath = Path.GetFullPath(browserDialog.SelectedPath);
            }
        }

        private void ZipDirectories()
        {
            revokeZip = new List<string>();
            foreach (string catalog in catalogPaths)
            {
                ZipFile.CreateFromDirectory(catalog, catalog + ".rar");
                Directory.Delete(catalog, true);
                filePaths.Add(catalog + ".rar");
                revokeZip.Add(catalog + ".rar");
            }
        }

       
        private void EncryptEveryFile()
        {
            filePaths.Sort();
            revoke = new List<string>();
            foreach (string file in filePaths)
            {
                revoke.Add(file);
                manualResetEvent.WaitOne();
                Encrypt(file);
                if (cancellationToken.IsCancellationRequested)
                {
                    foreach (string revokingFile in revoke)
                    {
                        Decrypt(revokingFile + ".aes");
                    }
                    foreach (string folder in revokeZip)
                    {
                        ZipFile.ExtractToDirectory(folder, folder.Remove(folder.Length - 4));
                        File.Delete(folder);
                    }
                    File.Delete(mainMD5ValuesPath);
                    progressBarCount = 0;
                    this.Invoke((Action)delegate
                    {
                        MessageBox.Show("Veiksmai atšaukti, grąžinta pradinė stadija");
                    });
                    return;
                }
                progressBarCount++;
                Thread.Sleep(250);
            }
            this.Invoke((Action)delegate
            {
                MessageBox.Show("Šifravimas baigtas");
            });
        }

        private void DecryptEveryFile()
        {
            filePaths.Sort();
            revoke = new List<string>();
            revokeZip = new List<string>();
            foreach (string file in filePaths)
            {
                CalculateMD5HashValuesSecondTime(file);
            }

            int i = -1;
            foreach (string file in filePaths)
            {
                revoke.Add(file);
                manualResetEvent.WaitOne();
                i++;
                if (md5HashValuesCalculatedBefore != null && md5HashValuesCalculatedAfter != null)
                {
                    if (md5HashValuesCalculatedBefore.ElementAt(i).Equals(md5HashValuesCalculatedAfter.ElementAt(i)))
                    {
                        Decrypt(file);
                        if (file.Contains(".rar"))
                        {
                            revokeZip.Add(file);
                            ZipFile.ExtractToDirectory(file.Remove(file.Length - 4), file.Remove(file.Length - 8));
                            File.Delete(file.Remove(file.Length - 4));
                        }
                        progressBarCount++;
                        Thread.Sleep(250);
                    }
                    else
                    {
                        Console.WriteLine("Nesutampa hashai");
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        foreach (string folder in revokeZip)
                        {
                            ZipFile.CreateFromDirectory(folder.Remove(folder.Length - 8), folder.Remove(folder.Length - 4));
                            Directory.Delete(folder.Remove(folder.Length - 8), true);
                        }
                        foreach (string revokingFile in revoke)
                        {
                            Encrypt(revokingFile.Remove(revokingFile.Length - 4));
                        }
                        progressBarCount = 0;
                        md5HashValuesCalculatedAfter = null;
                        md5HashValuesCalculatedBefore = null;
                        this.Invoke((Action)delegate
                        {
                            MessageBox.Show("Veiksmai atšaukti, grąžinta pradinė stadija");
                        });
                        return;
                    }
                }
            }
            DeleteMD5HashFile();
            this.Invoke((Action)delegate
            {
                MessageBox.Show("Dešifravimas baigtas");
            });
        }

        private void DeleteMD5HashFile()
        {
            File.Delete(mainMD5ValuesPath);
        }
        private string CreateTextFileForSavingMD5Values ()
        {
            string path = @"D:\KOLEGIJOS MEDZIAGA\DAUGIAGIJIS PROGRAMAVIMAS\Daugiagijio antras praktinis\md5 values";
            string newFile = path + "\\MD5 hash values for folder_" + mainFolderName;
            var create = System.IO.File.Create(newFile);
            create.Dispose();
            return newFile;
        }

        private void CalculateAndSaveMD5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    var hash = md5.ComputeHash(stream);
                    using (StreamWriter writer = File.AppendText(mainMD5ValuesPath))
                    {
                        writer.Write(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
                        writer.Write("\n");
                    }
                }
            }
        }

        private void SearchTextFile()
        {
            string path = @"D:\KOLEGIJOS MEDZIAGA\DAUGIAGIJIS PROGRAMAVIMAS\Daugiagijio antras praktinis\md5 values";
            foreach (string file in Directory.GetFiles(path))
            {
                if (file == path + "\\MD5 hash values for folder_" + mainFolderName)
                {
                    mainMD5ValuesPath = path + "\\MD5 hash values for folder_" + mainFolderName;
                }
            }
        }

        private void ReadMD5ValuesFromFile()
        {
            md5HashValuesCalculatedBefore = File.ReadAllLines(mainMD5ValuesPath).ToList();
        }

        private void CalculateMD5HashValuesSecondTime(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    var hash = md5.ComputeHash(stream);
                    md5HashValuesCalculatedAfter.Add(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
                }
            }
        }


        private void Encrypt(string file)
        {
            try
            {
                byte[] plainContent = File.ReadAllBytes(file);
                using (var AES = new RijndaelManaged())
                {
                    AES.IV = Encoding.UTF8.GetBytes(IVkey);
                    AES.Key = Encoding.UTF8.GetBytes(password);
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;

                    using (var memStream = new MemoryStream())
                    {
                        CryptoStream cryptoStream = new CryptoStream(memStream, AES.CreateEncryptor(), CryptoStreamMode.Write);

                        cryptoStream.Write(plainContent, 0, plainContent.Length);
                        cryptoStream.FlushFinalBlock();
                        File.WriteAllBytes(file, memStream.ToArray());

                        string addedExtension = file + ".aes";
                        File.Move(file, addedExtension);

                        CalculateAndSaveMD5(file + ".aes");

                        Console.WriteLine("Encrypted succesfully" + file);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void Decrypt(string file)
        {
            try
            {
                byte[] plainContent = File.ReadAllBytes(file);
                using (var AES = new RijndaelManaged())
                {
                    AES.IV = Encoding.UTF8.GetBytes(IVkey);
                    AES.Key = Encoding.UTF8.GetBytes(password);
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;

                    using (var memStream = new MemoryStream())
                    {
                        CryptoStream cryptoStream = new CryptoStream(memStream, AES.CreateDecryptor(), CryptoStreamMode.Write);

                        cryptoStream.Write(plainContent, 0, plainContent.Length);
                        cryptoStream.FlushFinalBlock();
                        File.WriteAllBytes(file, memStream.ToArray());

                        string removeExtension = Path.ChangeExtension(file, null);
                        File.Move(file, removeExtension);

                        Console.WriteLine("Decrypted succesfully" + file);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            manualResetEvent.Reset();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            manualResetEvent.Set();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
