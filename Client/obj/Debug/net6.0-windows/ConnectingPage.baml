   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5          Client.ConnectingPage   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:Client     5        N� 
Background
  ����  N� Title$ ConnectingPage��5
      .��5      �  J� OpacityMask 5   
   !� 5       !� Visual 6   ��  �� ElementName$
 mask��.+�5   
   �� -   5        $
��mask��5      
���   � �� CornerRadius$ 30��61   r� 5   
   -   5        $��mediaElement��5       r� LoadedBehavior$
 Play=�6+    r� UnloadedBehavior$ Manual=�6A    r� Stretch$ UniformToFill=�6�   � 5   
   -   5        	 �Name$	 Progress��5      
 � Minimum$
 0Q�6(    � Maximum$	 100Q�64    � Value$ 0Q�6B   $��1��6L   $��50��6Y   $	��700��6e   $��000,320,000,0q�6q   �� 5   
   -   5         ��Name$ ProgressLabel��5      $��25��6'   $$��No connection to update-server��64   $��50,400,550,0q�6]   �� 5   
   -   5         ��Name$ Connect��5      $��Connect��6"   $
��Left=�64   $��310,220,0,0q�6O   $	��Top=�6d   $��92��6�   $	��152��6�   $��14(�6�   ��5      �� 5       �� ImageSource$' !/Client;component/Video/cloud.png��6    5      �� 5      $' !/Client;component/Video/cloud.png��6   5!      �� 5#   
   -   5        $ Minimize��5#      $��__��6#   $
��Left=�60   $��	659,0,0,0q�6K   $	��Top=�6^   $��56��6�   $��70��6�    J� RenderTransformOrigin$ 0.5,0.5&�6�   $��22(�6�   z�5$      V� 5%      .~�5&      �� �� 5'      �� 5(       �� Angle$ 359.921Q�6&   T� 5)      5*      ��5,      �� 5-      $( "/Client;component/Video/cloud2.png��6    5/      �� 50      $( "/Client;component/Video/cloud2.png��6   52      �� 54   
   -   5        $
 Quit��54      $��X��6   $
��Left=�6+   $��	722,0,0,0q�6F   $	��Top=�6Y   $��59��6�   $��68��6�   $��22(�6�   ��55      �� 56      $( "/Client;component/Video/cloud3.png��6    58      �� 59      $( "/Client;component/Video/cloud3.png��6   5;      �� 5<   
   -   5         ��Name$ TextfieldIP��5<      $
��Left=�6'    �� TextWrapping$
 Wrap=�6B   $	��Top=�6V   $	��120��6n   $��331,196,0,0q�6z   $��24��6�   $��	127.0.0.1��6�   $��16(�6�   � 5>   
   -	   5        $��TopBar��5>      $��0,0,200,300q�6]   
������ 6r   � 5?   
   -
   5        $
��left��5?      $��	0,0,507,0q�6[   
������ 6n   � 5@   
   -   5        $	��bot��5@      $��	0,300,0,0q�6Z   
������ 6m   � 5A   
   -   5        $��right��5A      $��
480,50,0,0q�6\   
������ 6p   5C      5E      