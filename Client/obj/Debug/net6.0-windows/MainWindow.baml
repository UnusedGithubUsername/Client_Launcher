   M S B A M L   `   `   `  ����
  Client    Client.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � WindowStyle$
  None=�5   	    � 
ResizeMode$ CanResizeWithGrip=�5   	    � AllowsTransparency$
 True��5   	   
������  � Title$ 
MainWindow��5   	   $	��450��5	   	   $	��800��6   $	��300��5
   	   $	��450��6   .��5      � 
������  � RowDefinitions 5   
   �� 5      $��50 �6   �� 5      �� 5      .+�5   
   �� -   5        $
��mask��5   "   $��3��6   
���   � �� CornerRadius$ 30��6A   r� 5   
   -   5        $��mediaElement��5      $��3��6+    r� LoadedBehavior$
 Play=�6<    r� UnloadedBehavior$ Manual=�6R    r� Stretch$ UniformToFill=�6�   ]� 5   
   $	��Top=�6   $��0��6,   .+�5      � $��50��6   $	��Top=�6   .+�5      ]� $��Stretch=�6   $��Stretch=�6:   .+�5      �� -   5        	 ��Name$	 TopStack��5   !   $��Right=�63   $��Right=�6J   $	��Top=�6f   $��50��6~   
 �� Orientation$
 
Horizontal=�6�   .+�5      � -   5         �Name$ InvisibleDragBar��5      
������ $
��3000��6z   � 5      -   5        $ ProgressBar��5      .+�5       � -   5         �Name$ Progress��5    *    � Minimum$ 0Q�6<    � Maximum$	 100Q�6H    � Value$ 0Q�6V   $��1��6`   $��50��6m   $	��200��6y   $��	5,20,5,17q�6�   �� 5!      -   5         ��Name$ ProgressLabel��5!   $   $��25��6;   $$��No connection to update-server��6H   $��5,0,0,0q�6q   5"      �� 5%      -   5         ��Name$ Minimize��5%   !   $��41��6O   $��33��6Z   ��5&      �� 5'   "    �� ImageSource$ /Video/Minimize.png��6-    J� OpacityMask 5)      �� 5*   "   $ /Video/Minimize.png��6-   5,      �� 5.      -	   5        $
 Quit��5.   !   $��41��6N   $��35��6Y   ��5/      �� 50   "   $ /Video/X.png��6-    52      �� 53   "   $ /Video/X.png��6-   55      57      58      ��59      �� 5:      $% /Client;component/Video/Bar.png��6!   5=      5?       � 5@   
   -
   5          �Name$ 	mainFrame��5@      $��2��6     � NavigationUIVisibility$ Hidden=�60    5A   
   !� 5B       !� Visual 6   ��  �� ElementName$
 mask��5D      5F      