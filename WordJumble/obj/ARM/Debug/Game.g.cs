﻿

#pragma checksum "B:\Software Engineering Year 3 GMIT\Mobile App Development\Practice\WordJumble\WordJumble\Game.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "23BEE685A88208D48AC463658A5516DF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WordJumble
{
    partial class Game : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 192 "..\..\..\Game.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.whenInUse;
                 #line default
                 #line hidden
                #line 193 "..\..\..\Game.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyDown += this.enterKeyDown;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 200 "..\..\..\Game.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.enterWordClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


