using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
using WPFEcommerceApp.UserControls.Dialogs.AddProductDialog;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ShopRatingBlock.xaml
    /// </summary>
    public partial class ShopRatingBlock : UserControl
    {
        public ShopRatingBlock()
        {
            InitializeComponent();
            IsLoadingCheck.IsLoading--;
        }
    }
}
