   M S B A M L   `   `   `  ����!  Spikes.ArtistRepositoryPlugin4    .Spikes.Spikes_ArtistRepositoryPlugin.MyControl   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5        $��ArtistRepositoryBrowser��5	         VsBrush.WindowText#���   5      ��5      �� 5   
     �� ImageSource$  Resources/Artist.PNG��5       j� 
AlignmentX$ Right=�5       �� Opacity$	 0.1Q�5       j� 
AlignmentY$ Bottom=�5       j� Stretch$ Uniform=�6%   .��5      �  � RowDefinitions 5   
   �� 5      $	��300 �6   �� 5      $��5 �6   �� 5      $��* �6   .+�5   
   R� -   5        $��ProjectsTreeView��5      $��0��6+    J� 	AllowDrop . $��0q�5   �    � 	Resources 5         ArtefactContext �� 	StaysOpen��%�   &	       �� 5!       . .@�5"      m� -   5        $��
Properties��5"      63   �� 5#      m� 5$      -   5        $��Delete��5$      6/   5%      5&      ��5'      �� 5(      	 �� Color$	 Transparent��6"   5+      �� 5-   
   $��2��6   $��Stretch=�6"   $��0q�6@   $	��Top=�5.   	   
 �� ResizeDirection$

 Rows=�5/   	    �� ShowsPreview . $��1��51   	   � 53   
   $��2��6    54      �� 55      $
��Auto �6    �� 56      $��* �6    .+�59      �� -   5        $��LblDetailLabel��59      $��0��6/   $
��Wrap=�6<   $	��5,2q�6P   $
��Bold"�6]   6q   �� 5:      -   5        $��LblDetailDescription��5:      $��1��65   $
��Wrap=�6B   $	��5,2q�6V   $��10(�6c   6s   5;      5=      5@      