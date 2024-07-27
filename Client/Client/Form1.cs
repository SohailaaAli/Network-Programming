using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using File = System.IO.File;

namespace Client
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream ns;
        private BinaryReader br;
        private BinaryWriter bw;
        private bool isRunning = true;
        private string recev = string.Empty;
        private const int BufferSize = 1024;
        string data;
        private const string SaveDirectory = "ReceivedFolderContent";
        private const string SaveDirectory2 = "ReceivedFiles";
        private string selectedImageFormat;


        public Form1()
        {
            InitializeComponent();
            ChoiceFormatOfImage choiceFormatOfImage = new ChoiceFormatOfImage();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            client = new TcpClient("127.0.0.1", 9050);
            ns = client.GetStream();
            bw = new BinaryWriter(ns);
            br = new BinaryReader(ns);

            string data = await Task.Run(() => br.ReadString());
            txtChat.Text += $"{data}{Environment.NewLine}";

            // Ensure the directory exists
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            if (!Directory.Exists(SaveDirectory2))
            {
                Directory.CreateDirectory(SaveDirectory2);
            }

            while (true)
            {
                try
                {
                    data = await Task.Run(() => br.ReadString());

                    if (data.StartsWith("FILESIZE:"))
                    {
                        string fileSizeString = data.Substring(9); // Extract the file size
                        if (int.TryParse(fileSizeString, out int fileSize))
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead;
                            int totalBytesRead = 0;
                            string fileNameOnly = Path.GetFileName(txtFilePath.Text);
                            string savePath = Path.Combine(SaveDirectory2, fileNameOnly);
                            using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                            {
                                while (totalBytesRead < fileSize && (bytesRead = ns.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    fileStream.Write(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;
                                }
                            }

                            // MessageBox.Show($"File received successfully.");


                            // Display the contents of the file if needed
                            string fileContents = File.ReadAllText(savePath);
                            // Display the contents in textBox1 or wherever you want
                            txtFileContent.Text = fileContents;



                        }
                    }
                    else if (data.StartsWith("IMGSIZE:"))
                    {
                        int imageSize = int.Parse(data.Substring(8).Trim());
                        int fileLength = br.ReadInt32();
                        byte[] imageData = br.ReadBytes(fileLength);

                        if (selectedImageFormat == "ordinary")
                        {
                            using (var ms = new MemoryStream(imageData))
                            {
                                var image = System.Drawing.Image.FromStream(ms);
                                DisplayImage(image);
                            }

                            string imgNameOnly = Path.GetFileName(txtImgPath.Text);
                            string savePath = Path.Combine(SaveDirectory2, imgNameOnly);
                            File.WriteAllBytes(savePath, imageData);
                            Invoke((Action)(() => txtImgPath.Clear()));
                        }
                        else if (selectedImageFormat == "compressed")
                        {
                            string imgNameOnly = Path.GetFileNameWithoutExtension(txtImgPath.Text);
                            string compressedFileName = imgNameOnly + ".GZip.BCompressed";
                            string savePath = Path.Combine(SaveDirectory2, compressedFileName);
                            File.WriteAllBytes(savePath, imageData);

                            try
                            {
                                using (FileStream fs = new FileStream(savePath, FileMode.Open))
                                using (GZipStream gz = new GZipStream(fs, CompressionMode.Decompress))
                                using (MemoryStream decompressedStream = new MemoryStream())
                                {
                                    gz.CopyTo(decompressedStream);
                                    decompressedStream.Position = 0;
                                    var image = System.Drawing.Image.FromStream(decompressedStream);
                                    DisplayImage(image);

                                    string decompressedPath = Path.Combine(SaveDirectory2, imgNameOnly + "_DECOMPRESSED");
                                    File.WriteAllBytes(decompressedPath, decompressedStream.ToArray());
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error decompressing image: {ex.Message}");
                            }

                            Invoke((Action)(() => txtImgPath.Clear()));
                           
                        }
                    }
                    else if (data.StartsWith("VIDSIZE"))
                    {
                        int videoLength = br.ReadInt32();
                        byte[] videoData = br.ReadBytes(videoLength);
                        string videoNameOnly = Path.GetFileName(txtVideoPath.Text);
                        string savePath = Path.Combine(SaveDirectory2, videoNameOnly);
                        File.WriteAllBytes(savePath, videoData);

                        // Set the URL of the Windows Media Player control to the temporary file
                        axWindowsMediaPlayer1.URL = savePath;

                        // Optionally, you can start playing the video immediately
                        axWindowsMediaPlayer1.Ctlcontrols.play();

                        //File.WriteAllBytes(videoNameOnly, videoData);
                        Invoke((Action)(() => txtVideoPath.Clear()));
                    }


                    else if (data.StartsWith("VIDSIZE"))
                    {
                        int videoLength = br.ReadInt32();
                        byte[] videoData = br.ReadBytes(videoLength);
                        string videoNameOnly = Path.GetFileName(txtVideoPath.Text);
                        string savePath = Path.Combine(SaveDirectory2, videoNameOnly);
                        File.WriteAllBytes(savePath, videoData);

                        // Set the URL of the Windows Media Player control to the temporary file
                        axWindowsMediaPlayer1.URL = savePath;

                        // Optionally, you can start playing the video immediately
                        axWindowsMediaPlayer1.Ctlcontrols.play();

                        //File.WriteAllBytes(videoNameOnly, videoData);
                        Invoke((Action)(() => txtVideoPath.Clear()));
                    }

                    else if (data.StartsWith("DIR:"))
                    {

                        string dataa = data.Substring(9);
                        lstOfTreeDir.Items.Add(dataa);
                    }
                    else if (data.StartsWith("FILECOUNT:"))
                    {
                        int fileCount = int.Parse(data.Substring(10));
                        for (int i = 0; i < fileCount; i++)
                        {
                            string fileName = br.ReadString();
                            int fileSize = br.ReadInt32();
                            byte[] fileData = br.ReadBytes(fileSize);

                            string savePath = Path.Combine(SaveDirectory, fileName);
                            File.WriteAllBytes(savePath, fileData);



                            txtListDownloadList.Text += $"{fileName}{Environment.NewLine}";

                        }
                        MessageBox.Show("Files received successfully.");
                    }




                    else if (data.Contains(':'))
                    {

                        lstDirectories.Items.Add(data);
                    }
                    else
                    {

                        txtChat.Text += $"{data}{Environment.NewLine}";
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                }
                
            }

        }
        private void ReqSpecificImg_Click_1(object sender, EventArgs e)
        {
            string imgName = null;
            OpenFileDialog file = new OpenFileDialog()
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Select an Image to Send"
            };
            if (file.ShowDialog() == DialogResult.OK)
            {
                imgName = file.FileName;

            }
            Invoke((Action)(() => txtImgPath.Text = imgName));
            if (selectedImageFormat == "ordinary")
            {
                bw.Write("ord" + imgName);
                bw.Flush();
            }
            else if (selectedImageFormat == "compressed")
            {
                bw.Write("com" + imgName);
                bw.Flush();
            }


        }
        private void DisplayImage(System.Drawing.Image image)
        {
            ChoiceFormatOfImage form2 = new ChoiceFormatOfImage();
            form2.UpdateImage(image);
            form2.Show();
        }
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            try
            {
                bw.Write(txtMsg.Text);
                bw.Flush();
                txtChat.Text += $"{txtMsg.Text}{Environment.NewLine}";
                txtMsg.Text = string.Empty;
            }
            catch(Exception) { };
        }

        private void btnBackDirInfo_Click(object sender, EventArgs e)
        {
            bw.Write(txtDirPath.Text);
            bw.Flush();
            lstDirectories.Items.Clear();

        }

      

        private void ReqSpecificFile_Click(object sender, EventArgs e)
        {
            bw.Write(txtFilePath.Text);
            bw.Flush();

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bw.Write("DIR:" + txtDirTreePath.Text);
                bw.Flush();
                
                lstOfTreeDir.Items.Clear(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string vidName = null;
            OpenFileDialog openFile = new OpenFileDialog();
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                vidName = openFile.FileName;  
            }
            bw.Write(vidName);
            bw.Flush();

            Invoke((Action)(() => txtVideoPath.Text = vidName));
        }

        private void btnDownloadListOfFile_Click(object sender, EventArgs e)
        {

          
                string folderPath = txtDownloadFile.Text;
                bw.Write("FOLDER:" + folderPath);
                bw.Flush();
          

        }

        private void btnImageType_Click(object sender, EventArgs e)
        {
            ChoiceFormatOfImage form = new ChoiceFormatOfImage();
            form.ImageSelected += OnImageSelected; // Subscribe to the new event
            form.Show();
        }

        private void OnImageSelected(string format, string imagePath)
        {
            selectedImageFormat = format;
            txtImgPath.Text = imagePath; // Assuming you have a TextBox to display the selected image path
           
            if (format == "ordinary")
            {
                bw.Write("ord" + imagePath);
                bw.Flush();
            }
            else if (format == "compressed")
            {
                bw.Write("com" + imagePath);
                bw.Flush();
            }
            Invoke((Action)(() => txtImgPath.Text = imagePath));

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            br.Close();
            bw.Close();
            ns.Close();
            client.Close();

        }

       
    }
}

