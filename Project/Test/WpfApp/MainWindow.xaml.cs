﻿using System.Windows;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowText(string text)
        {
            System.Windows.MessageBox.Show(this, text);
        }

        public static void StaticMethod()
        {
            System.Windows.MessageBox.Show("static");
        }
    }
}
