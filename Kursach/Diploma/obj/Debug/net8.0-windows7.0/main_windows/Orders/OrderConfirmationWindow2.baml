   M S B A M L   `   `   `  ����  Diploma3    -Diploma.main_windows.OrderConfirmationWindow2   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$-  'Подтверждение заказа��5   	   $	��600��6&   $	��800��63   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $��0,0,0,29q�6    � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Детали заказа��6   $��24(�5      $
��Bold"�6"   $��Center=�5      $��	0,20,0,10q�61   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrderDetailsDataGrid��5         AutoGenerateColumns$ False��61   ���	#FF3C3C3C
�������$��0q�5   =   $��
20,0,20,10q�5         CanUserAddRows$ False��6'   $��1��6>      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Товар��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6T   	   
IsReadOnly$
	 True��6^   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 64   �� *ProductName+6^     5      $ Количество��6%   $ * 6[   $
	 True��6e   
 69   �� *OrderQuantity+6e     5      $" Цена за единицу��6%   $ * 6X   $
	 True��6b   
 6>   �� *Price+6b     5      $ 
Сумма��6%   $ * 6N   $
	 True��6X   
 64   �� *Total+6X      RowStyle 5      �� 5        �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�5!      ��  �� Property � �� �� Value �	#FF3C3C3C63   �� 5"       � ��
 �����63   �� 5#       � ��$ 14(�61   �� 5$       � ��$ 0q�68   5%         ColumnHeaderStyle 5)      �� 5*      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5+      ��  � �� �	#FF4D4D4D63   �� 5,       � ��
 �����63   �� 5-       � ��$
 Bold"�63   �� 5.       � ��$ 14(�61   �� 5/       � ��$ 10q�60   �� 50       � ��$ 0q�68   51      53      �� 56   
    �� Orientation$ Vertical=�6   $��2��6,   $
��20,0q�69   .+�57      �� -   5         ��Name$ TotalPriceTextBlock��57      ���	#FF00A651$��18(�6L   $
��Bold"�6Z   $��Right=�6l   �� 58      $ 
Horizontal=�6   $��Right=�62   $��0,20,0,0q�6N   .+�59      �� -   5        $��Отменить��59      ���	#FFD4AF37
�������$��0q�5;      $
��10,5q�6-   $��14(�6<    � Cursor$
 Hand��6J   $��0,0,10,0q�6X   �� 5<      -   5        $��Подтвердить��5<      ���	#FF00A651
�������$��0q�5>      $
��10,5q�6-   $��14(�6<   $
 Hand��6J   5?      5@      5A      5B      