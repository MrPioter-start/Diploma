﻿#pragma checksum "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "27B543DB8859C1C6F1511FA313584027679DE2F4"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Diploma.main_windows.admin.Promotions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Diploma.main_windows.admin.Promotions {
    
    
    /// <summary>
    /// LoyaltyLevelsWindow
    /// </summary>
    public partial class LoyaltyLevelsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 99 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid LoyaltyLevelsDataGrid;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border LevelForm;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LevelNameTextBox;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MinOrderTotalTextBox;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DiscountPercentageTextBox;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveLevelButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.2.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Diploma;component/main_windows/admin/promotions/loyaltylevelswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.2.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 92 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddLevel_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 93 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditLevel_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 94 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteLevel_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.LoyaltyLevelsDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 5:
            this.LevelForm = ((System.Windows.Controls.Border)(target));
            return;
            case 6:
            
            #line 119 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseLevelForm_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.LevelNameTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.MinOrderTotalTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.DiscountPercentageTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.SaveLevelButton = ((System.Windows.Controls.Button)(target));
            
            #line 131 "..\..\..\..\..\..\main_windows\admin\Promotions\LoyaltyLevelsWindow.xaml"
            this.SaveLevelButton.Click += new System.Windows.RoutedEventHandler(this.SaveLevelButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

