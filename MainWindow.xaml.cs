using Gma.System.MouseKeyHook;
using System.Windows;
using System.Windows.Forms;

namespace CapsLockNotifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //監聽鍵盤事件
        private IKeyboardMouseEvents _globalHook;
        //右下角隱藏圖示
        private NotifyIcon _notifyIcon;
        //右下角選單右鍵選項
        private ContextMenuStrip _contextMenu;

        public MainWindow()
        {
            InitializeComponent();
            //控制視窗位置
            this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
            this.Top = (SystemParameters.WorkArea.Height - this.Height) / 7 * 6;
            this.Visibility = Visibility.Hidden;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            InitializeNotifyIcon();
        }

        private void InitializeNotifyIcon()
        {
            // 創建托盤圖示
            _notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Information, // 可更換成自定義的圖示
                Visible = true,
                Text = "CapsLock Notifier"
            };

            // 創建右鍵選單
            _contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("exit", null, OnExitClick);
            _contextMenu.Items.Add(exitItem);
            _notifyIcon.ContextMenuStrip = _contextMenu;

            // 左鍵點擊托盤圖示顯示視窗
            _notifyIcon.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    ShowCapsLockStatus();
                }
            };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 設置全域鍵盤監聽
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += OnKeyDown;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 解除鍵盤監聽
            _globalHook.KeyDown -= OnKeyDown;
            _globalHook.Dispose();
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // 檢查是否按下 Caps Lock
            if (e.KeyCode == System.Windows.Forms.Keys.CapsLock)
            {
                ShowCapsLockStatus();
            }
        }

        private async void ShowCapsLockStatus()
        {
            // 更新狀態文字
            bool isCapsLockOn = Console.CapsLock;
            StatusText.Text = isCapsLockOn ? "a" : "A";

            // 顯示視窗並在0.5秒後自動隱藏
            this.Visibility = Visibility.Visible;
            await Task.Delay(500);
            this.Visibility = Visibility.Hidden;
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}