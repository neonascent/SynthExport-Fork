﻿#pragma checksum "..\..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DCEA2FC7BBDB1E16370A5C8D2497BE6F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace SynthExport {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.TextBlock textBlockWebsite;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.TextBlock textBlock2;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.GroupBox groupBox1;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.RadioButton radioButtonFromUrl;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.TextBox textBoxUrl;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.RadioButton radioButtonFromFile;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.TextBox textBoxFile;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button buttonBrowse;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.GroupBox groupBox2;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.CheckBox checkBoxPointClouds;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.ComboBox comboBoxOutputFormat;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.CheckBox checkBoxCameraParameters;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.CheckBox checkBoxMaxScript;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.CheckBox checkBoxMaxScriptPos;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.CheckBox checkBoxMaxScriptSpheres;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.GroupBox groupBox3;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.TextBlock statusTextBlock;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.Button buttonExport;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\MainWindow.xaml"
        internal System.Windows.Controls.ProgressBar progressBar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SynthExport;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.textBlockWebsite = ((System.Windows.Controls.TextBlock)(target));
            
            #line 22 "..\..\..\MainWindow.xaml"
            this.textBlockWebsite.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.textBlockWebsite_MouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.textBlock2 = ((System.Windows.Controls.TextBlock)(target));
            
            #line 24 "..\..\..\MainWindow.xaml"
            this.textBlock2.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.textBlock2_MouseDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.groupBox1 = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 4:
            this.radioButtonFromUrl = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 5:
            this.textBoxUrl = ((System.Windows.Controls.TextBox)(target));
            
            #line 42 "..\..\..\MainWindow.xaml"
            this.textBoxUrl.KeyDown += new System.Windows.Input.KeyEventHandler(this.textBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.radioButtonFromFile = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 7:
            this.textBoxFile = ((System.Windows.Controls.TextBox)(target));
            
            #line 55 "..\..\..\MainWindow.xaml"
            this.textBoxFile.KeyDown += new System.Windows.Input.KeyEventHandler(this.textBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.buttonBrowse = ((System.Windows.Controls.Button)(target));
            
            #line 56 "..\..\..\MainWindow.xaml"
            this.buttonBrowse.Click += new System.Windows.RoutedEventHandler(this.buttonBrowse_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.groupBox2 = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 10:
            this.checkBoxPointClouds = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.comboBoxOutputFormat = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 12:
            this.checkBoxCameraParameters = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 13:
            this.checkBoxMaxScript = ((System.Windows.Controls.CheckBox)(target));
            
            #line 78 "..\..\..\MainWindow.xaml"
            this.checkBoxMaxScript.Checked += new System.Windows.RoutedEventHandler(this.checkBox1_Checked);
            
            #line default
            #line hidden
            return;
            case 14:
            this.checkBoxMaxScriptPos = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 15:
            this.checkBoxMaxScriptSpheres = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 16:
            this.groupBox3 = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 17:
            this.statusTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 18:
            this.buttonExport = ((System.Windows.Controls.Button)(target));
            
            #line 98 "..\..\..\MainWindow.xaml"
            this.buttonExport.Click += new System.Windows.RoutedEventHandler(this.buttonExport_Click);
            
            #line default
            #line hidden
            return;
            case 19:
            this.progressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

