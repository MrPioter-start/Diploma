   M S B A M L   `   `   `  ����  Diploma:    4Kursach.main_windows.admin.PaymentConfirmationWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$-  'Подтверждение оплаты��5   	   $	��600��6&   $	��800��63   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Детали заказа��6   $��24(�5      $
��Bold"�6"   $��Center=�5      $��	0,20,0,10q�61   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrderDetailsDataGrid��5         AutoGenerateColumns$ False��61   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6   $��
20,0,20,20q�5         CanUserAddRows$ False��6   $��1��66      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Товар��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 6M   
   
IsReadOnly$

 True��6W   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *Name+6W     5      $ Количество��6%   $	 * 6[   $

 True��6e    69   �� *OrderQuantity+6e     5      $" Цена за единицу��6%   $	 * 6X   $

 True��6b    6>   �� *Price+6b     5      $ 
Сумма��6%   $	 * 6N   $

 True��6X    64   �� *Total+6X      	CellStyle 5      �� 5       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5      ��  �� Property � �� �� Value$ 0q�68   �� 5        � ��$ 10q�60   �� 5!       � ��$ 14(�61   �� 5"       � ��
 �����63   �� 5#       � �� �	#FF3C3C3C63   �� 5$       � ��$
 Left=�6<   �� 5%       � ��$ 10,0,0,0q�6/   5&         ColumnHeaderStyle 5)      �� 5*      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5+      ��  � �� �	#FF4D4D4D63   �� 5,       � ��
 �����63   �� 5-       � ��$
 Bold"�63   �� 5.       � ��$ 16(�61   �� 5/       � ��$ 10q�60   �� 50       � ��$ 0q�68   �� 51       � ��$
 Left=�6C   52         RowStyle 55      �� 56      )  #System.Windows.Controls.DataGridRow"  6   .�57      ��  � �� �	#FF3C3C3C63   �� 58       � ��
 �����63   �� 59       � ��$ 14(�61   �� 5:       � ��$ 0q�68   �� 5;       � ��$ 40��6/   �� 5<       � ��$
 Left=�6<   5=      5?      �� 5A   
    �� Orientation$ Vertical=�6   $��2��6,   $
��20,0q�69   .+�5B      �� -   5         ��Name$ OriginalTotalTextBlock��5B   :   $.��(Итого без скидки: 0.00 byn��6   
�������$��16(�6m   �� 5C      -   5        $ DiscountTextBlock��5C   0   $��Скидка: 0.00 byn��6   
�������$��16(�6^   �� 5D      -   5        $ TotalPriceTextBlock��5D   ?   $7��1Итого с учетом скидки: 0.00 byn��6   ���	#FF00A651$��18(�6s   $
��Bold"�6�   �� 5F      $��Сдача с:��6   
�������$��16(�6;   $��0,10,0,5q�6I   �� 5G      -   5        $ PaymentAmountTextBlock��5G      ���	#FF00A651$��16(�6O   �� 5I      $��Сдача:��6   
�������$��16(�69   $��0,10,0,5q�6G   �� 5J      -   5        $ ChangeTextBlock��5J      ���	#FF00A651$��16(�6H   �� 5L      $ 
Horizontal=�6   $��Right=�62   $��	0,20,0,30q�6N   .+�5M      �� -   5        $��Отменить��5M      ���	#FFD4AF37
�������$��0q�5O      $
��10,5q�6%   $��14(�64    � Cursor$
 Hand��6B   $��0,0,10,0q�6P   �� 5P      -   5        $��Оплатить��5P      ���	#FF00A651
�������$��0q�5R      $
��10,5q�6%   $��14(�64   $
 Hand��6B   5S      5T      5U      5V      