   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient      Client.ConnectingPage   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     &localclr-namespace:Client       N� 
Background
  ����  N� Title$ ConnectingPage��.���  J� OpacityMask !�  !� Visual ��  �� ElementName$
 mask��.+��� -   $
��mask��
���   � �� CornerRadius$ 30��r� -   $��mediaElement�� r� LoadedBehavior$
 Play=� r� UnloadedBehavior$ Manual=� r� Stretch$ UniformToFill=�� -   	 �Name$	 Progress��
 � Minimum$
 0Q� � Maximum$	 100Q� � Value$ 0Q�$��1��$��50��$	��700��$��000,320,000,0q��� -    ��Name$ ProgressLabel��$��25��$$��No connection to update-server��$��50,400,550,0q��� -    ��Name$ Connect��$��Connect��$
��Left=�$��310,220,0,0q�$	��Top=�$��92��$	��152��$��14(�����  �� ImageSource$' !/Client;component/Video/cloud.png�� �� $' !/Client;component/Video/cloud.png���� -   $ Minimize��$��__��$
��Left=�$��	659,0,0,0q�$	��Top=�$��56��$��70�� J� RenderTransformOrigin$ 0.5,0.5&�$��22(�z�V� .~��� �� ��  �� Angle$ 359.921Q�T� ���� $( "/Client;component/Video/cloud2.png�� �� $( "/Client;component/Video/cloud2.png���� -   $
 Quit��$��X��$
��Left=�$��	722,0,0,0q�$	��Top=�$��59��$��68��$��22(����� $( "/Client;component/Video/cloud3.png�� �� $( "/Client;component/Video/cloud3.png���� -    ��Name$ TextfieldIP��$
��Left=� �� TextWrapping$
 Wrap=�$	��Top=�$	��120��$��331,196,0,0q�$��24��$��	127.0.0.1��$��16(�� -	   $��TopBar��$��0,0,200,300q�
������ � -
   $
��left��$��	0,0,507,0q�
������ � -   $	��bot��$��	0,300,0,0q�
������ � -   $��right��$��
480,50,0,0q�
������ 