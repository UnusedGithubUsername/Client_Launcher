   M S B A M L   `   `   `  ���� 
  Client    Client.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � WindowStyle$
  None=� � 
ResizeMode$ CanResizeWithGrip=� � AllowsTransparency$
 True��
������  � Title$ 
MainWindow��$	��450��$	��800��$	��300��$	��450��.��� 
������  � RowDefinitions �� $��50 ��� �� .+��� -   $
��mask��$��3��
���   � �� CornerRadius$ 30��r� -   $��mediaElement��$��3�� r� LoadedBehavior$
 Play=� r� UnloadedBehavior$ Manual=� r� Stretch$ UniformToFill=�]� $	��Top=�$��0��.+�� $��50��$	��Top=�.+�]� $��Stretch=�$��Stretch=�.+��� -   	 ��Name$	 TopStack��$��Right=�$��Right=�$	��Top=�$��50��
 �� Orientation$
 
Horizontal=�.+�� -    �Name$ InvisibleDragBar��
������ $
��3000��� -   $ ProgressBar��.+�� -    �Name$ Progress�� � Minimum$ 0Q� � Maximum$	 100Q� � Value$ 0Q�$��1��$��50��$	��200��$��	5,20,5,17q��� -    ��Name$ ProgressLabel��$��25��$$��No connection to update-server��$��5,0,0,0q��� -    ��Name$ Minimize��$��41��$��33������  �� ImageSource$ /Video/Minimize.png�� J� OpacityMask �� $ /Video/Minimize.png���� -	   $
 Quit��$��41��$��35������ $ /Video/X.png�� �� $ /Video/X.png������ $% /Client;component/Video/Bar.png�� � -
     �Name$ 	mainFrame��$��2��  � NavigationUIVisibility$ Hidden=� !�  !� Visual ��  �� ElementName$
 mask��