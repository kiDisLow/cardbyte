using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text;
using System.Linq.Expressions;

namespace OCRAPITest
{
    public partial class MainForm : Form
    {
        public Rootobject ocrResult;
        public Root obj = new Root();
        
        public diliverymethods dmethod = new diliverymethods();
        public myobject myobject1 = new myobject();
        

        public string ImagePath { get; set; }
        public string PdfPath { get; set; }

        public Image img { get; set; }
        public string imagename = "";
        public string imageext = "";
        byte[] imageData;
        public int sizeincreser=1;
        public int leftmover = 1;
        public int hightincreser = 1;
        public List<string> s1 = new List<string>();

        bool filling;
        bool draging;
        bool drawingisneeded;
        int a;
        int b;
        int identifier;
       
       

        public MainForm()
        {
            InitializeComponent();
            cmbLanguage.SelectedIndex = 6;//English
        }

        private string getSelectedLanguage()
        {

            //https://ocr.space/OCRAPI#PostParameters

            //Czech = cze; Danish = dan; Dutch = dut; English = eng; Finnish = fin; French = fre; 
            //German = ger; Hungarian = hun; Italian = ita; Norwegian = nor; Polish = pol; Portuguese = por;
            //Spanish = spa; Swedish = swe; ChineseSimplified = chs; Greek = gre; Japanese = jpn; Russian = rus;
            //Turkish = tur; ChineseTraditional = cht; Korean = kor

            string strLang = "";
            switch (cmbLanguage.SelectedIndex)
            {
                case 0:
                    strLang = "ara";
                    break;

                case 1:
                    strLang = "chs";
                    break;

                case 2:
                    strLang = "cht";
                    break;
                case 3:
                    strLang = "cze";
                    break;
                case 4:
                    strLang = "dan";
                    break;
                case 5:
                    strLang = "dut";
                    break;
                case 6:
                    strLang = "eng";
                    break;
                case 7:
                    strLang = "fin";
                    break;
                case 8:
                    strLang = "fre";
                    break;
                case 9:
                    strLang = "ger";
                    break;
                case 10:
                    strLang = "gre";
                    break;
                case 11:
                    strLang = "hun";
                    break;
                case 12:
                    strLang = "jap";
                    break;
                case 13:
                    strLang = "kor";
                    break;
                case 14:
                    strLang = "nor";
                    break;
                case 15:
                    strLang = "pol";
                    break;
                case 16:
                    strLang = "por";
                    break;
                case 17:
                    strLang = "spa";
                    break;
                case 18:
                    strLang = "swe";
                    break;
                case 19:
                    strLang = "tur";
                    break;

            }
            return strLang;

        }

       private void button1_Click(object sender, EventArgs e)
        {
            PdfPath = ImagePath = ""; pictureBox.BackgroundImage = null;
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "jpeg and png files|*.png;*.jpg;*.JPG";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(fileDlg.FileName);
                if (fileInfo.Length > 5* 1024* 1024)
                {
                    //Size limit depends: Free API 1 MB, PRO API 5 MB and more
                    MessageBox.Show("Image file size limit reached (1MB free API)");
                    return;
                }
                img = Image.FromFile(fileDlg.FileName);
                img = Resize(img, 860, 600);
                string filename = fileInfo.Name;                          
                img.Save(fileInfo.Name, img.RawFormat);
                fileInfo = new FileInfo(filename);
                ImagePath = fileInfo.FullName;
                imagename = fileInfo.Name;
                imageext = fileInfo.Extension;
                img = Image.FromFile(fileInfo.FullName);
                pictureBox.Image = img;


                lblInfo.Text = "Image loaded: "+ fileInfo.Name;
                lblInfo.BackColor = Color.LightGreen;
            }
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            PdfPath = ImagePath = "";
            pictureBox.BackgroundImage = null;
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "pdf files|*.pdf;";
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(fileDlg.FileName);
                if (fileInfo.Length > 5* 1024 * 1024 )
                {
                    //Size limit depends: Free API 1 MB, PRO API 5 MB and more
                    MessageBox.Show("PDF file size should not be larger than 5Mb");
                    return;
                }
                PdfPath = fileDlg.FileName;
                //PDF files are loaded, but can not be displayed in the image control. That does not affect the OCR.
                lblInfo.Text = "PDF loaded [but not displayed]: " + fileInfo.Name;
                lblInfo.BackColor = Color.LightSalmon;
            }
        }

        private byte[] ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                return imageBytes;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {


            if (string.IsNullOrEmpty(ImagePath) && string.IsNullOrEmpty(PdfPath))
                return;

            //txtResult.Text = "";

            button1.Enabled = false;
            button2.Enabled = false;
            btnPDF.Enabled = false;

            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(1, 1, 1);


                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent("be3f1a688588957"), "apikey"); //Added api key in form data
                form.Add(new StringContent(getSelectedLanguage()), "language");

                form.Add(new StringContent("2"), "ocrengine"); 
                form.Add(new StringContent("true"), "scale");
                form.Add(new StringContent("true"), "istable");

                if (string.IsNullOrEmpty(ImagePath) == false)
                {
                    imageData = File.ReadAllBytes(ImagePath);
                    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
                }
                else if (string.IsNullOrEmpty(PdfPath) == false)
                {
                    byte[] imageData = File.ReadAllBytes(PdfPath);
                    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "PDF", "pdf.pdf");
                }

                HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);

                string strContent = await response.Content.ReadAsStringAsync();



                ocrResult = JsonConvert.DeserializeObject<Rootobject>(strContent);

  
                if (ocrResult.OCRExitCode == 1)
                  {
                    obj.imageWidth = img.Width;
                    obj.imageHeight = img.Height;
                    obj.imagePath = ImagePath;
                    obj.imageData = File.ReadAllBytes(ImagePath).ToString();
                    obj.version = "vaibhav test version 1";



                  }
                     else
                     {
                         MessageBox.Show("ERROR: " + strContent);
                     }
                    


            }
            catch (Exception exception)
            {
                MessageBox.Show("Ooops" + exception);
                //txtResult.Text = exception.ToString();
            }

            button1.Enabled = true;
            button2.Enabled = true;
            btnPDF.Enabled = true;
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {

        }

      

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private Image Resize(Image image, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.DrawImage(image, 0, 0, w, h);
            graphic.Dispose();

            return bmp;
        }

       
        private void drawrectangle()
        {
            Graphics gobject = pictureBox.CreateGraphics();
            Brush brush = new SolidBrush(Color.Red);
            List<Pen> pen = new List<Pen>();
            pen.Add(new Pen(Color.Red, 1));
            pen.Add(new Pen(Color.Green, 1));
            pen.Add(new Pen(Color.Blue, .5f));
            pen.Add(new Pen(Color.Black, .5f));
            this.Refresh();
            if (obj.shapes != null)
            {
                for (int i = 0; i < obj.shapes.Count; i++)
                {
                    if(obj.shapes[i].points != null)
                    for (int k = 0; k < obj.shapes[i].points.Count(); k++)
                    {

                        if (k != obj.shapes[i].points.Count() - 1)
                        {
                            gobject.DrawLine(pen[2], (float)obj.shapes[i].points[k][0], (float)obj.shapes[i].points[k][1], (float)obj.shapes[i].points[k + 1][0], (float)obj.shapes[i].points[k + 1][1]);
                        }
                        else if (k == obj.shapes[i].points.Count() - 1)
                        {
                            gobject.DrawLine(pen[3], (float)obj.shapes[i].points[k][0], (float)obj.shapes[i].points[k][1], (float)obj.shapes[i].points[0][0], (float)obj.shapes[i].points[0][1]);
                        }
                        Rectangle rectangle = new Rectangle((int)obj.shapes[i].points[k][0], (int)obj.shapes[i].points[k][1], 2, 2);

                        gobject.DrawRectangle(pen[2], rectangle);


                    }

                }

            }

        }

       


        #region  moving functions
        public void ittrater(int typ)
        {
          
            Graphics gobject = pictureBox.CreateGraphics();
            pictureBox.BackgroundImage = img;
            Brush brush = new SolidBrush(Color.Red);

            Pen redpen = new Pen(brush, 1);
            for (int i = 0; i < ocrResult.ParsedResults.Count(); i++)
            {
                for (int k = 0; k < ocrResult.ParsedResults[i].textOverlay.lines.Count(); k++)
                {
                    for (int j = 0; j < ocrResult.ParsedResults[i].textOverlay.lines[k].words.Count(); j++)
                    {
                        float left = (float)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Left);
                       // left = left - left / 80;
                        float top = (float)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Top  );
                       // top = top + top / 80;
                        float width = (float)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Width);
                       //// width = width + width / 10;
                        double hight = (float)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Height);
                       // hight = hight + hight / 10;
                       if(typ==1)
                        inscreasesize((int)left, (int)top, (int)(width), (int)(hight));
                       else if(typ ==2)
                         decreseesize((int)left, (int)top, (int)(width), (int)(hight));
                       else if(typ ==3)
                            movelft((int)left, (int)top, (int)(width), (int)(hight));
                       else if (typ ==4)
                            moveright((int)left, (int)top, (int)(width), (int)(hight));
                        else if (typ == 5)
                            moveup((int)left, (int)top, (int)(width), (int)(hight));



                    }
                }

            }

        }
     
        private void inscreasesize(int x, int y, int width, int hight)
        {
            width = width + width * sizeincreser / 60;
            hight = hight + hight * sizeincreser / 100;
            sizeincreser++;
            Rectangle rectangle = new Rectangle(x, y, width, hight);
            // object o =new object();
            //EventArgs e1= new EventArgs();
           // drawrectangle(rectangle);
            // return rectangle;

        }

        private void decreseesize(int x, int y, int width, int hight)
        {
            width = width - width * sizeincreser / 60;
            hight = hight - hight * sizeincreser / 100;
            sizeincreser--;
            Rectangle rectangle = new Rectangle(x, y, width, hight);
            // object o =new object();
            //EventArgs e1= new EventArgs();
            //drawrectangle(rectangle);
            // return rectangle;

        }

        private void movelft(int x,int y,int width,int hight)
        {
            x = x - leftmover;
           // hight = hight - hight * sizeincreser / 100;
            leftmover++;
            Rectangle rectangle = new Rectangle(x, y, width, hight);
            // object o =new object();
            //EventArgs e1= new EventArgs();
           // drawrectangle(rectangle);
            // return rectangle;


        }
        private void moveright(int x, int y, int width, int hight)
        {
            x = x + sizeincreser;
            // hight = hight - hight * sizeincreser / 100;
            sizeincreser--;
            Rectangle rectangle = new Rectangle(x, y, width, hight);
            // object o =new object();
            //EventArgs e1= new EventArgs();
            //drawrectangle(rectangle);
            // return rectangle;


        }

        private void moveup(int x, int y, int width, int hight)
        {
            hight = x + hightincreser;
            // hight = hight - hight * sizeincreser / 100;
            sizeincreser--;
            Rectangle rectangle = new Rectangle(x, y, width, hight);
            // object o =new object();
            //EventArgs e1= new EventArgs();
           // drawrectangle(rectangle);
            // return rectangle;


        }
        private void movedown(int x, int y, int width, int hight)
        {
            hight = x + hightincreser;
            // hight = hight - hight * sizeincreser / 100;
            sizeincreser--;
            Rectangle rectangle = new Rectangle(x, y, width, hight);
            // object o =new object();
            //EventArgs e1= new EventArgs();
           // drawrectangle(rectangle);
            // return rectangle;


        }


        #endregion

        #region moving bottons
        private void button3_Click(object sender, EventArgs e)
        {


            ittrater(1);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ittrater(2);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ittrater(3);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            ittrater(4);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            ittrater(4);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            ittrater(4);
        }

        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void pictureBox_Click(object sender, EventArgs e)
        {
            Point P = PointToScreen(new Point(pictureBox.Bounds.Left, pictureBox.Bounds.Top));
       
            if (pictureBox.Image == img  && filling == true && drawingisneeded==false)
            {
                #region  for name

                int x = MousePosition.X - P.X;
                int y = MousePosition.Y - P.Y;
                mlable1.Text = x.ToString();
                mlable2.Text = y.ToString();
                button9.Text = "save name";
                textprocessor t1 = new textprocessor();
                if (lblfield1.Text == "Name prifix")
                {
                    string[] s2 = t1.nameprocessor(x, y, ocrResult);


                    feild1.Text = s2[0];
                    feild2.Text = s2[1];
                    if (s2[2] == null)
                    {
                        feild4.Text = "";
                        feild3.Text = "";
                    }
                    else if (s2[3] == null)
                    {
                        feild4.Text = s2[2];

                    }
                    else
                    {
                        feild4.Text = s2[3];
                        feild3.Text = s2[2];
                    }

                }
                #endregion
                #region for address line 1

                if (lblfield1.Text == "floor1")
                {
                    button9.Text = "save adress 1";
                    label15.BackColor = Color.LightGreen;
                    s1 = t1.adressprocessor(x, y, ocrResult);


                    feild1.Text = s1[0];
                    feild2.Text = s1[1];
                    feild3.Text = s1[2];
                    feild4.Text = s1[3];
                    feild5.Text = s1[4];
                    feild6.Text = s1[5];
                    feild7.Text = s1[6];
                    feild8.Text = s1[7];
                    feild9.Text = s1[8];
                    feild10.Text = s1[9];
                    feild11.Text = s1[10];
                    feild12.Text = s1[11];
                    feild13.Text = s1[12];





                }
                #endregion
                #region  for adress line 2
                if (lblfield1.Text == "floor2")
                {
                    button9.Text = "save adress 2";
                    label12.BackColor = Color.LightGreen;
                    s1 = t1.adressprocessor(x, y, ocrResult);

                    feild1.Text = s1[0];
                    feild2.Text = s1[1];
                    feild3.Text = s1[2];
                    feild4.Text = s1[3];
                    feild5.Text = s1[4];
                    feild6.Text = s1[5];
                    feild7.Text = s1[6];
                    feild8.Text = s1[7];
                    feild9.Text = s1[8];
                    feild10.Text = s1[9];
                    feild11.Text = s1[10];
                    feild12.Text = s1[11];
                    feild13.Text = s1[12];



                }

                #endregion
                #region  for adress line 3
                if (lblfield1.Text == "floor3")
                {
                    button9.Text = "save adress 3";
                    label13.BackColor = Color.LightGreen;
                    s1 = t1.adressprocessor(x, y, ocrResult);

                    s1.AddRange(t1.adressprocessor(x, y, ocrResult));

                    feild1.Text = s1[0];
                    feild2.Text = s1[1];
                    feild3.Text = s1[2];
                    feild4.Text = s1[3];
                    feild5.Text = s1[4];
                    feild6.Text = s1[5];
                    feild7.Text = s1[6];
                    feild8.Text = s1[7];
                    feild9.Text = s1[8];
                    feild10.Text = s1[9];
                    feild11.Text = s1[10];
                    feild12.Text = s1[11];
                    feild13.Text = s1[12];



                }

                #endregion
                #region  for adress line 4
                if (lblfield1.Text == "floor4")
                {
                    button9.Text = "save adress 4";
                    label14.BackColor = Color.LightGreen;
                    s1 = t1.adressprocessor(x, y, ocrResult);

                    s1.AddRange(t1.adressprocessor(x, y, ocrResult));

                    feild1.Text = s1[0];
                    feild2.Text = s1[1];
                    feild3.Text = s1[2];
                    feild4.Text = s1[3];
                    feild5.Text = s1[4];
                    feild6.Text = s1[5];
                    feild7.Text = s1[6];
                    feild8.Text = s1[7];
                    feild9.Text = s1[8];
                    feild10.Text = s1[9];
                    feild11.Text = s1[10];
                    feild12.Text = s1[11];
                    feild13.Text = s1[12];



                }

                #endregion
                #region for email
                if (lblfield1.Text == "email")
                {
                    button9.Text = "save email";
                    string s1 = t1.emailprocessor(x, y, ocrResult);
                    feild1.Text = s1;
                    lineTextBox.Text = s1;
                }
                #endregion
                #region for mobile
                if (lblfield1.Text == "mobile no")
                {
                    button9.Text = "save phone";
                    string s1 = t1.mobileprocessor(x, y, ocrResult);
                    feild1.Text = s1;
                    lineTextBox.Text = s1;
                  
                }
                #endregion
                #region for company name
                if (lblfield1.Text == "company name")
                {
                    button9.Text = "save company";
                    feild1.Text = t1.getstring(x, y, ocrResult);
                }
                #endregion
                #region for date of birth
                if (lblfield1.Text == "dd")
                {
                    button9.Text = "save date of birth";
                    feild1.Text = t1.getstring(x, y, ocrResult);
                }
                #endregion
                #region for designation
                if (lblfield1.Text == "designation")
                {
                    button9.Text = "save designation";
                    feild1.Text = t1.getstring(x, y, ocrResult);
                }
                #endregion
                #region for website
                if (lblfield1.Text == "webadress")
                {
                    button9.Text = "save website";
                    feild1.Text = t1.getWord(x, y, ocrResult);
                    lineTextBox.Text = t1.getWord(x, y, ocrResult);

                }
                #endregion
                #region for buisness typ
                if (lblfield1.Text == "buisness typ")
                {
                    button9.Text = "save buisness typ";
                    feild1.Text = t1.getstring(x, y, ocrResult);
                }
                #endregion
                #region for others
                if (lblfield1.Text == "f1")
                {
                    button9.Text = "save others";
                    if (feild1.Text == "")
                    {
                       
                        feild1.Text = t1.getWord(x, y, ocrResult);
                    }
                    else if(feild2.Text =="")
                    {
                        feild2.Text = t1.getWord(x, y, ocrResult);

                    }
                    else if (feild3.Text == "")
                    {
                        feild3.Text = t1.getWord(x, y, ocrResult);

                    }
                    else if (feild4.Text == "")
                    {
                        feild4.Text = t1.getWord(x, y, ocrResult);

                    }
                    else if (feild5.Text == "")
                    {
                        feild5.Text = t1.getWord(x, y, ocrResult);

                    }
                    else if (feild6.Text == "")
                    {
                        feild6.Text = t1.getWord(x, y, ocrResult);

                    }
                    else if (feild7.Text == "")
                    {
                        feild7.Text = t1.getWord(x, y, ocrResult);

                    }
                    else if (feild8.Text == "")
                    {
                        feild8.Text = t1.getWord(x, y, ocrResult);

                    }
                    else if (feild9.Text == "")
                    {
                        feild9.Text = t1.getWord(x, y, ocrResult);

                    }

                }
                #endregion

               

            }
            
            


            

            if(drawingisneeded==true)
            {
                int x = MousePosition.X - P.X;
                int y = MousePosition.Y - P.Y;
                mlable1.Text = x.ToString();
                mlable2.Text = y.ToString();
                List<Double> p1 = new List<double>();
                p1.Add(x);
                p1.Add(y);
                obj.shapes[identifier].points.Add(p1);



            }
            drawrectangle();

        }
        private void label6_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text = "company name";
            lblfield1.Visible = true;
            feild1.Visible = true;
        }

        private void label9_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text = "designation";
            lblfield1.Visible = true;
            feild1.Visible = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text =  "Name prifix";
            lblfield2.Text = "Frist Name";
            lblfield3.Text = "Middle Name";
            lblfield4.Text = "Last Name";
            lblfield1.Visible = true;
            lblfield2.Visible = true;
            lblfield3.Visible = true;
            lblfield4.Visible = true;
            
            feild1.Visible = true;
            feild2.Visible = true;
            feild3.Visible = true;
            feild4.Visible = true;



        }

        private void lblfield1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
           
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }


        private void hidelbl()
        {
            feild1.Text = "";
            feild2.Text = "";
            feild3.Text = "";
            feild4.Text = "";
            feild5.Text = "";
            feild6.Text = "";
            feild7.Text = "";
            feild8.Text = "";
            feild9.Text = "";
            feild10.Text = "";
            feild11.Text = "";
            feild12.Text = "";
            feild13.Text = "";





            lblfield5.Visible = false;
            lblfield6.Visible = false;
            lblfield7.Visible = false;
            lblfield8.Visible = false;
            lblfield9.Visible = false;
            lblfield10.Visible = false;
            lblfield1.Visible = false;
            lblfield2.Visible = false;
            lblfield3.Visible = false;
            lblfield4.Visible = false;
            lblfield5.Visible = false;
            lblfield6.Visible = false;

            feild1.Visible = false;
            feild2.Visible = false;
            feild3.Visible = false;
            feild4.Visible = false;
            feild5.Visible = false;
            feild6.Visible = false;
            feild7.Visible = false;
            feild8.Visible = false;
            feild9.Visible = false;
            feild10.Visible = false;
            feild11.Visible = false;
            feild12.Visible = false;
            feild13.Visible = false;

            comboBox1.Visible = false;
            comboBox2.Visible = false;
            comboBox3.Visible = false;
            comboBox4.Visible = false;
            comboBox5.Visible = false;
            comboBox6.Visible = false;
            comboBox7.Visible = false;
            comboBox8.Visible = false;
            comboBox9.Visible = false;
            comboBox10.Visible = false;
            comboBox11.Visible = false;
            comboBox12.Visible = false;
            comboBox13.Visible = false;
            phone_combobox.Visible = false;



        }

        private void label4_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text = "email";
            lblfield1.Visible = true;
            feild1.Visible = true;
        }

        private void label5_Click(object sender, EventArgs e)
        {

            hidelbl();
            lblfield1.Text = "mobile no";
            lblfield2.Text = "fax";
            lblfield3.Text = "telephone";
            phone_combobox.Visible = true;
           // lblfield1.Visible = true;
            //lblfield2.Visible = true;
           // lblfield3.Visible = true;
            feild1.Visible = true;
           // feild2.Visible = true;
           // feild3.Visible = true;

        }

        private void label10_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text = "dd";
            lblfield2.Text = "mm";
            lblfield3.Text = "yy";
            lblfield1.Visible = true;
            lblfield2.Visible = true;
            lblfield3.Visible = true;
            feild1.Visible = true;
            feild2.Visible = true;
            feild3.Visible = true;
        }

        private void label8_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text = "webadress";
            lblfield1.Visible = true;
            feild1.Visible = true;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text = "buisness typ";
            lblfield1.Visible = true;
            feild1.Visible = true;

        }

        private void label11_Click(object sender, EventArgs e)
        {
            hidelbl();
            lblfield1.Text = "f1";
            lblfield2.Text = "f2";
            lblfield3.Text = "f3";
            lblfield4.Text = "f4";
            lblfield5.Text = "f5";
            lblfield6.Text = "f6";
            lblfield7.Text = "f7";
            lblfield8.Text = "f8";
            lblfield9.Text = "f9";


            lblfield1.Visible = true;
            lblfield2.Visible = true;
            lblfield3.Visible = true;
            lblfield4.Visible = true;
            lblfield5.Visible = true;
            lblfield6.Visible = true;
            lblfield7.Visible = true;
            lblfield8.Visible = true;
            lblfield9.Visible = true;


            feild1.Visible = true;
            feild2.Visible = true;
          
            feild3.Visible = true;
            feild4.Visible = true;
            feild5.Visible = true;
            feild6.Visible = true;
            feild7.Visible = true;
            feild8.Visible = true;
            feild9.Visible = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            hidelbl();
            button_drawing.Visible = false;
            
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {
            hidelbl();
            setnull();
            lblfield1.Text = "floor1";
            lblfield2.Text = "house no.";
            lblfield3.Text = "House name";
            lblfield4.Text = "street no.";
            lblfield5.Text = "street Name";
            lblfield6.Text = "city";
            lblfield7.Text = "pin";
            lblfield8.Text = "landmark";
            lblfield9.Text = "state";
            lblfield10.Text = "country";
            comboBox1.Visible = true;
            comboBox2.Visible = true;
            comboBox3.Visible = true;
            comboBox4.Visible = true;
            comboBox5.Visible = true;
            comboBox6.Visible = true;
            comboBox7.Visible = true;
            comboBox8.Visible = true;
            comboBox9.Visible = true;
            comboBox10.Visible = true;
            comboBox11.Visible = true;
            comboBox12.Visible = true;
            comboBox13.Visible = true;
            feild1.Visible = true;
            feild2.Visible = true;
            feild3.Visible = true;
            feild4.Visible = true;
            feild5.Visible = true;
            feild6.Visible = true;
            feild7.Visible = true;
            feild8.Visible = true;
            feild9.Visible = true;
            feild10.Visible = true;
            feild11.Visible = true;
            feild12.Visible = true;
            feild13.Visible = true;
        }

        private void label12_Click(object sender, EventArgs e)
        {
            hidelbl();
            setnull();
            lblfield1.Text = "floor2";
            lblfield2.Text = "house no.";
            lblfield3.Text = "House name";
            lblfield4.Text = "street no.";
            lblfield5.Text = "street Name";
            lblfield6.Text = "city";
            lblfield7.Text = "pin";
            lblfield8.Text = "landmark";
            lblfield9.Text = "state";
            lblfield10.Text = "country";
            comboBox1.Visible = true;
            comboBox2.Visible = true;
            comboBox3.Visible = true;
            comboBox4.Visible = true;
            comboBox5.Visible = true;
            comboBox6.Visible = true;
            comboBox7.Visible = true;
            comboBox8.Visible = true;
            comboBox9.Visible = true;
            comboBox10.Visible = true;
            comboBox11.Visible = true;
            comboBox12.Visible = true;
            comboBox13.Visible = true;
            feild1.Visible = true;
            feild2.Visible = true;
            feild3.Visible = true;
            feild4.Visible = true;
            feild5.Visible = true;
            feild6.Visible = true;
            feild7.Visible = true;
            feild8.Visible = true;
            feild9.Visible = true;
            feild10.Visible = true;
            feild11.Visible = true;
            feild12.Visible = true;
            feild13.Visible = true;
        }

        private void label13_Click(object sender, EventArgs e)
        {
            hidelbl();
            setnull();
            lblfield1.Text = "floor3";
            lblfield2.Text = "house no.";
            lblfield3.Text = "House name";
            lblfield4.Text = "street no.";
            lblfield5.Text = "street Name";
            lblfield6.Text = "city";
            lblfield7.Text = "pin";
            lblfield8.Text = "landmark";
            lblfield9.Text = "state";
            lblfield10.Text = "country";
            comboBox1.Visible = true;
            comboBox2.Visible = true;
            comboBox3.Visible = true;
            comboBox4.Visible = true;
            comboBox5.Visible = true;
            comboBox6.Visible = true;
            comboBox7.Visible = true;
            comboBox8.Visible = true;
            comboBox9.Visible = true;
            comboBox10.Visible = true;
            comboBox11.Visible = true;
            comboBox12.Visible = true;
            comboBox13.Visible = true;
            feild1.Visible = true;
            feild2.Visible = true;
            feild3.Visible = true;
            feild4.Visible = true;
            feild5.Visible = true;
            feild6.Visible = true;
            feild7.Visible = true;
            feild8.Visible = true;
            feild9.Visible = true;
            feild10.Visible = true;
            feild11.Visible = true;
            feild12.Visible = true;
            feild13.Visible = true;
        }

        private void label14_Click(object sender, EventArgs e)
        {
            hidelbl();
            setnull();
            lblfield1.Text = "floor4";
            lblfield2.Text = "house no.";
            lblfield3.Text = "House name";
            lblfield4.Text = "street no.";
            lblfield5.Text = "street Name";
            lblfield6.Text = "city";
            lblfield7.Text = "pin";
            lblfield8.Text = "landmark";
            lblfield9.Text = "state";
            lblfield10.Text = "country";
            comboBox1.Visible = true;
            comboBox2.Visible = true;
            comboBox3.Visible = true;
            comboBox4.Visible = true;
            comboBox5.Visible = true;
            comboBox6.Visible = true;
            comboBox7.Visible = true;
            comboBox8.Visible = true;
            comboBox9.Visible = true;
            comboBox10.Visible = true;
            comboBox11.Visible = true;
            comboBox12.Visible = true;
            comboBox13.Visible = true;
            feild1.Visible = true;
            feild2.Visible = true;
            feild3.Visible = true;
            feild4.Visible = true;
            feild5.Visible = true;
            feild6.Visible = true;
            feild7.Visible = true;
            feild8.Visible = true;
            feild9.Visible = true;
            feild10.Visible = true;
            feild11.Visible = true;
            feild12.Visible = true;
            feild13.Visible = true;
        }

       


        private void setnull()
        {
            feild1.Text = "";
            feild2.Text = "";
            feild3.Text = "";
            feild4.Text = "";
            feild5.Text = "";
            feild6.Text = "";
            feild7.Text = "";
            feild8.Text = "";
            feild9.Text = "";
            feild10.Text = "";
            feild11.Text = "";
            feild12.Text = "";
            feild13.Text = "";

        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (obj.shapes == null)
            {
                obj.shapes = new List<Shape>();
            }
            myobject1.mousex = int.Parse(mlable1.Text);
            myobject1.mousey= int.Parse(mlable2.Text);
            #region saving name

            if (button9.Text == "save name")
            {
                if (feild1.Text == "" && feild2.Text == "" && feild3.Text == "" && feild4.Text == "")
                {
                    MessageBox.Show("plz fill the fields 1st");
                }

                else
                {
                    if (dmethod.addname(obj, ocrResult, feild1.Text, feild2.Text, feild3.Text, feild4.Text,myobject1))
                    {
                        myobject my = new myobject();
                        my.label = "Card Area";
                        my.text = "card area";
                        dmethod.addcard(obj, ocrResult, my);
                        label2.BackColor = Color.LightGreen;
                        MessageBox.Show("name fields are saved"); }
                }



            }
            #endregion
            #region  saving adress 

            if (button9.Text == "save adress 1"|| button9.Text == "save adress 2"|| button9.Text == "save adress 3"|| button9.Text == "save adress 4")
            {
                if (feild1.Text != "" )
                {
                    List<myobject> o1 = new List<myobject>();
                    var combobox = new List<ComboBox> { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7, comboBox8, comboBox9, comboBox10, comboBox11, comboBox12, comboBox13, };
                    var textbox = new List<TextBox> { feild1, feild2, feild3, feild4, feild5, feild6, feild7, feild8, feild9, feild10, feild11, feild12, feild13 };
                    for(int i=0;i<combobox.Count();i++)
                    {
                        if (textbox[i].Text != null && combobox[i].SelectedIndex != -1)
                        {
                            var obj = new myobject();
                            obj.label = combobox[i].Text;
                            obj.text = textbox[i].Text;
                            obj.mousex = int.Parse(mlable1.Text);
                            obj.mousey = int.Parse(mlable2.Text);
                            o1.Add(obj);
                        }
                    }

                    if(dmethod.addadress(o1, obj, ocrResult))
                    {

                        
                            label3.BackColor = Color.LightGreen;
                            MessageBox.Show("a new adress line is saved saved");
                        
                    
                    }


                }

                else
                    MessageBox.Show("plz fill the fields 1st");

            }

            #endregion
            #region saving email
            if (button9.Text=="save email")
            {
                if(feild1.Text != null)
                {
                    myobject myobj = new myobject();
                        myobj.label = "Address";
                        myobj.text = feild1.Text;
                        myobj.searchtext = lineTextBox.Text;
                        myobj.mousex = int.Parse(mlable1.Text);
                        myobj.mousey = int.Parse(mlable2.Text);
                    if (dmethod.addemail(obj,ocrResult,myobj))
                    {
                         MessageBox.Show("name fields are saved");
                        label4.BackColor = Color.LightGreen;
                    }

                }
                else
                    MessageBox.Show("plz fill the fields 1st");



            }
            #endregion
            #region saving phone
            if (button9.Text == "save phone")
            {
                if (feild1.Text != null)
                {
                   List<myobject> myobj = new List<myobject>();

                    myobj.Add(new myobject());
                    myobj[0].label = phone_combobox.Text;
                    myobj[0].text = feild1.Text;
                    myobj[0].mousex = int.Parse(mlable1.Text);
                    myobj[0].mousey = int.Parse(mlable2.Text);
                    myobj[0].searchtext = lineTextBox.Text;

                   

                    if (dmethod.addphone(obj, ocrResult, myobj[0]))
                    {
                        MessageBox.Show("phone fields are saved");
                        label5.BackColor = Color.LightGreen;
                    }

                }
                else
                    MessageBox.Show("plz fill the fields 1st");



            }
            #endregion
            #region saving company name
            if (button9.Text == "save company")
            {

                if (feild1.Text != null)
                {
                    myobject1.label = "CompanyName";
                    myobject1.text = feild1.Text;
                    myobject1.mousex = int.Parse(mlable1.Text);
                    myobject1.mousey = int.Parse(mlable2.Text);

                    if (dmethod.addcompanyname(obj, ocrResult, myobject1))
                    {
                        MessageBox.Show("Company name fields are saved");
                        label6.BackColor = Color.LightGreen;
                    }
                }
                else
                {
                    MessageBox.Show("plz fill the fields 1st");
                }


            }

            #endregion
            #region saving date of birth
            if (button9.Text == "save date of birth")
            {
                if (feild1.Text != null)
                {
                    List<myobject> myobj = new List<myobject>();

                    myobj.Add(new myobject());
                    myobj[0].label = "DD";
                    myobj[0].text = feild1.Text;
                    myobj[0].mousex = int.Parse(mlable1.Text);
                    myobj[0].mousey = int.Parse(mlable2.Text);

                    if (feild2.Text != null)
                    {
                        myobj.Add(new myobject());
                        myobj[1].label = "MM";
                        myobj[1].text = feild2.Text;
                        myobj[1].mousex = int.Parse(mlable1.Text);
                        myobj[1].mousey = int.Parse(mlable2.Text);
                    }

                    if (feild2.Text != null)
                    {
                        myobj.Add(new myobject());
                        myobj[2].label = "YY";
                        myobj[2].text = feild3.Text;
                        myobj[2].mousex = int.Parse(mlable1.Text);
                        myobj[2].mousey = int.Parse(mlable2.Text);
                    }

                    if (dmethod.adddateofbirth(obj, ocrResult, myobj))
                    {
                        MessageBox.Show("date of birth fields are saved");
                        label10.BackColor = Color.LightGreen;
                    }

                }
                else
                    MessageBox.Show("plz fill the fields 1st");



            }
            #endregion
            #region saving designation
            if (button9.Text == "save designation")
            {
                if (feild1.Text != null)
                {
                    myobject1.text = feild1.Text;
                    myobject1.label = "designation";
                    myobject1.mousex = int.Parse(mlable1.Text);
                    myobject1.mousey = int.Parse(mlable2.Text);

                    if (dmethod.adddesignation(obj,ocrResult,myobject1))
                    {
                        MessageBox.Show("designation fields are saved");
                        label9.BackColor = Color.LightGreen;
                    }

                }
                else
                    MessageBox.Show("plz fill the fields 1st");
            }
           

            #endregion
            #region saving website
            if (button9.Text == "save website")
            {
                if (feild1.Text != null)
                {
                    myobject1.text = feild1.Text;
                    myobject1.label = "website";
                    myobject1.mousex = int.Parse(mlable1.Text);
                    myobject1.mousey = int.Parse(mlable2.Text);
                    myobject1.searchtext = lineTextBox.Text;
                    if (dmethod.addwebsite(obj, ocrResult, myobject1))
                    {
                        MessageBox.Show("website fields are saved");
                        label8.BackColor = Color.LightGreen;
                    }

                }
                else
                    MessageBox.Show("plz fill the fields 1st");

            }

            #endregion
            #region saving buisness types
            if (button9.Text == "save buisness typ")
            {
                if (feild1.Text != null)
                {
                    myobject1.text = feild1.Text;
                    myobject1.label = "buisness typ";
                    myobject1.mousex = int.Parse(mlable1.Text);
                    myobject1.mousey = int.Parse(mlable2.Text);
                    if (dmethod.addbusiness(obj, ocrResult, myobject1))
                    {
                        MessageBox.Show("buissness typ fields are saved");
                        label7.BackColor = Color.LightGreen;
                    }

                }
                else
                    MessageBox.Show("plz fill the fields 1st");
            }
           
            #endregion
            #region saving others
            if (button9.Text == "save others")
            {
                List<TextBox> t = new List<TextBox>() { feild1, feild2, feild3, feild4, feild5, feild6, feild7,feild8,feild9 };
                foreach (var t1 in t)
                {
                    if (t1.Text != "")
                    {
                        myobject1.text = t1.Text;
                        myobject1.label = "others";
                        myobject1.mousex = int.Parse(mlable1.Text);
                        myobject1.mousey = int.Parse(mlable2.Text);
                        if (dmethod.addother(obj, ocrResult, myobject1))
                        {
                           // MessageBox.Show("other fields are saved");
                            t1.BackColor = Color.LightGreen;
                        }

                    }
                    
                }
            }


            #endregion

            drawrectangle();
            
        }

        private void saveButton_Click(object sender, EventArgs e)
        {


            if (Pointscheck().Count() != 0)
            {
                drawingisneeded = true;
                identifier = Pointscheck()[0];
                button_drawing.Visible = true;
                MessageBox.Show(" you need to draw a box around  " + obj.shapes[identifier].text_field);
            }

            else if (Pointscheck().Count() == 0)
            {

                for (int i = 0; i < obj.shapes.Count; i++)
                {
                    obj.shapes[i].flags = new Flags2();

                    if (obj.shapes[i].text_field == "")
                    {

                        obj.shapes.RemoveAt(i);
                    }



                    else if (obj.shapes[i].points.Count() > 4)
                    {

                        obj.shapes[i].flags.Curved = true;
                        obj.shapes[i].shape_type = "polygon";
                    }

                    else if(obj.shapes[i].points.Count()==4)
                    {
                        obj.shapes[i].flags.Truncated = false;
                        double width = obj.shapes[i].points[0][0] - obj.shapes[i].points[3][0];
                        double hight = obj.shapes[i].points[0][1] - obj.shapes[i].points[3][1];
                        if (hight / width > .57735)
                        {

                            obj.shapes[i].flags.Slanted = true;

                        }
                                           
                    }
                   
                }


                var json = new JavaScriptSerializer().Serialize(obj);
                Byte[] jsonbyt = new UTF8Encoding(true).GetBytes(json);
                string[] s1 = imagename.Split('.');
                if (File.Exists(s1[0] + ".json"))
                {
                    File.Delete(s1[0] + ".json");
                }
                FileStream file = File.Create(s1[0] + ".json");
                file.Write(jsonbyt, 0, json.Length);
                file.Close();
                MessageBox.Show("file was saved sucessfully");
            }



          
            

            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if(button10.Text=="change rectangles")
            {
                button10.Text = "Done";
            }
            else if(button10.Text == "Done")
            {
                button10.Text = "change rectangles";
            }
           
           
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {           
            
            if(e.Button == MouseButtons.Right)
            {
                drawingisneeded = false;
                if (obj.shapes != null)
                {
                    for (int i = 0; i < obj.shapes.Count(); i++)
                    {
                        for (int k = 0; k < obj.shapes[i].points.Count(); k++)
                        {
                            Point P = PointToScreen(new Point(pictureBox.Bounds.Left, pictureBox.Bounds.Top));
                            int x = MousePosition.X - P.X;
                            int y = MousePosition.Y - P.Y;
                            Point p1 = new Point(x, y);
                            Point p2 = new Point((int)obj.shapes[i].points[k][0], (int)obj.shapes[i].points[k][1]);
                            double distance = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
                            if (distance < 25)
                            {
                                obj.shapes[i].points.Clear();
                                drawrectangle();
                            }
                        }

                    }
                }        
            }

          

            if (obj.shapes != null)
            {
                for (int i = 0; i < obj.shapes.Count(); i++)
                {
                    for (int k = 0; k < obj.shapes[i].points.Count(); k++)
                    {
                        Point P = PointToScreen(new Point(pictureBox.Bounds.Left, pictureBox.Bounds.Top));
                        int x = MousePosition.X - P.X;
                        int y = MousePosition.Y - P.Y;
                        Point p1 = new Point(x, y);
                        Point p2 = new Point((int)obj.shapes[i].points[k][0], (int)obj.shapes[i].points[k][1]);
                       double distance = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
                        if(distance<25)
                        {
                            draging = true;
                            filling = false;
                            a = i;
                            b = k;
                          
                        }
                    }

                }
            



            }
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            drawrectangle();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point P = PointToScreen(new Point(pictureBox.Bounds.Left, pictureBox.Bounds.Top));
            int x = MousePosition.X - P.X;
            int y = MousePosition.Y - P.Y;
            if (draging==true)
            {
                Point p1 = new Point(x, y);
                obj.shapes[a].points[b][0] = p1.X;
                obj.shapes[a].points[b][1] = p1.Y;
                this.Refresh();
                drawrectangle();

            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            draging = false;
            filling = true;

        }

        private void lineTextBox_TextChanged(object sender, EventArgs e)
        {

        }


        public List<int> Pointscheck()
        {
            var v1 = new List<int>();
            if (obj.shapes != null)
            {
                for (int i = 0; i < obj.shapes.Count; i++)
                {
                  if(obj.shapes[i].points.Count()<4)
                    {
                        v1.Add(i);

                    }

                }

            }
            return v1;
        }

        private void button_drawing_Click(object sender, EventArgs e)
        {
            drawingisneeded = false;
            button_drawing.Visible = false;
        }

        private void lblInfo_Click(object sender, EventArgs e)
        {

        }

        private void feild1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblfield6_Click(object sender, EventArgs e)
        {

        }

        private void lblfield8_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void phone_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblfield4_Click(object sender, EventArgs e)
        {

        }

        private void lblfield3_Click(object sender, EventArgs e)
        {

        }

        private void lblfield2_Click(object sender, EventArgs e)
        {

        }

       

        private void button3_Click_1(object sender, EventArgs e)
        {
            arranger();
            pointsallign();
        }


        private void arranger()
        {
            var p = new List<List<double>>();

            foreach (Shape s1 in obj.shapes)
            {
                p.Add(gettoplft(s1));
                p.Add(getbotlft(s1));
                p.Add(getbotright(s1));
                p.Add(gettopright(s1));

                s1.points.Clear();
                s1.points.AddRange(p);
                p.Clear();



            }




        }

        private List<double> gettopright(Shape s1)
        {
            double max1 = -10, max2 = -10;
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < s1.points.Count(); i++)
            {
                if (s1.points[i][0] > max1)
                { index2 = index1; max2 = max1; index1 = i; max1 = s1.points[i][0]; }

                else if (s1.points[i][0] > max2)
                { index2 = i; max2 = s1.points[i][0]; }
            }

            if (s1.points[index1][1] < s1.points[index2][1])
            { return s1.points[index1]; }
            else
                return s1.points[index2];


        }

        private List<double> getbotright(Shape s1)
        {
            double max1 = -10, max2 = -10;
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < s1.points.Count(); i++)
            {
                if (s1.points[i][0] > max1)
                { index2 = index1; max2 = max1; index1 = i; max1 = s1.points[i][0]; }
                else if (s1.points[i][0] > max2)
                { index2 = i; max2 = s1.points[i][0]; }
            }

            if (s1.points[index1][1] > s1.points[index2][1])
            { return s1.points[index1]; }
            else
                return s1.points[index2];

        }

        private List<double> getbotlft(Shape s1)
        {

            double max1 = 2000, max2 = 2000;
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < s1.points.Count(); i++)
            {
                if (s1.points[i][0] < max1)
                { index2 = index1; max2 = max1; index1 = i; max1 = s1.points[i][0]; }
                else if (s1.points[i][0] < max2)
                { index2 = i; max2 = s1.points[i][0]; }
            }

            if (s1.points[index1][1] > s1.points[index2][1])
            { return s1.points[index1]; }
            else
                return s1.points[index2];

        }

        private List<double> gettoplft(Shape s1)
        {
            double max1 = 2000, max2 = 11110;
            int index1 = 0;
            int index2 = 0;
            for (int i = 0; i < s1.points.Count(); i++)
            {
                if (s1.points[i][0] < max1)
                { index2 = index1; max2 = max1; index1 = i; max1 = s1.points[i][0]; }
                else if (s1.points[i][0] < max2)
                { index2 = i; max2 = s1.points[i][0]; }
            }

            if (s1.points[index1][1] < s1.points[index2][1])
            { return s1.points[index1]; }
            else
                return s1.points[index2];
        }

        private void pointsallign()
        {

            foreach (Shape s1 in obj.shapes)
            {
                foreach (Shape s2 in obj.shapes)
                {
                    if (s1 != s2)
                    {
                        double distance = (s2.points[3][0] - s1.points[0][0]) * (s2.points[3][0] - s1.points[0][0]) + (s2.points[3][1] - s1.points[0][1]) * (s2.points[3][1] - s1.points[0][1]);
                        if (distance < 900 && distance > 4)
                        {
                            double dist = s2.points[3][0] - s1.points[0][0];
                            s2.points[3][0] = s2.points[3][0] - (dist / 2) + dist / 5;
                            s1.points[0][0] = s1.points[0][0] + (dist / 2) - dist / 5;
                            // s2.points[1][0] = s2.points[1][0] + dist / 2-1;
                            // s1.points[2][0] = s1.points[2][0] - dist / 2+1;
                        }

                        double distance2 = (s2.points[1][0] - s1.points[2][0]) * (s2.points[1][0] - s1.points[2][0]) + (s2.points[1][1] - s1.points[2][1]) * (s2.points[1][1] - s1.points[2][1]);
                        if (distance2 < 900 && distance > 4)
                        {
                            double dist = s2.points[1][0] - s1.points[2][0];
                            s2.points[1][0] = s2.points[1][0] - (dist / 2) + dist / 5;
                            s1.points[2][0] = s1.points[2][0] + (dist / 2) - dist / 5;
                        }




                    }


                }


            }




        }
    }



       
}



 