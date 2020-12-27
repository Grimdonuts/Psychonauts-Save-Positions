using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveStatePsychonauts
{
    public partial class Form1 : Form
    {
        private static VAMemory Vam;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public Form1()
        {
            InitializeComponent();
            Vam = new VAMemory("psychonauts");
            if (!Vam.CheckProcess())
            {
                Environment.Exit(0);
            }
            _hookID = SetHook(_proc);
        }


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!Vam.CheckProcess())
            {
                UnhookWindowsHookEx(_hookID);
                Environment.Exit(0);
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)KeyDefinitions.F1 || vkCode == (int)KeyDefinitions.F2 || vkCode == (int)KeyDefinitions.F3
                    || vkCode == (int)KeyDefinitions.F4)
                {
                    int baseAddressX = Vam.ReadInt32((IntPtr)(Vam.getBaseAddress + 0x00383200));
                    int offsetsAddedX = Vam.ReadInt32((IntPtr)baseAddressX + 0x0);
                    offsetsAddedX = Vam.ReadInt32((IntPtr)offsetsAddedX + 0x10);
                    float x = Vam.ReadFloat((IntPtr)offsetsAddedX + 0x40);

                    int baseAddressY = Vam.ReadInt32((IntPtr)(Vam.getBaseAddress + 0x00386AE0));
                    int offsetsAddedY = Vam.ReadInt32((IntPtr)baseAddressY + 0x0);
                    float y = Vam.ReadFloat((IntPtr)offsetsAddedY + 0x44);

                    int baseAddressZ = Vam.ReadInt32((IntPtr)(Vam.getBaseAddress + 0x00383200));
                    int offsetsAddedZ = Vam.ReadInt32((IntPtr)baseAddressZ + 0x10);
                    offsetsAddedZ = Vam.ReadInt32((IntPtr)offsetsAddedZ + 0x18);
                    offsetsAddedZ = Vam.ReadInt32((IntPtr)offsetsAddedZ + 0x10);
                    float z = Vam.ReadFloat((IntPtr)offsetsAddedZ + 0x48);

                    Form1 form = (Form1)Application.OpenForms["Form1"];

                    if (vkCode == (int)KeyDefinitions.F1 || vkCode == (int)KeyDefinitions.F3)
                    {
                        if (vkCode == (int)KeyDefinitions.F1)
                        {
                            form.label10.Text = x.ToString();
                            form.label11.Text = y.ToString();
                            form.label12.Text = z.ToString();
                        }
                        else if (vkCode == (int)KeyDefinitions.F3)
                        {
                            form.label13.Text = x.ToString();
                            form.label14.Text = y.ToString();
                            form.label15.Text = z.ToString();
                        }
                    }
                    else if (vkCode == (int)KeyDefinitions.F2 || vkCode == (int)KeyDefinitions.F4)
                    {
                        if (vkCode == (int)KeyDefinitions.F2)
                        {
                            Vam.WriteFloat((IntPtr)offsetsAddedX + 0x40, float.Parse(form.label10.Text));
                            Vam.WriteFloat((IntPtr)offsetsAddedY + 0x44, float.Parse(form.label11.Text));
                            Vam.WriteFloat((IntPtr)offsetsAddedZ + 0x48, float.Parse(form.label12.Text));
                        }
                        else
                        {
                            Vam.WriteFloat((IntPtr)offsetsAddedX + 0x40, float.Parse(form.label13.Text));
                            Vam.WriteFloat((IntPtr)offsetsAddedY + 0x44, float.Parse(form.label14.Text));
                            Vam.WriteFloat((IntPtr)offsetsAddedZ + 0x48, float.Parse(form.label15.Text));
                        }
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
        }

        public enum KeyDefinitions
        {
            F1 = 112,
            F2 = 113,
            F3 = 114,
            F4 = 115,
            F5 = 116,
            F6 = 117,
            F7 = 118,
            F8 = 119,
            F9 = 120,
            F10 = 121,
            F11 = 122,
            F12 = 123,
            a = 65,
            b = 66,
            c = 67,
            d = 68,
            e = 69,
            f = 70,
            g = 71,
            h = 72,
            i = 73,
            j = 74,
            k = 75,
            l = 76,
            m = 77,
            n = 78,
            o = 79,
            p = 80,
            q = 81,
            r = 82,
            s = 83,
            t = 84,
            u = 85,
            v = 86,
            w = 87,
            x = 88,
            y = 89,
            z = 90,
            D1 = 49,
            D2 = 50,
            D3 = 51,
            D4 = 52,
            D5 = 53,
            D6 = 54,
            D7 = 55,
            D8 = 56,
            D9 = 57,
            D0 = 48
        }
    }
}
