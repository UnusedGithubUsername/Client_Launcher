   M S B A M L   `   `   `  ���� A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient      Client.Customization   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     &localclr-namespace:Client     -   
������   � Title$  MainMenuPage��$	��450��$	��800�� � AllowsTransparency$
 True�� � WindowStyle$
 None=� � DataContext ��  �� RelativeSource �� *Self+ � 	Resources    FriendElement �� 
TargetType �� Property �� Value	 �� 
TargetType
 ��Name J� Opacity ��Name  HoverButton  	WoodBrush
 � Tag  FriendRequestElement � ColumnDefinitions � RowDefinitions��%$  &	        7  7  &	     7  7  7  7  7  7  7  7  7  7  �� " ��.���  �@�OverridesDefaultStyle$
 True����  �@�Cursor$
 Hand����  � �� �� "	 ��.R��� $
 border��$ 1Q�._��� $ label��-   8��  8�� $��0q�
���z���$��13(�$��30��$��	10,0,10,0q����� *
FriendName+ �� *
FriendId+�� " ��.���  �@�OverridesDefaultStyle$
 True����  �@�Cursor$
 Hand����  � �� �� "	 ��.R��� $
 border��$ 1Q�._�� $ 1Q� �� �� ��  �� �� .+��� $��0��$��2��8��  8�� $��0q�
���z���$��13(�$��30������ *
FriendName+ �� *
FriendId+�� $��2��8�� 8�� $��0q�
���z���$��13(�$��30������ *
FriendId+ �� *
FriendId+�� -   $��0��$��1��$��20��8�� $��Accept��
������$��9(�8��  �� *
FriendId+�� -   $��1��$��1��$��20��8�� $��Reject��
�������$��9(�8��  �� *
FriendId+�� -   $��2��$��1��$��20��8�� $��Block��
���  ��$��9(�8��	  �� *
FriendId+.����  �� CornerRadius$ 30��$
}�True��._�� .+� � -     �Name$ 	mainFrame��  � NavigationUIVisibility$ Hidden=�� -    �Name$ JustForTheBorderMask��.+��� -   $
��mask��
���   �$ 30��� -	   $ DividerForDraggableTopbar��  BackgroundPic#��[  �� $��50 ���  J� OpacityMask !�  !� Visual ��  �� ElementName$
 mask��.+�� $��0��.+�]� $	��Top=�.+�� $��50��$	��Top=�.+�]� $��Stretch=�$��Stretch=�.+��� -
    ��Name$ TopStack��$��Right=�$��Right=�$	��Top=�$��50�� �� Orientation$ 
Horizontal=�.+�� -   $ InvisibleDragBar��
������ $
��3000���� -   $ Minimize��$��41��$��33������  �� ImageSource$ /Video/Minimize.png�� �� $ /Video/Minimize.png���� -   $
 Quit��$��41��$��35������ $ /Video/X.png�� �� $ /Video/X.png������ $% /Client;component/Video/Bar.png��� -   $ BaseGridBackground��$��0,0,0,0q�$��1�� �� $
��0.5* ��� $
��0.5* ��� $	��140 �.+�� $��0�� �� $��50 ��� .+�� -   $ InventoryBackground��$��1��$��Stretch=�$��Stretch=�$��11,11,11,11q�  	GreenFade#��[ .+��� -    ��Name$ ItemCTRL������ *CharacterData+���� .R�� ���� .R��� -   $��55��$��55��$��5,5,5,5q�
������ $��0q�#��[  �� *Index+.���  �� *
Opacityy+���� *
Background+ �� $4 ./Video/spellforceRuneTransparentBackground.png��.+��� $*��$/Video/spellforceRuneTransparent.png��� $��0��$��	15,0,15,0q�.+��� $ 
Horizontal=�.+��� -   $
��Play��#��[ #��[ $��24(�$��90��$��Century)�
���z����� $ 
Horizontal=�$��Right=�.+��� $��XP��$	��110��  DoubleNumberLabel#��[ $	 160Q�$��33��$��19(�$��Century)�
���z���#��[ $��15q� �� *xp+� -   $# CharacterCustomizationButtons��$��1��y��� *loadedCharacterIndex+  IntToVisibilityConverter �� 	Converter# [ .+�� -   $ CharStatGrid��$��0,0,0,10q� �� $��70 ��� .+�� -   $ 	SkillGrid��$��0��$	��350��$
��Left=�$��Bottom=�$��70�� �� �� �� �� �� �� �� �� �� ��  �� �� .+��� -   $ 0��$��0��$��1��  SkillButton#��[ #��[ ���� *CurrentUIStats.skillsImg[0]+�� -   $ 1��$��1��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[1]+�� -   $ 2��$��2��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[2]+�� -   $ 3��$��3��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[3]+�� -   $ 4��$��4��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[4]+�� -   $ 5��$��5��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[5]+�� -   $ 6��$��6��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[6]+�� -   $ 7��$��7��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[7]+�� -   $ 8��$��8��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[8]+�� -   $ 9��$��9��$��1��#��[ #��[ ���� *CurrentUIStats.skillsImg[9]+�� $��0��$�� 1��$��0�� 	 	WoodShort#��[	 $��Center=�$��Center=�
���r���$��18(��� $��1��$�� 2��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��2��$�� 3��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��3��$�� 4��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��4��$�� 5��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��5��$�� 6��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��6��$�� 7��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��7��$�� 8��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��8��$�� 9��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(��� $��9��$��10��$��0��#��[	 $��Center=�$��Center=�
���r���$��18(�� $��1��$
��Left=� �� $��0.45* ��� $��0.55* �.+�� $��0��� -    $ StatGrid��$��1��$	��200��$��10,0,0,0q� �� $��0.1383* ��� $��0.0883* ��� $��0.0883* ��� $
��0.1* ��� $
��0.1* ��� $
��0.1* ��� $
��0.1* ��� $��0.115* � �� $��0.166* ��� $��0.166* ��� $��0.166* ��� $��0.166* ��� $��0.166* ��� $��0.166* �.+��� $��0��$��0��$��4��$��14(�$
��Bold"�$��Century)�
���z���$��Center=�$��Center=�#��[ ���� *CurrentUIStats.Name+�� $��Level�� 
 DoubleTextLabel#��[
 $��0��$��2��$��1��$��11(�$��Century)�
���z���#��[  �� *CurrentUIStats.level+�� -!    ��Name$ StatpointsLeft��#��[ $��3��$��3��$��2��$��	30,0,30,0q�$��11(�$��Century)�
���z���#��[ ���� *&$CurrentUIStats.IncreasableStatpoints+ �� *&$CurrentUIStats.DecreasableStatpoints+�� -"   $ Skillpoints_LeftField��$��Skillpoints��#��[
 $��0��$��3��$��2��$��9(�$��Century)�
���z���#��[  �� *%#CurrentUIStats.AvailableSkillpoints+�� -#   $ Strength��$��3��$��3��#��[ $��14(�$
��Bold"�$��Century)�$��Strength��$��Center=�$��Center=�
���z����� -$   $ Vitality��$��3��$��4��#��[ $��14(�$
��Bold"�$��Century)�$��Vitality��$��Center=�$��Center=�
���z����� -%   $ Intelligence��$��3��$��5��#��[ $��14(�$
��Bold"�$��Century)�$��Intelligence��$��Center=�$��Center=�
���z����� -&   $ Wisdom��$��3��$��6��#��[ $��14(�$
��Bold"�$��Century)�$��Wisdom��$��Center=�$��Center=�
���z����� -'   $ Left1��$��3��$��3��$��5,0,0,0q�  NoContentBtn#��[ $ 0��$
��left�� 	 LeafL#��[ # [ y��� *CurrentUIStats+  IsDecreasable# [  �� ConverterParameter$ 0���� -(   $ Left2��$��3��$��4��$��5,0,0,0q�#��[ $ 1��$
��left��#��[ # [ y��� *CurrentUIStats+# [ $ 1���� -)   $ Left3��$��3��$��5��$��5,0,0,0q�#��[ $ 2��$
��left��#��[ # [ y��� *CurrentUIStats+# [ $ 2���� -*   $ Left4��$��3��$��6��$��5,0,0,0q�#��[ $ 3��$
��left��#��[ # [ y��� *CurrentUIStats+# [ $ 3���� -+   $
 Mid1��$��4��$��3��#��[	 $��Center=�$��Center=�
���z������� *CurrentUIStats.stats[0]+�� -,   $
 Mid2��$��4��$��4��#��[	 $��Center=�$��Center=�
���z������� *CurrentUIStats.stats[1]+�� --   $
 Mid3��$��4��$��5��#��[	 $��Center=�$��Center=�
���z������� *CurrentUIStats.stats[2]+�� -.   $
 Mid4��$��4��$��6��#��[	 $��Center=�$��Center=�
���z������� *CurrentUIStats.stats[3]+�� -/   $ Right1��$��5��$��3��$��0,0,5,0q�#��[ $ 0��$��right�� 	 LeafR#��[ # [ y��� *&$CurrentUIStats.IncreasableStatpoints+# [ $ 0���� -0   $ Right2��$��5��$��4��$��0,0,5,0q�#��[ $ 1��$��right��#��[ # [ y��� *&$CurrentUIStats.IncreasableStatpoints+# [ $ 0���� -1   $ Right3��$��5��$��5��$��0,0,5,0q�#��[ $ 2��$��right��#��[ # [ y��� *&$CurrentUIStats.IncreasableStatpoints+# [ $ 0���� -2   $ Right4��$��5��$��6��$��0,0,5,0q�#��[ $ 3��$��right��#��[ # [ y��� *&$CurrentUIStats.IncreasableStatpoints+# [ $ 0���� -3   $ Reskill��$��2��$��0��$��7��$��Century)�#��[ $��Reskill��
���z���#��[ # [ �� -4   $ Levelup��$��2��$��2��$��7��$��Century)�#��[ $��Levelup��
���z���#��[ # [ �� -5   $
 Save��$��2��$��4��$��7��$��Century)�#��[ $
��Save��
���z���#��[ # [ � -6   $ SkillchoiceBackground��$��1��  
GreenFade2#��[ $��Stretch=�$��50,50,50,50q�$��Stretch=�y��� *currentlyCustomizedSkill+# [ .+��� -7    ��Name$ SkillBackdrop��$
��Auto=�.���� -8   $ SkillsItemsControl������ *skillInfoLibrary+���� .R��� -9   $��50��$��50��$��5,5,5,5q�#��[  ^� InitialShowDelay$	 100��  ^� BetweenShowDelay$	  100�� �� *ID+! � ToolTip! _� #��[ $��0q�.���� $��25,15,25,15q�.+��� $
��Bold"�
���z������� *Name+�� 
���z������� *Description+����  �� *img+���� .R�� � -:   $ 
FriendChat��$��1��$��10q�y��� *loadedFriendIndex+# [  �� $��50 ��� �� $��40 �.+��� $��44��
����� �
���   ����� *CurrentChat.Guid+�� $��1��$
��Auto=�
�������$��5,5,5,5q�.���� ���� *CurrentChat.Chatoutput+���� .R��� ���� �� -;   $��ChatInputBox��$��2��$��30��$��Bottom=�$��2q�� -<   $ 
FriendGrid��$��2�� �� $��40 ��� �� $��50 �.+��� -=   $ UsernameTextfield��$��0��$��Right=�$	��Top=�$��33��$ 
Horizontal=�#��[ .+��� $��15,0,0,0q�
���r���$��15(�$��22������ *
Username+�� 
���r���$��15(�$��#��$��Right=�$��22���� $��0,0,15,0q�
���r���$��15(�$��22��$��Right=����� *
UserGuid+�� $��1��.+��� ->   
������ #��[ $��0q����� *friendsRequestList+�� -?   
������ #��[  $��0q����� *friendsList+�� $��2��$��Bottom=�$ Vertical=�.+��� -@   $��	AddFriend��$��25��$��	10,0,20,0q��� -A   $ AddFriend_Button��#��[ $��	AddFriend��$��22��" J� RenderTransformOrigin$" 0.562,1.838&�$��12(�$��Century)�
���z���#��[ $��	10,0,20,0q�