   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient      Client.Customization   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     &localclr-namespace:Client       N� 
Background
  ����  N� Title$ MainMenuPage��.���  � ColumnDefinitions �� $
��794* ��� $��7* �.+��� -   $
��mask��
���   � �� CornerRadius$ 30��� -    �Name$ BaseGridBackground��$��0,0,0,0q�����  �� ImageSource$, &/Client;component/Video/Background.png�� J� OpacityMask !�  !� Visual ��  �� ElementName$
 mask��.+��� -   	 ��Name$	 Descriptor_GUId��$
��GUId��$
��Left=�$��
733,68,0,0q�$	��Top=��� -   $	 GuidTextfield��$	��0
��$
��Left=�$��
743,85,0,0q�$	��Top=�� -   $# CharacterCustomizationButtons��$��0,0,0,0q�.+��� -   
 ��Name$
 Strength��   InventoryButtonStyle#��[  $��517,249,94,168q�$��33��$	��133��$��14(�$
��Bold"�$��Century)�
���z���$��Strength���� -   $
 Vitality��#��[  $��517,278,94,139q�$��33��$	��133��$��14(�$
��Bold"�$��Century)�
���z���$��Vitality���� -   $
 Intelligence��#��[  $��517,309,94,108q�$��33��$	��133��$��14(�$
��Bold"�$��Century)�
���z���$��Intelligence���� -	   $
 Wisdom��#��[  $��517,339,94,78q�$��33��$	��133��$��14(�$
��Bold"�$��Century)�
���z���$��Wisdom���� -
   $
 Left1��  
LeftButton#��[ $��640,249,50,168q�$��33��$��33���� -   $
 Left2��#��[ $��640,279,50,138q�$��33��$��33���� -   $
 Left3��#��[ $��640,309,50,108q�$��33��$��33���� -   $
 Left4��#��[ $��640,339,50,78q�$��33��$��33���� -   $
	 Mid1��$��678,249,16,168q�$��33��$��39��$	�� 13��  	WoodField#��[ �� -   $
	 Mid2��$��678,278,16,139q�$��33��$��39��$	�� 13��#��[ �� -   $
	 Mid3��$��678,309,16,108q�$��33��$��39��$	�� 13��#��[ �� -   $
	 Mid4��$��678,339,16,78q�$��33��$��39��$	�� 13��#��[ �� -   $
 Right1��  RightButton#��[ $��729,249,0,168q�$��33��$��33���� -   $
 Right2��#��[ $��729,279,0,138q�$��33��$��33���� -   $
 Right3��#��[ $��729,309,0,108q�$��33��$��33���� -   $
 Right4��#��[ $��729,339,0,78q�$��33��$��33���� -   $

 Save��$
��Save��  
WoodButton#��[ $��44��$��62��$��619,372,72,34q�� -   $ InventoryBackground��$
��Left=�$��	17,93,0,0q�$	��Top=�$	��326��$	��518������ $5 //Client;component/Video/InventoryBackground.png�� �� Opacity$ 10Q�.+��� $
��Auto=�.���� -    ��Name$ 	Inventory��.+��� -   $ InventoryTopBar��$��20�� �� Orientation$ 
Horizontal=�.+��� #��[  $
��62.5���� #��[  $
��62.5���� #��[  $
��62.5���� #��[  $
��62.5���� #��[  $
��62.5���� #��[  $
��62.5���� #��[  $
��62.5���� #��[  $
��62.5��� -   $
��Left=�$��10,0,0,0q�$	��Top=�$��54��$	��770������ $% /Client;component/Video/Bar.png���� -   $
 Minimize��  Minimize#��[ $��667,0,0,400q��� -   $

 Quit��  Quit#��[ $��734,0,0,400q�