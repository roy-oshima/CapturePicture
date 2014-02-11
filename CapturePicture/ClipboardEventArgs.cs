/**
 * クリップボードライブラリ
 * 
 * FOOTPRINT IT SERVICE CO LTD
 * Roy
 * 2014.02.01
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CapturePicture
{
    /// <summary>
    /// クリップボードイベント引数
    /// </summary>
    public class ClipboardEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        private Image img;
        /// <summary>
        /// 
        /// </summary>
        public Image Img
        {
            get { return this.img; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        public ClipboardEventArgs(Image img)
        {
            this.img = img;
        }
    }

    /// <summary>
    /// クリップボードイベントデリゲート
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="ev"></param>
    public delegate void cbEventHandler(object sender, ClipboardEventArgs ev);

    /// <summary>
    /// クリップボードビューワー
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    internal class MyClipboardViewer : NativeWindow
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWndNewViewer"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern IntPtr SetClipboardViewer(
                IntPtr hWndNewViewer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWndRemove"></param>
        /// <param name="hWndNewNext"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool ChangeClipboardChain(
                IntPtr hWndRemove, IntPtr hWndNewNext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public extern static int SendMessage(
                IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 
        /// </summary>
        private const int WM_DRAWCLIPBOARD = 0x0308;
        /// <summary>
        /// 
        /// </summary>
        private const int WM_CHANGECBCHAIN = 0x030D;
        /// <summary>
        /// 
        /// </summary>
        private IntPtr nextHandle;

        /// <summary>
        /// 親フォーム
        /// </summary>
        private Form parent;

        /// <summary>
        /// クリップボードイベントハンドラ
        /// </summary>
        public event cbEventHandler ClipboardHandler;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="f"></param>
        public MyClipboardViewer(Form f)
        {
            f.HandleCreated   += new EventHandler(this.OnHandleCreated);
            f.HandleDestroyed += new EventHandler(this.OnHandleDestroyed);
            this.parent = f;
        }

        /// <summary>
        /// ハンドルの作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnHandleCreated(object sender, EventArgs e)
        {
            AssignHandle(((Form)sender).Handle);
            // ビューアを登録
            nextHandle = SetClipboardViewer(this.Handle);
        }

        /// <summary>
        /// ハンドルの破壊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnHandleDestroyed(object sender, EventArgs e)
        {
            // ビューアを解除
            bool sts = ChangeClipboardChain(this.Handle, nextHandle);
            ReleaseHandle();
        }

        /// <summary>
        /// WndProc オーバーライド
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (Clipboard.ContainsImage())
                    {
                        // クリップボードの内容を取得してハンドラを呼び出す
                        ClipboardHandler(this, new ClipboardEventArgs(Clipboard.GetImage()));
                        Clipboard.Clear();
                    }
                    if ((int)nextHandle != 0)
                        SendMessage(
                            nextHandle, msg.Msg, msg.WParam, msg.LParam);
                    break;

                // クリップボード・ビューア・チェーンが更新された
                case WM_CHANGECBCHAIN:
                    if (msg.WParam == nextHandle)
                    {
                        nextHandle = (IntPtr)msg.LParam;
                    }
                    else if ((int)nextHandle != 0)
                        SendMessage(
                            nextHandle, msg.Msg, msg.WParam, msg.LParam);
                    break;
            }
            base.WndProc(ref msg);
        }
    }
}
