using System;
using System.Windows;

namespace WpfApp
{
    class BridgeDrop
    {
        public static void Files(object sender, DragEventArgs e, Action<string[]> core)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                core(files);
            }
        }
    }
}
