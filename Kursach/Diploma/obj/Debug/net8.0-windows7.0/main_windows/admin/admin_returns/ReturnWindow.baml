   M S B A M L   `   `   `  ����B  >Diploma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:Kursach.main_windows.adminKursach.main_windows.admin  5   	   -    'Kursach.main_windows.admin.ReturnWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:Kursach.main_windows.admin     5   	     � Title$!  Возврат товара��5   	   $	��400��6    $	��600��6-   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5
   	   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ ReturnItemsDataGrid��5         AutoGenerateColumns$ False��60   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6   $��20,20,20,20q�5         CanUserAddRows$ False��6       Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Товар��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 6T   
   
IsReadOnly$

 True��6^   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *ProductName+6^     5      $ Количество��6%   $	 * 6V   $

 True��6`    69   �� *
Quantity+6`     5      $" Цена за единицу��6%   $	 * 6X   $

 True��6b    6>   �� *Price+6b   4  .System.Windows.Controls.DataGridTemplateColumn  5      $ Вернуть��6)   $		 150 6:      CellTemplate 5      �� 5      .R�5      ��  �� Orientation$ 
Horizontal=�6)   .+�5   "   �� -   5        $��-��5   )   ���#FF0000
�������$��0q�5   !   $��5q�65    � Cursor$
 Hand��6A   
 � Tag 6O   �� *ProductName+�� 5    "   ���	#FF4D4D4D
����������	#FF555555$��1q�5"   :   $��5q�6N   $	��100��5#   "   $��14(�6.    �� TextAlignment$ Center=�6<   ��5    *   �� *ReturnQuantity+ �� UpdateSourceTrigger$ PropertyChanged=�5#   <   �� 5$   "   -   5        $��+��5$   )   ���	#FF00A651
�������$��0q�5&   !   $��5q�65   $
 Hand��6A    6O   �� *ProductName+5'      5(      5*         	CellStyle 5-      �� 5.       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5/      ��  �� Property � �� �� Value$ 0q�68   �� 50       � ��$ 10q�60   �� 51       � ��$ 14(�61   �� 52       � ��
 �����63   �� 53       � �� �	#FF3C3C3C63   �� 54       � ��$
 Left=�6C   �� 55       � ��$ 10,0,0,0q�6/   56         RowStyle 59      �� 5:      )  #System.Windows.Controls.DataGridRow"  6   .�5;      ��  � �� �	#FF3C3C3C63   �� 5<       � ��
 �����63   �� 5=       � ��$ 14(�61   �� 5>       � ��$ 0q�68   �� 5?       � ��$ Stretch=�6C   5@         ColumnHeaderStyle 5C      �� 5D      =	  7System.Windows.Controls.Primitives.DataGridColumnHeader" 	 6   .�5E      ��  � �� �	#FF4D4D4D63   �� 5F       � ��
 �����63   �� 5G       � ��$
 Bold"�63   �� 5H       � ��$ 16(�61   �� 5I       � ��$ 10q�60   �� 5J       � ��$ 0q�68   �� 5K       � ��$
 Left=�6C   5L      5N      �� 5P   
   $ 
Horizontal=�6   $��Right=�6.   $��	0,0,20,20q�6J   $��1��6]   .+�5Q      �� -   5        $��Отмена��5Q      ���	#FFD4AF37
�������$��0q�5S      $
��10,5q�6)   $��14(�68   $
 Hand��6F   $��0,0,10,0q�6T   �� 5T      -   5        $��Вернуть��5T      ���	#FF00A651
�������$��0q�5V      $
��10,5q�6)   $��14(�68   $
 Hand��6F   5W      5X      5Y      