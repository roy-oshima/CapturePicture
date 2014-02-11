/**
 * ピクチャーサイズ変更用フォーム
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
    /// 画像サイズ
    /// </summary>
    public partial class PictureSize : Form
    {
        /// <summary>
        /// サイズ１
        /// </summary>
        private Size size1;
        /// <summary>
        /// サイズ２　未使用
        /// </summary>
        private Size size2;
        // キャンセルボタン、左上の×ボタンをクリックした場合は、terminate となり、値を親フォームに伝えない
        private bool terminate = true;

        /// <summary>
        /// 画像サイズ変更イベント引数
        /// </summary>
        public class PictureSizeEventArgs : EventArgs
        {
            private Size pictureSize1;
            private Size pictureSize2;

            public Size Size1
            {
                get { return pictureSize1; }
            }

            public Size Size2
            {
                get { return pictureSize2; }
            }

            public PictureSizeEventArgs(Size NewSize1, Size NewSize2)
            {
                pictureSize1 = NewSize1;
                pictureSize2 = NewSize2;
            }
        }

        /// <summary>
        /// フォーム閉イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void closeEventHandler(object sender, PictureSizeEventArgs e);

        /// <summary>
        /// 画像サイズ変更イベントハンドラ
        /// </summary>
        public event closeEventHandler PictureSizeHandler;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        public PictureSize(Size size1, Size size2, Point point)
        {
            InitializeComponent();

            size1Width.Text = size1.Width.ToString();
            size1Height.Text = size1.Height.ToString();
            point.X += 50;
            point.Y += 50;
            this.Location = point;
        }

        /// <summary>
        /// フォーム閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureSize_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!terminate)
            {
                PictureSizeHandler(this, new PictureSizeEventArgs(size1, size2));
            }
            else
            {
                size1 = new Size();
                size2 = new Size();
            }

        }

        /// <summary>
        /// キャンセルボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            size1 = new Size();
            size2 = new Size();
            this.Close();
        }

        /// <summary>
        /// サイズ１幅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void size1Width_Leave(object sender, EventArgs e)
        {
            try
            {
                size1.Width = int.Parse(size1Width.Text);
            }
            catch (Exception ex)
            {
                ex.ToString();
                size1Width.Text = size1.Width.ToString();
            }
        }

        /// <summary>
        /// サイズ１高
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void size1Height_Leave(object sender, EventArgs e)
        {
            try
            {
                size1.Height = int.Parse(size1Height.Text);
            }
            catch (Exception ex)
            {
                ex.ToString();
                size1Height.Text = size1.Height.ToString();
            }
        }

        /// <summary>
        /// OKボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            terminate = false;
            this.Close();
        }
    }
}
