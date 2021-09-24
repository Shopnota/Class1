using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageSteganigraphyClass1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                //ofd.Filter = "Image File (*.png)|.png";
                ofd.Title = "Please select a image file!!!!";
                ofd.Multiselect = false;

                string imgfile = "";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imgfile = ofd.FileName;
                }


                Image image = Image.FromFile(imgfile);
                pictureBox1.Image = image;


                string secret_message = " Shuvro ";

                char[] binary_secret_message = messageBinaryFormat(secret_message);

                Bitmap cover_image = new Bitmap(imgfile);


                int bitnoS = 0;
                for (int y = 0; y < cover_image.Height; y++)
                {
                    for (int x = 0; x < cover_image.Width; x++)
                    {

                        if (bitnoS < binary_secret_message.Length)
                        {
                            Color color = cover_image.GetPixel(x, y);



                            int red = color.R;
                            int green = color.G;
                            int blue = color.B;

                            string red_binary = Convert.ToString(red, 2).PadLeft(8, '0');
                            string green_binary = Convert.ToString(green, 2).PadLeft(8, '0');
                            string blue_binary = Convert.ToString(blue, 2).PadLeft(8, '0');

                            int newred = Convert.ToInt32(red_binary.Remove(red_binary.Length - 1, 1) + binary_secret_message[bitnoS], 2);
                            int newgreen = Convert.ToInt32(green_binary.Remove(green_binary.Length - 1, 1) + binary_secret_message[bitnoS + 1], 2);
                            int newblue = Convert.ToInt32(blue_binary.Remove(blue_binary.Length - 1, 1) + binary_secret_message[bitnoS + 2], 2);

                            bitnoS = bitnoS + 3;

                            Color new_color = Color.FromArgb(newred, newgreen, newblue);
                            cover_image.SetPixel(x, y, new_color);


                            Console.WriteLine("X: " + x + " Y: " + y + " Color: (" + new_color.R + " " + new_color.G + " " + new_color.B + ")");

                        }
                        else
                        {
                            break;
                        }
                    }
                }

                cover_image.Save(@"C:\Users\DCL\OneDrive\Desktop\Stego_new.png");

                MessageBox.Show("Complete!!");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string messageBinary = "";
        public static char[] messageBinaryFormat(string message)
        {
            /*
            * Here we are creating messages bit to binary form
            */
            //message is converted to 8bit binary

            //foreach (char c in message.ToCharArray())
            //{
            //    string temp = Convert.ToString(c, 2).PadLeft(16, '0');
            //    sb += temp;
            //}

            string messageBinary = "";
            char[] msg = message.ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                string temp = Convert.ToString(msg[i], 2).PadLeft(8, '0');
                messageBinary += temp;
            }


            //to maintain error from pass length we have to add (extra 0 or 00)
            if (((messageBinary.Length) % 3) == 2)
            {
                messageBinary += "0";
            }
            else if (((messageBinary.Length) % 3) == 1)
            {
                messageBinary += "00";
            }

            return messageBinary.ToCharArray(); //all binary bit has been converted into array.
        }


        public static StringBuilder secretRealMessage = new StringBuilder();

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "Image File (*.png)|.png";
            ofd.Title = "Please select a image file!!!!";
            ofd.Multiselect = false;

            string imgfile = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imgfile = ofd.FileName;
            }


            Image image = Image.FromFile(imgfile);
            pictureBox2.Image = image;


            Bitmap stego_image = new Bitmap(imgfile);

            string final_string = "";

            for (int y = 0; y < stego_image.Height; y++)
            {
                for (int x = 0; x < stego_image.Width; x++)
                {
                    if(x < 8)
                    {
                        string temp = secretBitMetaData(stego_image, x, y);
                        final_string += temp;
                    }
                    else
                    {
                        break;
                    }

                }
            }

            char[] secretMessageBinary = final_string.ToCharArray();
            string bit8 = "";
            int aa = 0;


            //Console.WriteLine(secretMessage);
            for (int i = 0; i < secretMessageBinary.Length; i++)
            {
                if (aa != 8)
                {
                    bit8 = bit8 + secretMessageBinary[i].ToString();

                    aa++;
                }
                if (aa == 8)
                {
                    int acii = 32;
                    try
                    {
                        acii = Convert.ToInt32(bit8, 2);
                        secretRealMessage.Append(Char.ConvertFromUtf32(acii));
                    }
                    catch (Exception exxx)
                    {
                        secretRealMessage.Append(Char.ConvertFromUtf32(32));
                    }


                    bit8 = "";
                    aa = 0;
                }
            }
            textBox1.Text = secretRealMessage.ToString();

        }



        private static string secretBitMetaData(Bitmap img, int x, int y)
        {
            Color pixel = img.GetPixel(x, y);
            string red = Convert.ToString(pixel.R, 2).PadLeft(8, '0');
            string green = Convert.ToString(pixel.G, 2).PadLeft(8, '0');
            string blue = Convert.ToString(pixel.B, 2).PadLeft(8, '0');
            string s = (red.Last().ToString() + green.Last().ToString() + blue.Last().ToString());
            return s;
        }

    }
}
    