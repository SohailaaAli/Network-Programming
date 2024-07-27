using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ChoiceFormatOfImage : Form
    {
        public event Action<string, string> ImageSelected; // Updated to pass the image path
        public ChoiceFormatOfImage()
        {
            InitializeComponent();
        }

        private void btnCompressed_Click(object sender, EventArgs e)
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
                ImageSelected?.Invoke("compressed", imgName); // Pass the format and image path
            }
            this.Close();
        }

        private void btnOrdinary_Click(object sender, EventArgs e)
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
                ImageSelected?.Invoke("ordinary", imgName); // Pass the format and image path
            }
            this.Close();
        }

        public void UpdateImage(System.Drawing.Image image)
        {
            imgShowBox.Image = image;
        }
    }
}
