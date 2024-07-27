using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Server
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private TcpClient client;
        private NetworkStream ns;
        private BinaryReader br;
        private BinaryWriter bw;
        private bool isRunning = true;
        private object locker = new object(); // For thread safety
        private string recev = string.Empty;
        string dirPath;
        string filePath;
        string imgPath;
        string dtp;
        string videoPath;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            server = new TcpListener(IPAddress.Any, 9050);
            server.Start();
            Task.Run(() => AcceptConn());
        }

        private async Task AcceptConn()
        {
            while (isRunning)
            {
                client = await server.AcceptTcpClientAsync();
                _ = Task.Run(() => handleClient(client));
            }
        }

        private async Task handleClient(TcpClient client)
        {
            try
            {
                ns = client.GetStream();
                bw = new BinaryWriter(ns);
                br = new BinaryReader(ns);

                bw.Write("Welcome to my server");
                bw.Flush();

                while (client.Connected && isRunning)
                {
                    try
                    {
                        recev = await Task.Run(() => br.ReadString());

                        if (recev.EndsWith(".txt"))
                        {

                            try
                            {
                                byte[] fileBytes = File.ReadAllBytes(recev);

                                bw.Write("FILESIZE:" + fileBytes.Length); // Send the file size
                                bw.Flush();
                                ns.Write(fileBytes, 0, fileBytes.Length);
                                ns.Flush();

                                Invoke((Action)(() => txtChat.Text += $"{"File Content sent successfully"}{Environment.NewLine}"));
                                //  MessageBox.Show("File sent successfully");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error sending file: {ex.Message}");
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else if (recev.StartsWith("ord"))
                        {

                            try
                            {
                                string data = recev.Substring(3).Trim();
                                byte[] buffer = File.ReadAllBytes(data);

                                // Write the image size with a proper prefix
                                bw.Write($"IMGSIZE:{buffer.Length:D8}"); // Use a consistent length for the size
                                bw.Write(buffer.Length);
                                // Write the image data to the stream
                                bw.Write(buffer);
                                Invoke((Action)(() => txtChat.Text += $"{"Ordinary Image sent successfully"}{Environment.NewLine}"));
                                // MessageBox.Show("Image sent successfully");

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error sending image: {ex.Message}");
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else if (recev.StartsWith("com"))
                        {
                            try
                            {
                                string dataa = recev.Substring(3).Trim();
                                string imgNameOnly = Path.GetFileNameWithoutExtension(dataa);
                                string imagNameOnlyCompressed = imgNameOnly + ".GZip.BCompressed";

                                using (FileStream inputFileStream = new FileStream(dataa, FileMode.Open, FileAccess.Read))
                                using (FileStream compressedFileStream = new FileStream(imagNameOnlyCompressed, FileMode.Create, FileAccess.Write))
                                using (GZipStream gz = new GZipStream(compressedFileStream, CompressionMode.Compress))
                                {
                                    inputFileStream.CopyTo(gz);
                                }

                                byte[] buffer = File.ReadAllBytes(imagNameOnlyCompressed);
                                bw.Write($"IMGSIZE:{buffer.Length:D8}");
                                bw.Write(buffer.Length);
                                bw.Write(buffer);
                                bw.Flush();
                                Invoke((Action)(() => txtChat.Text += $"{"Compressed Image sent successfully"}{Environment.NewLine}"));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error sending compressed image: {ex.Message}");
                            }
                        }









                        else if (recev.StartsWith("DIR:"))
                        {

                            string directoryPath = recev.Substring(4);

                            dtp = directoryPath;
                            try
                            {
                                if (Directory.Exists(dtp))
                                {
                                    var rootDirectoryInfo = new DirectoryInfo(dtp);
                                    SendDirectoryTree(rootDirectoryInfo, "");

                                }
                                Invoke((Action)(() => txtChat.Text += $"{"Directory tree sent successfully"}{Environment.NewLine}"));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error getting directory info: {ex.Message}");
                            }
                        }
                        else if (recev.EndsWith(".mp4"))
                        {

                            try
                            {
                                if (File.Exists(recev))
                                {
                                    byte[] videoBytes = File.ReadAllBytes(recev);
                                    bw.Write($"VIDSIZE:{videoBytes.Length:D8}");
                                    bw.Write(videoBytes.Length); // Send the video size
                                    bw.Flush();
                                    ns.Write(videoBytes, 0, videoBytes.Length);
                                    ns.Flush();
                                    Invoke((Action)(() => txtChat.Text += $"{"Video sent successfully"}{Environment.NewLine}"));
                                    // MessageBox.Show("Video sent successfully");
                                }
                                else
                                {
                                    MessageBox.Show("Video file not found");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error sending video: {ex.Message}");
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else if (recev.StartsWith("FOLDER:"))
                        {
                            string folderPath = recev.Substring(7);
                            try
                            {
                                if (Directory.Exists(folderPath))
                                {
                                    string[] files = Directory.GetFiles(folderPath);
                                    bw.Write("FILECOUNT:" + files.Length); // Send the number of files
                                    bw.Flush();

                                    foreach (string file in files)
                                    {
                                        byte[] fileBytes = File.ReadAllBytes(file);
                                        bw.Write(Path.GetFileName(file)); // Send the file name
                                        bw.Write(fileBytes.Length); // Send the file size
                                        bw.Flush();
                                        ns.Write(fileBytes, 0, fileBytes.Length); // Send the file data
                                        ns.Flush();
                                    }
                                    Invoke((Action)(() => txtChat.Text += $"{"Folder Content sent successfully"}{Environment.NewLine}"));
                                }
                                else
                                {
                                    bw.Write("ERROR:Directory not found");
                                    bw.Flush();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error sending folder files: {ex.Message}");
                                bw.Write("ERROR:" + ex.Message);
                                bw.Flush();
                            }
                        }

                        else if (recev.Contains(':'))
                        {

                            dirPath = recev;
                            try
                            {
                                if (Directory.Exists(recev))
                                {

                                    string[] files = Directory.GetFiles(recev);
                                    string[] dirs = Directory.GetDirectories(recev);
                                    foreach (string dir in dirs) { bw.Write(dir); bw.Flush(); };
                                    foreach (string file in files) { bw.Write(file); bw.Flush(); };
                                    Invoke((Action)(() => txtChat.Text +=   $"{"Directory Content sent successfully"}{Environment.NewLine}"));
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error getting directory info: {ex.Message}");
                            }
                        }
                        


                        else
                        {
                            Invoke((Action)(() => txtChat.Text += $"{recev}{Environment.NewLine}"));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading from client: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client?.Close();
            }
        }

       
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            try
            {
                bw.Write(txtMsg.Text);
                bw.Flush();

                Invoke((Action)(() => txtChat.Text += $"{txtMsg.Text}{Environment.NewLine}"));
                txtMsg.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

      
        private void btnClose_Click(object sender, EventArgs e)
        {
            br.Close();
            bw.Close();
            ns.Close();
            server.Stop();
            client.Close();
            
        }

       
        private  void SendDirectoryTree(DirectoryInfo directoryInfo, string indent)
            {
                try
                {
                    bw.Write($"DIR:{indent}{directoryInfo.Name}/");

                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                         SendDirectoryTree(directory, indent + "    ");
                    }

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        bw.Write($"DIR:{indent}    {file.Name}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending directory info: {ex.Message}");
                }
            }

      
    }
}
