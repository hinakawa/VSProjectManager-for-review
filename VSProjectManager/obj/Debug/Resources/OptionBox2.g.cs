﻿#pragma checksum "..\..\..\Resources\OptionBox2.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C41892492CBEB6918280C196AC3C4C996B4166C2"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
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
using System.Windows.Shell;


namespace VSProjectManager {
    
    
    /// <summary>
    /// OptionBox2
    /// </summary>
    public partial class OptionBox2 : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\Resources\OptionBox2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Property;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Resources\OptionBox2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Value;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VSProjectManager;component/resources/optionbox2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Resources\OptionBox2.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 13 "..\..\..\Resources\OptionBox2.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Property = ((System.Windows.Controls.ComboBox)(target));
            
            #line 20 "..\..\..\Resources\OptionBox2.xaml"
            this.Property.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler(this.OnComboboxTextChanged));
            
            #line default
            #line hidden
            
            #line 23 "..\..\..\Resources\OptionBox2.xaml"
            this.Property.LostFocus += new System.Windows.RoutedEventHandler(this.PropertyLostFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Value = ((System.Windows.Controls.ComboBox)(target));
            
            #line 25 "..\..\..\Resources\OptionBox2.xaml"
            this.Value.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler(this.OnComboboxTextChanged));
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

