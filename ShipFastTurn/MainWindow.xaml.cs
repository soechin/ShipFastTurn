using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShipFastTurn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel Model;
        Winmm.TIMECALLBACK m_func1, m_func2;
        int m_timer1, m_timer2;
        long m_freq, m_tick;
        bool m_keyW, m_keyA, m_keyS, m_keyD, m_lastAD;
        int m_offset;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Model = DataContext as ViewModel;

            Kernel32.QueryPerformanceFrequency(out m_freq);
            Kernel32.QueryPerformanceCounter(out m_tick);
            m_keyW = false;
            m_keyA = false;
            m_keyS = false;
            m_keyD = false;
            m_lastAD = false;
            m_offset = 0;

            m_func1 = new Winmm.TIMECALLBACK(Timer1_Elapsed);
            m_func2 = new Winmm.TIMECALLBACK(Timer2_Elapsed);
            m_timer1 = Winmm.timeSetEvent(100, 1, m_func1, IntPtr.Zero, 1/*TIME_PERIODIC*/);
            m_timer2 = Winmm.timeSetEvent(1, 1, m_func2, IntPtr.Zero, 1/*TIME_PERIODIC*/);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Winmm.timeKillEvent(m_timer1);
            Winmm.timeKillEvent(m_timer2);
        }

        private void Timer1_Elapsed(int uTimerID, int uMsg, IntPtr dwUser, IntPtr dw1, IntPtr dw2)
        {
            m_keyW = (User32.GetAsyncKeyState('W') & 0x8000) != 0;
            m_keyA = (User32.GetAsyncKeyState('A') & 0x8000) != 0;
            m_keyS = (User32.GetAsyncKeyState('S') & 0x8000) != 0;
            m_keyD = (User32.GetAsyncKeyState('D') & 0x8000) != 0;
        }

        private void Timer2_Elapsed(int uTimerID, int uMsg, IntPtr dwUser, IntPtr dw1, IntPtr dw2)
        {
            double speed, accel, time, t1, t2, n1, n2;
            int count, sign;

            if (!Model.Enabled || m_keyW || m_keyS || (m_keyA == m_keyD))
            {
                m_lastAD = false;
                return;
            }

            Kernel32.QueryPerformanceCounter(out long tick);

            if (!m_lastAD)
            {
                m_lastAD = true;
                m_tick = tick;
                m_offset = 0;
            }

            sign = m_keyA ? (-1) : m_keyD ? (+1) : 0;
            speed = Model.Speed;
            accel = Model.Accel;
            time = (double)(tick - m_tick) / m_freq;
            t1 = Math.Min(time, accel);
            t2 = time - t1;
            n1 = (speed / accel) * t1 * t1 / 2;
            n2 = speed * t2;
            count = (int)(n1 + n2);

            while (m_offset < count)
            {
                User32.MOUSEINPUT mi = new User32.MOUSEINPUT
                {
                    dx = sign,
                    dy = 0,
                    dwFlags = 0x0001/*MOUSEEVENTF_MOVE*/,
                };

                User32.SendInput(mi);
                m_offset++;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Model.Enabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Model.Enabled = false;
        }
    }

    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _enabled;
        public bool Enabled { get => _enabled; set => SetField(ref _enabled, value); }

        private double _speed;
        public double Speed { get => _speed; set => SetField(ref _speed, value); }

        private double _accel;
        public double Accel { get => _accel; set => SetField(ref _accel, value); }
    }
}
