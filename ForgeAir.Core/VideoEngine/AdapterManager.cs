using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Forms;
using ForgeAir.Core.SystemWrappers;

namespace ForgeAir.Core.VideoEngine
{
    public class AdapterManager
    {
        public AdapterManager()
        {

        }


        // Structure for Display Device information
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        // Structure for Display Settings
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEVMODE
        {
            public const int DM_PELSWIDTH = 0x80000;
            public const int DM_PELSHEIGHT = 0x100000;
            public const int DM_BITSPIXEL = 0x40000;
            public const int DM_PELSPERSEC = 0x200000;
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;

            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }


        public void RestoreOriginalResolution(DEVMODE originalSettings)
        {
            int result = SystemWrappers.User32.ChangeDisplaySettings(ref originalSettings, SystemWrappers.User32.CDS_UPDATEREGISTRY);
            if (result != 0)
            {
                new Exception("fatal: failed to restore resolution");
            }
        }

        public void GetCurrentDisplaySettings(ref DEVMODE devMode)
        {
            devMode.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            SystemWrappers.User32.ChangeDisplaySettings(ref devMode, SystemWrappers.User32.ENUM_CURRENT_SETTINGS);
        }

        public Task<Screen[]> GetDisplayAdapters()
        {
            Screen[] screens = new Screen[Screen.AllScreens.Length];
            int index = 0;
            foreach (var screen in Screen.AllScreens)
            {
                screens[index++] = screen;
            }
            return Task.FromResult(screens);
        }
        public async Task ChangeResolutionAsync()
        {
            var displayAdapters = await GetDisplayAdapters();
            if (displayAdapters.Length > 0)
            {
                ChangeResolution(displayAdapters[0], 1280, 720, 60);
            }
            else
            {
                MessageBox.Show("No displays found.");
            }
        }

        public void ChangeResolution(Screen displayAdapter, int width, int height, int refreshRate)
        {
            long RetVal = 0;

            DEVMODE dm = new DEVMODE();

            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

            dm.dmPelsWidth = width;
            dm.dmPelsHeight = height;
            dm.dmDisplayFrequency = refreshRate;
            dm.dmBitsPerPel = 32; // Default 32-bit color depth, can change depending on your monitor support

            dm.dmFields = DEVMODE.DM_PELSWIDTH | DEVMODE.DM_PELSHEIGHT;

            dm.dmFields = DEVMODE.DM_PELSWIDTH | DEVMODE.DM_PELSHEIGHT | DEVMODE.DM_BITSPIXEL;

            dm.dmFields |= DEVMODE.DM_PELSPERSEC;


            RetVal = User32.ChangeDisplaySettings(ref dm, User32.CDS_UPDATEREGISTRY);

            MessageBox.Show(RetVal.ToString());
        }

    }
}
