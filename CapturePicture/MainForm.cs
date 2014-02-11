/**
 * メインフォーム
 * 
 * FOOTPRINT IT SERVICE CO LTD
 * Roy
 * 2014.02.01
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CapturePicture
{
    /// <summary>
    /// メイン
    /// </summary>
    public partial class MainForm : Form
    {
        // 表示する画像
        private Bitmap currentImage;
        // 倍率変更後の画像のサイズと位置
        private Rectangle drawRectangle;
        // マウスX
        private int mousePosX;
        // マウスY
        private int mousePosY;
        // マウススイッチ
        private bool mouseSwitch = false;
        // クリップボードビューアー
        private MyClipboardViewer viewer;
        // 画像サイズ入力フォーム
        private PictureSize pictureSize;
        // レジストリ
        private RegstoryHandler regHandler;
        // 画像拡大率
        private double pictureZoom =1d;
        // 画像左上座標
        private Point pictureTopLeft = new Point(0,0);
        // 保存先
        private string destination;
        // 自動保存
        private bool autosave;
        // メインフォームの表示位置
        private Point location;
        // 場所移動を記憶させるフラッグ
        private bool allowLocation = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm()  
        {
            InitializeComponent();

            // クリップボードビューア
            viewer = new MyClipboardViewer(this);

            // レジストリハンドラ
            regHandler = new RegstoryHandler();
            pictureZoom = regHandler.PictureZoom;
            pictureTopLeft = regHandler.PictureTopLeft;
            destination = regHandler.Destination;
            autosave = regHandler.Autosave;
            chkAutoSave.Checked = autosave;
            location = regHandler.Location;

            // フォームの大きさを調整する
            this.formResize(regHandler.Size1);

            // イベントハンドラを登録
            viewer.ClipboardHandler += this.OnClipBoardChanged;

            // フォームの位置を調整する
            this.Location = location;

            // フォームの位置を調整してから、LocationChanged イベントをインプリメントする
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);

            // 画像拡大、縮小イイベント実装
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
        }

        /// <summary>
        /// マウスがのったらフォーカス 
        /// フォーカスが無いとマウススクロールでイベントが発生しないから
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        /// <summary>
        /// マウスホイールの操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (currentImage == null)
            {
                return;
            }
            double zoomRatio = (double)e.Delta;
            if (zoomRatio > 0)
            {
                double h = (double)drawRectangle.Height * 1.1;
                drawRectangle.Height = (int)h;
                double w = (double)drawRectangle.Width * 1.1;
                drawRectangle.Width = (int)w;
            }
            else
            {
                double h = (double)drawRectangle.Height / 1.1;
                drawRectangle.Height = (int)h;
                double w = (double)drawRectangle.Width / 1.1;
                drawRectangle.Width = (int)w;
            }
            // 画像を表示する
            pictureBox1.Invalidate();

            pictureZoom = (double)drawRectangle.Width / (double)currentImage.Width;
            regHandler.PictureZoom = pictureZoom;
        }

        /// <summary>
        /// クリップボードにテキストがコピーされると呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnClipBoardChanged(object sender, ClipboardEventArgs args)
        {
            // クリップボードのイメージをビットマップ化する
            currentImage = new Bitmap(args.Img);

            // 初期化
            drawRectangle = new Rectangle(pictureTopLeft.X, pictureTopLeft.Y, (int)(currentImage.Width * pictureZoom), (int)(currentImage.Height * pictureZoom));

            // 画像を表示する
            pictureBox1.Invalidate();

            // 通常サイズのウィンドウを表示する
            this.WindowState = FormWindowState.Normal;

            // フォーカス
            this.Focus();
            this.Activate();

            // ボタンにフォーカス
            this.btnSave.Focus();
        }

        /// <summary>
        /// マウスダウンで、画像移動開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePosX = e.X;
            mousePosY = e.Y;
            mouseSwitch = true;
            //mouseIniX = e.X;
            //mouseIniY = e.Y;
        }

        /// <summary>
        /// マウスアップで、画像移動終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseSwitch = false;
            pictureTopLeft = new Point(drawRectangle.X, drawRectangle.Y);
            regHandler.PictureTopLeft = pictureTopLeft;
        }

        /// <summary>
        /// マウスムーブ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // マウスボタンが押されていない場合はイベントを無視する
            if (!mouseSwitch)
                return;

            int posX = mousePosX - e.X;
            int posY = mousePosY - e.Y;

            mousePosX = e.X;
            mousePosY = e.Y;

            drawRectangle.X -= posX;
            drawRectangle.Y -= posY;

            // 画像を表示する
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (currentImage != null)
            {
                // 画像を指定された位置、サイズで描画する
                e.Graphics.DrawImage(currentImage, drawRectangle);
            }
        }

        /// <summary>
        /// 画像サイズ設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureSize = new PictureSize(regHandler.Size1, regHandler.Size2, regHandler.Location);
            pictureSize.PictureSizeHandler += new PictureSize.closeEventHandler(pictureSize_PictureSizeHandler);
            pictureSize.Show();
        }

        /// <summary>
        /// 画像サイズ変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureSize_PictureSizeHandler(object sender, PictureSize.PictureSizeEventArgs e)
        {
            if (!e.Size1.IsEmpty)
            {
                regHandler.Size1 = e.Size1;
                this.formResize(e.Size1);
            }
            if( !e.Size2.IsEmpty )
                regHandler.Size2 = e.Size2;
        }

        /// <summary>
        /// フォームリサイズ
        /// </summary>
        /// <param name="size"></param>
        private void formResize(Size size)
        {
            this.pictureBox1.ClientSize = size;
            this.ClientSize = new System.Drawing.Size(size.Width + 30, size.Height + 43 + 20);
        }

        /// <summary>
        /// 保存先の設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileDestination();
        }

        /// <summary>
        /// 保存先の設定
        /// </summary>
        private void fileDestination()
        {
            folderBrowserDialog1.Description = "取得した画像の保存場所を指定してください。";
            folderBrowserDialog1.SelectedPath = destination;
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != null)
            {
                destination = folderBrowserDialog1.SelectedPath;
                regHandler.Destination = destination;
            } 
        }

        /// <summary>
        /// アプリケーション終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 画像保存ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            savePicture();
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 画像の保存
        /// </summary>
        private void savePicture()
        {
            if (currentImage == null)
            {
                return;
            }

            if (destination == "" || destination == null)
            {
                fileDestination();
            }

            Bitmap canvas = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            Graphics g = Graphics.FromImage(canvas);

            g.DrawImage(currentImage, drawRectangle);

            Rectangle srcRect = new Rectangle(-drawRectangle.X, -drawRectangle.Y, pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            Rectangle desRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);

            g.DrawImage(canvas, desRect, srcRect, GraphicsUnit.Pixel);
            g.Dispose();

            string fileName = getFileName();
            if (fileName == "" || fileName == null)
            {
                return;
            }
            pictureBox1.Image = canvas;
            pictureBox1.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

            // グラフィックで書き込んだ画像を消去する
            pictureBox1.Image = null;

            // クリップボードから書き込んだ画像を消去する
            currentImage = null;
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// ファイル名の取得・設定
        /// </summary>
        /// <returns></returns>
        private string getFileName()
        {
            if (this.chkAutoSave.Checked)
            {
                // 自動保存
                // ファイル名の決定 SIMG_20130201001.jpg
                // 
                string fileName = createFileName();
                return System.IO.Path.Combine(destination, fileName);
            }
            else
            {
                // 手動保存
                saveFileDialog1.Title = "保存するファイル名を指定してください。";
                saveFileDialog1.InitialDirectory = destination;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    return saveFileDialog1.FileName;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// ファイル名を生成する
        /// </summary>
        /// <returns></returns>
        private string createFileName()
        {
            string baseName = "SIMG_" 
                + DateTime.Today.Year 
                + DateTime.Today.Month.ToString().PadLeft(2,'0') 
                + DateTime.Today.Day.ToString().PadLeft(2, '0');

            string[] files = System.IO.Directory.GetFiles(destination, baseName + "*", System.IO.SearchOption.AllDirectories);
            int fileNumber;
            int maxFileNumber = 0;
            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                fileName = fileName.Replace(baseName + "_", null);
                fileName = fileName.Replace(".jpg", null);
                fileNumber = int.Parse(fileName.Replace(baseName, ""));
                if (maxFileNumber < fileNumber)
                {
                    maxFileNumber = fileNumber;
                }
            }
            maxFileNumber += 1;
            string runningNumber = "_" + maxFileNumber.ToString().PadLeft(3,'0');

            return baseName + runningNumber + ".jpg";
        }

        /// <summary>
        /// 自動保存チェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            regHandler.Autosave = chkAutoSave.Checked;
        }

        /// <summary>
        /// 場所が変わった場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            // 最初の１回目は空イベントが走るので、処理なしで戻す
            if (!allowLocation)
            {
                allowLocation = true;
                this.Location = regHandler.Location;
                return;
            }
            // 最小化した場合は座標の書き戻しをしない
            if (this.Location.X == -32000 && this.Location.Y == -32000)
            {
                return;
            }
            // 座標の書き戻し
            regHandler.Location = this.Location;
        }
    }
}
