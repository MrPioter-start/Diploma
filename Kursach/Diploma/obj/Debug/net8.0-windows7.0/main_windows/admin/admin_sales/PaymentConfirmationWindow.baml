   M S B A M L   `   `   `  ����  Diploma:    4Kursach.main_windows.admin.PaymentConfirmationWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$-  'Подтверждение оплаты��5   	   $	��800��6&   $	��900��63   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      �� 5      $
��Auto �6   $��304.117��6*   .+�5   
   �� $��Детали заказа��6   $��24(�5      $
��Bold"�6"   $��Center=�5      $��	0,20,0,10q�61   $	��166��6D   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrderDetailsDataGrid��5         AutoGenerateColumns$ False��61   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6   $��
20,0,20,20q�5         CanUserAddRows$ False��6   $��1��66      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Товар��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 6R   
   
IsReadOnly$

 True��6\      
Visibility$ Hidden=�6n   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *	ProductID+6n     5      $ 
Товар��6%   $	 * 6M   $

 True��6W    64   �� *Name+6W     5      $ 
Бренд��6%   $	 * 6N   $

 True��6X    64   �� *Brand+6X     5      $ CategoryName��6%   $	 * 6\   $

 True��6f   $ Hidden=�6x    6;   �� *CategoryName+6x     5      $ Количество��6%   $	 * 6[   $

 True��6e    69   �� *OrderQuantity+6e     5      $" Цена за единицу��6%   $	 * 6X   $

 True��6b    6>   �� *Price+6b     5      $ 
Сумма��6%   $	 * 6N   $

 True��6X    64   �� *Total+6X      	CellStyle 5       �� 5!       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5"      ��  �� Property � �� �� Value$ 0q�68   �� 5#       � ��$ 10q�60   �� 5$       � ��$ 14(�61   �� 5%       � ��
 �����63   �� 5&       � �� �	#FF3C3C3C63   �� 5'       � ��$
 Left=�6<   �� 5(       � ��$ 10,0,0,0q�6/   5)         ColumnHeaderStyle 5,      �� 5-      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5.      ��  � �� �	#FF4D4D4D63   �� 5/       � ��
 �����63   �� 50       � ��$
 Bold"�63   �� 51       � ��$ 16(�61   �� 52       � ��$ 10q�60   �� 53       � ��$ 0q�68   �� 54       � ��$
 Left=�6C   55         RowStyle 58      �� 59      )  #System.Windows.Controls.DataGridRow"  6   .�5:      ��  � �� �	#FF3C3C3C63   �� 5;       � ��
 �����63   �� 5<       � ��$ 14(�61   �� 5=       � ��$ 0q�68   �� 5>       � ��$ 40��6/   �� 5?       � ��$
 Left=�6<   5@      5B      �� 5D   
    �� Orientation$ Vertical=�6   $��2��6,   $��	20,2,20,0q�69   .+�5E      �� -   5         ��Name$ OriginalTotalTextBlock��5E   :   $.��(Итого без скидки: 0.00 byn��6   
�������$��16(�6m   �� 5F      -   5        $ DiscountTextBlock��5F   0   $��Скидка: 0.00 byn��6   
�������$��16(�6^   �� 5G      -   5        $ TotalPriceTextBlock��5G   ?   $7��1Итого с учетом скидки: 0.00 byn��6   ���	#FF00A651$��18(�6s   $
��Bold"�6�   �� 5H      -   5        $ PromotionsTextBlock��5H      
�������$��14(�6H   $��0,10,0,5q�6V   $
��Wrap=�6h   �� 5J      $��Сдача с:��6   
�������$��16(�6;   $��0,10,0,5q�6I   �� 5K      -   5        $ PaymentAmountTextBlock��5K      ���	#FF00A651$��16(�6O   �� 5M      $��Сдача:��6   
�������$��16(�69   $��0,10,0,5q�6G   �� 5N      -   5        $ ChangeTextBlock��5N      ���	#FF00A651$��16(�6H   �� 5P      $ 
Horizontal=�6   $��Right=�62   $��	0,20,0,30q�6N   .+�5Q      �� -   5        $��Отменить��5Q      ���	#FFD4AF37
�������$��0q�5S      $
��10,5q�6%   $��14(�64    � Cursor$
 Hand��6B   $��0,0,10,0q�6P   �� 5T      -	   5        $��Оплатить��5T      ���	#FF00A651
�������$��0q�5V      $
��10,5q�6%   $��14(�64   $
 Hand��6B   5W      5X      5Y      5Z      