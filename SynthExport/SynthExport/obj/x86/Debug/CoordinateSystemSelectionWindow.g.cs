﻿#pragma checksum "..\..\..\CoordinateSystemSelectionWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B5AFEAF28420E298F6F1D406C808272D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
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
    /// CoordinateSystemSelectionWindow
    /// </summary>
    public partial class CoordinateSystemSelectionWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\..\CoordinateSystemSelectionWindow.xaml"
        internal System.Windows.Controls.ListView listViewCoordinateSystems;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\CoordinateSystemSelectionWindow.xaml"
        internal System.Windows.Controls.Button buttonCancel;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\CoordinateSystemSelectionWindow.xaml"
        internal System.Windows.Controls.Button buttonOK;
        
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
            System.Uri resourceLocater = new System.Uri("/SynthExport;component/coordinatesystemselectionwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CoordinateSystemSelectionWindow.xaml"
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
            this.listViewCoordinateSystems = ((System.Windows.Controls.ListView)(target));
            return;
            case 2:
            this.buttonCancel = ((System.Windows.Controls.Button)(target));
            
            #line 60 "..\..\..\CoordinateSystemSelectionWindow.xaml"
            this.buttonCancel.Click += new System.Windows.RoutedEventHandler(this.buttonCancel_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.buttonOK = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\..\CoordinateSystemSelectionWindow.xaml"
            this.buttonOK.Click += new System.Windows.RoutedEventHandler(this.buttonOK_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
