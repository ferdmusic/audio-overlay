   M S B A M L   `   `   `  ���� J  FAudioMonitor.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:AudioMonitor.UI.ViewsAudioMonitor.UI.Views  &     AudioMonitor.UI.Views.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     _ [MaterialDesignThemes.Wpf, Version=5.2.2.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  _ [Hardcodet.NotifyIcon.Wpf, Version=2.0.0.0, Culture=neutral, PublicKeyToken=682384a853a08aad)tb http://www.hardcodet.net/taskbar  /local#clr-namespace:AudioMonitor.UI.Views  -     � Title$  AudioMonitor��$	��500��$	��900��$	��400��$	��600��
������  � WindowStyle$
 None=� � AllowsTransparency$
 True�� � Icon$ 
/image.ico�� � 	Resources 3  -Hardcodet.Wpf.TaskbarNotification.TaskbarIcon  Name   MyNotifyIconResource   
IconSource   ToolTipText � ContextMenu  BgColorDarkest	 �� Color  BgColorDark  BgColorMedium  BgColorLightHover  TextColorPrimary  TextColorSecondary  AccentColorBlue  AccentColorGreen 	 AccentColorYellow 
 AccentColorRed  BorderColor  GlassBackground
 �� Opacity  GlassBackgroundHover  GlassBorder  ModernButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  PrimaryButtonStyle �� BasedOn �� Triggers  ModernComboBoxStyle d�Name �� 	ClickMode d� 	IsChecked �� Path �� Mode �� RelativeSource � ColumnDefinitions F�Name ��Name J� IsHitTestVisible �� SelectionBoxItem  �� SelectionBoxItemTemplate! �Name" �� IsDropDownOpen# � AllowsTransparency$ ��Name% �� MaxDropDownHeight& ��Name' J� SnapsToDevicePixels-  'System.Windows.Input.KeyboardNavigation(   DirectionalNavigation) �� 
TargetName  ModernTextBoxStyle4   .AudioMonitor.UI.Views.DeviceTypeGroupConverter  DeviceTypeGroupConverter��%r
  &	        &	 �     &	 �     &	 �     &	 �     &	 �     &	 �     &	      &	     &		 &    &	
 9    &	 L    &	 _    &	     &	 �    &	 �    7  7  7  7  &	 =    7  7  7  &	 �    7  7  7  7  7  7  7  7  7  7  &	 �    7  7  7  &	 U	      $ MyNotifyIcon��$ 
/image.ico��$ AudioMonitor UI�� �� .@�m� -   $
��Show��m� -   $��Settings��m� -   $
��Exit���� $	 #121212���� $	 #1e1e1e���� $	 #2a2a2e���� $	 #38383c���� $	 #e0e0e0���� $	 #a0a0a0���� $	 #0078d4���� $	 #107c10���� $	 #f0a30a���� $	 #d13438���� $	 #404040���� $	 	#03ffffff��$

 0.03Q��� $	 	#0fffffff��$

 0.06Q��� $	 	#1affffff��$	
 0.1Q��� " ��.���  � ��8   ��  � ��8  ��  � ��8  ��  � ��$ 1q���  � ��$	 8,8q���  � ��$ 14(���  � ��$ SemiBold"���  �@�Cursor$
 Hand����  � �� �� " ��.R��� #��z��#��z��#��z��$ 4��._��� $��Center=�$��Center=�#��z�� P�  �@J�IsMouseOver$
 True��. ��� 8   � ����  � �� �#555�� 8   " ��.���  � ��8  ��  � ��
 �������  � ��8   P�  �@J�IsMouseOver$
 True��. ���  � �� �#005a9e��  � �� �#005a9e�� " ��.���  � ��8   ��  � ��8  ��  � ��8  ��  � ��$ 1q���  � ��$
 10,8q���  � ��$ 14(���  � �� �� " ��.���  � ��8  ��  � ��8  ��  � ��$
 10,8q� P�  �@J�IsMouseOver$
 True��. ���  � ��8  P�  �@��
IsSelected$
 True��. ���  � ��8  ��  � ��
 �������  � �� �� " ��.R�� .+�d� $ ToggleButton��$|�false��$ Press=�#��z��#��z��#��z�� �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+���� " d�.���  � �� �� " d�.R��� #��z��#��z��#��z��$ 4��._��  �� $��* ��� $
��Auto �.+��� $��0��$
��Left=�$��Center=�#��z��#��z��F� $ Arrow��$��1��"����        � 	=  	= � z     	#��z��$��Center=�$��Center=�$��0,0,8,0q��� $ ContentSite��$ False��#��z #��z  #��z��#��z��$��Center=�$
��Left=�� $! Popup��$��Bottom=�#��z" $
# True��$|�False��$��Slide=�.���� $$ DropDownBorder��8�� 8�� $��1q�$ 4��#��z% ._��� $& ScrollViewer��$
' True��.���� $( 	Contained=� P�  �@J�IsMouseOver$
 True��. ��� $) ToggleButton��8 	  � ��P�  � {�$ False��. ���  �@J�Opacity$
 0.56Q��� " ��.���  � ��8   ��  � ��8  ��  � ��8  ��  � ��$ 1q���  � ��$	 8,8q���  � ��$ 14(�  .���� #��[ $ 8��* J� Effect* 3  -System.Windows.Media.Effects.DropShadowEffect  +   
BlurRadius$+ 20Q�,   ShadowDepth$, 0Q�-   Color$- Black��.   Opacity$	. 0.4Q�._�� / � RowDefinitions/ �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��#��[ #��[ $��0,0,0,1q�0 �� Padding$0 20,15q�._��  �� $��* ��� $
��Auto �.+��� $��AudioMonitor��$��20(�$��SemiBold"�#��[ $��Center=��� $��1��1 �� Orientation$1 
Horizontal=�.+��� -   $��Settings��#��[ $
��10,0q��� -   $	��✕��#��[ $��30��$��30��$��12(�� $��1��$��20q�/ �� $
��Auto ��� $
��Auto ��� $��* �.+��� $��0��$��Audio Input Device��$��16(�$��SemiBold"�#��[ $��0,0,0,15q��� -   2 ��Name$2 DeviceComboBox��$��1��#��[ $	��500��$��40��$
��Left=�$��0,0,0,20q�3 �� 
GroupStyle3 (  "System.Windows.Controls.GroupStyle  4   HeaderTemplate4 �� .R��� #��[ #��[ $��0,0,0,1q�$	0 8,4q�._��� $��SemiBold"�$��12(�#��[ ���� 5 �� 	Converter#5 [ �� $��2��${��uAudioMonitor is running and monitoring audio levels in real-time. Configure settings using the Settings button above.��$��14(�#��[ $
��Wrap=�$	��Top=�$��0,20,0,0q��� $��2��#��[ #��[ $��0,1,0,0q�$0 20,10q�._��� $1 
Horizontal=�$��Right=�.+��� $��Status: Monitoring��$��12(�#��[ $��Center=�$��0,0,15,0q�