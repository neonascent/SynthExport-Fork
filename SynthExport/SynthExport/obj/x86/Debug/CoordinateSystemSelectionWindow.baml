   M S B A M L   `   `   `  ����  SynthExport1    +SynthExport.CoordinateSystemSelectionWindow   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	     � Title$)  #Select coordinate systems to export��5   	   $	��300��65   $	��250��6A    � 
ResizeMode$ NoResize=�6N    � WindowStartupLocation$ CenterOwner=�6d    � 	Resources 5         pointsColumn �� Width �� CellTemplate �� Triggers �� Binding �� Value	 �� 
TargetName
 �� Property �� Value��%A  &	        �� 5   
   $��Points��6.   $ 80��6>    5      �� 5      .R�5	      �� $��	textBlock��6    ��61   �� *PointCloud.NumberOfPoints+ 5
      �� 5       6&   �� *
PointCloud+ 6E   Z� .a�5      �� $	 	textBlock��6%   
 � ��$ -��6L   5      5      5      5      .��5      � $��5q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0,0,0,5q�6!   $
��Wrap=�62   `^The synth contains more than one coordinate system. Check those whose data should be exported.5   9   �� 5   
   -   5        $��listViewCoordinateSystems��5      $��1��64    �� View 5      �� 5      P�5       �� 5!       5"      �� 5#   "   .R�5$   &   ��  d� 	IsChecked 6/   �� *ShouldBeExported+5%   #   5'      �� 5(      $��ID��6)   $ 30��65    �� DisplayMemberBinding 6@   �� *ID+�� 5)      $��Images��6)   $ 80��69    6D   �� *$"CameraParameterList.NumberOfImages+57      59      ]� 5;   
   $��2��6   $��0,5,0,0q�6!   ��.  .+�5<      �� -   5        $��buttonCancel��5<      $��Right=�6)   .��6[   Cancel6c   �� 5=      -   5        $��buttonOK��5=      $��Right=�6%   $��0,0,5,0q�6<   .��6d   OK6h   5>      5?      5@      