   M S B A M L   `   `   `  ����  Diploma,    &Kursach.main_windows.admin.SalesWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$'  !Оформление заказа��5   	   $	��600��6#   $
��1100��60   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $
��1000��6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $'��!Оформление заказа��6   $��24(�5      $
��Bold"�6   $
��Left=�5      $��720,10,0,20q�6,   ]� 5   
   $��0��6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��Center=�62   $
��20,0q�6M   .+�5      �� $��Поиск:��6   
�������$��Center=�6=   $��0,0,10,0q�6X   �� 5      -   5         ��Name$ SearchTextBox��5      ���	#FF4D4D4D
����������	#FF555555$��1q�5   *   $��5q�6>   $��30��6J   $	��200��5      $��14(�6   6,   �� 5      $��Фильтр:��6   
�������$��Center=�6>   $��10,0,5,0q�6Y   �� 5      -   5         ��Name$ FilterComboBox��5      $	��200��63   $��30��6?   $��10q�6~   ��5      �� 5       �� 
TargetType" ��6    .�5      ��  �� Property � �� �� Value �	#FF4D4D4D6;   �� 5        � ��
 �   �6;   �� 5!       � ��$ 14(�69   �� 5"       � ��$ 5q�68   �� 5#       � ��$ 1q�6@   �� 5$       �@�Cursor$
 Hand��67   5%      .@�5'      �� $��Все товары��6#   	 �� 
IsSelected$
	 True��68   �� 5(      $'��!Часто продаваемые��6#   5)      5*      5+      �� 5-   
   -   5        
 ��Name$
 RecommendationsListBox��5-      $��1��5.   
   $��20,10q�5/   
   ���	#FF3C3C3C
�������$��0q�52   
   ��53      �� 54      .R�55      �� ���	#FF4D4D4D$	��0,5q�64    �� CornerRadius$ 5��6A    �� Padding$ 10q�6R   ._�56      �� $ 
Horizontal=�6%   .+�57      �� $
��Bold"�6>   $	��150��6P   ��6(   �� *Name+6P   �� 58      $
��10,0q�6F   ��6(   �� *CategoryName+6F   �� 59      $
��10,0q�6?   ��6(   �� *Price+6?   5:      5;      5<      5>      &   System.Windows.Controls.DataGrid  5@   
   -   5          Name$ ProductsDataGrid��5@         AutoGenerateColumns$ False��6-   ���	#FF3C3C3C
�������$��0q�5B         GridLinesVisibility$
 None=�6   $��
20,0,20,20q�5C         CanUserAddRows$ False��6   $��1��66      Columns 5D      0  *System.Windows.Controls.DataGridTextColumn  5E      ,  &System.Windows.Controls.DataGridColumn   Header$ Название��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6U      
IsReadOnly$
 True��6_      
Visibility$ Hidden=�6q   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *	ProductID+6q     5F      $ Название��6%   $ * 6P   $
 True��6Z    67   �� *Name+6Z     5G      $ Категория��6%   $ * 6Y   $
 True��6c    68   �� *CategoryName+6c     5H      $ 
Бренд��6%   $ * 6N   $
 True��6X    64   �� *Brand+6X     5I      $ Цена��6%   $ * 6M   $
 True��6W    63   �� *Price+6W     5J      $ Наличие��6%   $ * 6�   $
 True��6�    66   �� *
Quantity+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   4  .System.Windows.Controls.DataGridTemplateColumn  5K      $ Количество��6)   $	 150 6=      CellTemplate 5L      �� 5M      .R�5N      �� $ 
Horizontal=�6)   .+�5O   "   �� -   5        $��-��5O   )   ���#FF0000
�������$��0q�5Q   !   $��5q�65    � Cursor$
 Hand��6A   
 � Tag 6O   �� *Name+�� 5S   "   -   5        ���	#FF4D4D4D
����������	#FF555555$��1q�5T   :   $��5q�6N   $��30��6Z   $	��100��5U   "   $��14(�6.    �� TextAlignment$ Center=�6<   ��5S   *   �� *OrderQuantity+$ PropertyChanged=� 5V   J   �� *Name+�� 5X   "   -   5        $��+��5X   )   ���	#FF00A651
�������$��0q�5Z   !   $��5q�65   $
 Hand��6A    6O   �� *Name+5[      5\      5^         RowStyle 5a      �� 5b      )  #System.Windows.Controls.DataGridRow"  6   .�5c      ��  � �� �	#FF3C3C3C63   �� 5d       � ��
 �����63   �� 5e       � ��$ 14(�61   �� 5f       � ��$ 0q�68   �� 5g       � ��$ Stretch=�6C   5h         	CellStyle 5k      �� 5l      *  $System.Windows.Controls.DataGridCell"  6   .�5m      ��  � �� �	#FF3C3C3C63   �� 5n       � ��
 �����63   �� 5o       � ��$ 14(�61   �� 5p       � ��$ 0q�68   �� 5q       � ��$ 10q�60   �� 5r       � ��$ Stretch=�6C   �� 5s       � ��$ Center=�6A   �� 5t       � ��$ 10,0,0,0q�6/   5u         ColumnHeaderStyle 5x      �� 5y      =	  7System.Windows.Controls.Primitives.DataGridColumnHeader" 	 6   .�5z      ��  � �� �	#FF4D4D4D63   �� 5{       � ��
 �����63   �� 5|       � ��$
 Bold"�63   �� 5}       � ��$ 16(�61   �� 5~       � ��$ 10q�60   �� 5       � ��$ 0q�68   �� 5�       � ��$
 Left=�6C   5�      5�      �� 5�   
   $ 
Horizontal=�6   $��Right=�6.   $��Center=�6J   $��	0,0,20,20q�6e   $��2��6x   .+�5�      �� -   5        $��Exel заказ��5�      ���	#FF00A651
�������$��0q�5�      $
��10,5q�6!   $��14(�60   $
 Hand��6>   $��0,0,10,0q�5�      $	��108��6   �� 5�      -	   5        $��Шаблон��5�      ���	#FF00A651
�������$��0q�5�      $
��10,5q�6!   $��14(�60   $
 Hand��6>   $��76��6L   �� 5�      $��Скидка:��6   
�������$��14(�6:   $
��Bold"�6H   $��	10,0,10,0q�6Z   �� 5�      -
   5        $ DiscountTextBox��5�      ���	#FF4D4D4D
����������	#FF555555$��1q�5�   &   $��5q�6:   $��30��6F   $	��100��5�      $��14(�6   $ Center=�6(   6?   �� 5�      -   5        $ DiscountTypeComboBox��5�      ���	#FF4D4D4D
���   ����	#FF555555$��1q�5�   '   $��5q�6;   $��30��6G   $��57��5�      $��14(�6   $��	10,0,20,0q�6(   .@�5�      �� $��Руб.��6   $
	 True��6.   �� 5�      $��%��6   5�      �� 5�      $��Сдача с:��6   
�������$��14(�6;   $
��Bold"�6I   $��0,0,10,0q�6[   �� 5�      -   5        $ PaymentAmountTextBox��5�      ���	#FF4D4D4D
����������	#FF555555$��1q�5�   "   $��5q�66   $��30��6B   $	��100��5�   
   $��14(�6   $ Center=�6$   ��5�   
   �� *PaymentAmount+$ PropertyChanged=��� 5�      $ 
Horizontal=�6   $��Right=�62   $��Center=�6N   $��	20,0,20,0q�6i   $��2��6|   .+�5�      �� $��Итого:��6   
�������$��18(�6=   $
��Bold"�6K   $��0,0,10,0q�6]   �� 5�      -   5          ��Name$  TotalPriceTextBlock��5�      $��0.00 byn��69   ���	#FF00A651$��18(�6`   $
��Bold"�6n   5�      �� 5�      -   5        $'��!Подтвердить заказ��5�      ���	#FF00A651
�������$��0q�5�      $
��10,5q�6!   $��14(�60   $
 Hand��6>   $��20,0,0,0q�6L   5�      5�      5�      