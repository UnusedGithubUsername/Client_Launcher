   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5          Client.Customization   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:Client     5      
������   � Title$  MainMenuPage��5	      $	��450��5
      $	��800��6    � AllowsTransparency$
 True��5       � WindowStyle$
 None=�5       � DataContext 5      ��  �� RelativeSource �� *Self+ � 	Resources 5         FriendElement �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name J� Opacity ��Name  HoverButton  	WoodBrush
 � Tag  FriendRequestElement � ColumnDefinitions � RowDefinitions��%�  &	        7  7  &	 �    7  7  7  7  7  7  7  7  7  7  �� 5   
   " ��6&   .�5      ��  �@�OverridesDefaultStyle$
 True��66   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� $
 border��6!   $ 1Q�62   ._�5      �� $ label��6%   -   5        8��  8�� $��0q�5   N   
���z���$��13(�5   >   $��30��6L   $��	10,0,10,0q�5   D   ��6%   �� *
FriendName+ 5   %   �� *Guid+5      5      5       5!      �� 5#   
   " ��6-   .�5$      ��  �@�OverridesDefaultStyle$
 True��66   �� 5%       �@�Cursor$
 Hand��6'   �� 5&       � �� 5'      �� 5(      "	 ��6&   .R�5)      �� $
 border��6!   $ 1Q�62   ._�5*      � $ 1Q�6$    5+   "   �� 5,   &   �� 5-   &   �� 5.   &    50   "   �� 51   &   �� 52   &   .+�54   "   �� $��0��6)   $��2��69   8��  8�� $��0q�6�   
���z���$��13(�6�   $��30��6�   ��55   %   �� *
FriendName+ 6D   �� *
FriendId+�� 57   "   $��2��6)   8�� 8�� $��0q�6�   
���z���$��13(�6�   $��30��6�   ��58   %   �� *
FriendId+ 6B   �� *
FriendId+�� 5:   "   -   5        $��0��5:   )   $��1��69   $��20��6F   8�� $��Accept��6�   
������$��9(�6�   8��  6z   �� *
FriendId+6�   �� 5;   "   -   5        $��1��5;   )   $��1��69   $��20��6F   8�� $��Reject��6�   
�������$��9(�6�   8��  6z   �� *
FriendId+6�   �� 5<   "   -   5        $��2��5<   )   $��1��69   $��20��6F   8�� $��Block��6�   
���  ��$��9(�6�   8��	  6z   �� *
FriendId+6�   5=      5>      5?      5A      5B      5C      .��5E      ��  �� CornerRadius$ 30��6   $
}�True��6   ._�5F   
   � .+�5G       � -   5          �Name$ 	mainFrame��5G        � NavigationUIVisibility$ Hidden=�6'   � 5H      -   5         �Name$ JustForTheBorderMask��5H      .+�5I      �� -   5        $
��mask��5I      
���   �$ 30��68   � 5J      -   5        $ DividerForDraggableTopbar��5J        BackgroundPic#��[ 6:    5K      �� 5L      $��50 �6(   �� 5M       J� OpacityMask 5O      !� 5P       !� Visual 6&   ��  �� ElementName$
 mask��.+�5R      � $��0��6   .+�5S      ]� $	��Top=�6$   .+�5T      � $��50��6#   $	��Top=�6/   .+�5U   "   ]� $��Stretch=�6,   $��Stretch=�6J   .+�5V   &   �� -	   5         ��Name$ TopStack��5V   1   $��Right=�6C   $��Right=�6Z   $	��Top=�6v   $��50��6�    �� Orientation$ 
Horizontal=�6�   .+�5X   *   � -
   5        $ InvisibleDragBar��5X   /   
������ $
��3000��6�   �� 5Z   *   -   5        $ Minimize��5Z   1   $��41��6_   $��33��6j   ��5[   .   �� 5\   2    �� ImageSource$ /Video/Minimize.png��6=    5^   .   �� 5_   2   $ /Video/Minimize.png��6=   5a   +   �� 5c   *   -   5        $
 Quit��5c   1   $��41��6^   $��35��6i   ��5d   .   �� 5e   2   $ /Video/X.png��6=    5g   .   �� 5h   2   $ /Video/X.png��6=   5j   +   5l   '   5m   #   ��5n   "   �� 5o   &   $% /Client;component/Video/Bar.png��61   5q      5r      5s      � 5t      -   5        $ BaseGridBackground��5t      $��0,0,0,0q�67   $��1��6H    5u      �� 5v      $
��0.5* �6/   �� 5w      $
��0.5* �6/   �� 5x      $	��140 �6/   .+�5{      � $��0��6    5|      �� 5}   "   $��50 �60   �� 5~   "   .+�5�      � -   5        $ InventoryBackground��5�   0   $��1��6#   $��Stretch=�6M   $��Stretch=�6k   $��11,11,11,11q�6�     	GreenFade#��[ 6�   .+�5�   "   �� -   5         ��Name$ ItemCTRL��5�   /   ��6A   �� *CharacterData+��5�   &   �� 5�   *   .R�5�   .   � 5�   +   ��5�   &   �� 5�   *   .R�5�   .   �� -   5        $��55��5�   5   $��55��6@   $��5,5,5,5q�6L   
������ $��0q�6w     HoverButtonImage#��[ 5�   ]    6G   �� *Index+.��5�   2   �  6:   �� *
Opacityy+��6X   �� *
Background+ 5�   6   �� 5�   :   $4 ./Video/spellforceRuneTransparentBackground.png��6E   .+�5�   6   �� $*��$/Video/spellforceRuneTransparent.png��6<   5�   3   5�   /   5�   +   5�   #   5�      � 5�      $��0��6#   $��	15,0,15,0q�60   .+�5�   "   �� $ 
Horizontal=�6-   .+�5�   &   �� -   5        $
��Play��5�   -   #��[ 6O   #��[ 6w   $��24(�6�   $��90��6�   $��Century)�6�   
���z���6�   5�   #   �� 5�   "   $ 
Horizontal=�6-   $��Right=�6F   .+�5�   &   �� $��XP��6,   $	��110��6N     DoubleNumberLabel#��[ 6\   $	 160Q�6�   $��33��6�   $��19(�6�   $��Century)�6�   
���z���#��[ 6�   $��15q�6   6;   �� *xp+6  5�   #   5�      5�       � 5�      -   5        $" CharacterCustomizationButton��5�      $��1��6   $ Hidden=�6D   � 5�      -   5        $ 
FriendGrid��5�   /   $��2��6    5�      �� 5�   "   $��40 �60   �� 5�   "   �� 5�   "   $��50 �60   .+�5�      �� -   5        $ UsernameTextfield��5�   �   $��0��6)   $��Right=�67   $	��Top=�6S   $��33��6k   $ 
Horizontal=�6w   #��[ 6�   .+�5�   "   �� $��15,0,0,0q�6,   
���r���$��15(�6l   $��22��6z   ��6>   �� *
Username+6z   �� 5�   "   
���r���$��15(�6A   $��#��6O   $��Right=�6X   $��22��6t   �� 5�   "   $��0,0,15,0q�6,   
���r���$��15(�6l   $��22��6{   $��Right=�6�   ��6>   �� *
UserGuid+6�   5�      �� 5�      $��1��6)   .+�5�   "   �� -   5        
������ #��[ 5�      $��0q�6z   ��5�   E   �� *friendsRequestList+5�   #   �� 5�   "   -   5        
������ #��[  5�      $��0q�6s   ��5�   E   �� *friendsList+5�   #   5�      �� 5�      $��2��6)   $��Bottom=�66   $ Vertical=�6Q   .+�5�   "   �� -   5        $��	AddFriend��5�   *   $��25��6;   $��	10,0,20,0q�6H   �� 5�   "   -   5        $ AddFriend_Button��5�   )   #��[ 6E   $��	AddFriend��6m   $��22��6�    J� RenderTransformOrigin$ 0.562,1.838&�6�   $��12(�6�   $��Century)�6�   
���z���#��[ 6  $��	10,0,20,0q�68  5�      5�      5�      5�      5�      5�      5�      5�      