using System;
using System.Collections.Generic;
using System.Linq;
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
using ForgeAir.Core.Shared;
using ForgeAir.Database.Models;

namespace ForgeAir.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for SweeperHookBoxControl.xaml
    /// </summary>
    public partial class SweeperHookBoxControl : UserControl
    {
        public SweeperHookBoxControl()
        {
            InitializeComponent();
            SweeperShared.Instance.updateListUI += UpdateListBox;
        }

        public void UpdateListBox(object sender, EventArgs e)
        {

            if (SweeperShared.Instance.sweeper != null && SweeperShared.Instance.targetTrack != null)
            {
                AddSweeperUI(SweeperShared.Instance.targetTrack, SweeperShared.Instance.sweeper, (TimeSpan)(SweeperShared.Instance.targetTrack.Intro - SweeperShared.Instance.sweeper.Duration));
            }
        }
        public void ClearSweeperUI()
        {
            if (sweeperHookBox.Items.Count == 0)
            {
                return;
            }
            else
            {
                sweeperHookBox.Items.Clear();
            }
            return;
        }
        public void AddSweeperUI(Track targetTrack, Track sweeper, TimeSpan hookTime)
        {
            Dispatcher.Invoke(() =>
            {
                ClearSweeperUI();
                sweeperHookBox.Items.Add($"HOOK:{sweeper.Title} >> {targetTrack.Title}@{hookTime.ToString(@"hh\:mm\:ss")}");
                sweeperHookBox.Items.Refresh();
            });

        }
    }
}
