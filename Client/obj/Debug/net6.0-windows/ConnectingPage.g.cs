﻿#pragma checksum "..\..\..\ConnectingPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "52BBFC27C0A4DEF2E1A349610A0EBC39498C05B2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Client;
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


namespace Client {
    
    
    /// <summary>
    /// ConnectingPage
    /// </summary>
    public partial class ConnectingPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border mask;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MediaElement mediaElement;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar Progress;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ProgressLabel;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Connect;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Minimize;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Quit;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextfieldIP;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid TopBar;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid left;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid bot;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\ConnectingPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid right;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Client;component/connectingpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ConnectingPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mask = ((System.Windows.Controls.Border)(target));
            return;
            case 2:
            this.mediaElement = ((System.Windows.Controls.MediaElement)(target));
            
            #line 21 "..\..\..\ConnectingPage.xaml"
            this.mediaElement.MediaEnded += new System.Windows.RoutedEventHandler(this.MediaElement_MediaRepeat);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Progress = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 4:
            this.ProgressLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.Connect = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\ConnectingPage.xaml"
            this.Connect.Click += new System.Windows.RoutedEventHandler(this.Connect_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Minimize = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\ConnectingPage.xaml"
            this.Minimize.Click += new System.Windows.RoutedEventHandler(this.Connect_Minimize);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Quit = ((System.Windows.Controls.Button)(target));
            
            #line 52 "..\..\..\ConnectingPage.xaml"
            this.Quit.Click += new System.Windows.RoutedEventHandler(this.Connect_Quit);
            
            #line default
            #line hidden
            return;
            case 8:
            this.TextfieldIP = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.TopBar = ((System.Windows.Controls.Grid)(target));
            
            #line 62 "..\..\..\ConnectingPage.xaml"
            this.TopBar.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Grid_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 10:
            this.left = ((System.Windows.Controls.Grid)(target));
            
            #line 63 "..\..\..\ConnectingPage.xaml"
            this.left.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Grid_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 11:
            this.bot = ((System.Windows.Controls.Grid)(target));
            
            #line 64 "..\..\..\ConnectingPage.xaml"
            this.bot.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Grid_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 12:
            this.right = ((System.Windows.Controls.Grid)(target));
            
            #line 65 "..\..\..\ConnectingPage.xaml"
            this.right.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Grid_PreviewMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

