using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.UI.Win32.Controls.Interfaces;

namespace ForgeAir.UI.Win32.Controls
{
    public class DirectoryBrowser : IDirectoryBrowser
    {
        private FolderBrowserDialog _dialog;
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Environment.SpecialFolder RootDirectory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SelectedPath { get => _dialog.SelectedPath ?? String.Empty; }
        
        public DirectoryBrowser() {
            _dialog = new FolderBrowserDialog();
            _dialog.Description = Title;
            _dialog.ShowNewFolderButton = false;
            _dialog.RootFolder = RootDirectory;
            _dialog.UseDescriptionForTitle = true;
            _dialog.ShowPinnedPlaces = true;
            _dialog.AutoUpgradeEnabled = true;
            
        }

       

        public bool ShowDialog()
        {
            if (_dialog == null)
            {
                throw new NullReferenceException();
            }
            switch (_dialog.ShowDialog())
            {
                case DialogResult.OK:
                    
                    return true;
                default:
                    return false;
            }
        }
    }
}
