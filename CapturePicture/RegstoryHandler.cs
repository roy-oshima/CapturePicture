/**
 * レジスターハンドルライブラリ
 * 
 * FOOTPRINT IT SERVICE CO LTD
 * Roy
 * 2014.02.01
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Drawing;

namespace CapturePicture
{
    /// <summary>
    /// レジストリ一括処理
    /// </summary>
    class RegstoryHandler
    {
        private RegistryKey sizeKey1 = Registry.CurrentUser.CreateSubKey(@"Software\capturePicture\size1");
        private RegistryKey sizeKey2 = Registry.CurrentUser.CreateSubKey(@"Software\capturePicture\size2");
        private Size size1;
        private Size size2;
        private Size defaultSize1 = new Size(320, 240);
        private Size defaultSize2 = new Size(320, 240);
        string width;
        string height;
        private Point pictureTopLeft;
        private double pictureZoom;
        private string destination;
        private bool autosave;
        private System.Text.RegularExpressions.MatchCollection mc;
        private Point location;

        /// <summary>
        /// リーダーの初期化
        /// </summary>
        public RegstoryHandler()
        {
            if (sizeKey1.ValueCount != 0)
            {
                // 画像の大きさ
                {
                    string s1 = sizeKey1.GetValue("sizeKey1").ToString();
                    mc = System.Text.RegularExpressions.Regex.Matches(s1, @"(Width=)(\d+)");
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        width = m.Value;
                        width = width.Replace("Width=", "");
                    }
                    size1.Width = int.Parse(width);
                    mc = System.Text.RegularExpressions.Regex.Matches(s1, @"(Height=)(\d+)");
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        height = m.Value;
                        height = height.Replace("Height=", "");
                    }
                    size1.Height = int.Parse(height);
                }
                // 画像の左上の位置
                {
                    string s2 = sizeKey1.GetValue("pictureTopLeft").ToString();
                    string X = "";
                    mc = System.Text.RegularExpressions.Regex.Matches(s2, @"(X=)(\d+)|(X=)\-(\d+)");
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        X = m.Value;
                        X = X.Replace("X=", "");
                    }
                    pictureTopLeft.X = int.Parse(X);
                    string Y = "";
                    mc = System.Text.RegularExpressions.Regex.Matches(s2, @"(Y=)(\d+)|(Y=)\-(\d+)");
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        Y = m.Value;
                        Y = Y.Replace("Y=", "");
                    }
                    pictureTopLeft.Y = int.Parse(Y);
                }
                // 画像のズーム
                {
                    string s3 = sizeKey1.GetValue("pictureZoom").ToString();
                    pictureZoom = double.Parse(s3) / 100;
                }
                // 画像の保存場所
                {
                    destination = sizeKey1.GetValue("destination").ToString();
                }
                // 自動保存
                {
                    autosave = sizeKey1.GetValue("autosave").ToString() == "True";
                }
                // メインフォームの表示位置
                {
                    string s3 = sizeKey1.GetValue("location").ToString();
                    string X = "";
                    mc = System.Text.RegularExpressions.Regex.Matches(s3, @"(X=)(\d+)|(X=)\-(\d+)");
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        X = m.Value;
                        X = X.Replace("X=", "");
                    }
                    location.X = int.Parse(X);
                    string Y = "";
                    mc = System.Text.RegularExpressions.Regex.Matches(s3, @"(Y=)(\d+)|(Y=)\-(\d+)");
                    foreach (System.Text.RegularExpressions.Match m in mc)
                    {
                        Y = m.Value;
                        Y = Y.Replace("Y=", "");
                    }
                    location.Y = int.Parse(Y);
                }
            }
            else
            {
                size1 = defaultSize1;
                Size1 = size1;

                pictureTopLeft = new Point(0, 0);
                PictureTopLeft = pictureTopLeft;

                pictureZoom = 1d;
                PictureZoom = pictureZoom;

                destination = "";
                Destination = destination;

                autosave = true;
                Autosave = autosave;

                location = new Point(0, 0);
                Location = location;
            }
            if (sizeKey2.ValueCount != 0)
            {
                string s = sizeKey2.GetValue("sizeKey2").ToString();
                mc = System.Text.RegularExpressions.Regex.Matches(s, @"(Width=)(\d+)");
                foreach (System.Text.RegularExpressions.Match m in mc)
                {
                    width = m.Value;
                    width = width.Replace("Width=", "");
                }
                size2.Width = int.Parse(width);
                mc = System.Text.RegularExpressions.Regex.Matches(s, @"Height=\d+");
                foreach (System.Text.RegularExpressions.Match m in mc)
                {
                    height = m.Value;
                    height = height.Replace("Height=", "");
                }
                size2.Height = int.Parse(height);
            }
            else
            {
                size2 = defaultSize2;
                Size2 = size2;
            }
        }

        /// <summary>
        /// ひとつ目の画像サイズを指定、取得する
        /// </summary>
        public Size Size1
        {
            get { return size1; }
            set 
            {
                size1 = value;
                sizeKey1.SetValue("sizeKey1", size1);
            }
        }

        /// <summary>
        /// ふたつ目の画像サイズを指定、取得する
        /// </summary>
        public Size Size2
        {
            get { return size2; }
            set
            {
                size2 = value;
                sizeKey2.SetValue("sizeKey2", size2);
            }
        }

        /// <summary>
        /// 画像左上座標
        /// </summary>
        public Point PictureTopLeft
        {
            get { return pictureTopLeft; }
            set
            {
                pictureTopLeft = value;
                sizeKey1.SetValue("pictureTopLeft", pictureTopLeft);
            }
        }

        /// <summary>
        /// 画像の拡大・縮小率
        /// </summary>
        public double PictureZoom
        {
            get { return pictureZoom; }
            set
            {
                pictureZoom = value;
                sizeKey1.SetValue("pictureZoom", pictureZoom * 100d);
            }
        }

        /// <summary>
        /// ファイル保存先の設定
        /// </summary>
        public string Destination
        {
            get { return destination; }
            set
            {
                destination = value;
                sizeKey1.SetValue("destination",destination);
            }
        }

        /// <summary>
        /// 自動保存フラッグ
        /// </summary>
        public bool Autosave
        {
            get { return autosave; }
            set
            {
                autosave = value;
                sizeKey1.SetValue("autosave", autosave);
            }
        }

        /// <summary>
        /// メインフォームの表示位置
        /// </summary>
        public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                sizeKey1.SetValue("location", location);
            }
        }
    }
}
