﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Security.Permissions;

using javax.microedition.lcdui;



namespace CellGameEdit.PM
{


    [Serializable]
    public partial class ImagesForm : Form, ISerializable , IEditForm
    {
        public static System.Drawing.Color ColorKey = System.Drawing.Color.FromArgb(0xff,0xff,0xff,0);
        public static System.Drawing.Color ColorTileID = System.Drawing.Color.FromArgb(0xff, 0, 0xff, 0);

        public String id;

        int CellW = 16;
        int CellH = 16;
        public ArrayList dstImages;
        public ArrayList dstDataKeys;

        //[NoneSerializable]
        javax.microedition.lcdui.Image srcImage;
        System.Drawing.Rectangle srcRect;
        Boolean srcRectIsDown = false;
        int srcPX;
        int srcPY;
        int srcQX;
        int srcQY;
        int srcSize = 1;


        System.Drawing.Color backColor = System.Drawing.Color.Magenta;
        System.Drawing.Color keyColor = System.Drawing.Color.MediumBlue;

        private Boolean is_change_image = false;
        ArrayList outStreamLen = null;

        public ImagesForm(String name)
        {
            InitializeComponent();

            id = name; this.Text = id;
            CellW = 16;
            CellH = 16;
            dstImages = new ArrayList();
            dstDataKeys = new ArrayList();
            toolStripTextBox1.Text = CellW.ToString();
            toolStripTextBox2.Text = CellH.ToString();

            // src
            pictureBox1.Width = 1;
            pictureBox1.Height = 1;
            pictureBox1.Image = new System.Drawing.Bitmap(1,1);
            srcImage = Image.createImage(pictureBox1.Width, pictureBox1.Height);
            

            srcRectIsDown = false;
            srcRect = new System.Drawing.Rectangle(0, 0, 1, 1);

           
            //
            pictureBox2.Width = 1;
            pictureBox2.Height = 1;
            
            dstSelected = null;
            
            dstRect = new System.Drawing.Rectangle(0, 0, 1, 1);

            is_change_image = true;
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected ImagesForm(SerializationInfo info, StreamingContext context)
        {
            try
            {
                InitializeComponent();
                pictureBox2.Width = 1;
                pictureBox2.Height = 1;
                // load start
                id = (String)info.GetValue("id", typeof(String));
                this.Text = id;
                CellW = (int)info.GetValue("CellW", typeof(int));
                CellH = (int)info.GetValue("CellH", typeof(int));
                ArrayList output = (ArrayList)info.GetValue("output", typeof(ArrayList));
                ArrayList outX = (ArrayList)info.GetValue("outX", typeof(ArrayList));
                ArrayList outY = (ArrayList)info.GetValue("outY", typeof(ArrayList));
                ArrayList outK;
                

                try
                {
                    outK = (ArrayList)info.GetValue("outK", typeof(ArrayList));
                }
                catch (Exception err) 
                { 
                    outK = new ArrayList();
                    for (int i = 0; i < output.Count; i++)
                    {
                        outK.Add(false);
                    }
                }

                try
                {
                    dstDataKeys = (ArrayList)info.GetValue("dstDataKeys", typeof(ArrayList));
                }
                catch (Exception err)
                {
                    dstDataKeys = new ArrayList();
                    for (int i = 0; i < output.Count; i++)
                    {
                        dstDataKeys.Add("");
                    }
                }


                try
                {
                    outStreamLen = (ArrayList)info.GetValue("outStreamLen", typeof(ArrayList));
                }
                catch (Exception err) { }

                System.IO.FileStream images_fs = null;

                if (outStreamLen != null)
                {

                    String dir = "\\tiles\\" + this.id;

                    images_fs = new System.IO.FileStream(
                        ProjectForm.workSpace + dir + ".tiles",
                        System.IO.FileMode.Open);
                }


                dstImages = new ArrayList();

                for (int i = 0; i < output.Count; i++)
                {
                    try
                    {
                        String name = (String)output[i];
                        int x = (int)outX[i];
                        int y = (int)outY[i];
                        Boolean kill = (Boolean)outK[i];
                        int len = -1;
                        if (outStreamLen != null)
                        {
                            len = (int)outStreamLen[i];
                        }


                        Image img;

                        System.IO.MemoryStream ms = null;
                        if (len < 0)
                        {
                            ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(ProjectForm.workSpace + name));
                        }
                        else 
                        {
                            byte[] data = new byte[len];
                            images_fs.Read(data, 0, len);
                            ms = new System.IO.MemoryStream(data);
                        }
                        System.Drawing.Image dimg = System.Drawing.Image.FromStream(ms);
                        ms.Close();

                        img = new Image(dimg);
                        img.x = x;
                        img.y = y;
                        img.killed = kill;

                        if (!img.killed)
                        {
                            pictureBox2.Width = Math.Max(pictureBox2.Width, img.x + img.getWidth());
                            pictureBox2.Height = Math.Max(pictureBox2.Height, img.y + img.getHeight() );
                        }

                        dstImages.Add(img);
                    }
                    catch (Exception err)
                    {
                        dstImages.Add(null);
                        Console.WriteLine(this.id + " : Tile["+i+"] : " + err.StackTrace + "  at  " +err.Message);
                    }

                    
                 
                }

                if (images_fs != null)
                {
                    images_fs.Close();
                }
                // load end

                toolStripTextBox1.Text = CellW.ToString();
                toolStripTextBox2.Text = CellH.ToString();

                // src
                pictureBox1.Width = 1;
                pictureBox1.Height = 1;
                pictureBox1.Image = new System.Drawing.Bitmap(1, 1);
                srcImage = Image.createImage(pictureBox1.Width, pictureBox1.Height);

                srcRectIsDown = false;
                srcRect = new System.Drawing.Rectangle(0, 0, 1, 1);


                //
                //pictureBox2.Width = 1;
                //pictureBox2.Height = 1;

                dstSelected = null;

                dstRect = new System.Drawing.Rectangle(0, 0, 1, 1);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.StackTrace + "  at  " +err.Message);
            }

            is_change_image = false;
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Console.WriteLine("Serializ Images : " + id);

            try{
                info.AddValue("id",id);
                info.AddValue("CellW", CellW);
                info.AddValue("CellH", CellH);

                ArrayList output = new ArrayList();
                ArrayList outX = new ArrayList();
                ArrayList outY = new ArrayList();
                ArrayList outK = new ArrayList();
                

                String dir =  "\\tiles\\" + this.id;
                System.IO.Directory.CreateDirectory(ProjectForm.workSpace + "\\tiles");
                //System.IO.Directory.CreateDirectory(ProjectForm.workSpace + dir);

                // first put all image to stream
                if (is_change_image)
                {
                    outStreamLen = new ArrayList();

                    System.IO.FileStream fs = new System.IO.FileStream(
                                 ProjectForm.workSpace + dir + ".tiles",
                                 System.IO.FileMode.Create);

                    for (int i = 0; i < dstImages.Count; i++)
                    {
                        try
                        {
                            String name = dir + "\\" + i.ToString() + ".tile";

                            Image img = getDstImage(i);

                            if (img != null)
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                                System.Drawing.Image dimage = img.getDImage();
                                dimage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                                byte[] data = ms.ToArray();
                                ms.Close();
                                fs.Write(data, 0, data.Length);
                                fs.Flush();

                                outStreamLen.Add(data.Length);
                            }
                            else
                            {
                                outStreamLen.Add(-1);
                            }
                        }
                        catch (Exception err)
                        {
                            outStreamLen.Add(-1);

                            Console.WriteLine(this.id + " : " + i + " : " + err.StackTrace + "  at  " + err.Message);
                        }
                    }

                    fs.Close();

                }

                // put image property
                for (int i = 0; i < dstImages.Count; i++)
                {
                    try
                    {
                        Image img = getDstImage(i);

                        String name = dir + "\\" + i.ToString() + ".tile";

                        if (img != null)
                        {
                            output.Add(name);
                            outX.Add(img.x);
                            outY.Add(img.y);
                            outK.Add(img.killed);
                        }
                        else
                        {
                            output.Add("");
                            outX.Add(0);
                            outY.Add(0);
                            outK.Add(false);
                        }
                    }
                    catch (Exception err)
                    {
                        output.Add("");
                        outX.Add(0);
                        outY.Add(0);
                        outK.Add(false);

                        Console.WriteLine(this.id + " : " + i + " : " + err.StackTrace + "  at  " + err.Message);
                    }
                }

                info.AddValue("output", output);
                info.AddValue("outX", outX);
                info.AddValue("outY", outY);
                info.AddValue("outK", outK);

                info.AddValue("outStreamLen", outStreamLen);

                info.AddValue("dstDataKeys", dstDataKeys);

                is_change_image = false;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.StackTrace + "  at  " +err.Message);
            }
        }

        public void DelAllImages()
        {
           
        }

       

        public void outputAllImages(String dir,String type,Boolean tile,Boolean group)
        {
            try
            {
                System.Drawing.Imaging.EncoderParameters param = null;
                System.Drawing.Imaging.ImageCodecInfo info = null;
                System.Drawing.Imaging.ImageFormat format = null;
                //
                if (type.Equals("png", StringComparison.CurrentCultureIgnoreCase))
                    format = System.Drawing.Imaging.ImageFormat.Png;
                if (type.Equals("bmp", StringComparison.CurrentCultureIgnoreCase))
                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                if (type.Equals("jpg", StringComparison.CurrentCultureIgnoreCase))
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                if (type.Equals("gif", StringComparison.CurrentCultureIgnoreCase))
                    format = System.Drawing.Imaging.ImageFormat.Gif;

                //info.
                
                if (format == null)return;
                        
                try
                {
                    if (group)
                    {
                        int outW = 0;
                        int outH = 0;
                        for (int i = 0; i < getDstImageCount(); i++)
                        {
                            if (getDstImage(i) != null && !getDstImage(i).killed)
                            {
                                outW = Math.Max(outW, getDstImage(i).x + getDstImage(i).getWidth());
                                outH = Math.Max(outH, getDstImage(i).y + getDstImage(i).getHeight());
                            }
                        }

                        Image outputImage = Image.createImage(outW, outH);
                        Graphics g = outputImage.getGraphics();
                        for (int i = 0; i < getDstImageCount(); i++)
                        {
                            if (getDstImage(i) == null || getDstImage(i).killed) continue;
                            g.drawImage(getDstImage(i), getDstImage(i).x, getDstImage(i).y, 0);
                            
                        }
                        //
                        outputImage.getDImage().Save(dir + "\\" + this.id + "." + type, format);
                    }
                }
                catch (Exception err) { Console.WriteLine(this.id + " : save group : " + err.StackTrace + "  at  " +err.Message); }
                if (tile)
                {
                    String tileDir = dir + "\\" + this.id + "\\" ;
                    if (!System.IO.Directory.Exists(tileDir))
                    {
                        System.IO.Directory.CreateDirectory(tileDir);
                    }
                    for (int i = 0; i < getDstImageCount(); i++)
                    {
                        if (getDstImage(i) == null || getDstImage(i).killed) continue;
                        try
                        {
                            getDstImage(i).getDImage().Save(tileDir + i + "." + type, format);

                        }
                        catch (Exception err) { Console.WriteLine(this.id + " : save tile : " + err.StackTrace + "  at  " +err.Message); }
                    }
                }
            }
            catch (Exception err){Console.WriteLine(this.id + " : " + err.StackTrace + "  at  " +err.Message);}
            
        }

//        public void Output(System.IO.StringWriter sw)
//        {
//            String head = "/" + this.id+"/tile_";

//            sw.WriteLine("final static public void buildImages_" + this.id + "(IImages stuff)");
//            sw.WriteLine("{");
//            sw.WriteLine("    stuff.buildImages(null, " + this.getDstImageCount() + ");");
//            sw.WriteLine("    for(int i=0;i<" + this.getDstImageCount() + ";i++){");
//            sw.WriteLine("        stuff.setTileImage(CIO.loadImage(\"" + head + "\"+i+\".png" + "\"));");
//            sw.WriteLine("        stuff.addTile();");
//            sw.WriteLine("    }");
//            sw.WriteLine("    stuff.gc();");
//            sw.WriteLine("}");

//            sw.WriteLine("final static public void buildClipImages_" + this.id + "(IImages stuff)");
//            sw.WriteLine("{");
//            sw.WriteLine("    stuff.buildImages(CIO.loadImage(\"/" + this.id + ".png\"), " + this.getDstImageCount() + ");");
//for (int i = 0; i < getDstImageCount(); i++){if (getDstImage(i) != null){//
//            sw.WriteLine("    stuff.addTile(" + getDstImage(i).x + "," + getDstImage(i).y + "," + getDstImage(i).getWidth() + "," + getDstImage(i).getHeight() + ");");
//}}//
//            sw.WriteLine("    stuff.gc();");
//            sw.WriteLine("}");

//            SaveAllImages(ProjectForm.workSpace,"png",false,true);

//        }

        public void OutputCustom(int index, String script, System.IO.StringWriter output, String outDir,
            String imageType, bool imageTile, bool imageTileData, bool imageGroup, bool imageGroupData)
        {
            lock (this)
            {
                try
                {
                    String images = Util.getFullTrunkScript(script, "#<IMAGES>", "#<END IMAGES>");

                    Boolean isIgnoreNullTile = false;
                    try
                    {
                        isIgnoreNullTile = Util.getCommandScript(images, "<IGNORE NULL CLIP>").Equals("true", StringComparison.CurrentCultureIgnoreCase);
                    }
                    catch (Exception err) { }

                    bool fix = false;
                    do
                    {
                        String[] clips = new string[getDstImageCount()];
                        for (int i = 0; i < getDstImageCount(); i++)
                        {
                            if (getDstImage(i) != null && getDstImage(i).killed==false)
                            {
                                string X = getDstImage(i).killed ? "0" : getDstImage(i).x.ToString();
                                string Y = getDstImage(i).killed ? "0" : getDstImage(i).y.ToString();
                                string W = getDstImage(i).killed ? "0" : getDstImage(i).getWidth().ToString();
                                string H = getDstImage(i).killed ? "0" : getDstImage(i).getHeight().ToString();
                                string DATA = getDstImage(i).killed ? "" : (String)dstDataKeys[i];
                                clips[i] = Util.replaceKeywordsScript(images, "#<CLIP>", "#<END CLIP>",
                                    new string[] { "<INDEX>", "<X>", "<Y>", "<W>", "<H>" ,"<DATA>"},
                                    new string[] { i.ToString(), X, Y, W, H ,DATA}
                                    );
                            }
                            else 
                            {
                                if (isIgnoreNullTile == false)
                                {
                                    clips[i] = Util.replaceKeywordsScript(images, "#<CLIP>", "#<END CLIP>",
                                          new string[] { "<INDEX>", "<X>", "<Y>", "<W>", "<H>", "<DATA>" },
                                          new string[] { i.ToString(), "0", "0", "0", "0", "" }
                                          );
                                }
                                else
                                {
                                    clips[i] = "";
                                    Console.WriteLine("Ignore null clip : " + i);
                                }
                                
                            }
                        }
                        string temp = Util.replaceSubTrunksScript(images, "#<CLIP>", "#<END CLIP>", clips);

                        if (temp == null)
                        {
                            fix = false;
                        }
                        else
                        {
                            fix = true;
                            images = temp;
                        }

                    } while (fix);


                    images = Util.replaceKeywordsScript(images, "#<IMAGES>", "#<END IMAGES>",
                        new string[] { "<NAME>", "<IMAGES INDEX>", "<COUNT>" },
                        new string[] { this.id, index.ToString(), this.getDstImageCount().ToString() }
                        );

                    output.WriteLine(images);
                    //Console.WriteLine(images);

                    outputAllImages(outDir, imageType, imageTile, imageGroup);

                }
                catch (Exception err) { Console.WriteLine(this.id + " : " + err.StackTrace + "  at  " + err.Message); }
            }
        }


        private void ImagesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
	        {
                e.Cancel = true;
                this.Hide();
	        }
        }
        private void ImagesForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        public String getID()
        {
            return id;
        }

        public Form getForm()
        {
            return this;
        }

        public void setSrcImage()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image buf = Image.createImage(openFileDialog1.FileName);

                srcImage = Image.createImage(buf.getWidth(), buf.getHeight());
                srcImage.getGraphics().drawImage(buf,0,0,0);

                buf.dimg.Dispose();
                buf = null;

                pictureBox1.Width = srcImage.getWidth() * srcSize;
                pictureBox1.Height = srcImage.getHeight() * srcSize;

                toolStripButton2.Checked = false;
                toolStripButton3.Checked = false;

                toolStripStatusLabel3.Text =
                  " : 图片宽=" + srcImage.getWidth() +
                  " : 图片高=" + srcImage.getHeight()
                ;
            }
         
        }


        public void addDst(Image img)
        {
            is_change_image = true;

            for (int i = 0; i < dstImages.Count; i++)
            {
                if (dstImages[i] == null || getDstImage(i).killed)
                {
                    dstImages[i] = img;
                    try
                    {
                        // save to src pic
                        String name = "\\tiles\\" + this.id + "\\" + i.ToString() + ".tile";
                        if (System.IO.File.Exists(ProjectForm.workSpace + name))
                        {
                            System.IO.File.Delete(ProjectForm.workSpace + name);
                        }
                        //getDstImage(i).dimg.Save(ProjectForm.workSpace + name, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception err) { }
                    return;
                }
            }
            dstImages.Add(img);
            dstDataKeys.Add("");

        }

        public void addDirImages()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                for (int i = 0; i < openFileDialog1.FileNames.Length;i++ )
                {
                    try
                    {
                        Image img = Image.createImage(openFileDialog1.FileNames[i]);
                        img.x = 0;
                        img.y = pictureBox2.Height / dstSize;
                        addDst(img);

                        pictureBox2.Width = Math.Max(pictureBox2.Width, img.getWidth() * dstSize);
                        pictureBox2.Height += (img.getHeight() * dstSize);
                    }catch(Exception err){
                        Console.WriteLine(err.StackTrace + "  at  " +err.Message);
                    }
                }
            }
        }

        public void addDstImage()
        {
            if (srcImage == null)
            {
                System.Windows.Forms.MessageBox.Show("设置原始图片先");
            }
            else
            {
                if (srcRect.Width != 0 && srcRect.Height != 0)
                {
                    Image img = Image.createImage(
                        srcImage,
                        srcRect.X,
                        srcRect.Y,
                        srcRect.Width,
                        srcRect.Height,
                        0);
                    img.x = 0;
                    img.y = pictureBox2.Height / dstSize;
                    addDst(img);

                    pictureBox2.Width = Math.Max(pictureBox2.Width, img.getWidth() * dstSize);
                    pictureBox2.Height += (img.getHeight() * dstSize);
                }
            }
        }
        
        public void addDstImages()
        {
            if (srcImage == null)
            {
                System.Windows.Forms.MessageBox.Show("设置原始图片先");
            }
            else
            {
                if (srcRect.Width != 0 && srcRect.Height != 0)
                {
                    for (int y = 0; y < srcRect.Height / CellH; y++)
                    {
                        for (int x = 0; x < srcRect.Width / CellW; x++)
                        {

                            if (CellW != 0 && CellH != 0)
                            {
                                Image img = Image.createImage(
                                    srcImage,
                                    srcRect.X + x * CellW,
                                    srcRect.Y + y * CellH,
                                    CellW,
                                    CellH,
                                    0);
                                img.x = x * (CellW);
                                img.y = y * (CellH) + pictureBox2.Height / dstSize;
                                addDst(img);
                            }

                        }
                    }
                    pictureBox2.Width = Math.Max(pictureBox2.Width, (srcRect.Width / CellW * CellW) ) * dstSize;
                    pictureBox2.Height += ((srcRect.Height / CellH * CellH)) * dstSize;

                }
            }

        }
       
        public Image getSrcImage()
        {
            return srcImage;
        }
       
        public Image getDstImage(int index)
        {
            if (index >= dstImages.Count || index < 0) return null;
            return (((Image)(dstImages[index])));
        }

        public int getDstImageCount()
        {
            return dstImages.Count;
        }

      
        public bool moveDstImage(int index,int dx,int dy)
        {
            if (getDstImage(index)==null) return false;
           
            getDstImage(index).x += dx;
            getDstImage(index).y += dy;

            if (pictureBox2.Width < getDstImage(index).x * dstSize + getDstImage(index).getWidth() * dstSize) pictureBox2.Width += dx * dstSize;
            if (pictureBox2.Height < getDstImage(index).y * dstSize + getDstImage(index).getHeight() * dstSize) pictureBox2.Height += dy * dstSize;

            System.Drawing.Rectangle src = new System.Drawing.Rectangle(
                getDstImage(index).x,
                getDstImage(index).y,
                getDstImage(index).getWidth(),
                getDstImage(index).getHeight()
                );

            System.Drawing.Rectangle scope = new System.Drawing.Rectangle(
                0, 0,
                pictureBox2.Width / dstSize,
                pictureBox2.Height / dstSize
                );

            if (scope.Contains(src) == false)
            {
                getDstImage(index).x -= dx;
                getDstImage(index).y -= dy;
                return false;
            }
         

            for (int i = 0; i < getDstImageCount(); i++)
            {
                if (getDstImage(i) != null && getDstImage(i).killed==false && i!=index )
                {
                    System.Drawing.Rectangle dst = new System.Drawing.Rectangle(
                       getDstImage(i).x,
                       getDstImage(i).y,
                       getDstImage(i).getWidth(),
                       getDstImage(i).getHeight()
                       );

                    if (src.IntersectsWith(dst))
                    {
                        getDstImage(index).x -= dx;
                        getDstImage(index).y -= dy;
                        return false;
                    }
                    
                }
            }

            return true;
        }

        public bool moveDstImages(int dx, int dy)
        {
            if (dx == 0 && dy == 0) return true;

            System.Drawing.Rectangle src = new System.Drawing.Rectangle();
            System.Drawing.Rectangle dst = new System.Drawing.Rectangle();

            for (int i = 0; i < getDstImageCount(); i++)
            {
                if (getDstImage(i) != null && getDstImage(i).killed == false && getDstImage(i).selected)
                {
                    src.X = getDstImage(i).x + dx;
                    src.Y = getDstImage(i).y + dy;
                    src.Width = getDstImage(i).getWidth();
                    src.Height = getDstImage(i).getHeight();

                    if (src.X < 0 || src.Y < 0)
                    {
                        return false;
                    }
                    for (int j = 0; j < getDstImageCount(); j++)
                    {
                        if (i != j && getDstImage(j) != null && getDstImage(j).killed == false && getDstImage(j).selected == false)
                        {
                            dst.X = getDstImage(j).x;
                            dst.Y = getDstImage(j).y;
                            dst.Width = getDstImage(j).getWidth();
                            dst.Height = getDstImage(j).getHeight();

                            if (src.IntersectsWith(dst))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < getDstImageCount(); i++)
            {
                if (getDstImage(i) != null && getDstImage(i).killed == false && getDstImage(i).selected)
                {
                    getDstImage(i).x += dx;
                    getDstImage(i).y += dy;

                    if (pictureBox2.Width < getDstImage(i).x * dstSize + getDstImage(i).getWidth() * dstSize) pictureBox2.Width += dx * dstSize;
                    if (pictureBox2.Height < getDstImage(i).y * dstSize + getDstImage(i).getHeight() * dstSize) pictureBox2.Height += dy * dstSize;

                }
            }

            return true;
        }

        public void changeDstImage(int index)
        {
            
            if (getDstImage(index) == null) return ;

            is_change_image = true;

            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    int oy = getDstImage(index).y;
                    int ox = getDstImage(index).x;

                    Image changed = Image.createImage(openFileDialog1.FileName);

                    dstImages[index] = changed;

                    // test size
                    bool isSameSize = true;
                    System.Drawing.Rectangle src = new System.Drawing.Rectangle(
                        getDstImage(index).x,
                        getDstImage(index).y,
                        getDstImage(index).getWidth(),
                        getDstImage(index).getHeight()
                        );
                    for (int i = 0; i < getDstImageCount(); i++)
                    {
                        if (getDstImage(i) != null && !getDstImage(i).killed && i != index)
                        {
                            System.Drawing.Rectangle dst = new System.Drawing.Rectangle(
                               getDstImage(i).x,
                               getDstImage(i).y,
                               getDstImage(i).getWidth(),
                               getDstImage(i).getHeight()
                               );

                            if (src.IntersectsWith(dst))
                            {
                                isSameSize = false;
                                break;
                            }

                        }
                    }
                    if (isSameSize)
                    {
                        changed.x = ox;
                        changed.y = oy;
                    }
                    else
                    {
                        changed.x = pictureBox2.Width / dstSize;
                        changed.y = oy;
                    }


                    pictureBox2.Width += changed.getWidth() * dstSize;
                    pictureBox2.Height = Math.Max(pictureBox2.Height, changed.getHeight() * dstSize);

                    // save to src pic
                    String name = "\\tiles\\" + this.id + "\\" + index.ToString() + ".tile";
                    if (System.IO.File.Exists(ProjectForm.workSpace + name))
                    {
                        System.IO.File.Delete(ProjectForm.workSpace + name);
                    }
                    //changed.dimg.Save(ProjectForm.workSpace + name, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception err) {
                Console.WriteLine(this.id + " : " + err.StackTrace + "  at  " +err.Message);
            }
       

        }

        public void changeDstImageFormSrc(int index)
        {

            if (getDstImage(index) == null) return;
            if (srcImage == null) return;

            is_change_image = true;

            try
            {
                if (srcRect.Width != 0 && srcRect.Height != 0)
                {

                    Image changed = Image.createImage(
                        srcImage,
                        srcRect.X,
                        srcRect.Y,
                        srcRect.Width,
                        srcRect.Height,
                        0);

                    int oy = getDstImage(index).y;
                    int ox = getDstImage(index).x;

                    dstImages[index] = changed;

                    // test size
                    bool isSameSize = true;
                    System.Drawing.Rectangle src = new System.Drawing.Rectangle(
                        getDstImage(index).x,
                        getDstImage(index).y,
                        getDstImage(index).getWidth(),
                        getDstImage(index).getHeight()
                        );
                    for (int i = 0; i < getDstImageCount(); i++)
                    {
                        if (getDstImage(i) != null && !getDstImage(i).killed && i != index)
                        {
                            System.Drawing.Rectangle dst = new System.Drawing.Rectangle(
                               getDstImage(i).x,
                               getDstImage(i).y,
                               getDstImage(i).getWidth(),
                               getDstImage(i).getHeight()
                               );

                            if (src.IntersectsWith(dst))
                            {
                                isSameSize = false;
                                break;
                            }

                        }
                    }
                    if (isSameSize)
                    {
                        changed.x = ox;
                        changed.y = oy;
                    }
                    else
                    {
                        changed.x = pictureBox2.Width / dstSize;
                        changed.y = oy;
                    }

                    pictureBox2.Width += changed.getWidth() * dstSize;
                    pictureBox2.Height = Math.Max(pictureBox2.Height, changed.getHeight() * dstSize);

                    // save to src pic
                    String name = "\\tiles\\" + this.id + "\\" + index.ToString() + ".tile";
                    if (System.IO.File.Exists(ProjectForm.workSpace + name))
                    {
                        System.IO.File.Delete(ProjectForm.workSpace + name);
                    }
                    //changed.dimg.Save(ProjectForm.workSpace + name, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(this.id + " : " + err.StackTrace + "  at  " +err.Message);
            }


        }

        public void delDstImage(int index)
        {
            getDstImage(index).killed = true;
        }

        public void renderSrcImage(Graphics g, int x, int y)
        {
            if (srcImage == null) return;
            g.drawImage(srcImage, x, y, 0, 0, srcSize);
        }

        public void renderDstImage(Graphics g, int x, int y)
        {

            for (int i = 0; i < getDstImageCount(); i++)
            {
                if (getDstImage(i) != null && getDstImage(i).killed == false)
                {
                    g.drawImage(getDstImage(i), x + getDstImage(i).x * dstSize, y + getDstImage(i).y * dstSize, 0, 0, dstSize);
                    if (toolStripButton13.Checked)
                    {
                        g.setColor(0x7fffffff);
                        g.drawRect(x + getDstImage(i).x * dstSize, y + getDstImage(i).y * dstSize, getDstImage(i).getWidth() * dstSize, getDstImage(i).getHeight() * dstSize);
                    }
                    if (chkIsShowkey.Checked)
                    {
                        int tx = x + getDstImage(i).x * dstSize;
                        int ty = y + getDstImage(i).y * dstSize;
                        g.setColor(0xff, 0, 0, 0);
                        g.drawString((String)dstDataKeys[i], tx + 1, ty + 1, 0);
                        g.setColor(ImagesForm.ColorKey.ToArgb());
                        g.drawString((String)dstDataKeys[i], tx, ty, 0);
                    }
                    if (chkIsShowTileID.Checked)
                    {
                        int tx = x + getDstImage(i).x * dstSize;
                        int ty = y + (getDstImage(i).y + getDstImage(i).getHeight() - (int)Graphics.font.GetHeight()) * dstSize;
                        g.setColor(0xff, 0, 0, 0);
                        g.drawString(i.ToString(), tx + 1, ty + 1, 0);
                        g.setColor(ImagesForm.ColorTileID.ToArgb());
                        g.drawString(i.ToString(), tx, ty, 0);
                    }
                    if (multiSelect.Checked)
                    {
                        if (getDstImage(i).selected)
                        {
                            g.setColor(0x40ffffff);
                            g.fillRect(x + getDstImage(i).x * dstSize, y + getDstImage(i).y * dstSize, getDstImage(i).getWidth() * dstSize, getDstImage(i).getHeight() * dstSize);
                            g.setColor(0x7fffffff);
                            g.drawRect(x + getDstImage(i).x * dstSize, y + getDstImage(i).y * dstSize, getDstImage(i).getWidth() * dstSize, getDstImage(i).getHeight() * dstSize);
                        }
                    }
                }
            }
        }


        // src edit
        private void toolStripTextBox1_Leave(object sender, EventArgs e)
        {
            if (Cell.Util.stringIsDigit(toolStripTextBox1.Text, 0, toolStripTextBox1.Text.Length) >= toolStripTextBox1.Text.Length &&
                Cell.Util.stringDigitToInt(toolStripTextBox1.Text, 0, toolStripTextBox1.Text.Length) >= 1)
            {
                CellW = Cell.Util.stringDigitToInt(toolStripTextBox1.Text, 0, toolStripTextBox1.Text.Length);
            }
            else
            {
                MessageBox.Show("只能输入大于0的数字！");
                toolStripTextBox1.Focus();
            }
        }
        private void toolStripTextBox2_Leave(object sender, EventArgs e)
        {
            if (Cell.Util.stringIsDigit(toolStripTextBox2.Text, 0, toolStripTextBox2.Text.Length) >= toolStripTextBox2.Text.Length &&
              Cell.Util.stringDigitToInt(toolStripTextBox2.Text, 0, toolStripTextBox2.Text.Length) >= 1)
            {
                CellH = Cell.Util.stringDigitToInt(toolStripTextBox2.Text, 0, toolStripTextBox2.Text.Length);
            }
            else
            {
                MessageBox.Show("只能输入大于0的数字！");
                toolStripTextBox2.Focus();
            }

        }
        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
           setSrcImage();
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            addDirImages();
        }
        private void toolStripButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton2.Checked)
            {
                toolStripButton3.Checked = false;
            }

            srcRect.X = 0;
            srcRect.Width = 0;
            srcRect.Y = 0;
            srcRect.Height = 0;

            srcRectIsDown = false;
            pictureBox1.Refresh();
        }
        private void toolStripButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton3.Checked)
            {
                toolStripButton2.Checked = false;
            }
            srcRect.X = 0;
            srcRect.Width = 0;
            srcRect.Y = 0;
            srcRect.Height = 0;

            srcRectIsDown = false;
            pictureBox1.Refresh();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (toolStripButton2.Checked)
            {
                addDstImage();
            }
            if (toolStripButton3.Checked)
            {
                addDstImages();
            }
            //pictureBox2.Refresh();
        }

        private void btnSrcSelectAll_Click(object sender, EventArgs e)
        {
            srcRect.X = 0;
            srcRect.Y = 0;
            srcRect.Width = srcImage.getWidth();
            srcRect.Height = srcImage.getHeight();
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //MapRegion.Image = new System.Drawing.Bitmap(MapRegion.Width, MapRegion.Height);
            //System.Drawing.Graphics dg = System.Drawing.Graphics.FromImage(MapRegion.Image);
            System.Drawing.Graphics dg = e.Graphics;
            Graphics g = new Graphics(dg);
            renderSrcImage(g, 0, 0);


            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0x80, 0xff, 0xff, 0xff));
            System.Drawing.Brush brush = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0x80, 0x80, 0x80, 0x80)).Brush;


            dg.DrawLine(pen, srcQX * srcSize, 0, srcQX * srcSize, pictureBox1.Height);
            dg.DrawLine(pen, 0, srcQY * srcSize, pictureBox1.Width, srcQY * srcSize);

            pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0xFF, 0, 0, 0));

            if (btnIsShowGrid.Checked)
            {
                g.setColor(0x7fffffff);
                for (int x = 0; x < pictureBox1.Width; x += CellW * srcSize)
                {
                    g.drawLine(x , 0, x , pictureBox1.Height);
                }
                for (int y = 0; y < pictureBox1.Height; y += CellH * srcSize)
                {
                    g.drawLine(0, y , pictureBox1.Width, y );
                }
            }

            if (toolStripButton2.Checked)
            {
                dg.FillRectangle(brush, srcRect.X * srcSize, srcRect.Y * srcSize, (srcRect.Width ) * srcSize, (srcRect.Height ) * srcSize);
                dg.DrawRectangle(pen, srcRect.X * srcSize, srcRect.Y * srcSize, (srcRect.Width ) * srcSize, (srcRect.Height ) * srcSize);
            }
            else if (toolStripButton3.Checked)
            {
                dg.FillRectangle(brush, srcRect.X * srcSize, srcRect.Y * srcSize, (srcRect.Width ) * srcSize, (srcRect.Height ) * srcSize);
                for (int x = srcRect.X; x < srcRect.X + srcRect.Width; x += CellW)
                {
                    dg.DrawLine(System.Drawing.Pens.White, x * srcSize, srcRect.Y * srcSize, x * srcSize, srcRect.Y * srcSize + (srcRect.Height) * srcSize );
                }
                for (int y = srcRect.Y; y < srcRect.Y + srcRect.Height; y += CellH)
                {
                    dg.DrawLine(System.Drawing.Pens.White, srcRect.X * srcSize, y * srcSize, srcRect.X * srcSize + (srcRect.Width ) * srcSize, y * srcSize);
                }
                dg.DrawRectangle(pen, srcRect.X * srcSize, srcRect.Y * srcSize, (srcRect.Width ) * srcSize, (srcRect.Height ) * srcSize);
            }
        }

        
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = "";
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel3.Text =
                        " : X=" + e.X / srcSize +
                        " : Y=" + e.Y / srcSize +
                        " : 图片宽=" + pictureBox1.Width / srcSize +
                        " : 图片高=" + pictureBox1.Height / srcSize 
                    ;
            srcQX = (e.X < 0 ? 0 : (e.X > pictureBox1.Width ? pictureBox1.Width : e.X)) / srcSize;
            srcQY = (e.Y < 0 ? 0 : (e.Y > pictureBox1.Height ? pictureBox1.Height : e.Y)) / srcSize;

            if (toolStripButton2.Checked || toolStripButton3.Checked)
            {
                if (srcRectIsDown)
                {
                    srcRect.X = Math.Min(srcPX, srcQX);
                    srcRect.Width = Math.Abs(srcPX - srcQX);
                    srcRect.Y = Math.Min(srcPY, srcQY);
                    srcRect.Height = Math.Abs(srcPY - srcQY);

                    toolStripStatusLabel3.Text +=
                        " : 选择宽=" + srcRect.Width +
                        " : 选择高=" + srcRect.Height;

                    if (toolStripButton3.Checked)
                    {
                        toolStripStatusLabel3.Text +=
                        " : 横向数量=" + srcRect.Width / CellW +
                        " : 纵向数量=" + srcRect.Height / CellH
                        ;
                    }

                   
                }
            }

            pictureBox1.Refresh();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            srcPX = e.X / srcSize;
            srcPY = e.Y / srcSize;
            srcQX = e.X / srcSize;
            srcQY = e.Y / srcSize;

            if (toolStripButton2.Checked || toolStripButton3.Checked)
            {
                srcRect.X = srcPX;
                srcRect.Width = 0;
                srcRect.Y = srcPY;
                srcRect.Height = 0;
            }


            srcRectIsDown = true;
            pictureBox1.Refresh();


        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

            srcQX = e.X / srcSize;
            srcQY = e.Y / srcSize;

            srcRectIsDown = false;
            pictureBox1.Refresh();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            srcSize += 1;
            if (srcSize > 8) srcSize = 8;

            try
            {
                if (getSrcImage() != null)
                {
                    pictureBox1.Width = getSrcImage().getWidth() * srcSize;
                    pictureBox1.Height = getSrcImage().getHeight() * srcSize;
                    pictureBox1.Refresh();
                }
            }catch(Exception err){}

        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {

            srcSize -= 1;
            if (srcSize < 1) srcSize = 1;

            try
            {
                if (getSrcImage() != null)
                {
                    pictureBox1.Width = getSrcImage().getWidth() * srcSize;
                    pictureBox1.Height = getSrcImage().getHeight() * srcSize;
                    pictureBox1.Refresh();
                }
            }
            catch (Exception err) { }
        }
        // dst edit

        Image dstSelected = null;
        int dstSelectIndex = -1;
        System.Drawing.Rectangle dstRect = new System.Drawing.Rectangle();

        bool dstDown = false;

        int dstPX;
        int dstPY;
        int dstSX;
        int dstSY;

        int dstSize = 1;



        System.Drawing.Rectangle ScopeRect = new System.Drawing.Rectangle();
        Boolean IsScopeSelected = false;
        int ScopePX;
        int ScopePY;

        // scope
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            dstSize += 1;
            if (dstSize > 8) dstSize = 8;

            try
            {
                int outW = 0;
                int outH = 0;
                for (int i = 0; i < getDstImageCount(); i++)
                {
                    if (getDstImage(i) != null && !getDstImage(i).killed)
                    {
                        outW = Math.Max(outW, getDstImage(i).x + getDstImage(i).getWidth());
                        outH = Math.Max(outH, getDstImage(i).y + getDstImage(i).getHeight());
                    }
                }

                pictureBox2.Width = outW * dstSize;
                pictureBox2.Height = outH * dstSize;
                pictureBox2.Refresh();
            }
            catch (Exception err)
            {
            }

        }
        private void toolStripButton11_Click(object sender, EventArgs e)
        {

            dstSize -= 1;
            if (dstSize < 1) dstSize = 1;

            try
            {
                int outW = 0;
                int outH = 0;
                for (int i = 0; i < getDstImageCount(); i++)
                {
                    if (getDstImage(i) != null && !getDstImage(i).killed)
                    {
                        outW = Math.Max(outW, getDstImage(i).x + getDstImage(i).getWidth());
                        outH = Math.Max(outH, getDstImage(i).y + getDstImage(i).getHeight());
                    }
                }

                pictureBox2.Width = outW * dstSize;
                pictureBox2.Height = outH * dstSize;
                pictureBox2.Refresh();
            }
            catch (Exception err)
            {
            }
        }
        // change image
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            try
            {
                changeDstImage(dstSelectIndex);
                dstRect.X = getDstImage(dstSelectIndex).x;
                dstRect.Y = getDstImage(dstSelectIndex).y;
                dstRect.Width = getDstImage(dstSelectIndex).getWidth();
                dstRect.Height = getDstImage(dstSelectIndex).getHeight();
                pictureBox2.Refresh();
            }
            catch (Exception err) { }
        }
        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            try
            {
                changeDstImageFormSrc(dstSelectIndex);
                dstRect.X = getDstImage(dstSelectIndex).x;
                dstRect.Y = getDstImage(dstSelectIndex).y;
                dstRect.Width = getDstImage(dstSelectIndex).getWidth();
                dstRect.Height = getDstImage(dstSelectIndex).getHeight();
                pictureBox2.Refresh();
            }
            catch (Exception err) { }
        }

        //select image
        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = new Graphics(e.Graphics);

            renderDstImage(g, 0, 0);

           

            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            System.Drawing.Brush brush = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0x80, 0xff, 0xff, 0xff)).Brush;

            if (dstRect != null)
            {
                e.Graphics.FillRectangle(brush, dstRect.X * dstSize, dstRect.Y * dstSize, (dstRect.Width) * dstSize, (dstRect.Height) * dstSize);
                e.Graphics.DrawRectangle(pen, dstRect.X * dstSize, dstRect.Y * dstSize, (dstRect.Width) * dstSize, (dstRect.Height) * dstSize);
            }

            if (ScopeRect != null)
            {
                pen.Color = System.Drawing.Color.FromArgb(0xff, 0, 0xff, 0);
                e.Graphics.DrawRectangle(pen, ScopeRect.X * dstSize, ScopeRect.Y * dstSize, (ScopeRect.Width) * dstSize, (ScopeRect.Height) * dstSize);
            }
            
        }
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            dstDown = true;

            if (multiSelect.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    dstRect.X = -1;
                    dstRect.Y = -1;
                    dstRect.Width = 1;
                    dstRect.Height = 1;

                    dstSelected = null;
                    dstSelectIndex = -1;


                    dstSX = e.X / dstSize;
                    dstSY = e.Y / dstSize;
                    dstPX = dstSX;
                    dstPY = dstSY;

                    if (ScopeRect.Contains(dstSX, dstSY))
                    {
                        ScopePX = dstSX - ScopeRect.X;
                        ScopePY = dstSY - ScopeRect.Y;

                        IsScopeSelected = true;
                    }
                    else
                    {
                        IsScopeSelected = false;

                        ScopeRect.X = dstSX;
                        ScopeRect.Y = dstSY;
                        ScopeRect.Width = 1;
                        ScopeRect.Height = 1;

                        System.Drawing.Rectangle dst = new System.Drawing.Rectangle(0, 0, 1, 1);

                        for (int i = 0; i < getDstImageCount(); i++)
                        {
                            if (getDstImage(i) != null)
                            {
                                dst.X = getDstImage(i).x;
                                dst.Y = getDstImage(i).y;
                                dst.Width = getDstImage(i).getWidth();
                                dst.Height = getDstImage(i).getHeight();

                                getDstImage(i).selected = ScopeRect.IntersectsWith(dst);

                            }
                        }
                    }
                }
            }
            else
            {

                ScopeRect.X = -1;
                ScopeRect.Y = -1;
                ScopeRect.Width = 1;
                ScopeRect.Height = 1;

                dstSelected = null;
                dstSelectIndex = -1;

                dstRect.X = -1;
                dstRect.Y = -1;
                dstRect.Width = 1;
                dstRect.Height = 1;

                dstPX = e.X / dstSize;
                dstPY = e.Y / dstSize;

                System.Drawing.Rectangle dst = new System.Drawing.Rectangle();
                for (int i = 0; i < getDstImageCount(); i++)
                {
                    if (getDstImage(i) != null && getDstImage(i).killed == false)
                    {
                        dst.X = getDstImage(i).x;
                        dst.Y = getDstImage(i).y;
                        dst.Width = getDstImage(i).getWidth();
                        dst.Height = getDstImage(i).getHeight();

                        if (dst.Contains(dstPX, dstPY))
                        {
                            dstDown = true;
                            dstSelected = getDstImage(i);
                            dstSelectIndex = i;
                            dstRect = dst;

                            toolStripStatusLabel1.Text =
                                "目标Tile：[" + dstSelectIndex + "]" +
                                " X=" + dstSelected.x +
                                " Y=" + dstSelected.y +
                                " W=" + dstSelected.getWidth() +
                                " H=" + dstSelected.getHeight() +
                                " Key=\"" + ((String)dstDataKeys[dstSelectIndex]) + "\""
                                ;

                            if (e.Button == MouseButtons.Left)
                            {
                            }
                            else if (e.Button == MouseButtons.Right)
                            {
                                clipMenu.Show(pictureBox2, e.X, e.Y);
                            }

                            break;
                        }
                    }
                }
            }

            pictureBox2.Refresh();
            
            
        }
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
           
            dstDown = false;
            dstPX = e.X / dstSize;
            dstPY = e.Y / dstSize;

            pictureBox2.Refresh();
        }
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (dstDown)
            {
                if (multiSelect.Checked)
                {
                    if (IsScopeSelected)
                    {
                        int px = (e.X / dstSize - dstPX);
                        int py = (e.Y / dstSize - dstPY);
                        dstPX = e.X / dstSize;
                        dstPY = e.Y / dstSize;

                        //Console.WriteLine(" px="+px+" py="+py);

                        if (moveDstImages(px, py))
                        {
                            ScopeRect.X += px;
                            ScopeRect.Y += py;
                            //ScopeRect.X = dstPX - ScopePX;
                            //ScopeRect.Y = dstPY - ScopePY;
                        }
                    }
                    else
                    {
                        dstPX = e.X / dstSize;
                        dstPY = e.Y / dstSize;

                        ScopeRect.X = Math.Min(dstSX, dstPX);
                        ScopeRect.Y = Math.Min(dstSY, dstPY);
                        ScopeRect.Width = (dstSX - dstPX == 0) ? 1 : Math.Abs(dstSX - dstPX);
                        ScopeRect.Height = (dstSY - dstPY == 0) ? 1 : Math.Abs(dstSY - dstPY);

                        System.Drawing.Rectangle dst = new System.Drawing.Rectangle(0, 0, 1, 1);

                        for (int i = 0; i < getDstImageCount(); i++)
                        {
                            if (getDstImage(i) != null)
                            {
                                dst.X = getDstImage(i).x;
                                dst.Y = getDstImage(i).y;
                                dst.Width = getDstImage(i).getWidth();
                                dst.Height = getDstImage(i).getHeight();

                                getDstImage(i).selected = ScopeRect.IntersectsWith(dst);
                            }
                        }
                    }
                 
                }
                else
                {
                    if (dstSelectIndex >= 0)
                    {
                        int px = (e.X / dstSize - dstPX);
                        int py = (e.Y / dstSize - dstPY);
                        dstPX = e.X / dstSize;
                        dstPY = e.Y / dstSize;

                        if (moveDstImage(dstSelectIndex, px, py))
                        {
                            dstRect.X += px;
                            dstRect.Y += py;
                        }

                        toolStripStatusLabel1.Text =
                                       "目标Tile：[" + dstSelectIndex + "]" +
                                       " X=" + dstSelected.x +
                                       " Y=" + dstSelected.y +
                                       " W=" + dstSelected.getWidth() +
                                       " H=" + dstSelected.getHeight() +
                                       " Key=\"" + ((String)dstDataKeys[dstSelectIndex]) + "\""

                                       ;
                    }
                }
                pictureBox2.Refresh();
            }
            
        }

        // del image
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("警告", "确认删除？", MessageBoxButtons.OKCancel)== DialogResult.OK)
                {
                    if (multiSelect.Checked)
                    {
                        for (int i = 0; i < getDstImageCount(); i++)
                        {
                            if (getDstImage(i) != null && getDstImage(i).selected)
                            {
                                delDstImage(i);
                            }
                        }
                    }
                    else
                    {
                        delDstImage(dstSelectIndex);
                    }
                    dstRect.X = -1;
                    dstRect.Y = -1;
                    dstRect.Width = 1;
                    dstRect.Height = 1;
                    dstSelectIndex = -1;
                    pictureBox2.Refresh();
                }
            }
            catch (Exception err) { }
        }

        private void 从目录替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton9_Click(null,null);
        }

        private void 从左边替换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton14_Click(null,null);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton12_Click(null,null);
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String name = (String)dstDataKeys[dstSelectIndex];
                TextDialog nameDialog = new TextDialog(name);
                if (nameDialog.ShowDialog() == DialogResult.OK)
                {
                    dstDataKeys[dstSelectIndex] = nameDialog.getText();
                    pictureBox2.Refresh();
                }
            }
            catch (Exception err) { }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------
        
        public MapForm createMapForm(String name)
        {
            if (dstImages.Count < 1)
            {
                MessageBox.Show("Tile容量不能为0！");
                return null;
            }
           
            //for (int i = 0; i < dstImages.Count; i++)
            //{
            //    if (((Image)dstImages[i]) == null || ((Image)dstImages[i]).killed) continue;
            //    if (((Image)dstImages[i]).getWidth() != CellW ||
            //        ((Image)dstImages[i]).getHeight() != CellH)
            //    {
            //        MessageBox.Show("地图的Tile大小必须相等！");
            //        return null;
            //    }
            //}

            MapForm ret = new MapForm(name,CellW, CellH, this);
            return ret;
        }

        public SpriteForm createSpriteForm(String name)
        {
            if (dstImages.Count < 1)
            {
                MessageBox.Show("Tile容量不能为0！");
                return null;
            }
            for (int i = 0; i < dstImages.Count; i++)
            {
                if (((Image)dstImages[i]) == null) continue;
                //if (((Image)dstImages[i]).getWidth() != CellW ||
                //    ((Image)dstImages[i]).getHeight() != CellH)
                //{
                //    MessageBox.Show("地图的Tile大小必须相等！");
                //    return null;
                //}
            }

            SpriteForm ret = new SpriteForm(name,this);
            return ret;
        }

        //change color
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = pictureBox1.BackColor;
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.BackColor = MyDialog.Color;
            }
        }
        private void BtnSelectKeyColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = ColorKey;
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
               ColorKey = MyDialog.Color;
            }
        }
        private void BtnSelectTileIDColor_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = ColorTileID;
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                ColorTileID = MyDialog.Color;
            }
        }

        //
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            pictureBox2.Refresh();
        }

        private void ImagesForm_TextChanged(object sender, EventArgs e)
        {
            this.id = this.Text;
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            pictureBox2.Refresh();
        }
        private void toolStripButton15_Click_1(object sender, EventArgs e)
        {
            pictureBox2.Refresh();
        }
        private void 添加切片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolStripButton2.Checked)
            {
                addDstImage();
            }
            if (toolStripButton3.Checked)
            {
                addDstImages();
            }
        }

        private void 添加多张图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addDirImages();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void 导出图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               // String name = (String)dstDataKeys[dstSelectIndex];
                Image image = (Image)dstImages[dstSelectIndex];
                //TextDialog nameDialog = new TextDialog(name);
                //if (nameDialog.ShowDialog() == DialogResult.OK)
                //{
                //    dstDataKeys[dstSelectIndex] = nameDialog.getText();
                //    pictureBox2.Refresh();
                //}
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".png";
                sfd.AddExtension = true;
                sfd.Filter = "PNG file (*.png)|*.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    image.getDImage().Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception err) { }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private Boolean moveImage(Image srcImage, System.Drawing.Rectangle region, int dx, int dy)
        {
            if (srcImage == null || srcImage.killed) return false;

            if (dx == 0 && dy == 0) return false;


            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(
                srcImage.x+dx, srcImage.y+dy, srcImage.getWidth(), srcImage.getHeight()
                );

            if (!region.Contains(srcRect)) 
            {
                return false;
            }

            for (int i = 0; i < dstImages.Count; i++)
            {
                if (dstImages[i] != null)
                {
                    Image img = (Image)dstImages[i];

                    if (!img.killed && img != srcImage)
                    {
                        System.Drawing.Rectangle dstRect = new System.Drawing.Rectangle(
                             img.x, img.y, img.getWidth(), img.getHeight()
                             );
                        if (srcRect.IntersectsWith(dstRect))
                        {
                            return false;
                        }
                    }
                }
            }

            srcImage.x += dx;
            srcImage.y += dy;

            return true;
        }

        private void moveAllImages(int dx, int dy)
        {
            System.Drawing.Rectangle region = new System.Drawing.Rectangle(0, 0, pictureBox2.Width, pictureBox2.Height);
            while (true)
            {
                Boolean ismoved = false;

                for (int i = 0; i < dstImages.Count; i++)
                {
                    if (moveImage((Image)dstImages[i], region, dx, dy))
                    {
                        ismoved = true;
                    }
                }

                if (!ismoved)
                {
                    break;
                }
            }
        }

        private void btnUpAllImage_Click(object sender, EventArgs e)
        {
            moveAllImages(0, -1);

            pictureBox2.Refresh();
        }

        private void btnLeftAllImage_Click(object sender, EventArgs e)
        {
            moveAllImages(-1, 0);

            pictureBox2.Refresh();
        }




        class ImagesCompareY : IComparer
        {

            public int Compare(Object a, Object b)
            { 
                if (a!=null && b!=null)
                {
                    return ((Image)a).y - ((Image)b).y;
                }
                return -1;
            }
        }






    }
}