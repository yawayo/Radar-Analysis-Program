﻿#pragma checksum "..\..\Window1.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4065F46392450D49C94601120E570AA499C29DC71C367B637514CCC29A36C8CE"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
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
using UI_test_01;


namespace Radar_Analysis_Program {
    
    
    /// <summary>
    /// Window1
    /// </summary>
    public partial class Window1 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Main_grid;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label test_label;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas Data_Draw;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MediaElement mediaElement;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider slider;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPlay;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnStop;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSelectFile;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CheckBox;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Clear_btn;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer Text_scroll;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Data_Text;
        
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
            System.Uri resourceLocater = new System.Uri("/Radar_Analysis_Program;component/window1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Window1.xaml"
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
            this.Main_grid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            
            #line 33 "..\..\Window1.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Form_Close_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 39 "..\..\Window1.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Form_Resize_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 47 "..\..\Window1.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Form_Hide_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.test_label = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.Data_Draw = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.mediaElement = ((System.Windows.Controls.MediaElement)(target));
            
            #line 81 "..\..\Window1.xaml"
            this.mediaElement.Unloaded += new System.Windows.RoutedEventHandler(this.btnStop_Click);
            
            #line default
            #line hidden
            
            #line 81 "..\..\Window1.xaml"
            this.mediaElement.MediaOpened += new System.Windows.RoutedEventHandler(this.mediaElement_MediaOpened);
            
            #line default
            #line hidden
            
            #line 82 "..\..\Window1.xaml"
            this.mediaElement.MediaEnded += new System.Windows.RoutedEventHandler(this.mediaElement_MediaEnded);
            
            #line default
            #line hidden
            
            #line 82 "..\..\Window1.xaml"
            this.mediaElement.MediaFailed += new System.EventHandler<System.Windows.ExceptionRoutedEventArgs>(this.mediaElement_MediaFailed);
            
            #line default
            #line hidden
            return;
            case 8:
            this.slider = ((System.Windows.Controls.Slider)(target));
            
            #line 84 "..\..\Window1.xaml"
            this.slider.AddHandler(System.Windows.Controls.Primitives.Thumb.DragStartedEvent, new System.Windows.Controls.Primitives.DragStartedEventHandler(this.slider_DragStarted));
            
            #line default
            #line hidden
            
            #line 84 "..\..\Window1.xaml"
            this.slider.AddHandler(System.Windows.Controls.Primitives.Thumb.DragCompletedEvent, new System.Windows.Controls.Primitives.DragCompletedEventHandler(this.slider_DragCompleted));
            
            #line default
            #line hidden
            
            #line 84 "..\..\Window1.xaml"
            this.slider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.slider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnPlay = ((System.Windows.Controls.Button)(target));
            
            #line 87 "..\..\Window1.xaml"
            this.btnPlay.Click += new System.Windows.RoutedEventHandler(this.btnPlay_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btnStop = ((System.Windows.Controls.Button)(target));
            
            #line 92 "..\..\Window1.xaml"
            this.btnStop.Click += new System.Windows.RoutedEventHandler(this.btnStop_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnSelectFile = ((System.Windows.Controls.Button)(target));
            
            #line 98 "..\..\Window1.xaml"
            this.btnSelectFile.Click += new System.Windows.RoutedEventHandler(this.BtnSelectFile_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.CheckBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 104 "..\..\Window1.xaml"
            this.CheckBox.Checked += new System.Windows.RoutedEventHandler(this.CheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 104 "..\..\Window1.xaml"
            this.CheckBox.Unchecked += new System.Windows.RoutedEventHandler(this.CheckBox_Unchecked);
            
            #line default
            #line hidden
            return;
            case 13:
            this.Clear_btn = ((System.Windows.Controls.Button)(target));
            return;
            case 14:
            this.Text_scroll = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 15:
            this.Data_Text = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

