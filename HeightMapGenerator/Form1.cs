using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;

namespace HeightMapGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            var repMaps = new DirectoryInfo("maps");
            foreach (var file in repMaps.GetFiles("*.png"))
            {
                byte[] height = new byte[512 * 512];
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(file.FullName);
                int i = 0;
                for(int x = 0;x < 512;x++)
                {
                    for (int y = 0; y < 512; y++)
                    {
                        height[i] = bitmap.GetPixel(x, y).R;
                        i++;
                    }
                }
                save(file.FullName.Replace(".png", ".height"), height);
            }
        }

        private void save(string fileName, byte[] data)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
                {
                    using (var zipStream = new GZipStream(fs, CompressionMode.Compress))
                    {
                        zipStream.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
