   M S B A M L   `   `   `  ����  SkeletonBasics-WPF8    2Microsoft.Samples.Kinect.SkeletonBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$&   Gesture Recognition Using Kinect��5   	   $	��630��62   $
��1328��6?    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SquareCheckBox �� 
TargetType �� Orientation �Name	 ��Name
 ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%1  &	        &	 #     &	 F     '�� i     &	 �     7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5	   
   $ 	#ff00BCF2��62   �� 5
   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   � $ SquareCheckBoxChecked��6'   .+�5   &   �� $	 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5   &   $	 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5   #   � 5   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�5   &   �� $	 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5   &   $	 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   5   #   �� 5   "   $
 SquareCheckBoxText��6,   #��z��6H   $
 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   5      5       5      P� 5       �@d�	IsChecked$ falseY�6;   . �5    "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5!   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5"      P� 5#       �@J�IsMouseOver . . �5$   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5%   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5&   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5'   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5(   "   8  $ SquareCheckBoxText��6h    � ��5)      5+      5-      5.      5/      .��51      � -   5        $��
layoutGrid��51      $��	10 0 10 0q�6    � RowDefinitions 52   
   �� 53      $��59 �6   �� 54      $��32 �6   �� 55      $
��551* �6   �� 56      $
��Auto �6   �� 57      $
��Auto �6   .+�59   
   ]� $��0,0,0,12q�6   .+�5:      �� $��1��6   $��Images\Status.png��6$   $
��None=�6?   $��Center=�6N   $��0 0 0 5q�6k   �� 5;      -   5        $��label1��5;   K   $&�� Gesture Recognition Using Kinect��6   $��43��6?   $��20(�6Y   $
��Bold"�6g   $��Normal$�6y   �� 5<      -   5        $��	checkBox1��5<   :   $��
Train Mode��6   $
��Auto��6,   #��[ 6K   6�   �� 5=      -   5        $��label2��5=       $��35��6   $��18(�6.   $��Segoe UI)�6<   $��53��6R   
���  ��6]   5>      (� 5?   
   $��1��6    (� Stretch$ Uniform=�6   $��Center=�61   $��0,8,0,0q�6N   $��2��6_   6r   �� 5@   
   -   5        $��checkBoxSeatedMode��5@   �   $��3��6   #��[ 6    $��Seated Mode��6H   $
��Auto��6^   $
��Left=�6l   $��Center=�6�   $��
0 10 10 10q�6�   6�   �� 5A   
   -   5        $��	statusBar��5A   ?   $��4��6   $��Stretch=�6!   $��Bottom=�6P   
�������#��[  6~   .@�5B      �� $��0 0 0 10q�6   61   5C      �� 5D   
   -   5        $��textBox1��5D   Z   $��3��6   $��23��6   $��Right=�6+   $��	0,11,83,0q�6G   $	��Top=�6j   $	��225��6�   �� 5E   
   -	   5        $��button1��5E   u   $��Start Recording��6   $��3��6+   $��23��68   $��Right=�6D   $��0,10,1008,0q�6`   $	��Top=�6�   $	��100��6�   6�   �� 5F   
   -
   5        $��button4��5F   i   $��About��6   $��3��6!   $��23��6.   $��Right=�6:   $��	0,10,12,0q�6V   $	��Top=�6x   $��65��6�   6�   �� 5G   
   -   5        $��Image��5G      $	��640��6   $	��480��6)   $��
0,8,666,34q�66   $��1��6J   $��3��6W   �� 5H   
   -   5        $��image1��5H   C   $��3��6   $	��480��6!   $��654,8,12,34q�6.   $	��640��6Q   $��1��6]   �� 5I   
   -   5        $��label3��5I   \   $��Label��6   $��2��6    $
��Left=�6-   $��
415,69,0,0q�6H   $	��463��6j   $��36(�6v   $��Center=�6�   $��Center=�6�   $y�Hidden=�6�   
���  ��$	��178��6�   $	��Top=�6�   �� 5J   
   -   5        $��button2��5J   t   $��Restart Testing��6   $��2��6+   $��43��68   $
��Left=�6D   $��415,253,0,0q�6_   $	��Top=�6�   $	��464��6�   $y�Hidden=�6�   $��18(�6�   6�   5K      5L      