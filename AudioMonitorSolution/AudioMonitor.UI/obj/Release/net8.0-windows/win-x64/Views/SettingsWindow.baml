   M S B A M L   `   `   `  ���� J  FAudioMonitor.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:AudioMonitor.UI.ViewsAudioMonitor.UI.Views  *    $AudioMonitor.UI.Views.SettingsWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5local#clr-namespace:AudioMonitor.UI.Views     -     � Title$@  :Audio-Pegelwarnung für Focusrite Scarlett - Einstellungen��$	��700��$	��650��$	��600��$	��600��
������  � WindowStyle$
 None=� � AllowsTransparency$
 True�� � Icon$ 
/image.ico�� � 	Resources 4   .AudioMonitor.UI.Views.DeviceTypeGroupConverter   DeviceTypeGroupConverter  BgColorDarkest �� Color  BgColorDark  BgColorMedium  BgColorLightHover  TextColorPrimary  TextColorSecondary  AccentColorBlue  AccentColorGreen 	 AccentColorYellow 
 AccentColorRed  BorderColor  GlassBackground �� Opacity  GlassBackgroundHover  GlassBorder  ModernButtonStyle �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  PrimaryButtonStyle �� BasedOn �� Triggers  ModernComboBoxStyle d�Name �� 	ClickMode d� 	IsChecked �� Path �� Mode �� RelativeSource � ColumnDefinitions F�Name ��Name J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate �Name �� IsDropDownOpen � AllowsTransparency  ��Name! �� MaxDropDownHeight" ��Name# J� SnapsToDevicePixels-  'System.Windows.Input.KeyboardNavigation$   DirectionalNavigation% �� 
TargetName  ModernTextBoxStyle  ModernToggleSwitchStyle& A�Name' N� EnterActions( �� 
TargetName) �� TargetProperty	* W� To+ g� Duration, N� ExitActions  ModernSliderStyle  GlassCardStyle-  'System.Windows.Media.Effects.BlurEffect-   Radius��%�  &	        &	      &	      &	 +     &	 >     &	 Q     &	 d     &	 w     &	 �     &		 �     &	
 �     &	 �     &	 �     &	 �     &	     &	 5    7  7  7  7  &	 �    7  7  7  &	 e    7  7  7  7  7  7  7  7  7  7  &	 G    7  7  7  &	 �    7  7  7  &	 �
    7  7  &	 �
    7  7    �� $ #121212���� $ #1e1e1e���� $ #2a2a2e���� $ #38383c���� $ #e0e0e0���� $ #a0a0a0���� $ #0078d4���� $ #107c10���� $ #f0a30a���� $ #d13438���� $ #404040���� $ 	#03ffffff��$
 0.03Q��� $ 	#0fffffff��$
 0.06Q��� $ 	#1affffff��$	 0.1Q��� " ��.���  � ��8	   ��  � ��8	  ��  � ��8	  ��  � ��$	 1q���  � ��$		 8,8q���  � ��$	 14(���  � ��$	 SemiBold"���  �@�Cursor$
	 Hand����  � ��	 �� "
 ��.R��� #��z��#��z��#��z��$ 4��._��� $��Center=�$��Center=�#��z�� P�  �@J�IsMouseOver$
 True��. ��� 8	   � ����  � ��	 �#555�� 8   " ��.���  � ��8	  ��  � ��
	 �������  � ��8	   P�  �@J�IsMouseOver$
 True��. ���  � ��	 �#005a9e��  � ��	 �#005a9e�� " ��.���  � ��8	   ��  � ��8	  ��  � ��8	  ��  � ��$	 1q���  � ��$
	 10,8q���  � ��$	 14(���  � ��	 �� " ��.���  � ��8	  ��  � ��8	  ��  � ��$
	 10,8q� P�  �@J�IsMouseOver$
 True��. ���  � ��8	  P�  �@��
IsSelected$
 True��. ���  � ��8	  ��  � ��
	 �������  � ��	 �� "
 ��.R�� .+�d� $ ToggleButton��$|�false��$ Press=�#��z��#��z��#��z�� �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+���� " d�.���  � ��	 �� "
 d�.R��� #��z��#��z��#��z��$ 4��._��  �� $��* ��� $
��Auto �.+��� $��0��$
��Left=�$��Center=�#��z��#��z��F� $ Arrow��$��1��"����        � 	=  	= � z     	#��z��$��Center=�$��Center=�$��0,0,8,0q��� $ ContentSite��$ False��#��z #��z #��z��#��z��$��Center=�$
��Left=�� $ Popup��$��Bottom=�#��z $
 True��$|�False��$��Slide=�.���� $  DropDownBorder��8�� 8�� $��1q�$ 4��#��z! ._��� $" ScrollViewer��$
# True��.���� $$ 	Contained=� P�  �@J�IsMouseOver$
 True��. ��� $% ToggleButton��8	 	  � ��P�  � {�$ False��. ���  �@J�Opacity$
	 0.56Q��� " ��.���  � ��8	   ��  � ��8	  ��  � ��8	  ��  � ��$	 1q���  � ��$		 8,8q���  � ��$	 14(��� " ��.���  � ��	 �� "
 ��.R�� .+��� $  SwitchTrack��$��44��$��24��8��  8�� $��1q�$ 12��A� $& SwitchThumb��$��18��$��18��
�������$
��Left=�$��3,0,0,0q�z�T� ThumbTransform�� P�  �@d�	IsChecked$
 TrueY�. ��� $% SwitchTrack��8	   � ��' �� .���� .��W� $( ThumbTransform��$) X�* 20$+ 0:0:0.2D�, �� .���� .��W� $( ThumbTransform��$) X�* 0$+ 0:0:0.2D��� " ��.���  � ��8	   ��  � ��8	  ��  � ��$	 6���� " ��.���  � ��8	   ��  � ��8	  ��  � ��$	 1q���  �@��CornerRadius$	 8����  �@J�Effect	   $- 0Q�.���� #��[ $ 8��. J� Effect. 3  -System.Windows.Media.Effects.DropShadowEffect  /   
BlurRadius$/ 20Q�0   ShadowDepth$0 0Q�1   Color$1 Black��2   Opacity$	2 0.4Q�._�� 3 � RowDefinitions3 �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��#��[ #��[ $��0,0,0,1q�4 �� Padding$4 20,10q�._��  �� $��* ��� $
��Auto �.+��� $��Audio-Pegelwarnung��$��16(�$��SemiBold"�#��[ $��Center=��� $��1��5 �� Orientation$5 
Horizontal=�.+��� -   #��[ $��30��$��30��$	��5,0q�
������ 
������ .��F� ����� � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� � �  	= Ӑe�  	=  -1p�s  -1 � � -1�e� �e�  -1 �  -1�p�s  -1 	= �e�  	=  � � 	= p�s p�s  	=  �  	= 	#��[ $��16��$��16���� -   $	��✕��#��[ $��30��$��30��
������ 
������ $��12(��� $��1��$
��Auto=�$��Disabled=�$��25,20q�.���� .+��� $��Einstellungen��$��28(�$
��Bold"�#��[ $��0,0,0,20q��� #��[ $4 25q�._��� .+�� $��0,0,0,12q� �� $��* ��� $
��Auto �.+��� $��Audio-Eingangsgerät��$��16(�$��SemiBold"�#��[ �� $��1�����#4c9383$ 8��$	4 6,2q�._��� $
��FA01��$��10(�$
��Bold"�
��������� #��[ $ 6��$��60��$��0,0,0,15q�._�� $
��15,0q�3 �� $
��Auto ��� $��* �.+�� $��0��$��0,8,0,8q� �� $��* ��� $
��Auto �.+��� -   6 ��Name$6 CurrentLevelText��$!��Aktueller Pegel: -18.2 dBFS��$��12(�#��[ �� -   $6 PeakLevelText��$��1��$��Peak: -12.1 dBFS��$��12(�#��[ �� $��1��#��[ $ 6��$��12��._��� -   7 ��Name$7 AudioLevelBar��#��[ $
��Left=�$��0��8 �� RadiusX$8 6��9 �� RadiusY$9 6���� -   : ��Name$: AudioInputComboBox��#��[ ; �� DisplayMemberPath$
; Name��< �� 
GroupStyle< (  "System.Windows.Controls.GroupStyle  =   HeaderTemplate= �� .R��� $��SemiBold"�$��13(�#��[ $��8,6,8,4q����� *Name+> �� 	Converter#> [  ?   ContainerStyle? �� " ��.���  � ��	 �� "
 ��.R��� .+��� �� #��[ #��[ $��0,0,0,1q�$��8,0,8,4q�._��� $��12,0,0,0q��� #��[ $4 25q�._��� .+�� $��0,0,0,12q� �� $��* ��� $
��Auto �.+��� $��Warnschwellenwerte��$��16(�$��SemiBold"�#��[ �� $��1�����#4c9383$ 8��$	4 6,2q�._��� $
��FA03��$��10(�$
��Bold"�
��������� $A��;Pegelstufen und zugehörige dBFS-Schwellenwerte definieren.��$��12(�#��[ $��0,0,0,15q��� #��[ $ 6��$4 20q�#��[ $��1,0,0,0q�._��� .+�� $��0,0,0,12q� �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�A� $��20��$��20��#��[ $��0,0,15,0q��� $��1��$��Sicher (Grün) bis:��$��14(�#��[ $��Center=�$	��120���� -   @ ��Name$@ ThresholdSafeTextBox��$��3��$	��-12��#��[ $��80��$��Right=�A �� TextAlignment$A Right=�$��0,0,15,0q��� $��4��$
��dBFS��$��14(�#��[ $��Center=�� $��0,0,0,12q� �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�A� $��20��$��20��#��[	 $��0,0,15,0q��� $��1��$��Achtung (Gelb) bis:��$��14(�#��[ $��Center=�$	��120���� -	   $@ ThresholdWarningTextBox��$��3��$��-6��#��[ $��80��$��Right=�$A Right=�$��0,0,15,0q��� $��4��$
��dBFS��$��14(�#��[ $��Center=��  �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�A� $��20��$��20��#��[
 $��0,0,15,0q��� $��1��$��Kritisch (Rot) ab:��$��14(�#��[ $��Center=�$	��120���� -
   $@ ThresholdCriticalTextBox��$��3��$��-3��#��[ $��80��$��Right=�$A Right=�$��0,0,15,0q��� $��4��$
��dBFS��$��14(�#��[ $��Center=��� $U��OFarbverlauf: Standard Grün-Gelb-Rot. (Konfigurierbarkeit idealerweise später)��$��12(�#��[ $��0,15,0,0q��� #��[ $4 25q�._��� .+�� $��0,0,0,12q� �� $��* ��� $
��Auto �.+��� $��Visuelles Overlay��$��16(�$��SemiBold"�#��[ �� $��1�����#4c9383$ 8��$	4 6,2q�._��� $
��FA04��$��10(�$
��Bold"�
��������  �� $��* ��� $��* �3 �� $
��Auto ��� $
��Auto �.+��� $��0��$��0,0,8,0q�.+��� $��Position (Monitorrand):��$��12(�#��[ $��0,0,0,8q��� -   $: OverlayPositionComboBox��#��[ .@��� $��Oberer Rand��B �� 
IsSelected$
B True���� $��Unterer Rand���� $��Linker Rand���� $��Rechter Rand���� $��1��$��8,0,0,0q�.+��� $��Dicke/Breite:��$��12(�#��[ $��0,0,0,8q��  �� $��* ��� $
��Auto �.+��� -   $@ OverlayThicknessTextBox��$��5��#��[ $��0,0,8,0q��� $��1��$��px��$��14(�#��[ $��Center=��� $X��RDynamische Transparenz und Verhalten im kritischen Bereich gemäß FA04.2, FA04.3.��$��12(�#��[ $��0,15,0,0q��� #��[ $4 25q�._��� .+�� $��0,0,0,15q� �� $��* ��� $
��Auto �.+��� $��Akustische Warnung��$��16(�$��SemiBold"�#��[ �� $��1�����#4c9383$ 8��$	4 6,2q�._��� $
��FA05��$��10(�$
��Bold"�
�������� $��0,0,0,15q� �� $��* ��� $
��Auto �.+��� $0��*Akustische Warnung (Sinuswelle) aktivieren��$��14(�#��[ $��Center=��� -   C ��Name$C AcousticWarningsCheckBox��$��1��#��[ $
 TrueY��  �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��Lautstärke:��$��14(�#��[ $��Center=�$��80��$��0,0,16,0q��� -   D ��Name$D AcousticVolumeSlider��$��1��#��[ E � Minimum$E 0Q�F � Maximum$	F 100Q�G � Value$G 75Q��� -   $6 AcousticVolumeText��$��2��$	��75%��$��14(�$��SemiBold"�#��[ $��Center=�$��40��H �� TextAlignment$H Right=�$��16,0,0,0q��� #��[ $4 25q�._��  �� $��* ��� $
��Auto ��� $
��Auto �.+��� .+��� $��Mit Windows starten��$��16(�$��SemiBold"�#��[ �� $��1�����#4c9383$ 8��$	4 6,2q�$��0,0,15,0q�._��� $
��FA07��$��10(�$
��Bold"�
��������� -   $C AutostartCheckBox��$��2��#��[ $
 TrueY��� $��2��#��[ #��[ $��0,1,0,0q�$4 25,15q�._��� $5 
Horizontal=�$��Right=�.+��� -   $$��Einstellungen speichern (FA08)��#��[ $��0,0,10,0q��� -   $��Überwachung starten��#��[ $��0,0,10,0q��� -   $��In Tray minimieren (FA07)��#��[ 